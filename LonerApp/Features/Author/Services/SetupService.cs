using LonerApp.Features.Author.Signup.Models;

namespace LonerApp.Features.Author.Services
{
    public class SetupService : ISetupService
    {
        private readonly IApiService _apiService;

        public SetupService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<SetUpResponse?> SetupDateOfBirthAsync(SetUpDOBRequest request)
        {
            try
            {
                return await _apiService.PostAsync<SetUpResponse>(EnvironmentsExtensions.ENDPOINT_DATE_OF_BIRTH, request);
            }
            catch (Exception ex)
            {
                return new SetUpResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<SetUpResponse?> SetupGenderAsync(SetUpGenderRequest request)
        {
            try
            {
                return await _apiService.PostAsync<SetUpResponse>(EnvironmentsExtensions.ENDPOINT_GENDER, request);
            }
            catch (Exception ex)
            {
                return new SetUpResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<SetUpResponse?> SetupGenderShowMeAsync(SetUpShowGenderRequest request)
        {
            try
            {
                return await _apiService.PostAsync<SetUpResponse>(EnvironmentsExtensions.ENDPOINT_GENDER_SHOW_ME, request);
            }
            catch (Exception ex)
            {
                return new SetUpResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<SetUpResponse?> SetupInterestsAsync(SetUpInterestRequest request)
        {
            try
            {
                return await _apiService.PostAsync<SetUpResponse>(EnvironmentsExtensions.ENDPOINT_INTERESTS, request);
            }
            catch (Exception ex)
            {
                return new SetUpResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<SetUpResponse?> SetupNameAsync(SetUpNameRequest request)
        {
            try
            {
                return await _apiService.PostAsync<SetUpResponse>(EnvironmentsExtensions.ENDPOINT_SETUP_NAME, request);
            }
            catch (Exception ex)
            {
                return new SetUpResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<SetUpResponse?> SetupPhotosAsync(SetUpPhotosRequest request)
        {
            try
            {
                return await _apiService.PostAsync<SetUpResponse>(EnvironmentsExtensions.ENDPOINT_PHOTOS, request);
            }
            catch (Exception ex)
            {
                return new SetUpResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<SetUpResponse?> SetupUniversityAsync(SetUpUniversityRequest request)
        {
            try
            {
                return await _apiService.PostAsync<SetUpResponse>(EnvironmentsExtensions.ENDPOINT_UNIVERSITY, request);
            }
            catch (Exception ex)
            {
                return new SetUpResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }
    }
}