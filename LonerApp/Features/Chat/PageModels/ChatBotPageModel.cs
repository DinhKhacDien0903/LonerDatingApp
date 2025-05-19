using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class ChatBotPageModel : BasePageModel
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
        public int _currentPage = 1;
        private const int PageSize = 30;
        private UserChatModel _partner;
        [ObservableProperty]
        private string _partnerName;
        [ObservableProperty]
        private bool _isVisibleOverlay;
        [ObservableProperty]
        private bool _isVisibleOption;
        [ObservableProperty]
        private bool _isBlocked;
        private readonly IReportService _reportService;
        public bool IsNeedLoadUsersData = true;
        private CancellationTokenSource cancellationToastToken = new CancellationTokenSource();
        public ChatBotPageModel(
            INavigationService navigationService,
            IChatService chatService,
            IReportService reportService)
            : base(navigationService, true)
        {
            _reportService = reportService;
            _chatService = chatService;
            IsVisibleNavigation = true;
            HasBackButton = true;

        }

        public override async Task InitAsync(object? initData)
        {
            if (initData is UserChatModel user)
            {
                _partner = user;
                PartnerName = user.UserName;
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
        [RelayCommand]
        async Task OnOptionButtonAsync(object param)
        {
            if (OptionButtonCommand.IsRunning || IsBusy)
                return;
            try
            {
                IsBusy = true;
                IsVisibleOverlay = true;
                IsVisibleOption = true;
                await Task.Delay(1);
            }
            finally
            {
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
            try
            {
                IsBusy = true;
                await base.LoadDataAsync();
                _currentUserId = !string.IsNullOrEmpty(_currentUserId) ? _currentUserId : UserSetting.Get(StorageKey.UserId);
                string queryParams = $"?PaginationRequest.PageNumber={_currentPage}&PaginationRequest.PageSize={PageSize}&PaginationRequest.UserId={_currentUserId}&PaginationRequest.MatchId={_partner.MatchId}";
                var data1 = await _chatService.GetMessagesAsync(EnvironmentsExtensions.ENDPOINT_GET_MESSAGES, queryParams);
                Messages = [.. data1?.Messages?.Items ?? []];
                _currentPage++;
                var request = new CheckBlockedRequest
                {
                    BlockerId = UserSetting.Get(StorageKey.UserId) ?? "",
                    BlockedId = _partner?.UserId ?? "",
                    TypeBlocked = 1,
                };
                IsBlocked = !(await _reportService.CheckBlockedAsync(request))?.IsUnChatBlocked ?? true;
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }
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
                    IsRead = false,
                    IsMessageOfChatBot = true
                };

                Messages.Add(message);
                MessageEntryValue = string.Empty;
                var result = await _chatService.SendMessagesAsync(new SendMessageRequest { MessageRequest = message });
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

        public void GetChatListCollectionView()
        {
            _chatList ??= Shell.Current.CurrentPage.FindByName<CollectionView>("ChatMessageList");
        }
    }

    public class MessageChatBotItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MineMessageTextItemTemplate { get; set; } = new();
        public DataTemplate OtherMessageTextItemTemplate { get; set; } = new();
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var collectionView = container as CollectionView;

            if (collectionView?.ItemsSource is not ObservableCollection<MessageModel> items || item is not MessageModel message)
                return MineMessageTextItemTemplate;

            return message.IsCurrentUserSend ? MineMessageTextItemTemplate : OtherMessageTextItemTemplate;
        }
    }
}