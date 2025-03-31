using LonerApp.UI.GlobalPages;

namespace LonerApp.Navigation;

    public static class NavGraph
    {
        public static void RegisterRoute()
        {
            Routing.RegisterRoute(nameof(MainSwipePage), typeof(MainSwipePage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
            Routing.RegisterRoute(nameof(EmailAuthor), typeof(EmailAuthor));
            Routing.RegisterRoute(nameof(PhoneNumberAuthor), typeof(PhoneNumberAuthor));
            Routing.RegisterRoute(nameof(SetupDateOfBirthPage), typeof(SetupDateOfBirthPage));
            Routing.RegisterRoute(nameof(SetupGenderPage), typeof(SetupGenderPage));
            Routing.RegisterRoute(nameof(SetupInterestPage), typeof(SetupInterestPage));
            Routing.RegisterRoute(nameof(SetupNamePage), typeof(SetupNamePage));
            Routing.RegisterRoute(nameof(SetupPhotosPage), typeof(SetupPhotosPage));
            Routing.RegisterRoute(nameof(SetupShowGenderForMe), typeof(SetupShowGenderForMe));
            Routing.RegisterRoute(nameof(SetupUniversityPage), typeof(SetupUniversityPage));
            Routing.RegisterRoute(nameof(VerifyPhoneNumberAuthorPage), typeof(VerifyPhoneNumberAuthorPage));
            Routing.RegisterRoute(nameof(ImageCroppingPage), typeof(ImageCroppingPage));
            Routing.RegisterRoute(nameof(MainSwipePage), typeof(MainSwipePage));
            Routing.RegisterRoute(nameof(DetailProfilePage), typeof(DetailProfilePage));
        }
    }
