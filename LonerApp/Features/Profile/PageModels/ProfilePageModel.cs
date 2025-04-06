using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class ProfilePageModel : BasePageModel
    {
        [ObservableProperty]
        ObservableCollection<ImageSource> _images = new();
        [ObservableProperty]
        ObservableCollection<string> _interests = new();
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
        private int _selectedIndex;
        [ObservableProperty]
        private ObservableCollection<UserModel> _users = new();
        [ObservableProperty]
        private bool _isCurrentOtherUser = true;
        [ObservableProperty]
        private UserModel _myProfile;
        ContentPage? _previousPage;
        SwipePageModel? _swipePageModel;
        public ProfilePageModel(INavigationService navigationService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
        }

        public override async Task InitAsync(object? initData)
        {
            //TODO: Handle when user click on profile
            _previousPage = AppShell.Current.CurrentPage as ContentPage;
            if (_previousPage != null)
                _swipePageModel = _previousPage.BindingContext as SwipePageModel;
            IsCurrentOtherUser = _previousPage is MainSwipePage;
            if (initData is UserModel user)
            {
                MyProfile = user;
                await InitImages();
            }
            await base.InitAsync(initData);
        }

        public async Task InitImages()
        {
            IsBusy = true;
            await Task.Delay(1);
            var x = new ObservableCollection<ImageSource>(MyProfile.ListImage.Where(x => !x.IsDefaultImage).Select(x => x.ImagePath));
            foreach(var item in x)
            {
                if(item is ImageSource k)
                {
                    Images.Add(k);
                }
            }
            await Task.Delay(100);
            //Images = new ObservableCollection<string>
            //{
            //    "bbbb.jpeg",
            //    "lllll.jpeg",
            //    "mmm.jpeg",
            //    "nnn.jpeg",
            //    "image_user_1.jpeg",
            //    "image_user_2.jpeg"
            //};
            SelectedIndex = 0;
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (BackCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PopPageAsync(isPopModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnCloseDetailProfileAsync(object param)
        {
            if (CloseDetailProfileCommand.IsRunning && IsBusy)
                return;

            IsBusy = true;
            await NavigationService.PopPageAsync(isPopModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnDislikePressedAsync(object param)
        {
            //TODO: Add adnimation when dislike pressed
            if (DislikePressedCommand.IsRunning  || IsBusy)
                return;
            if(_swipePageModel != null)
            {
                _swipePageModel.DislikePressedCommand.Execute(null);
            }

            await OnCloseDetailProfileAsync(null);
        }

        [RelayCommand]
        async Task OnStarPressedAsync(object param)
        {
            //TODO: Add adnimation when superlike pressed
            if (StarPressedCommand.IsRunning  || IsBusy)
                return;
            if(_swipePageModel != null)
            {
                _swipePageModel.StarPressedCommand.Execute(null);
            }

            await OnCloseDetailProfileAsync(null);
        }

        [RelayCommand]
        async Task OnLikePressedAsync(object param)
        {
            //TODO: Add adnimation when like pressed
            if (LikePressedCommand.IsRunning  || IsBusy)
                return;
            if(_swipePageModel != null)
            {
                _swipePageModel.LikePressedCommand.Execute(null);
            }

            await OnCloseDetailProfileAsync(null);
        }
    }
}