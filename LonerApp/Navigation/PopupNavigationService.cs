using Mopups.Interfaces;
using Mopups.Pages;
using Mopups.Services;

namespace LonerApp.Navigation
{
    public class PopupNavigationService : IPopupNavigationService
    {
        protected IPopupNavigation PopupNavigation
        {
            get
            {
                IPopupNavigation popupNavigation = MopupService.Instance;
                if (popupNavigation is not null)
                    return popupNavigation;
                else
                    throw new Exception();
            }
        }

        public async Task PopAsync(bool animate = true)
        {
            try
            {
                if (PopupNavigation.PopupStack.Count > 0)
                {
                    await PopupNavigation.PopAsync(animate);
                }
                else
                {
                    this.Log("No Popup Pages to navigate back to!");
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task PopAllAsync(bool animate = true)
        {
            try
            {
                if (PopupNavigation.PopupStack.Count > 0)
                {
                    await PopupNavigation.PopAllAsync(animate);
                }
                else
                {
                    this.Log("No Popup Pages to navigate back to!");
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task PushAsync<T>(object param = null, bool animate = true)
            where T : PopupPage
        {
            T currentPage = AppShell.Current?.CurrentPage as T;
            var currentViewModel = App.CurrentPageModel;
            if (currentPage == null)
                currentPage = PopupNavigation.PopupStack.LastOrDefault() as T;

            currentViewModel.IsBusy = true;
            var page = NaviMethodExtension.ResolvePage<T>();
            if (page is not null)
            {
                page.Appearing += OnAppearing;
                var toPageModel = NaviMethodExtension.GetBasePageModel(page);
                if (toPageModel is not null)
                {
                    toPageModel.IsBusy = true;
                    await toPageModel.InitAsync(param);
                    await PopupNavigation.PushAsync(page, animate: animate);
                    page.Disappearing += OnNavigatedFromAsync;
                    toPageModel.IsBusy = false;
                }
            }
            else
            {
                this.Log($"Unable to resolve type {typeof(T).FullName}");
                currentViewModel.IsBusy = false;
            }

            currentViewModel.IsBusy = false;
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            var currentViewModel = NaviMethodExtension.GetBasePageModel(sender as ContentPage);
            currentViewModel.IsBusy = true;
            _ = currentViewModel.ViewIsAppearingAsync();
            currentViewModel.IsBusy = false;
        }

        private async void OnNavigatedFromAsync(object sender, EventArgs e)
        {
            bool isForwardNavigation = PopupNavigation.PopupStack.Count > 1
               && PopupNavigation.PopupStack[^2] == sender;

            if (sender is PopupPage thisPage)
            {
                if (!isForwardNavigation)
                {
                    thisPage.NavigatedFrom -= OnNavigatedFromAsync;
                }

                await CallNavigatedFromAsync(thisPage);
            }
        }

        private Task CallNavigatedFromAsync(PopupPage thisPage)
        {
            var fromPageModel = NaviMethodExtension.GetBasePageModel(thisPage);

            if (fromPageModel is not null)
            {
                if (PopupNavigation == null || !PopupNavigation.PopupStack.Contains(thisPage))
                    return fromPageModel.ViewIsRemovedAsync();
                return fromPageModel.ViewIsDisAppearingAsync();
            }

            return Task.CompletedTask;
        }
    }
}