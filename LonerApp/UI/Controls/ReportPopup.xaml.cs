namespace LonerApp.UI.Controls;

public partial class ReportPopup : CustomCommunityPopup
{
	public ReportPopup(ReportPageModel reportPageModel)
	{
		InitializeComponent();
		BindingContext = reportPageModel;
	}
}