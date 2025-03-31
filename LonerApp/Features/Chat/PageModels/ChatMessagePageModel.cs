using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class ChatMessagePageModel : BasePageModel
    {
        [ObservableProperty]
        ObservableCollection<MessageModel> _messages = new();
        public bool IsNeedLoadUsersData = true;
        [ObservableProperty]
        private bool _isShowError;
        [ObservableProperty]
        private bool _isContinue;
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        private string _messageEntryValue = string.Empty;
        private CollectionView _chatList;

        public ChatMessagePageModel(INavigationService navigationService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            HasBackButton = true;
        }

        public override async Task InitAsync(object? initData)
        {
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
            LoadSampleMessages();
            IsBusy = false;
        }

        public async override Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            ShouldLoadData = false;
            IsBusy = true;
            await InitDataAsync();
            IsBusy = false;
            ShouldLoadData = true;
            IsNeedLoadUsersData = false;
        }

        [RelayCommand]
        async Task OnLoadMoreMessagesAsync(object obj)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            await Task.Delay(100);
            //var oldMessages = await GetMessagesAsync(_page);
            //if (oldMessages.Count > 0)
            //{
            //    int prevCount = Messages.Count;
            //    foreach (var message in oldMessages)
            //    {
            //        Messages.Insert(0, message);
            //    }

            //    // Giữ nguyên vị trí cuộn
            //    await Task.Delay(100); // Tránh lag khi thêm tin nhắn
            //    ChatList.ScrollTo(Messages[oldMessages.Count], position: ScrollToPosition.Start, animate: false);

            //    _page++;
            //}

            IsBusy = false;
        }

        [RelayCommand]
        async Task OnSendMessageAsync(object obj)
        {
            if (string.IsNullOrEmpty(MessageEntryValue))
                return;
            IsBusy = true;
            try
            {
                var message = new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderId = "1",
                    ReceiverId = "2",
                    Message = MessageEntryValue,
                    Type = MessageType.Text,
                    Timestamp = DateTime.Now,
                    IsMine = true,
                    IsRead = true
                };
                Messages.Add(message);
                MessageEntryValue = string.Empty;
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

                if (((string)choice).Equals(takePhoto))
                {
                    await OnExcuteTakePhotoAsync();
                }
                else if (((string)choice).Equals(choosePhoto))
                {
                    await OnExcuteChoosePhotoAsync();
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
        private async Task OnExcuteChoosePhotoAsync()
        {
            var filename = Path.GetRandomFileName() + ".png";
            if (await this.AllowTakePhotoAsync())
            {
                var _currentLocalImage = await CameraPluginExtensions.CancelableChoosePhotoAsync(filename);
                if (_currentLocalImage != null)
                {
                    _currentLocalImage = await ProcessSelectedImageAsync(_currentLocalImage);
                }
            }
        }

        private async Task<string> ProcessSelectedImageAsync(string _currentLocalImage)
        {
            IsBusy = true;
            using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
            {
                _currentLocalImage = await CameraPluginExtensions.TrimImageAsync(stream, _currentLocalImage);
            }

            if (!string.IsNullOrEmpty(_currentLocalImage) && await UploadImageAsync(_currentLocalImage))
            {
                using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
                using (BinaryReader br = new BinaryReader(stream))
                {
                    byte[] imageBytes = br.ReadBytes((int)stream.Length);
                    //itemSelected.ImagePath = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
            }
            else
            {
                _currentLocalImage = string.Empty;
            }

            IsBusy = false;
            return _currentLocalImage;
        }

        private async Task OnExcuteTakePhotoAsync()
        {
            IsBusy = true;
            if (await this.AllowTakePhotoAsync())
            {
                var filename = Path.GetRandomFileName() + ".png";
                var _currentLocalImage = await CameraPluginExtensions.CancelableTakePhotoAsync(filename);
                if (!string.IsNullOrEmpty(_currentLocalImage))
                {
                    using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
                    {
                        _currentLocalImage = await CameraPluginExtensions.TrimImageAsync(stream, _currentLocalImage);
                    }
                }

                if (!string.IsNullOrEmpty(_currentLocalImage) && await UploadImageAsync(_currentLocalImage))
                {
                    using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        byte[] imageBytes = br.ReadBytes((int)stream.Length);
                        //itemSelected.ImagePath = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    }
                }
                else
                {
                    //itemSelected.ImagePath = olderLocalImagePath;
                }
            }

            IsBusy = false;
        }

        private async Task<bool> UploadImageAsync(string imagePath)
        {
            //var isUpLoadImage = await AlertHelper.ShowConfirmationAlertAsync(new AlertConfigure
            //{
            //    Title = I18nHelper.Get("SetupPhotoPage_SubmitPicture_Title"),
            //    Message = I18nHelper.Get("SetupPhotoPage_SubmitPicture_Message")
            //});

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

        private void LoadSampleMessages()
        {
            Messages.Add(new MessageModel { Id = "1", SenderId = "1", ReceiverId = "2", Message = "Chào bạn!", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-10), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "2", SenderId = "2", ReceiverId = "1", Message = "Chào bạn! Bạn khỏe không?", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-9), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "3", SenderId = "1", ReceiverId = "2", Message = "Mình khỏe, bạn thì sao?", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-8), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "4", SenderId = "2", ReceiverId = "1", Message = "Mình cũng khỏe, cảm ơn!", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-7), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "5", SenderId = "1", ReceiverId = "2", Message = "Dạo này bạn làm gì?", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-6), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "6", SenderId = "2", ReceiverId = "1", Message = "Mình mới đi du lịch, đẹp lắm! 🌴", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-5), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "7", SenderId = "1", ReceiverId = "2", Message = "Wow, thật tuyệt!", Type = MessageType.Emoji, Timestamp = DateTime.Now.AddMinutes(-4), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "8", SenderId = "2", ReceiverId = "1", Message = "Mình có gửi hình cho bạn xem nè.", Type = MessageType.Image, Timestamp = DateTime.Now.AddMinutes(-3), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "9", SenderId = "1", ReceiverId = "2", Message = "Ảnh đẹp quá!", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-2), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "10", SenderId = "2", ReceiverId = "1", Message = "Cảm ơn bạn!", Type = MessageType.Emoji, Timestamp = DateTime.Now.AddMinutes(-1), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "1", SenderId = "1", ReceiverId = "2", Message = "Chào bạn!", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-10), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "2", SenderId = "2", ReceiverId = "1", Message = "Chào bạn! Bạn khỏe không?", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-9), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "3", SenderId = "1", ReceiverId = "2", Message = "Mình khỏe, bạn thì sao?", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-8), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "4", SenderId = "2", ReceiverId = "1", Message = "Mình cũng khỏe, cảm ơn!", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-7), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "5", SenderId = "1", ReceiverId = "2", Message = "Dạo này bạn làm gì?", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-6), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "6", SenderId = "2", ReceiverId = "1", Message = "Mình mới đi du lịch, đẹp lắm! 🌴", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-5), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "7", SenderId = "1", ReceiverId = "2", Message = "Wow, thật tuyệt!", Type = MessageType.Emoji, Timestamp = DateTime.Now.AddMinutes(-4), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "8", SenderId = "2", ReceiverId = "1", Message = "Mình có gửi hình cho bạn xem nè.", Type = MessageType.Image, Timestamp = DateTime.Now.AddMinutes(-3), IsMine = false, IsRead = true });
            Messages.Add(new MessageModel { Id = "9", SenderId = "1", ReceiverId = "2", Message = "Ảnh đẹp quá!", Type = MessageType.Text, Timestamp = DateTime.Now.AddMinutes(-2), IsMine = true, IsRead = true });
            Messages.Add(new MessageModel { Id = "10", SenderId = "2", ReceiverId = "1", Message = "Cảm ơn bạn!", Type = MessageType.Emoji, Timestamp = DateTime.Now.AddMinutes(-1), IsMine = false, IsRead = true });
        }
    }

    public class MessageChatItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MineMessageItemTemplate { get; set; } = new();
        public DataTemplate OtherMessageItemTemplate { get; set; } = new();
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var collectionView = container as CollectionView;

            //TODO: add empty template
            if (collectionView?.ItemsSource is not ObservableCollection<MessageModel> items || item is not MessageModel message)
                return MineMessageItemTemplate;

            return message.IsMine ? MineMessageItemTemplate : OtherMessageItemTemplate;
        }
    }

    public class MessageTextOrImageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessageItemTextTemplate { get; set; } = new();
        public DataTemplate MessageImageTextTemplate { get; set; } = new();
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var collectionView = container as CollectionView;

            ////TODO: add empty template
            //if (collectionView?.ItemsSource is not ObservableCollection<MessageModel> items || item is not MessageModel message)
            //    return MineMessageItemTemplate;

            //return message.IsMine ? MineMessageItemTemplate : OtherMessageItemTemplate;
            return MessageImageTextTemplate;
        }
    }
}