using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace LonerApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        protected async override void OnNavigated(ShellNavigatedEventArgs args)
        {
            // ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor("#FF7854", "#FE3675");
        }

        public async Task RemoveRootAsync()
        {
            List<Task> tasks = new();
            foreach (var item in this.CurrentItem.Items)
            {
                var pageModel = (item.CurrentItem as IShellContentController)?.Page?.BindingContext as BasePageModel;

                if (pageModel != null)
                {
                    var task = pageModel?.ViewIsRemovedAsync();
                    if (task != null)
                        tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks);
        }

        public Page? GetPreviousPage()
        {
            var navigation = Shell.Current?.Navigation;
            if (navigation != null && navigation.NavigationStack.Count > 1)
            {
                var previousPage = navigation.NavigationStack[navigation.NavigationStack.Count - 2];
                return previousPage;
            }

            return null;
        }
    }
}