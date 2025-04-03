using System;

namespace LonerApp.Models;

public partial class DistrictLocationModel : BaseModel
{
    //TODO: Add to Realms
    public string ID {get; set;} = string.Empty;
    public string Name { get; set; } = string.Empty;

    [ObservableProperty]
    Location _location = new ();
}
