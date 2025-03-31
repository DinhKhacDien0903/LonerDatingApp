namespace LonerApp.Models
{
    public partial class Interest : BaseModel
    {
        public int ID { get; set; }
        public string Value { get; set; } = string.Empty;
        [ObservableProperty]
        bool _isSelected;
    }
}