using Newtonsoft.Json;

namespace LonerApp.Helpers
{
    public enum StorageKey
    {
        PhoneCode,
        Language,
        Locale,
        CurrentTheme,
        CognitoId,
        IsAutoStart,
        LastReadMessage,
        PermissionRationaleStatus,
        IsFirstLogin,
        IsAcceptedCollectLogs,
        HasDisplayedLogsPermissionBox,
        IsOverrideInstallApp,
        IsFirstInstallApp,
        IsShowNotificationPermission,
        AutoTestJson,
        PhotosAndVideosPermission,
        FirstTimeGetDealers,
        TimesDisplayAlert,
        IsShowAppFeature,
        IsDealerPHDataInRealm,
        CurrentLocation,
        UserId,
        AccessToken,
        RefreshToken
    }

    public static partial class UserSetting
    {
        static readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public static string Get(StorageKey key)
        {
            return Get(key.ToString());
        }

        public static void Set(StorageKey key, string value)
        {
            Set(key.ToString(), value);
        }

        public static string Get(string key)
        {
            _cache.TryGetValue(key, out string? value);
            if(!String.IsNullOrEmpty(value))
            {
                return value;
            }

            _semaphoreSlim.Wait();
            try
            {
                Task.Run(async () =>
                {
                    value = await SecureStorage.Default.GetAsync(key);
                }).Wait();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            _cache[key] = value;
            return value;
        }

        private static void Set(string key, string value)
        {
            var isSuccessful = false;
            if(value == null)
            {
                isSuccessful = Remove(key);
            }
            else
            {
                _cache[key] = value;
                _semaphoreSlim.Wait();
                try
                {
                    Task.Run(async () =>
                    {
                        await SecureStorage.Default.SetAsync(key, value);
                    }).Wait();
                    isSuccessful = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    _semaphoreSlim.Release();
                }

                //TODO: Set language for app
                //if (isSuccessful && nameof(StorageKey.Locale).Equals(key))
                //{
                //    Constant.initialLanguageList();
                //}
            }
        }

        public static T GetObject<T>(StorageKey key)
            where T : class
        {
            var obj = Get(key);
            return obj != null ? JsonConvert.DeserializeObject<T>(obj) : null;
        }

        public static void SetObject(StorageKey key, object obj)
        {
            if (obj == null)
                Remove(key.ToString());
            else
            {
                string registrationJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
                Set(key, registrationJson);
            }
        }

        public static bool Remove(string key)
        {
            _cache.Remove(key);
            return SecureStorage.Default.Remove(key);
        }
    }
}