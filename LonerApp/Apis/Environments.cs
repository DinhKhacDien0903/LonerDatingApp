namespace LonerApp.Apis;

public class Environments
{
    // Twilio
    public const string TWILIO_ACCOUNT_SID = "AC42be1218f22e662224f57255a40e61db";
    public const string TWILIO_AUTH_TOKEN = "cae95aa66aa256c7c192200f3d2232ec";
    public const string TWILIO_PATH_SERVICE_SID = "VA197516f6d68a53f646a7274fd2f3cadd";

    // URL Server
    public const string URl_SERVER_HTTPS_EMULATOR = "https://10.0.2.2:7165/api/";
    public const string URl_SERVER_HTTP_EMULATOR = "http://10.0.2.2:5099/api/";
    public const string URl_SERVER_HTTPS_DEVICE_4G = "https://192.168.43.14:7165/api/";
    public const string URl_SERVER_HTTPS_DEVICE_WIFI = "https://192.168.43.14:7165/api/";
    public const string URl_SERVER_HTTP_DEVICE = "https://192.168.43.14:5099/api/";

    //URL ChatHub Connection
    public const string URl_SERVER_HTTPS_DEVICE_WIFI_CHAT_HUB = "https://192.168.43.14:7165/chat";
    public const string URl_SERVER_HTTPS_DEVICE_WIFI_NOTIFICATION_HUB = "https://192.168.43.14:7165/notification";
    public const string URl_SERVER_HTTPS_EMULATOR_CHAT_HUB = "https://10.0.2.2:7165/chat";
    public const string URl_SERVER_HTTPS_EMULATOR_NOTIFICATION_HUB = "https://10.0.2.2:7165/notification";

    //Cloudinary
    public const string CLOUDINARY_CLOUD_NAME = "de0werx80";
    public const string CLOUDINARY_API_KEY = "696268286966162";
    public const string CLOUDINARY_API_SECRET = "szRw2jLYY8W09IgHeNf7l3-KOwM";
    public const string CLOUDINARY_URL = $"cloudinary://{CLOUDINARY_API_KEY}:{CLOUDINARY_API_SECRET}@{CLOUDINARY_CLOUD_NAME}";
}

public static class EnvironmentsExtensions
{
    //Endpoints
    public const string ENDPOINT_GET_PROFILES = "Swipe/profiles";
    public const string ENDPOINT_GET_PROFILE_DETAIL = "User/profile-detail";
    public const string ENDPOINT_SEND_MAIL_OTP = "Auth/send-mail-otp";
    public const string ENDPOINT_VERIFY_AND_REGISTER_MAIL = "Auth/verify-mail-otp-and-register";
    public const string ENDPOINT_SWIPE_USER = "Swipe/swipe-user";
    public const string ENDPOINT_GET__MESSAGE_MATCHED = "Message/get-user-matched-active";
    public const string ENDPOINT_GET__USER_MESSAGES = "Message/get-user-message";
    public const string ENDPOINT_GET_MESSAGES = "Message/get-messages";
    public const string ENDPOINT_UPDATE_LOCATION = "User/update-Location";
    public const string ENDPOINT_GET_BY_LOCATION_RADIUS = "User/get-by-location-radius";

    //Query Params
    public const string QUERY_PARAMS_PAGINATION_REQUEST = "?PaginationRequest.UserId=";
    public const string QUERY_PARAMS_USER_ID = "?UserId=";
}