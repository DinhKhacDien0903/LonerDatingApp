namespace LonerApp.Models;

public partial class ReportModel : BaseModel
{
    [ObservableProperty]
    private string? id;
    [ObservableProperty]
    public string? label_vi;
    [ObservableProperty]
    public string? label_en;
    [ObservableProperty]
    public List<Reason>? reasons;
}

public partial class Reason : BaseModel
{
    public string? Id;
    [ObservableProperty]
    private string? label_vi;
    [ObservableProperty]
    private string? label_en;
    [ObservableProperty]
    private bool _isSelected;
}