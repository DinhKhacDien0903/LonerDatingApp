using CommunityToolkit.Mvvm.Input;

namespace LonerApp.PageModels
{
    public partial class ChatMessageImagePageModel : BasePageModel
    {
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        private byte[]? _imageData;

        public ChatMessageImagePageModel(INavigationService navigationService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            HasBackButton = true;
        }

        public override async Task InitAsync(object? initData)
        {
            await base.InitAsync(initData);
            if (initData is byte[] data)
            {
                ImageData = data;
            }
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.PopPageAsync();
                await Task.Delay(100);
                IsBusy = false;
            }
        }
    }
}