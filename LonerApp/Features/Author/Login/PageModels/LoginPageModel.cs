using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using System.Collections.ObjectModel;

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