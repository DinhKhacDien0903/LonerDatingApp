using CommunityToolkit.Maui.Views;
using System.Reflection;

namespace LonerApp.Helpers.Extensions
{
    public static class BindableObjectExtensions
    {
        public static void InvokeViewAndViewModelAction<T>(this BindableObject view, Action<T> action)
           where T : class
        {
            if (view is T viewAsT)
            {
                action(viewAsT);
            }

            if (view is BindableObject element && element.BindingContext is T viewModelAsT)
            {
                action(viewModelAsT);
            }
        }

        public static async Task InvokeViewAndViewModelActionAsync<T>(this BindableObject view, Func<T, Task> action)
            where T : class
        {
            if (view is T viewAsT)
            {
                await action(viewAsT);
            }

            if (view is BindableObject element && element.BindingContext is T viewModelAsT)
            {
                await action(viewModelAsT);
            }
        }

        public static Page GetCurrentPage(this Page mainPage) =>
           _getCurrentPage(mainPage);

        private static Func<Page, Page> _getCurrentPage = mainPage =>
        {
            var page = mainPage;
            Page child = null;

            var lastModal = page.Navigation.ModalStack.LastOrDefault();
            if (lastModal != null)
                page = lastModal;

            if (page is AppShell appShell)
                child = appShell.CurrentPage;
            else
                child = page;
            return child;
        };

        public static TViewModel GetViewModel<TViewModel>(this BindableObject element)
            where TViewModel : class
        {
            return element.BindingContext as TViewModel;
        }

        public static Page GetPage(this BindableObject binableObj)
        {
            if (binableObj is Page page)
                return page;

            if (binableObj is Element element)
                return GetPage(element.Parent);

            return null;
        }

        public static Popup GetRootPopup(this Element element)
        {
            try
            {
                if (element is Popup popup)
                    return popup;
                return GetRootPopup(element.Parent);
            }
            catch
            {
            }

            return null;
        }

        public static bool IsAlive(this Element element)
        {
            if (element == null)
                return false;
            if (element == App.Current)
                return true;
            return IsAlive(element.Parent);
        }

        public static BasePageModel GetPageViewModel(this BindableObject binableObj)
        {
            if (binableObj == null)
                return null;

            if (binableObj.BindingContext is BasePageModel viewmodel)
                return viewmodel;

            if (binableObj is Page)
                return null;

            if (binableObj is Element element)
                return GetPageViewModel(element.Parent);

            return null;
        }

        public static string GetPropertyBindingPath(this BindableObject obj, BindableProperty property)
        {
            string pathValue = null;
            if (obj == null)
                return pathValue;
            var getContextMethod = typeof(BindableObject).GetRuntimeMethods().First(x => x.Name == "GetContext");
            object propertyContext = getContextMethod?.Invoke(obj, new object[] { property });

            var bindingsField = propertyContext
                .GetType()
                .GetRuntimeField("Bindings")?
                .GetValue(propertyContext);
            var values = bindingsField
                .GetType()
                .GetField("_values", BindingFlags.NonPublic | BindingFlags.Instance);
            if (values?.GetValue(bindingsField) is BindingBase[] bindingList)
            {
                var firstBinding = bindingList.FirstOrDefault(b => b is Binding) as Binding;
                return firstBinding?.Path ?? pathValue;
            }

            return pathValue;
        }
    }
}