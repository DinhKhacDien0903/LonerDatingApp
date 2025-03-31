
namespace LonerApp.UI.Controls;

public partial class CustomNavigationBar : ContentView
{
    public static readonly BindableProperty HasBackButtonProperty =
        BindableProperty.Create(nameof(HasBackButton),
        typeof(bool),
        typeof(CustomNavigationBar),
        defaultValue: false);

    public static readonly BindableProperty BackButtonCommandProperty =
        BindableProperty.Create(nameof(BackButtonCommand),
        typeof(ICommand),
        typeof(CustomNavigationBar),
        defaultValue: null);

    public static readonly BindableProperty OptionButtonCommandProperty =
        BindableProperty.Create(nameof(OptionButtonCommand),
        typeof(ICommand),
        typeof(CustomNavigationBar),
        defaultValue: null);

    public static readonly BindableProperty TitlePageProperty =
        BindableProperty.Create(nameof(TitlePage),
        typeof(string),
        typeof(CustomNavigationBar),
        defaultValue: default(string),
        propertyChanged: OnTitlePageChanged);

    public static readonly BindableProperty OptionPageProperty =
        BindableProperty.Create(nameof(OptionPage),
        typeof(string),
        typeof(CustomNavigationBar),
        defaultValue: default(string),
        propertyChanged: OnOptionageChanged);

    public bool HasBackButton
    {
        get => (bool)GetValue(HasBackButtonProperty);
        set => SetValue(HasBackButtonProperty, value);
    }
    public ICommand BackButtonCommand
    {
        get => (ICommand)GetValue(BackButtonCommandProperty);
        set => SetValue(BackButtonCommandProperty, value);
    } 
 
    public ICommand OptionButtonCommand
    {
        get => (ICommand)GetValue(OptionButtonCommandProperty);
        set => SetValue(OptionButtonCommandProperty, value);
    }

    public string TitlePage
    {
        get => (string)GetValue(TitlePageProperty);
        set => SetValue(TitlePageProperty, value);
    }

    public string OptionPage
    {
        get => (string)GetValue(OptionPageProperty);
        set => SetValue(OptionPageProperty, value);
    }

    private static void OnTitlePageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CustomNavigationBar navBar || newValue is not string newTitle || newValue == oldValue)
            return;

        if (navBar.FindByName<Label>("CustomTitleLabel") is Label titleLabel)
            titleLabel.Text = newTitle;
    }

    private static void OnOptionageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CustomNavigationBar navBar || newValue is not string newTitle || newValue == oldValue)
            return;

        if (navBar.FindByName<Label>("CustomOptionLabel") is Label optionLabel)
            optionLabel.Text = newTitle;
    }
    public CustomNavigationBar()
    {
        InitializeComponent();
    }
}