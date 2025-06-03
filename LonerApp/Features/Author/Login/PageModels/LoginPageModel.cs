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
        private readonly INavigationOtherShellService _navigationOtherShell;
        ContentPage? _previousPage;
        SettingPageModel? _settingPageModel;

        public LoginPageModel(
            INavigationService navigationService,
            INavigationOtherShellService navigationOtherShell)
            : base(navigationService, true)
        {
            Countries = new ObservableCollection<Country>
                {
                    new Country { Name = "VN +84", ID = 0 },
                    new Country { Name = "USA +1", ID = 1 },
                };

            SelectCountry = Countries[0].Name ?? string.Empty;
            _navigationOtherShell = navigationOtherShell;
        }

        public override async Task InitAsync(object? initData)
        {
            _previousPage = AppShell.Current?.CurrentPage as ContentPage;
            if (_previousPage != null)
                _settingPageModel = _previousPage.BindingContext as SettingPageModel;
            if (initData is string data)
                PhoneNumberValue = data;
            else
                PhoneNumberValue = "";
            HasBackButton = true;
            IsVisibleNavigation = true;
            await base.InitAsync(initData);
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
                if (_settingPageModel != null)
                {
                    _settingPageModel.PhoneNumberValue = PhoneNumberValue;
                    await NavigationService.PopPageAsync();
                    return;
                }
                IsShowError = false;
                //TODO: Handle OTP.
                var accountSid = Environments.TWILIO_ACCOUNT_SID;
                var authToken = Environments.TWILIO_AUTH_TOKEN;
                TwilioClient.Init(accountSid, authToken);

                var verification = VerificationResource.Create(
                    to: Environments.TWILIO_PHONE_NUMBER_IS_AUTHORIED,
                    channel: "sms",
                    pathServiceSid: Environments.TWILIO_PATH_SERVICE_SID
                );
                await _navigationOtherShell.NavigateToAsync<VerifyPhoneNumberAuthorPage>(param: $"{PhoneNumberValue} ", isPushModal: true);
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
            var validatorResult = _verifiedPhoneNumber.Validate(this);
            if (validatorResult.IsValid)
            {
                var accountSid = Environments.TWILIO_ACCOUNT_SID;
                var authToken = Environments.TWILIO_AUTH_TOKEN;
                TwilioClient.Init(accountSid, authToken);

                var verification = VerificationCheckResource.Create(
                    to: Environments.TWILIO_PHONE_NUMBER_IS_AUTHORIED,
                    code: VerifyPhoneNumberValue,
                    pathServiceSid: Environments.TWILIO_PATH_SERVICE_SID
                );

                if (verification.Status == "approved")
                {
                    ErrorTextValue = "Nhập đúng";
                }
                else
                    ErrorTextValue = "Nhập sai";
                IsShowError = false;
                await _navigationOtherShell.GoBackAsync();
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
            try
            {
                IsBusy = true;

                EmailValue = EmailValue?.Trim() ?? string.Empty;

                var validationResult = _emailNumberValidator.Validate(this);
                if (!validationResult.IsValid)
                {
                    DisplayError(validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid email");
                    return;
                }
                if (_settingPageModel != null)
                {
                    _settingPageModel.EmailValue = EmailValue;
                    await NavigationService.PopPageAsync();
                    return;
                }

                var _authorService = ServiceHelper.GetService<IAuthorService>();
                if (_authorService is null)
                    return;

                bool isLoggingIn = IsLoginActionStatus();
                var sendMailResponse = await _authorService.SendMailOtpAsync(new()
                {
                    Email = EmailValue,
                    IsLoggingIn = isLoggingIn
                });
                if (sendMailResponse?.IsSuccess == true)
                {
                    ClearError();
                    await _navigationOtherShell.NavigateToAsync<VerifyPhoneEmailAuthorPage>(param: EmailValue, isPushModal: false);
                    await Task.Delay(100);
                }
                else
                {
                    DisplayError(sendMailResponse?.Message ?? "An error occurred while sending verification email");
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Unexpected error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool IsLoginActionStatus()
        {
            var isLoggingInValue = UserSetting.Get(StorageKey.IsLoggingIn);
            if (string.IsNullOrEmpty(isLoggingInValue))
                return false;
            return Convert.ToBoolean(isLoggingInValue);
        }

        private void DisplayError(string message)
        {
            IsShowError = true;
            ErrorTextValue = message;
        }
        private void ClearError()
        {
            IsShowError = false;
            ErrorTextValue = string.Empty;
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (!IsBusy)
            {
                if (AppShell.Current != null)
                    await NavigationService.PopPageAsync(isPopModal: false);
                else
                    await _navigationOtherShell.GoBackAsync();
            }
        }
    }
}