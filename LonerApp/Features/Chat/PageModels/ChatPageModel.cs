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
        ObservableCollection<UserChatModel> _userChats = new();
        [ObservableProperty]
        private ObservableCollection<UserModel> _users = new();

        public ChatPageModel(INavigationService navigationService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
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
            UserChats = new ObservableCollection<UserChatModel>
            {
                new UserChatModel { UserId = 1, UserName = "Alice", ProfilePicture = "bbbb.jpeg", LastMessage = "Hello!", IsUnRead = true },
                new UserChatModel { UserId = 2, UserName = "Bob", ProfilePicture = "lllll.jpeg", LastMessage = "How are you?", IsUnRead = false },
                new UserChatModel { UserId = 3, UserName = "Charlie", ProfilePicture = "mmm.jpeg", LastMessage = "See you later!", IsUnRead = true },
                new UserChatModel { UserId = 4, UserName = "David", ProfilePicture = "nnn.jpeg", LastMessage = "Good night!", IsUnRead = false },
                new UserChatModel { UserId = 5, UserName = "Eve", ProfilePicture = "image_user_1.jpeg", LastMessage = "What's up?", IsUnRead = true },
                new UserChatModel { UserId = 6, UserName = "Frank", ProfilePicture = "image_user_2.jpeg", LastMessage = "Talk soon!", IsUnRead = false },
                new UserChatModel { UserId = 1, UserName = "Alice", ProfilePicture = "bbbb.jpeg", LastMessage = "Hello!", IsUnRead = true },
                new UserChatModel { UserId = 2, UserName = "Bob", ProfilePicture = "lllll.jpeg", LastMessage = "How are you?", IsUnRead = false },
                new UserChatModel { UserId = 3, UserName = "Charlie", ProfilePicture = "mmm.jpeg", LastMessage = "See you later!", IsUnRead = true },
                new UserChatModel { UserId = 4, UserName = "David", ProfilePicture = "nnn.jpeg", LastMessage = "Good night!", IsUnRead = false },
                new UserChatModel { UserId = 5, UserName = "Eve", ProfilePicture = "image_user_1.jpeg", LastMessage = "What's up?", IsUnRead = true },
                new UserChatModel { UserId = 6, UserName = "Frank", ProfilePicture = "image_user_2.jpeg", LastMessage = "Talk soon!", IsUnRead = false },
                new UserChatModel { UserId = 1, UserName = "Alice", ProfilePicture = "bbbb.jpeg", LastMessage = "Hello!", IsUnRead = true }
            };
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
            await ServiceHelper.GetService<INavigationService>().PushToPageAsync<MessageChatPage>();
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

            if (collectionView?.ItemsSource is not ObservableCollection<UserChatModel> items || item is not UserChatModel user)
                return UserChatItemTemplate;

            return items.IndexOf(user) == 0 ? GetSwipeItemTemplate : UserChatItemTemplate;
        }
    }
}