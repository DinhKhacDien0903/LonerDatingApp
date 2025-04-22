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
    private readonly VerifiedEmailValidator _emailNumberValidator = new();

    public VerfyEmailPageModel(INavigationService navigationService, IAuthorService authorService)
        : base(navigationService, true)
    {
        _authorService = authorService;
        HasBackButton = true;
        IsVisibleNavigation = true;
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

            var verifyResponse = await _authorService.VerifyEmailAsync(new()
            {
                Email = EmailValue,
                Otp = VerifyEmailValue,
                IsLoggingIn = true
            });

            if (verifyResponse?.IsVerified == true)
            {
                ClearError();
                UserSetting.SetObject(StorageKey.IsFirstLogin, verifyResponse.UserId);
                if (currentId == null)
                    UserSetting.SetObject(StorageKey.UserId, verifyResponse.UserId);

                if (!verifyResponse.IsSetupedAccount)
                    await NavigationService.PushToPageAsync<SetupNamePage>(isPushModal: false);
                else
                    await NavigationService.PushToPageAsync<MainSwipePage>(param: verifyResponse.UserId, isPushModal: false);
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