using LonerApp.Helpers.Exceptions;

namespace LonerApp.Helpers
{
    public static class ServiceHelper
    {
        /// <summary>
        /// Gets the current service provider for each working platform.
        /// Current.Service: Returns the current service that the application is working with.
        /// </summary>
        public static IServiceProvider Current => IPlatformApplication.Current?.Services;

        public static T GetService<T>() => Current.GetService<T>();

        /// <summary>
        /// TViewModel: Generic type parameter, should be placed within quotation marks.
        /// Generic parameter type: Defines a data type that can operate with any data type it inherits.
        /// <typeparam name="TPageModel"></typeparam>
        /// <returns></returns>
        /// </summary>
        public static TPageModel GetPageModel<TPageModel>() where TPageModel : BasePageModel
        {
            return GetService<TPageModel>() ?? throw new GetPageModelException<TPageModel>();
        }

        public static TViewModel GetPageModelObservable<TViewModel>()
                 where TViewModel : ObservableObject
        {
            return Current.GetService<TViewModel>() ?? throw new Exception("Failed to resolve PageModel for TabbedPage");
        }
    }
}