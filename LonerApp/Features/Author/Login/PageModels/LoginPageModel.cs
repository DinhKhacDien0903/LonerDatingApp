using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using System.Collections.ObjectModel;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace LonerApp.PageModels
{
    public class LoginPhoneNumberValidator : AbstractValidator<LoginPageModel>
    {
        public LoginPhoneNumberValidator()
        {
            RuleFor(vm => vm.PhoneNumberValue)
                .FiledNotEmpty()
                .PhoneNumber();
        }
    }

    public class LoginEmailValidator : AbstractValidator<LoginPageModel>
    {
        public LoginEmailValidator()
        {
            RuleFor(vm => vm.EmailValue)
                .FiledNotEmpty()
                .Email();
        }
    }

    public class VerifiedPhoneNumberValidator : AbstractValidator<LoginPageModel>
    {
        public VerifiedPhoneNumberValidator()
        {
            RuleFor(vm => vm.VerifyPhoneNumberValue)
                .FiledNotEmpty()
                .VerifyCode();
        }
    }
    public partial class LoginPageModel : BasePageModel
    {
        [ObservableProperty]
        private bool _isOpened;
        [ObservableProperty]
        private string _selectCountry;
        [ObservableProperty]
        private ObservableCollection<Country> _countries;
        [ObservableProperty]
        private string _entryValue = string.Empty;
        [ObservableProperty]
        private string _errorTextValue = string.Empty;
        [ObservableProperty]
        private string _phoneNumberValue = string.Empty;
        [ObservableProperty]
        private string _emailValue = string.Empty;
        [ObservableProperty]
        private string _verifyPhoneNumberValue = string.Empty;
        [ObservableProperty]
        private bool _isShowError;
        [ObservableProperty]
        private bool _isContinue;
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;
        private readonly LoginPhoneNumberValidator _phoneNumberValidator = new();
        private readonly VerifiedPhoneNumberValidator _verifiedPhoneNumber = new();
        private readonly LoginEmailValidator _emailNumberValidator = new();

        public LoginPageModel(INavigationService navigationService)
            : base(navigationService, true)
        {
            Countries = new ObservableCollection<Country>
                {
                    new Country { Name = "VN +84", ID = 0 },
                    new Country { Name = "USA +1", ID = 1 },
                };

            SelectCountry = Countries[0].Name ?? string.Empty;
        }

        public override async Task InitAsync(object? initData)
        {
            await base.InitAsync(initData);
            if (initData is string data)
                PhoneNumberValue = data;
            else
                PhoneNumberValue = "";
            HasBackButton = true;
            IsVisibleNavigation = true;
        }

        [RelayCommand]
        public void OnSelectCountry(object selectItem)
        {
            if (selectItem is Country selectedCountry)
            {
                IsOpened = false;
                OnPropertyChanged(nameof(IsOpened));
                SelectCountry = selectedCountry.Name ?? string.Empty;
            }
        }

        [RelayCommand]
        async Task OnPhoneNumberContinueAsync(object param)
        {
            IsBusy = true;
            PhoneNumberValue = PhoneNumberValue.Trim();
            var validatorResult = _phoneNumberValidator.Validate(this);
            if (validatorResult.IsValid)
            {
                IsShowError = false;
                //TODO: Handle OTP.
                var accountSid = "AC42be1218f22e662224f57255a40e61db";
                var authToken = "cae95aa66aa256c7c192200f3d2232ec";
                TwilioClient.Init(accountSid, authToken);

                var verification = VerificationResource.Create(
                    to: "+84777712640",
                    channel: "sms",
                    pathServiceSid: "VA197516f6d68a53f646a7274fd2f3cadd"
                );
                await NavigationService.PushToPageAsync<VerifyPhoneNumberAuthorPage>(param: $"{PhoneNumberValue} ", isPushModal: true);
            }
            else
            {
                IsShowError = true;
                ErrorTextValue = validatorResult.Errors[0].ErrorMessage;
            }
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnPhoneNumberIdentityContinueAsync(object param)
        {
            IsBusy = true;
            VerifyPhoneNumberValue = VerifyPhoneNumberValue.Trim();
            //TODO: Check confirm code to database
            var accountSid = "AC42be1218f22e662224f57255a40e61db";
            var authToken = "cae95aa66aa256c7c192200f3d2232ec";
            TwilioClient.Init(accountSid, authToken);

            var verification = VerificationCheckResource.Create(
                to: "+84777712640",
                code: VerifyPhoneNumberValue,
                pathServiceSid: "VA197516f6d68a53f646a7274fd2f3cadd"
            );

            if (verification.Status == "approved")
            {
                ErrorTextValue = "Nhập đúng";
            }
            else
                ErrorTextValue = "Nhập sai";
            var validatorResult = _verifiedPhoneNumber.Validate(this);
            if (validatorResult.IsValid)
            {
                IsShowError = false;
                await NavigationService.PushToPageAsync<SetupNamePage>(isPushModal: false);
            }
            else
            {
                IsShowError = true;
                ErrorTextValue = validatorResult.Errors[0].ErrorMessage;
            }
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnEmailContinueAsync(object param)
        {
            IsBusy = true;
            EmailValue = EmailValue.Trim();
            var validatorResult = _emailNumberValidator.Validate(this);
            if (validatorResult.IsValid)
            {
                IsShowError = false;
                await NavigationService.PushToPageAsync<SetupNamePage>(isPushModal: false);
            }
            else
            {
                IsShowError = true;
                ErrorTextValue = validatorResult.Errors[0].ErrorMessage;
            }
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (!IsBusy)
                await NavigationService.PopPageAsync(isPopModal: true);
        }
    }
}