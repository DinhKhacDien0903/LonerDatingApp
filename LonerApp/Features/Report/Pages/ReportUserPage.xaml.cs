namespace LonerApp.Features.Pages;

public partial class ReportUserPage : BasePage
{
	public ReportUserPage(ReportPageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}

	private void MoreInformationEditor_Unfocused(object sender, FocusEventArgs e)
	{
		if (sender is not Editor editor)
		{
			return;
		}

		ServiceHelper.GetService<IDeviceService>().HideKeyboard();
	}
}