using CommunityToolkit.Mvvm.Input;
using FluentValidation;

namespace LonerApp.PageModels;
public class VerifiedEmailValidator : AbstractValidator<VerfyEmailPageModel>
{
    public VerifiedEmailValidator()
    {
        RuleFor(vm => vm.VerifyEmailValue)
            .FiledNotEmpty()
            .VerifyCode();
    }
}

public partial class VerfyEmailPageModel : BasePageModel
{
    [ObservableProperty]
    private string _verifyEmailValue = string.Empty;
    [ObservableProperty]
    private string _errorTextValue = string.Empty;
    [ObservableProperty]
    private string _emailValue = string.Empty;
    [ObservableProperty]
    private bool _isShowError;
    [ObservableProperty]
    private bool _isContinue;
    [ObservableProperty]
    private bool _isVisibleNavigation;
    [ObservableProperty]
    private bool _hasBackButton;
    private readonly IAuthorService _authorService;
    private readonly INavigationOtherShellService _navigationOtherShell;
    private readonly VerifiedEmailValidator _emailNumberValidator = new();

    public VerfyEmailPageModel
        (INavigationService navigationService,
        IAuthorService authorService,
        INavigationOtherShellService navigationOtherShell)
        : base(navigationService, true)
    {
        _authorService = authorService;
        HasBackButton = true;
        IsVisibleNavigation = true;
        _navigationOtherShell = navigationOtherShell;
    }

    public override async Task InitAsync(object? initData)
    {
        await base.InitAsync(initData);
        if (initData is string data)
            EmailValue = data.Trim();
        else
            EmailValue = "";
    }

    [RelayCommand]
    async Task OnEmailIdentityContinueAsync(object param)
    {
        try
        {
            IsBusy = true;
            VerifyEmailValue = VerifyEmailValue?.Trim() ?? string.Empty;
            var validationResult = _emailNumberValidator.Validate(this);
            var currentId = UserSetting.Get(StorageKey.UserId);
            if (!validationResult.IsValid)
            {
                DisplayError(validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid Code");
                return;
            }
            //UserSetting.Remove("IsLoggedIn");
            var isLoggingInValue = UserSetting.Get(StorageKey.IsLoggingIn);
            bool isLoggingIn;
            if (string.IsNullOrEmpty(isLoggingInValue))
                isLoggingIn = false;
            else
                isLoggingIn = Convert.ToBoolean(isLoggingInValue);

            var verifyResponse = await _authorService.VerifyEmailAsync(new()
            {
                Email = EmailValue,
                Otp = VerifyEmailValue,
                IsLoggingIn = isLoggingIn
            });

            if (verifyResponse?.IsVerified == true)
            {
                ClearError();
                UserSetting.SetObject(StorageKey.IsLoggedIn, verifyResponse.IsVerified);
                UserSetting.Set(StorageKey.IsAccountSetup, verifyResponse.IsAccountSetup.ToString());
                if (currentId == null)
                    UserSetting.Set(StorageKey.UserId, verifyResponse.UserId);

                if (!verifyResponse.IsAccountSetup)
                    await _navigationOtherShell.NavigateToAsync<SetupNamePage>(isPushModal: false);
                else
                    AppHelper.RefreshApp();
                // await _navigationOtherShell.NavigateToAsync<MainSwipePage>(param: verifyResponse.UserId, isPushModal: false);

                await Task.Delay(100);
            }
            else
            {
                DisplayError("An error occurred while verification email");
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
}