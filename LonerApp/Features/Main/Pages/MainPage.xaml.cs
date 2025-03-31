namespace LonerApp.Features.Pages;

public partial class MainPage : BasePage
{
    private readonly MainPageModel _vm;
    public MainPage(MainPageModel vm)
    {
        BindingContext = _vm = vm;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        _vm.IsBusy = false;
    }
    public void Welcome_SizeChanged(object sender, EventArgs e)
    {
        if (sender is Label label)
        {
            label.WidthRequest = Constants.WidthDevice * 0.8;
        }
    }
}