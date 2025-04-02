using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class SwipePageModel : BasePageModel
    {
        public bool IsNeedLoadUsersData = true;
        [ObservableProperty]
        private string _entryValue = string.Empty;
        [ObservableProperty]
        private string _errorTextValue = string.Empty;
        [ObservableProperty]
        private bool _isShowError;
        [ObservableProperty]
        private bool _isContinue;
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        private ObservableCollection<UserModel> _users = new();

        public SwipePageModel(INavigationService navigationService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
        }

        public override async Task InitAsync(object? initData)
        {
            await base.InitAsync(initData);
            InitUsers();
            IsNeedLoadUsersData = false;
        }

        public void InitUsers()
        {
            IsBusy = true;
            Users = new ObservableCollection<UserModel>
            {
                new UserModel
                {
                    Name = "John Doe 1",
                    Age = 25,
                    Status = "Single",
                    Image = "bbbb.jpeg",
                },
                new UserModel
                {
                    Name = "John Doe 2",
                    Age = 26,
                    Status = "Single",
                    Image = "lllll.jpeg",
                },
                new UserModel
                {
                    Name = "John Doe 3",
                    Age = 27,
                    Status = "Single",
                    Image ="mmm.jpeg",
                },
                new UserModel
                {
                    Name = "John Doe 3",
                    Age = 27,
                    Status = "Single",
                    Image ="nnn.jpeg",
                },
                new UserModel
                {
                    Name = "John Doe 3",
                    Age = 27,
                    Status = "Single",
                    Image ="image_user_1.jpeg",
                },
                new UserModel
                {
                    Name = "John Doe 3",
                    Age = 27,
                    Status = "Single",
                    Image ="image_user_2.jpeg",
                },
            };
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.PopPageAsync(isPopModal: true);
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnFilterPressedAsync(object param)
        {
            if (FilterPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            IsNeedLoadUsersData = false;
            await NavigationService.PushToPageAsync<FilterMapPage>();
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnOpenUserProfileAsync(object param)
        {
            if (OpenUserProfileCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PopPageAsync(isPopModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnDislikePressedAsync(object param)
        {
            if (DislikePressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnStarPressedAsync(object param)
        {
            if (StarPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnLikePressedAsync(object param)
        {
            if (LikePressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnOpenDetailProfileAsync(object param)
        {
            if (OpenDetailProfileCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            IsNeedLoadUsersData = false;
            await NavigationService.PushToPageAsync<DetailProfilePage>(isPushModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }
    }
}