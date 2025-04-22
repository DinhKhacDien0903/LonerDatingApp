namespace LonerApp.Navigation
{
    public interface INavigationOtherShellService
    {
        Task NavigateToAsync<TPage>(object? param = null, bool isPushModal = false, bool isAnimation = true) where TPage : Page;
        Task GoBackAsync();
    }
}
