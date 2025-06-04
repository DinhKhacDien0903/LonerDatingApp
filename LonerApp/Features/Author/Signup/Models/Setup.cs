namespace LonerApp.Features.Author.Signup.Models
{
    public class SetUpNameRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }

    public class SetUpDOBRequest
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime Dob { get; set; } = DateTime.MinValue;
    }

    public class SetUpGenderRequest
    {
        public string UserId { get; set; } = string.Empty;
        public bool Gender { get; set; } = false;
    }

    public class SetUpShowGenderRequest
    {
        public string UserId { get; set; } = string.Empty;
        public bool ShowGender { get; set; } = false;
    }

    public class SetUpUniversityRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
    }

    public class SetUpInterestRequest
    {
        public string UserId { get; set; } = string.Empty;
        public IEnumerable<string> Interests { get; set; } = Enumerable.Empty<string>();
    }

    public class SetUpPhotosRequest
    {
        public string UserId { get; set; } = string.Empty;
        public IEnumerable<string> Photos { get; set; } = Enumerable.Empty<string>();
    }

    //Response
    public record SetUpResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}