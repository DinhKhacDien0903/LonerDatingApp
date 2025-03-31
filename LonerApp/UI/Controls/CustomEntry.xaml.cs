namespace LonerApp.UI.Controls;

public partial class CustomEntry : ContentView
{
    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder),
        typeof(string),
        typeof(CustomEntry),
        defaultValue: "");

    public static readonly BindableProperty EntryValueProperty =
        BindableProperty.Create(nameof(EntryValue),
        typeof(string),
        typeof(CustomEntry),
        defaultValue: "",
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: OnTextChanged);

    public static readonly BindableProperty CharacterSpacingProperty =
        BindableProperty.Create(nameof(CharacterSpacing),
        typeof(double),
        typeof(CustomEntry),
        defaultValue: 2d);

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize),
        typeof(double),
        typeof(CustomEntry),
        defaultValue: 16d);

    public static readonly BindableProperty CustomEntryHeightRequestProperty =
        BindableProperty.Create(nameof(CustomEntryHeightRequest),
        typeof(double),
        typeof(CustomEntry),
        defaultValue: 50d);

    public static readonly BindableProperty MaxValueLengthProperty =
        BindableProperty.Create(nameof(MaxValueLength),
        typeof(int),
        typeof(CustomEntry),
        defaultValue: 10);

    public static readonly BindableProperty CustomMiniumWidthRequestProperty =
        BindableProperty.Create(nameof(CustomMiniumWidthRequest),
        typeof(double),
        typeof(CustomEntry),
        defaultValue: 320d);

    public static readonly BindableProperty CustomWidthRequestProperty =
        BindableProperty.Create(nameof(CustomWidthRequest),
        typeof(double),
        typeof(CustomEntry),
        defaultValue: 320d);

    public static readonly BindableProperty KeyboardTypeProperty =
        BindableProperty.Create(nameof(KeyboardType),
        typeof(Keyboard),
        typeof(CustomEntry),
        defaultValue: Keyboard.Default);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }
    public string EntryValue
    {
        get => (string)GetValue(EntryValueProperty);
        set => SetValue(EntryValueProperty, value);
    }

    public double CharacterSpacing
    {
        get => (double)GetValue(CharacterSpacingProperty);
        set => SetValue(CharacterSpacingProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public double CustomEntryHeightRequest
    {
        get => (double)GetValue(CustomEntryHeightRequestProperty);
        set => SetValue(CustomEntryHeightRequestProperty, value);
    }

    public double CustomMiniumWidthRequest
    {
        get => (double)GetValue(CustomMiniumWidthRequestProperty);
        set => SetValue(CustomMiniumWidthRequestProperty, value);
    }

    public double CustomWidthRequest
    {
        get => (double)GetValue(CustomWidthRequestProperty);
        set => SetValue(CustomWidthRequestProperty, value);
    }

    public int MaxValueLength
    {
        get => (int)GetValue(MaxValueLengthProperty);
        set => SetValue(MaxValueLengthProperty, value);
    }

    public Keyboard KeyboardType
    {
        get => (Keyboard)GetValue(KeyboardTypeProperty);
        set => SetValue(KeyboardTypeProperty, value);
    }
    public event EventHandler<TextChangedEventArgs> TextChanged;

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.TextChanged?.Invoke(control, new TextChangedEventArgs((string)oldValue, (string)newValue));
    }
    private void EntryName_TextChanged(object sender, TextChangedEventArgs e)
    {
        EntryValue = e.NewTextValue;
        TextChanged?.Invoke(this, e);
    }

    public CustomEntry()
    {
        InitializeComponent();
    }
}