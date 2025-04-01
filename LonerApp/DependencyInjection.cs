using LonerApp.Platforms.Android.Services;
using LonerApp.UI.GlobalPages;
using Mopups.Interfaces;
using Mopups.Services;

namespace LonerApp;
public static class DependencyInjection
{
    public static IServiceCollection AddApplications(this IServiceCollection services)
    {
#if ANDROID
        services.AddSingleton<IPopupNavigation>(MopupService.Instance);
        services.AddSingleton<ISystemStyleManager, SystemStyleManager>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IPopupNavigationService, PopupNavigationService>();
        services.AddSingleton<IOpenSetting, OpenSetting>();
        services.AddSingleton<IDeviceService, DeviceService>();
#endif
        return services;
    }

    public static IServiceCollection RegisterPageModels(this IServiceCollection services)
    {
        services.AddTransient<MainPageModel>();
        services.AddTransient<LoginPageModel>();
        services.AddTransient<SetupPageModel>();
        services.AddTransient<SwipePageModel>();
        services.AddTransient<ProfilePageModel>();
        services.AddTransient<ChatPageModel>();
        services.AddTransient<ChatMessagePageModel>();
        services.AddTransient<ChatMessageImagePageModel>();
        services.AddTransient<FilterMapPageModel>();
        return services;
    }

    public static IServiceCollection RegisterPages(this IServiceCollection services)
    {
        services.AddTransient<MainPage>();
        services.AddTransient<SignInPage>();
        services.AddTransient<PhoneNumberAuthor>();
        services.AddTransient<EmailAuthor>();
        services.AddTransient<VerifyPhoneNumberAuthorPage>();
        services.AddTransient<SetupNamePage>();
        services.AddTransient<SetupDateOfBirthPage>();
        services.AddTransient<SetupGenderPage>();
        services.AddTransient<SetupShowGenderForMe>();
        services.AddTransient<SetupUniversityPage>();
        services.AddTransient<SetupInterestPage>();
        services.AddTransient<SetupPhotosPage>();
        services.AddTransient<ImageCroppingPage>();
        services.AddTransient<MainSwipePage>();
        services.AddTransient<DetailProfilePage>();
        services.AddTransient<ChatPage>();
        services.AddTransient<MessageChatPage>();
        services.AddTransient<ImageViewerFullScreenPage>();
        services.AddTransient<FilterMapPage>();
        return services;
    }
}