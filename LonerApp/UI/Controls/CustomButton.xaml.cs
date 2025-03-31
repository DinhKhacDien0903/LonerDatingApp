namespace LonerApp.UI.Controls;

public partial class CustomButton : ContentView
{
    public static BindableProperty CustomCommandProperty =
        BindableProperty.Create(nameof(CustomCommand), typeof(ICommand), typeof(CustomButton), null);

    public static BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomButton), null);

    public static BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(CustomButton), null);
    public ICommand CustomCommand
    {
        get => (ICommand)GetValue(CustomCommandProperty);
        set => SetValue(CustomCommandProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public CustomButton()
    {
        InitializeComponent();
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            border.BackgroundColor = Color.FromArgb("#f7d6cd");
        }

        CustomCommand.Execute(null);
    }
}