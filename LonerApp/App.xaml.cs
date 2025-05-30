﻿using Android.OS;

namespace LonerApp
{
    public partial class App : Application
    {
        public static BasePageModel? CurrentPageModel
        {
            get
            {
                if (Shell.Current != null && Shell.Current.CurrentPage != null)
                {
                    return Shell.Current.CurrentPage?.BindingContext as BasePageModel;
                }
                else
                {
                    return AppHelper.CurrentMainPage?.BindingContext as BasePageModel;
                }
            }
        }
        public App()
        {
            try
            {
                InitializeComponent();
                NavGraph.RegisterRoute();
                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BoderlessEntry), (handler, view) =>
                {
#if ANDROID
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                    {
                        handler.PlatformView.TextCursorDrawable?.SetTint(Android.Graphics.Color.ParseColor("#FE3675"));
                    }
#endif
                });

                Microsoft.Maui.Handlers.EditorHandler.Mapper
                .AppendToMapping(nameof(BorderlessEditor),
                (handler, view) =>
                {
                    handler.PlatformView.BackgroundTintList =
                        Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                });

                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    //var exception = e.ExceptionObject as Exception;
                    if(e.ExceptionObject is Exception exception && exception != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[UnhandledException] {exception?.Message}");
                        throw exception;
                    }
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        public static void RefreshApp()
        {
            //if ((App.Current as App).IsAppSleepingOrCovered)
            //    return;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (AppHelper.CurrentMainPage is AppShell appShell)
                    await appShell?.RemoveRootAsync();
                AppHelper.SetMainPage(new AppShell()); // REQUIRE RUN MAIN THREAD
                //ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor(ThemeUtil.GetBackgroundCoverColor());
            });
        }
    }
}