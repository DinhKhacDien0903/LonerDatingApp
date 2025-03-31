namespace Loner.Helpers.Awares
{
    interface IApplicationLifecycleAware
    {
        /// <summary>
        /// Call when the application is resumed
        /// </summary>
        void OnResume();

        /// <summary>
        /// Call when application is sleep
        /// </summary>
        void OnSleep();
    }
}
