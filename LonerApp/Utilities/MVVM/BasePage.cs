using Loner.Helpers.Awares;

namespace LonerApp.Utilities.MVVM
{
    public abstract class BasePage : ContentPage, IApplicationLifecycleAware
    {
        public virtual void OnResume()
        {

        }

        public virtual void OnSleep()
        {

        }
    }
}
