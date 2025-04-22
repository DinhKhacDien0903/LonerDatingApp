using System.Text.Json;
namespace LonerApp.Helpers;

public static class ApiResponseHelper
{
    public static async Task<T?> HandleResponse<T>(HttpResponseMessage response)
    {
        if (response == null)
        {
            await AlertHelper.ShowErrorAlertAsync("Không nhận được phản hồi từ máy chủ.", "Lỗi");
            return default;
        }

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return default;
            }
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                await AlertHelper.ShowErrorAlertAsync("Lỗi khi xử lý dữ liệu từ máy chủ.", "Lỗi");
                return default;
            }
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                var errorObject = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await AlertHelper.ShowErrorAlertAsync(errorObject?.Error ?? "Dữ liệu không tồn tại", "Lỗi");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    await AlertHelper.ShowErrorAlertAsync(errorObject?.Error ?? "Yêu cầu không hợp lệ.", "Lỗi");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    await AlertHelper.ShowErrorAlertAsync("Đã có lỗi xảy ra ở máy chủ.", "Lỗi");
                }
                else
                {
                    await AlertHelper.ShowErrorAlertAsync($"Đã xảy ra lỗi: {response.StatusCode}", "Lỗi");
                }
            }
            catch (JsonException)
            {
                await AlertHelper.ShowErrorAlertAsync($"Lỗi khi xử lý thông tin lỗi từ máy chủ: {errorContent}", "Lỗi");
            }

            return default;
        }
    }
}

public class ErrorResponse
{
    public string? Error { get; set; }
}