using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Mopups.Services;
namespace LonerApp.Navigation;

public class NavigationCommunityPopupService : INavigationCommunityPopupService
{
    protected Stack<Popup> PopupStack = new Stack<Popup>();
    private bool _isProcessing;
    readonly IServiceProvider _serviceProvider;
    static Page CurrentPage =>
        Shell.Current?.CurrentPage ?? throw new NullReferenceException("CurrentPage is null.");

    static readonly IDictionary<Type, Type> viewModelToViewMappings = new Dictionary<Type, Type>();

    internal static void AddTransientPopup<TPopupView, TPopupViewModel>(IServiceCollection services)
    where TPopupView : Popup
    where TPopupViewModel : BasePageModel
    {
        viewModelToViewMappings.Add(typeof(TPopupViewModel), typeof(TPopupView));

        services.AddTransient(typeof(TPopupView));
        services.AddTransient(typeof(TPopupViewModel));
    }

    public NavigationCommunityPopupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<object> PushAsync<TViewModel>(object param = null)
        where TViewModel : BasePageModel
    {
        var currentPage = AppShell.Current.CurrentPage;
        var currentViewModel = App.CurrentPageModel;
        if (currentPage == null)
            currentPage = MopupService.Instance.PopupStack.LastOrDefault();

        currentViewModel.IsBusy = true;
#if ANDROID
        var tapGestureRecognizer = new TapGestureRecognizer();
        // TODO:  https://github.com/dotnet/maui/issues/10252
        (currentPage as ContentPage)?.Content.GestureRecognizers.Add(tapGestureRecognizer);
#endif
        var popup = GetPopup(typeof(TViewModel));
        if (popup != null && ServiceHelper.GetService<INavigationCommunityPopupService>().GetPopupStack()?.LastOrDefault()?.ToString() != popup?.ToString())
        {
            var resultTask = await NaviMethodExtension.PushToNavigationStackAsync(popup.ToString());
            if (resultTask)
            {
                popup.Opened += OnPopupOpened;
                var toViewModel = popup?.BindingContext as BasePageModel;
                toViewModel.IsBusy = true;
                await toViewModel.InitAsync(param);
                toViewModel.IsBusy = false;
                popup.Closed += OnPopupClosedAsync;
                PopupStack.Push(popup);
                try
                {
                    var result = CurrentPage.ShowPopupAsync(popup);
                    currentViewModel.IsBusy = false;
#if ANDROID
                    // TODO:  https://github.com/dotnet/maui/issues/10252
                    (currentPage as ContentPage)?.Content.GestureRecognizers.Remove(tapGestureRecognizer);
#endif
                    return result;
                }
                catch (Exception)
                {
                    currentViewModel.IsBusy = false;
                }
            }
        }

        currentViewModel.IsBusy = false;
#if ANDROID
        // TODO:  https://github.com/dotnet/maui/issues/10252
        (currentPage as ContentPage)?.Content.GestureRecognizers.Remove(tapGestureRecognizer);
#endif
        return null;
    }

    private async void OnPopupClosedAsync(object sender, PopupClosedEventArgs e)
    {
        if (sender is Popup thisPopup)
        {
            thisPopup.Closed -= OnPopupClosedAsync;
            thisPopup.Opened -= OnPopupOpened;
            await CallNavigatedFromAsync(thisPopup);
        }
    }

    private async Task<Task> CallNavigatedFromAsync(Popup thisPopup)
    {
        var currentViewModel = thisPopup?.BindingContext as BasePageModel;
        if (currentViewModel != null)
        {
            var result = await NaviMethodExtension.PopToNavigationStackAsync(thisPopup.ToString());
            if (!result)
                return Task.CompletedTask;
            if (PopupStack == null || !PopupStack.Contains(thisPopup))
                return currentViewModel.ViewIsRemovedAsync();
            return currentViewModel.ViewIsDisAppearingAsync();
        }

        return Task.CompletedTask;
    }

    private void OnPopupOpened(object sender, PopupOpenedEventArgs e)
    {
        var currentViewModel = (sender as Popup)?.BindingContext as BasePageModel;
        if (currentViewModel != null)
        {
            currentViewModel.IsBusy = true;
            _ = currentViewModel.ViewIsAppearingAsync();
            currentViewModel.IsBusy = false;
        }
    }

    Popup GetPopup(Type viewModelType)
    {
        var popup = this._serviceProvider.GetService(viewModelToViewMappings[viewModelType]) as Popup;

#if DEBUG
        if (popup is null)
        {
            throw new InvalidOperationException(
                $"Register DI call 'services.AddTransientPopup<TView, TViewModel>();'.");
        }
#endif
        return popup;
    }

    public void PopPopup()
    {
        if (_isProcessing)
            return;

        try
        {
            _isProcessing = true;
            if (PopupStack.Count > 0)
            {
                var lastPopup = PopupStack.Pop();
                lastPopup.Close();
            }

            _isProcessing = false;
        }
        catch (Exception)
        {
            _isProcessing = false;
        }
    }

    public Stack<Popup> GetPopupStack() => PopupStack;
}