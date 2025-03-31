using Camera.MAUI;
using CommunityToolkit.Maui;
using DotNet.Meteor.HotReload.Plugin;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;
using Sharpnado.CollectionView;
using Syncfusion.Maui.Toolkit.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using PanCardView;
using Mopups.Hosting;
using Plugin.Maui.SwipeCardView;

namespace LonerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
                })
                .UseSwipeCardView()
                .ConfigureMopups()
                .UseSkiaSharp()
                .UseMauiCameraView()
                .UseCardsView()
                .UseFFImageLoading()
                .UseSharpnadoCollectionView(loggerEnable: false, debugLogEnable: false)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialFontFamily");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                    fonts.AddFont("gothamrnd_bold.otf", "GothamBold");
                    fonts.AddFont("gothamrnd_light.otf", "GothamLight");
                    fonts.AddFont("gothamrnd_medium.otf", "GothamMedium");
                });

            //register DI
            builder.Services.AddApplications();
            builder.Services.RegisterPageModels();
            builder.Services.RegisterPages();
#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
            builder.EnableHotReload();
#endif

            return builder.Build();
        }
    }
}