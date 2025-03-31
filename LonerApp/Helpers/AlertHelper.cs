namespace LonerApp.Helpers
{
    public class AlertHelper
    {
        private static Page _currentPage
        {
            get
            {
                return AppHelper.CurrentMainPage;
            }
        }

        public static Task<bool> ShowConfirmationAlertAsync(string message, string title = null)
        {
            return ShowConfirmationAlertAsync(new AlertConfigure
            {
                Message = message,
                Title = title
            });
        }

        public static Task ShowErrorAlertAsync(string message, string title = null)
        {
            return ShowErrorAlertAsync(new AlertConfigure
            {
                Title = title,
                Message = message
            });
        }

        public static Task ShowErrorAlertAsync(AlertConfigure alertConfigure)
        {
            return _currentPage.DisplayAlert(alertConfigure.Title ?? I18nHelper.Get("Common_Text_Error"), alertConfigure.Message, alertConfigure.OK);
        }

        public static Task<bool> ShowConfirmationAlertAsync(AlertConfigure alertConfigure)
        {
            return _currentPage.DisplayAlert(
                           alertConfigure.Title ?? I18nHelper.Get("Common_Text_Confirmation"),
                           alertConfigure.Message,
                           alertConfigure.OK,
                           alertConfigure.Cancel);
        }
    }

    public class AlertConfigure
    {
        public string OK { get; set; } = I18nHelper.Get("Common_OK");
        public string Cancel { get; set; } = I18nHelper.Get("Common_Cancel");
        private string? _title;
        public string? Title
        {
            get
            {
                if (_title != null)
                    _title = _title.TrimEnd('.');
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public string? Message { get; set; }
    }
}