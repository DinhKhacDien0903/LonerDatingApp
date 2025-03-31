namespace LonerApp.UI.Controls;

public partial class SetupAccountControl : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title),
        typeof(string),
        typeof(SetupAccountControl),
        defaultValue: "");

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(nameof(Description),
        typeof(string),
        typeof(SetupAccountControl),
        defaultValue: "");

    public static readonly BindableProperty IsNameProperty =
        BindableProperty.Create(nameof(IsName),
        typeof(bool),
        typeof(SetupAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsDateOfBirthProperty =
        BindableProperty.Create(nameof(IsDateOfBirth),
        typeof(bool),
        typeof(SetupAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsGenderProperty =
        BindableProperty.Create(nameof(IsGender),
        typeof(bool),
        typeof(SetupAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsShowGenderForMeProperty =
        BindableProperty.Create(nameof(IsShowGenderForMe),
        typeof(bool),
        typeof(SetupAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsUniversityProperty =
        BindableProperty.Create(nameof(IsUniversity),
        typeof(bool),
        typeof(SetupAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsInterestProperty =
        BindableProperty.Create(nameof(IsInterest),
        typeof(bool),
        typeof(SetupAccountControl),
        defaultValue: false);

    public static readonly BindableProperty IsAddPhotoProperty =
        BindableProperty.Create(nameof(IsAddPhoto),
        typeof(bool),
        typeof(SetupAccountControl),
        defaultValue: false);

    public static readonly BindableProperty ContinueCommandProperty =
        BindableProperty.Create(nameof(ContinueCommand),
        typeof(ICommand),
        typeof(SetupAccountControl)
        );

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool IsName
    {
        get => (bool)GetValue(IsNameProperty);
        set => SetValue(IsNameProperty, value);
    }

    public bool IsDateOfBirth
    {
        get => (bool)GetValue(IsDateOfBirthProperty);
        set => SetValue(IsDateOfBirthProperty, value);
    }

    public bool IsGender
    {
        get => (bool)GetValue(IsGenderProperty);
        set => SetValue(IsGenderProperty, value);
    }

    public bool IsShowGenderForMe
    {
        get => (bool)GetValue(IsShowGenderForMeProperty);
        set => SetValue(IsShowGenderForMeProperty, value);
    }

    public bool IsUniversity
    {
        get => (bool)GetValue(IsUniversityProperty);
        set => SetValue(IsUniversityProperty, value);
    }

    public bool IsInterest
    {
        get => (bool)GetValue(IsInterestProperty);
        set => SetValue(IsInterestProperty, value);
    }

    public bool IsAddPhoto
    {
        get => (bool)GetValue(IsAddPhotoProperty);
        set => SetValue(IsAddPhotoProperty, value);
    }

    public ICommand ContinueCommand
    {
        get => (ICommand)GetValue(ContinueCommandProperty);
        set => SetValue(ContinueCommandProperty, value);
    }
    public SetupAccountControl()
    {
        InitializeComponent();
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is CustomEntry entry && BindingContext is SetupPageModel viewModel)
        {
            viewModel.IsContinue = !string.IsNullOrEmpty(entry.EntryValue);
        }
    }
}