using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class ChatMessagePageModel : BasePageModel
    {
        [ObservableProperty]
        ObservableCollection<MessageModel> _messages = new();
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        private string _messageEntryValue = string.Empty;
        private CollectionView _chatList;
        private readonly IChatService _chatService;
        private static string _currentUserId = string.Empty;
        private int _currentPage = 1;
        private const int PageSize = 30;
        private UserChatModel _partner;
        private readonly HubConnection _connection;
        public ChatMessagePageModel(
            INavigationService navigationService,
            IChatService chatService)
            : base(navigationService, true)
        {
            _chatService = chatService;
            IsVisibleNavigation = true;
            HasBackButton = true;
            _connection = new HubConnectionBuilder()
                .WithUrl(Environments.URl_SERVER_HTTPS_DEVICE_WIFI_CHAT_HUB, options =>
                {
                    options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                    };

                    options.AccessTokenProvider = async () => await JWTHelper.GetValidAccessToken();
                })
                .Build();

            _connection.On<MessageModel>("ReceiveSpecificMessage", (message) =>
            {
                if(message != null)
                    Messages.Add(message);
                Console.WriteLine(" >>>>>>>>>>>>>>>> " + message.Content);
            });

            StartConnectionAsync();
        }

        private async void StartConnectionAsync()
        {
            try
            {
                await _connection.StartAsync();
                Console.WriteLine(" >>>>>>>>>>>>>>>> SignalR connection started.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" >>>>>>>>>>>>>>>> SignalR connection error: {ex.Message}");
            }
        }

        public async Task DisconnectAsync()
        {
            if (_connection?.State != HubConnectionState.Disconnected)
            {
                await _connection.StopAsync();
                Console.WriteLine(" >>>>>>>>>>>>>>>> SignalR connection stopped.");
            }
        }

        public override async Task InitAsync(object? initData)
        {
            if (initData is UserChatModel user)
            {
                _partner = user;
            }

            await base.InitAsync(initData);
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.PopPageAsync();
                IsBusy = false;
            }
        }

        public async Task InitDataAsync()
        {
            IsBusy = true;
            await Task.Delay(1);
            IsBusy = false;
        }

        public async override Task LoadDataAsync()
        {
            // _currentPage = 0;
            await base.LoadDataAsync();
            IsBusy = true;
            _currentUserId = !string.IsNullOrEmpty(_currentUserId) ? _currentUserId : UserSetting.Get(StorageKey.UserId);
            string queryParams = $"?PaginationRequest.PageNumber={_currentPage}&PaginationRequest.PageSize={PageSize}&PaginationRequest.UserId={_currentUserId}&PaginationRequest.MatchId={_partner.MatchId}";
            var data1 = await _chatService.GetMessagesAsync(EnvironmentsExtensions.ENDPOINT_GET_MESSAGES, queryParams);
            Messages = [.. data1?.Messages?.Items ?? []];
            _currentPage++;
            IsBusy = false;
        }

        private bool _isLoadingMoreMessages;
        [RelayCommand]
        async Task OnLoadMoreMessagesAsync(object obj)
        {
            if (IsBusy || _isLoadingMoreMessages)
                return;
            IsBusy = true;
            _isLoadingMoreMessages = true;
            await Task.Delay(100);

            _currentUserId = !string.IsNullOrEmpty(_currentUserId) ? _currentUserId : UserSetting.Get(StorageKey.UserId);
            string queryParams = $"?PaginationRequest.PageNumber={_currentPage}&PaginationRequest.PageSize={PageSize}&PaginationRequest.UserId={_currentUserId}&PaginationRequest.MatchId={_partner.MatchId}";
            var data1 = (await _chatService.GetMessagesAsync(EnvironmentsExtensions.ENDPOINT_GET_MESSAGES, queryParams))?.Messages?.Items ?? [];
            foreach (var message in data1)
            {
                Messages.Insert(0, message);
            }

            _currentPage = data1.Any() ? _currentPage + 1 : _currentPage;
            _ = Task.Delay(400).ContinueWith(t =>
            {
                _isLoadingMoreMessages = false;
            });

            IsBusy = false;
        }

        [RelayCommand]
        async Task OnSendMessageAsync(object obj)
        {
            if (string.IsNullOrEmpty(MessageEntryValue.Trim()))
                return;
            IsBusy = true;
            try
            {
                var message = new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderId = _currentUserId,
                    MatchId = _partner.MatchId,
                    ReceiverId = _partner.UserId,
                    Content = MessageEntryValue.Trim(),
                    IsCurrentUserSend = true,
                    IsImage = false,
                    SendTime = DateTime.Now,
                    IsRead = false
                };

                Messages.Add(message);
                MessageEntryValue = string.Empty;
                await _connection.InvokeAsync("SendMessageToPerson", message);
                await Task.Delay(100);
                _chatList.ScrollTo(Messages.Last(), position: ScrollToPosition.End);
            }
            catch (Exception e)
            {

            }
            finally
            {

                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnMessageImageItemClickedAsync(object obj)
        {
            if (MessageImageItemClickedCommand.IsRunning || IsBusy)
                return;
            ShouldLoadData = false;
            try
            {
                IsBusy = true;
                var message = obj as MessageModel;
                if (message != null)
                {
                    await NavigationService.PushToPageAsync<ImageViewerFullScreenPage>(param: message.Content, isPushModal: true);
                    await Task.Delay(100);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnSendImageAsync(object obj)
        {
            if (SendImageCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            try
            {
                //_chatList.ScrollTo(Messages.Last(), position: ScrollToPosition.End);
                string takePhoto = I18nHelper.Get("SetupPhotoPage_ImageDialog_TakePhoto");
                string choosePhoto = I18nHelper.Get("SetupPhotoPage_ImageDialog_ChoosePhoto");
                string[] buttons = { takePhoto, choosePhoto };
                var choice = await AppHelper.CurrentMainPage.DisplayActionSheet(
                                    I18nHelper.Get("SetupPhotoPage_ImageDialog_Title"), I18nHelper.Get("Common_Cancel"), null, buttons);

                var message = new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    // SenderId = "1",
                    // ReceiverId = "2",
                    // Content = "",
                    // Type = MessageType.Image,
                    // Timestamp = DateTime.Now,
                    // ImageData = null,
                    // IsMine = true,
                    IsRead = true
                };

                if (((string)choice).Equals(takePhoto))
                {
                    await OnExcuteTakePhotoAsync(message);
                }
                else if (((string)choice).Equals(choosePhoto))
                {
                    await OnExcuteChoosePhotoAsync(message);
                }

                if (!string.IsNullOrEmpty(message.Content))
                {
                    Messages.Add(message);
                    await Task.Delay(100);
                    _chatList.ScrollTo(Messages.Last(), position: ScrollToPosition.End);
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                IsBusy = false;
            }

        }

        private async Task OnExcuteChoosePhotoAsync(MessageModel message)
        {
            var filename = Path.GetRandomFileName() + ".png";
            if (await this.AllowTakePhotoAsync())
            {
                var _currentLocalImage = await CameraPluginExtensions.CancelableChoosePhotoAsync(filename);
                IsBusy = true;

                if (!string.IsNullOrEmpty(_currentLocalImage) && await UploadImageAsync(_currentLocalImage))
                {
                    using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        byte[] imageBytes = br.ReadBytes((int)stream.Length);
                        // message.ImageData = imageBytes;
                    }
                }
                else
                {
                    message.Content = "mmm.jpeg";
                }

                IsBusy = false;
            }
        }

        public byte[] GetDefaultImageBytes(string fileName)
        {
            try
            {
                using (Stream stream = FileSystem.OpenAppPackageFileAsync(fileName).Result)
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        return br.ReadBytes((int)stream.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading default image: {ex.Message}");
                return null;
            }
        }

        private async Task OnExcuteTakePhotoAsync(MessageModel message)
        {
            //TODO: handle upload image in server
            IsBusy = true;
            if (await this.AllowTakePhotoAsync())
            {
                var filename = Path.GetRandomFileName() + ".png";
                var _currentLocalImage = await CameraPluginExtensions.CancelableTakePhotoAsync(filename);

                if (!string.IsNullOrEmpty(_currentLocalImage) && await UploadImageAsync(_currentLocalImage))
                {
                    using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        byte[] imageBytes = br.ReadBytes((int)stream.Length);
                        // message.ImageData = imageBytes;
                    }
                }
                else
                {
                    message.Content = "mmm.jpeg";
                }
            }

            IsBusy = false;
        }

        private async Task<bool> UploadImageAsync(string imagePath)
        {
            var isUpLoadImage = true;

            if (isUpLoadImage)
            {
                //TODO: handle upload image in server

                //_mailInquiry = await DataService.GetMailInquiryAsync();
                //EncryptImageModel encrypt;
                //imagePath = await ImageHelper.ImageToBase64Async(imagePath);
                //while (!(encrypt = await DataService.EncryptImageAsync([imagePath], _mailInquiry.InquiryNo)).IsSuccess)
                //{
                //    if (!await AlertHelper.ShowConfirmationAlertAsync(new AlertConfigure()
                //    {
                //        Title = I18nHelper.Get("PhotoRegistrationPage_ReUploadDialog_Title"),
                //        Message = I18nHelper.Get("PhotoRegistrationPage_ReUploadDialog_Message"),
                //        OK = I18nHelper.Get("PhotoRegistrationPage_ReUploadDialog_ReUpload")
                //    }))
                //    {
                //        return false;
                //    }
                //}

                //_imageUrl = encrypt.ImageUrl;
                return true;
            }

            return false;
        }

        public void GetChatListCollectionView()
        {
            _chatList ??= Shell.Current.CurrentPage.FindByName<CollectionView>("ChatMessageList");
        }
    }

    public class MessageChatItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MineMessageTextItemTemplate { get; set; } = new();
        public DataTemplate OtherMessageTextItemTemplate { get; set; } = new();
        public DataTemplate MineMessageImageItemTemplate { get; set; } = new();
        public DataTemplate OtherMessageImageItemTemplate { get; set; } = new();
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var collectionView = container as CollectionView;

            //TODO: add empty template
            if (collectionView?.ItemsSource is not ObservableCollection<MessageModel> items || item is not MessageModel message)
                return MineMessageTextItemTemplate;

            if (!message.IsImage)
            {
                return message.IsCurrentUserSend ? MineMessageTextItemTemplate : OtherMessageTextItemTemplate;
            }
            else
            {
                return message.IsCurrentUserSend ? MineMessageImageItemTemplate : OtherMessageImageItemTemplate;
            }
        }
    }
}