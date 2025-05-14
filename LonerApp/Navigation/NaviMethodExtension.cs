namespace LonerApp.Navigation
{
    public static class NaviMethodExtension
    {
        private static TaskCompletionSource<bool> _navigateCompletionSource;
        private static readonly object _syncLock = new object();
        private static HashSet<string> _navigationStack = new HashSet<string>();

        public static T ResolvePage<T>()
            where T : ContentPage
        {
            try
            {
                return ServiceHelper.GetService<IServiceProvider>().GetService<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
        }

        public static BasePageModel GetBasePageModel(ContentPage page)
        {
            return page.BindingContext as BasePageModel ?? throw new Exception("PageModel is not exist!");
        }

        public static async Task<bool> PushToNavigationStackAsync(string page)
        {
            if (_navigateCompletionSource != null && !_navigateCompletionSource.Task.IsCompleted)
                await _navigateCompletionSource.Task;

            _navigateCompletionSource = new TaskCompletionSource<bool>();

            if (_navigationStack.Any() && _navigationStack.Contains(page))
            {
                _navigateCompletionSource.TrySetResult(false);
                await _navigateCompletionSource.Task;
            }

            lock (_syncLock)
            {
                _navigationStack.Add(page);
            }

            _navigateCompletionSource.TrySetResult(true);
            return await _navigateCompletionSource.Task;
        }

        public static async Task<bool> PopToNavigationStackAsync(string page)
        {
            if (_navigateCompletionSource != null && !_navigateCompletionSource.Task.IsCompleted)
            {
                await _navigateCompletionSource.Task;
            }

            _navigateCompletionSource = new TaskCompletionSource<bool>();
            if (_navigationStack.Count == 0 && !_navigationStack.Contains(page))
            {
                _navigateCompletionSource.TrySetResult(false);
                return await _navigateCompletionSource.Task;
            }

            lock (_syncLock)
            {
                _navigationStack.Remove(page);
            }

            _navigateCompletionSource.TrySetResult(true);
            return await _navigateCompletionSource.Task;
        }
    }
}