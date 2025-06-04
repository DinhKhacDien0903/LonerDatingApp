using LonerApp.Features.Author.Signup.Models;

namespace LonerApp.Features.Author.Services
{
    public interface ISetupService
    {
        Task<SetUpResponse?> SetupNameAsync(SetUpNameRequest request);
        Task<SetUpResponse?> SetupDateOfBirthAsync(SetUpDOBRequest request);
        Task<SetUpResponse?> SetupGenderAsync(SetUpGenderRequest request);
        Task<SetUpResponse?> SetupGenderShowMeAsync(SetUpShowGenderRequest request);
        Task<SetUpResponse?> SetupUniversityAsync(SetUpUniversityRequest request);
        Task<SetUpResponse?> SetupInterestsAsync(SetUpInterestRequest request);
        Task<SetUpResponse?> SetupPhotosAsync(SetUpPhotosRequest request);
    }
}