using CommunityToolkit.Mvvm.Input;
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
        private string _searchUserValue;
        [ObservableProperty]
        private string _errorValue;
        [ObservableProperty]
        ObservableCollection<UserChatModel> _userChats = new();
        [ObservableProperty]
        private ObservableCollection<UserProfileResponse> _users = new();
        [ObservableProperty]
        private bool _isVisibleFilterContainer;
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

        [RelayCommand]
        async Task OnSearchPressedAsync(object param)
        {
            if (SearchPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            IsVisibleFilterContainer = !IsVisibleFilterContainer;
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnSearchUserPressedAsync(object param)
        {
            if (SearchUserPressedCommand.IsRunning || IsBusy)
                return;
            try
            {
                IsBusy = true;
                SearchUserValue = SearchUserValue?.Trim() ?? string.Empty;
                if (!IsSeacrhValueValid(SearchUserValue))
                {
                    IsShowError = true;

                }
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                IsShowError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool IsSeacrhValueValid(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length >= 3;
        }

        public async Task InitDataAsync()
        {
            IsBusy = true;
            await Task.Delay(1);
            IsBusy = false;
        }

        public async override Task LoadDataAsync()
        {
            _currentPage = 0;
            await base.LoadDataAsync();
            ShouldLoadData = false;
            IsBusy = true;
            _currentUserId = !string.IsNullOrEmpty(_currentUserId) ? _currentUserId : UserSetting.Get(StorageKey.UserId);
            string queryParams = $"?PaginationRequest.PageNumber={_currentPage}&PaginationRequest.PageSize={PageSize}&PaginationRequest.UserId={_currentUserId}";
            var data1 = await _chatService.GetMatchedActiveUserAsync(EnvironmentsExtensions.ENDPOINT_GET__MESSAGE_MATCHED, queryParams);
            var data2 = await _chatService.GetUserMessageAsync(EnvironmentsExtensions.ENDPOINT_GET__USER_MESSAGES, queryParams);
            var temp = new List<UserProfileResponse>
            {
                new UserProfileResponse()
            };
            temp.AddRange([.. data1?.User?.Items ?? []]);
            Users = new ObservableCollection<UserProfileResponse>(temp);
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
                    await LoadDataAsync();
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }


        [RelayCommand]
        async Task OnUserChatItemClickedAsync(object user)
        {
            if (UserChatItemClickedCommand.IsRunning || user == null || IsBusy)
                return;
            UserChatModel param = user as UserChatModel ?? null;
            if (param == null)
            {
                var userProfile = user as UserProfileResponse;
                param = new UserChatModel
                {
                    UserId = userProfile?.Id ?? "",
                    MatchId = userProfile?.MatchId ?? "",
                    UserName = userProfile?.Username ?? "",
                };
            }
            IsBusy = true;
            await ServiceHelper.GetService<INavigationService>().PushToPageAsync<MessageChatPage>(param: param);
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