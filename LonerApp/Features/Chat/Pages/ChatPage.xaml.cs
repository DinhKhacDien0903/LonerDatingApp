namespace LonerApp.Features.Pages;

public partial class ChatPage : BasePage
{
    private ChatPageModel _vm;
	public ChatPage(ChatPageModel vm)
	{
        BindingContext = _vm = vm;
        InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        if(!_vm.IsPushPageWithNavService && _vm.IsNeedLoadUsersData)
        {
            await _vm.InitDataAsync();
            await _vm.ViewIsAppearingAsync();
        }
        _vm.IsBusy = false;
    }
}