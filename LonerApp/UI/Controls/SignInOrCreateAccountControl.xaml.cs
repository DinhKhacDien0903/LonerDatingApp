namespace LonerApp.UI.Controls;

public partial class SignInOrCreateAccountControl : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title),
        typeof(string),
        typeof(SignInOrCreateAccountControl),
        defaultValue: "");

    public static readonly BindableProperty ErrorValueProperty =
        BindableProperty.Create(nameof(ErrorValue),
        typeof(string),
        typeof(SignInOrCreateAccountControl),
        defaultValue: "");

    public static readonly BindableProperty PhoneNumberValueProperty =
        BindableProperty.Create(nameof(PhoneNumberValue),
        typeof(string),
        typeof(SignInOrCreateAccountControl),
        defaultValue: "");

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(nameof(Description),
        typeof(string),
        typeof(SignInOrCreateAccountControl),
        defaultValue: "");

    public static readonly BindableProperty ContinueCommandProperty =
        BindableProperty.Create(nameof(ContinueCommand),
        typeof(ICommand),
        typeof(SignInOrCreateAccountControl)
        );

    public static readonly BindableProperty IsPhoneNumberProperty =
        BindableProperty.Create(nameof(IsPhoneNumber),
        typeof(bool),
        typeof(SignInOrCreateAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsVerifyPhoneNumberProperty =
        BindableProperty.Create(nameof(IsVerifyPhoneNumber),
        typeof(bool),
        typeof(SignInOrCreateAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsVerifyEmailProperty =
        BindableProperty.Create(nameof(IsVerifyEmail),
        typeof(bool),
        typeof(SignInOrCreateAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsEmailProperty =
        BindableProperty.Create(nameof(IsEmail),
        typeof(bool),
        typeof(SignInOrCreateAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsShowErrorProperty =
        BindableProperty.Create(nameof(IsShowError),
        typeof(bool),
        typeof(SignInOrCreateAccountControl),
        defaultValue: false);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string ErrorValue
    {
        get => (string)GetValue(ErrorValueProperty);
        set => SetValue(ErrorValueProperty, value);
    }

    public string PhoneNumberValue
    {
        get => (string)GetValue(PhoneNumberValueProperty);
        set => SetValue(PhoneNumberValueProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public ICommand ContinueCommand
    {
        get => (ICommand)GetValue(ContinueCommandProperty);
        set => SetValue(ContinueCommandProperty, value);
    }

    public bool IsPhoneNumber
    {
        get => (bool)GetValue(IsPhoneNumberProperty);
        set => SetValue(IsPhoneNumberProperty, value);
    }

    public bool IsVerifyPhoneNumber
    {
        get => (bool)GetValue(IsVerifyPhoneNumberProperty);
        set => SetValue(IsVerifyPhoneNumberProperty, value);
    }

    public bool IsVerifyEmail
    {
        get => (bool)GetValue(IsVerifyEmailProperty);
        set => SetValue(IsVerifyEmailProperty, value);
    }
    public bool IsEmail
    {
        get => (bool)GetValue(IsEmailProperty);
        set => SetValue(IsEmailProperty, value);
    }

    public bool IsShowError
    {
        get => (bool)GetValue(IsShowErrorProperty);
        set => SetValue(IsShowErrorProperty, value);
    }

    public SignInOrCreateAccountControl()
    {
        InitializeComponent();
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is CustomEntry entry && BindingContext is LoginPageModel viewModel)
        {
            viewModel.IsContinue = !string.IsNullOrEmpty(entry.EntryValue);
        }
    }
}