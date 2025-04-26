using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Chat.Services;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class ChatPageModel : BasePageModel
    {
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
        ObservableCollection<UserChatModel> _userChats = new();
        [ObservableProperty]
        private ObservableCollection<UserProfileResponse> _users = new();
        private readonly IChatService _chatService;
        private int _currentPage = 1;
        private int countUser = 0;
        private const int PageSize = 30;
        private static string _currentUserId = string.Empty;

        public ChatPageModel(
            INavigationService navigationService,
            IChatService chatService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            _chatService = chatService;
        }

        public override async Task InitAsync(object? initData)
        {
            await base.InitAsync(initData);
            //await InitDataAsync();
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
            // UserChats = new ObservableCollection<UserChatModel>
            // {
            //     new UserChatModel { UserId = 1, UserName = "Alice", ProfilePicture = "bbbb.jpeg", LastMessage = "Hello!", IsUnRead = true },
            //     new UserChatModel { UserId = 2, UserName = "Bob", ProfilePicture = "lllll.jpeg", LastMessage = "How are you?", IsUnRead = false },
            //     new UserChatModel { UserId = 3, UserName = "Charlie", ProfilePicture = "mmm.jpeg", LastMessage = "See you later!", IsUnRead = true },
            //     new UserChatModel { UserId = 4, UserName = "David", ProfilePicture = "nnn.jpeg", LastMessage = "Good night!", IsUnRead = false },
            //     new UserChatModel { UserId = 5, UserName = "Eve", ProfilePicture = "image_user_1.jpeg", LastMessage = "What's up?", IsUnRead = true },
            //     new UserChatModel { UserId = 6, UserName = "Frank", ProfilePicture = "image_user_2.jpeg", LastMessage = "Talk soon!", IsUnRead = false },
            //     new UserChatModel { UserId = 1, UserName = "Alice", ProfilePicture = "bbbb.jpeg", LastMessage = "Hello!", IsUnRead = true },
            //     new UserChatModel { UserId = 2, UserName = "Bob", ProfilePicture = "lllll.jpeg", LastMessage = "How are you?", IsUnRead = false },
            //     new UserChatModel { UserId = 3, UserName = "Charlie", ProfilePicture = "mmm.jpeg", LastMessage = "See you later!", IsUnRead = true },
            //     new UserChatModel { UserId = 4, UserName = "David", ProfilePicture = "nnn.jpeg", LastMessage = "Good night!", IsUnRead = false },
            //     new UserChatModel { UserId = 5, UserName = "Eve", ProfilePicture = "image_user_1.jpeg", LastMessage = "What's up?", IsUnRead = true },
            //     new UserChatModel { UserId = 6, UserName = "Frank", ProfilePicture = "image_user_2.jpeg", LastMessage = "Talk soon!", IsUnRead = false },
            //     new UserChatModel { UserId = 1, UserName = "Alice", ProfilePicture = "bbbb.jpeg", LastMessage = "Hello!", IsUnRead = true }
            // };
            IsBusy = false;
        }

        public async override Task LoadDataAsync()
        {
            _currentPage = 0;
            // https://localhost:7165/api/Message/get-user-matched-active?PaginationRequest.ValidPageSize=30&PaginationRequest.UserId=14f06ee5-c275-470a-b2cb-3c5aa4c3a6de
            await base.LoadDataAsync();
            ShouldLoadData = false;
            IsBusy = true;
            _currentUserId = !string.IsNullOrEmpty(_currentUserId) ? _currentUserId : UserSetting.Get(StorageKey.UserId);
            string queryParams = $"?PaginationRequest.PageNumber={_currentPage}&PaginationRequest.PageSize={PageSize}&PaginationRequest.UserId={_currentUserId}";
            var data1 = await _chatService.GetMatchedActiveUserAsync(EnvironmentsExtensions.ENDPOINT_GET__MESSAGE_MATCHED, queryParams);
            var data2 = await _chatService.GetUserMessageAsync(EnvironmentsExtensions.ENDPOINT_GET__USER_MESSAGES, queryParams);
            Users = [.. data1?.User?.Items ?? []];
            UserChats = [.. data2?.UserMessages?.Items ?? []];
            _currentPage++;
            IsBusy = false;
            ShouldLoadData = true;
            // IsNeedLoadUsersData = false;
        }

        [RelayCommand]
        void ReloadList(object obj)
        {
            IsRefreshing = false;
            RefreshCommand.Execute(null);
        }

        [RelayCommand]
        async Task OnRefreshAsync(object obj)
        {
            if (RefreshCommand.IsRunning || IsBusy)
                return;
            if (ShouldLoadData && Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                IsBusy = true;
                try
                {
                    //await DataService.UpdateMasterAsync();
                    await LoadDataAsync();
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        async Task OnUserChatItemClickedAsync(UserChatModel user)
        {
            if (UserChatItemClickedCommand.IsRunning || user == null || IsBusy)
                return;
            IsBusy = true;
            await ServiceHelper.GetService<INavigationService>().PushToPageAsync<MessageChatPage>(param: user);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnGetSwipePressedAsync(object obj)
        {
            if (GetSwipePressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await ServiceHelper.GetService<INavigationService>().PushToPageAsync<MainSwipePage>();
            await Task.Delay(100);
            IsBusy = false;
        }
    }

    public class UserChatDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GetSwipeItemTemplate { get; set; } = new();
        public DataTemplate UserChatItemTemplate { get; set; } = new();
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var collectionView = container as CollectionView;

            if (collectionView?.ItemsSource is not ObservableCollection<UserProfileResponse> items || item is not UserProfileResponse user)
                return UserChatItemTemplate;

            return items.IndexOf(user) == 0 ? GetSwipeItemTemplate : UserChatItemTemplate;
        }
    }
}