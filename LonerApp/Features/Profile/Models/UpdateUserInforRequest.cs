namespace LonerApp.Models
{
    public class UpdateUserInforRequest
    {
        public EditInforRequest EditRequest { get; init; } = new();
    }

    public class EditInforRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string? About { get; set; }
        public string? University { get; set; }
        public string? Work { get; set; }
        public bool Gender { get; set; }
        public IEnumerable<string> Photos { get; set; } = [];
        public IEnumerable<string> Interests { get; set; } = [];
    }

    public class UpdateUserInforResponse
    {
        public bool IsSuccess { get; set; }
    }
}