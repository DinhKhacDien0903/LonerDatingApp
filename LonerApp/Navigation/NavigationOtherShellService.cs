namespace LonerApp.Navigation
{
    public class NavigationOtherShellService : INavigationOtherShellService
    {
            private readonly Func<Type, ContentPage> _pageResolver;
            private bool _isProcessing;

            public NavigationOtherShellService(Func<Type, ContentPage> pageResolver)
            {
                _pageResolver = pageResolver;
            }

            public async Task NavigateToAsync<TPage>(object? param = null, bool isPushModal = false, bool isAnimation = true) where TPage : Page
            {
                if (_isProcessing)
                    return;

                _isProcessing = true;

                try
                {
                    var page = _pageResolver(typeof(TPage));
                    if (page == null)
                        throw new Exception($"Page of type {typeof(TPage).Name} not found!");

                    if (page.BindingContext is not BasePageModel toPageModel)
                        throw new Exception($"PageModel for {typeof(TPage).Name} not found!");

                    Shell.SetBackButtonBehavior(page, new BackButtonBehavior { Command = toPageModel.BackButtonCommand });
                    toPageModel.IsPushPageWithNavService = true;

                    // Khởi tạo ViewModel với tham số
                    await toPageModel.InitAsync(param);

                    page.Appearing += OnAppearingAsync;
                    page.Disappearing += OnDisappearingAsync;

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var currentPage = Application.Current?.MainPage;
                        if (currentPage is NavigationPage navPage)
                        {
                            if (isPushModal)
                            {
                                await navPage.Navigation.PushModalAsync(page, isAnimation);
                                ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor("#ffffff");
                            }
                            else
                            {
                                await navPage.Navigation.PushAsync(page, isAnimation);
                            }
                        }
                        else
                        {
                            Application.Current.MainPage = new NavigationPage(page);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Navigation error: {ex}");
                    throw;
                }
                finally
                {
                    _isProcessing = false;
                }
            }

            public async Task GoBackAsync()
            {
                if (_isProcessing)
                    return;

                _isProcessing = true;

                try
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var currentPage = Application.Current?.MainPage;
                        if (currentPage is NavigationPage navPage && navPage.Navigation.NavigationStack.Count > 1)
                        {
                            await navPage.PopAsync();
                        }
                        else if (currentPage is NavigationPage modalNavPage && modalNavPage.Navigation.ModalStack.Count > 0)
                        {
                            await modalNavPage.Navigation.PopModalAsync();
                        }
                    });
                }
                finally
                {
                    _isProcessing = false;
                }
            }

            public async Task ClearNavigationStackAndNavigateToAsync<TPage>(object? param = null) where TPage : Page
            {
                if (_isProcessing)
                    return;

                _isProcessing = true;

                try
                {
                    var page = _pageResolver(typeof(TPage));
                    if (page == null)
                        throw new Exception($"Page of type {typeof(TPage).Name} not found!");

                    if (page.BindingContext is not BasePageModel toPageModel)
                        throw new Exception($"PageModel for {typeof(TPage).Name} not found!");

                    Shell.SetBackButtonBehavior(page, new BackButtonBehavior { Command = toPageModel.BackButtonCommand });
                    toPageModel.IsPushPageWithNavService = true;
                    await toPageModel.InitAsync(param);

                    page.Appearing += OnAppearingAsync;
                    page.Disappearing += OnDisappearingAsync;

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(page);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Navigation error: {ex}");
                    throw;
                }
                finally
                {
                    _isProcessing = false;
                }
            }

        private async void OnDisappearingAsync(object sender, EventArgs e)
        {
            if (sender is not ContentPage thisPage)
                return;

            var navPage = Application.Current?.MainPage as NavigationPage;
            bool isForwardNavigation = navPage != null && navPage.Navigation.NavigationStack.Count > 1 && navPage.Navigation.NavigationStack[^2] == thisPage;

            if (thisPage is ContentPage)
            {
                if (!isForwardNavigation)
                {
                    var toPageModel = thisPage.BindingContext as BasePageModel;
                    if (toPageModel != null && !toPageModel.LoadDataOnAppearing)
                    {
                        thisPage.Appearing -= OnAppearingAsync;
                    }
                }

                // Gọi CallNavigatedFromAsync
                await CallNavigatedFromAsync(thisPage);
            }
        }

        private Task CallNavigatedFromAsync(ContentPage thisPage)
        {
            var fromPageModel = NaviMethodExtension.GetBasePageModel(thisPage);
            var navPage = Application.Current?.MainPage as NavigationPage;
            if (fromPageModel is not null)
            {
                if (navPage == null || !navPage.Navigation.NavigationStack.Contains(thisPage))
                {
                    thisPage.NavigatedFrom -= OnDisappearingAsync;
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

    }
}
