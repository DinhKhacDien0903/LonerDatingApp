using CommunityToolkit.Maui.Views;
using LonerApp.Features.Chat.Services;
using LonerApp.Features.Filter.Services;
using LonerApp.Features.Services;
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
        services.AddSingleton<ILocalNotificationService, LocalNotificationService>();
        //services.AddSingleton<INavigationOtherShellService, NavigationOtherShellService>();
        services.AddSingleton<IPopupNavigationService, PopupNavigationService>();
        services.AddSingleton<IOpenSetting, OpenSetting>();
        services.AddSingleton<IDeviceService, DeviceService>();
        services.AddSingleton<IApiService, ApiService>();
        services.AddSingleton<ISwipeService, SwipeService>();
        services.AddSingleton<IProfileService, ProfileService>();
        services.AddSingleton<IAuthorService, AuthorService>();
        services.AddSingleton<IChatService, ChatService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IFilterService, FilterService>();
        services.AddSingleton<INotificationManagerService, NotificationManagerService>();
        services.AddSingleton<INavigationCommunityPopupService, NavigationCommunityPopupService>();
        services.AddSingleton<IReportService, ReportService>();
        // services.AddTransientPopup<ReportPopup, ReportPageModel>();
#endif
        return services;
    }

    public static IServiceCollection RegisterPageModels(this IServiceCollection services)
    {
        services.AddTransient<MainPageModel>();
        services.AddTransient<LoginPageModel>();
        services.AddTransient<VerfyEmailPageModel>();
        services.AddTransient<SetupPageModel>();
        services.AddTransient<SwipePageModel>();
        services.AddTransient<ProfilePageModel>();
        services.AddTransient<MyProfilePageModel>();
        services.AddTransient<EditProfilePageModel>();
        services.AddTransient<ChatPageModel>();
        services.AddTransient<ChatMessagePageModel>();
        services.AddTransient<ChatMessageImagePageModel>();
        services.AddTransient<FilterMapPageModel>();
        services.AddTransient<SettingPageModel>();
        services.AddTransient<NotificationPageModel>();
        services.AddTransient<ReportPageModel>();
        return services;
    }

    public static IServiceCollection RegisterPages(this IServiceCollection services)
    {
        services.AddTransient<MainPage>();
        services.AddTransient<SignInPage>();
        services.AddTransient<PhoneNumberAuthor>();
        services.AddTransient<EmailAuthor>();
        services.AddTransient<VerifyPhoneNumberAuthorPage>();
        services.AddTransient<VerifyPhoneEmailAuthorPage>();
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
        services.AddTransient<MyProfilePage>();
        services.AddTransient<EditProfilePage>();
        services.AddTransient<SettingPage>();
        services.AddTransient<NotificationPage>();
        services.AddTransient<ReportUserPage>();
        // services.AddTransient<ReportPopup>();
        return services;
    }
    public static IServiceCollection AddTransientPopup<TPopupView, TPopupViewModel>(this IServiceCollection services)
    where TPopupView : Popup
    where TPopupViewModel : BasePageModel
    {
        NavigationCommunityPopupService.AddTransientPopup<TPopupView, TPopupViewModel>(services);
        return services;
    }
}