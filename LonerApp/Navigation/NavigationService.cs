
namespace LonerApp.Navigation
{
    public class NavigationService : INavigationService
    {
        public bool IsProcessing;
        private INavigation Navigation
        {
            get
            {
                INavigation? navigation = Shell.Current?.Navigation;
                if (navigation is not null)
                    return navigation;

                throw new Exception("No exist navigation!");
            }
        }
        public int GetNumberStack()
            => Navigation.NavigationStack.Count;

        public async Task PopPageAsync(bool isPopModal = false, bool isAnimation = true)
        {
            try
            {
                if (Navigation.NavigationStack != null && Navigation.NavigationStack.Count > 1 && !isPopModal)
                {
                    await Shell.Current.GoToAsync("..", animate: isAnimation);
                }
                else if (Navigation.NavigationStack != null && Navigation.NavigationStack.Count > 0 && isPopModal)
                {
                    await Navigation.PopModalAsync(animated: isAnimation);
                }
                else
                {
                    Console.WriteLine("No exist page in stack!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
        }

        public async Task PopToPageInStackAsync<T>(Type targetPageType, bool isPopModal = false, bool isAnimation = true)
        {
            throw new NotImplementedException();
        }

        public Task PopToRootAsync(bool isAnimation = true)
        {
            throw new NotImplementedException();
        }

        public async Task PushToPageAsync<T>(object? param = null, bool isPushModal = false, bool isAnimation = true)
            where T : ContentPage
        {
            if (IsProcessing)
                return;
            IsProcessing = true;
            var page = NaviMethodExtension.ResolvePage<T>();
            if (page is null)
                throw new Exception("Page not found!");

            var toPageModel = NaviMethodExtension.GetBasePageModel(page);
            if (toPageModel is null)
                throw new Exception("PageModel not found!");

            Shell.SetBackButtonBehavior(page, new BackButtonBehavior { Command = toPageModel.BackButtonCommand });
            toPageModel.IsPushPageWithNavService = true;
            await toPageModel.InitAsync(param);
            page.Appearing += OnAppearingAsync;
            page.Disappearing += OnDisAppearingAsync;
            if (isPushModal)
            {
                await Navigation.PushModalAsync(page, animated: isAnimation);
                ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor("#ffffff");
            }
            else
            {
                await Navigation.PushAsync(page, animated: isAnimation);
            }

            IsProcessing = false;
        }

        private async void OnDisAppearingAsync(object? sender, EventArgs e)
        {
            var isForwardNavigation = Navigation.NavigationStack.Count > 1 && Navigation.NavigationStack[^2] == sender;
            if (sender is ContentPage thisPage)
            {
                if (!isForwardNavigation)
                {
                    var toPageModel = NaviMethodExtension.GetBasePageModel(thisPage);
                    if (!toPageModel.LoadDataOnAppearing)
                    {
                        thisPage.Appearing -= OnAppearingAsync;
                    }
                }

                //TODO: Implement this method
                await CallNavigatedFromAsync(thisPage);
            }
        }

        private Task CallNavigatedFromAsync(ContentPage thisPage)
        {
            var fromPageModel = NaviMethodExtension.GetBasePageModel(thisPage);
            if (fromPageModel is not null)
            {
                if (Navigation == null || !Navigation.NavigationStack.Contains(thisPage))
                {
                    thisPage.NavigatedFrom -= OnDisAppearingAsync;
                }

                return fromPageModel.ViewIsDisAppearingAsync();
            }

            return Task.CompletedTask;
        }

        private async void OnAppearingAsync(object sender, EventArgs e)
        {
            if (sender is ContentPage contentPage)
            {
                var toPageModel = NaviMethodExtension.GetBasePageModel(contentPage);
                await toPageModel.ViewIsAppearingAsync();
            }
        }

        public Task PushToWebViewAsync<T>(object param) where T : ContentPage
        {
            throw new NotImplementedException();
        }
    }
}