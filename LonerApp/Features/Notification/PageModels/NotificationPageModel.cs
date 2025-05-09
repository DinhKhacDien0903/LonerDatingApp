using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;

namespace LonerApp.PageModels;
public partial class NotificationPageModel : BasePageModel
{
    [ObservableProperty]
    private bool _isVisibleNavigation;
    [ObservableProperty]
    private bool _hasBackButton;
    [ObservableProperty]
    private ObservableCollection<NotificationResponse> _notifications;
    private static string _currentUserId = string.Empty;
    private int _currentPage = 1;
    private const int PageSize = 30;
    public CollectionView _notificationCollection;
    private readonly INotificationManagerService _notificationService;
    private CancellationTokenSource cancellationToastToken = new CancellationTokenSource();
    public NotificationPageModel(
        INotificationManagerService notificationService,
        INavigationService navigationService)
        : base(navigationService, true)
    {
        IsVisibleNavigation = true;
        HasBackButton = true;
        _notificationService = notificationService;
    }

    public override async Task LoadDataAsync()
    {
        _currentUserId = UserSetting.Get(StorageKey.UserId);
        _currentUserId = "00bc5583-430e-455b-95b5-90ea51c14342";
        if (string.IsNullOrEmpty(_currentUserId))
            return;
        var response = await _notificationService.GetNotificationAsync(_currentPage, PageSize, _currentUserId);
        Notifications = [.. response?.Notifications?.Items ?? []];
        await base.LoadDataAsync();
    }

    [RelayCommand]
    async Task OnRemoveNotificationAsync(object param)
    {
        if (RemoveNotificationCommand.IsRunning || IsBusy)
            return;
        if (param is not NotificationResponse notification)
            return;

        notification.IsDeleted = true;
        Notifications.Remove(notification);
        var response = await _notificationService.RemoveNotification(new RemoveNotificationRequest
        {
            Notification = notification
        });
        if (response.IsSuccess)
            await ShowToast(response?.Message ?? "Notification removed successfully");
        else
            await ShowToast("Failed to remove notification");
        // ScrollToTop();
        await Task.Delay(100);
    }

    [RelayCommand]
    async Task OnReadNotificationAsync(object param)
    {
        if (ReadNotificationCommand.IsRunning || IsBusy)
            return;
        if (param is not NotificationResponse notification)
            return;

        notification.IsRead = true;
        await ResortNotifications();
        var response = await _notificationService.ReadNotification(new ReadNotificationRequest
        {
            Notification = notification
        });
        if (response.IsSuccess)
            await ShowToast(response?.Message ?? "Notification read successfully");
        else
            await ShowToast("Failed to read notification");
        ScrollToTop();
        await Task.Delay(100);
    }

    private async Task ResortNotifications()
    {
        var sortedNotifications = Notifications.OrderBy(x => x.IsRead).ThenByDescending(x => x.CreatedAt).ToList();
        Notifications.Clear();
        foreach (var sortedItem in sortedNotifications)
        {
            Notifications.Add(sortedItem);
            await Task.Delay(1);
        }
    }

    private async Task ShowToast(string content)
    {
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(content, duration, fontSize);

        await toast.Show(cancellationToastToken.Token);
    }

    private void ScrollToTop()
    {
        _notificationCollection.ScrollTo(0, position: ScrollToPosition.Start, animate: true);
    }

    [RelayCommand]
    async Task OnClearNotificationsAsync(object param)
    {
        if (Notifications.Count() < 1 || ClearNotificationsCommand.IsRunning || IsBusy)
            return;

        if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có chắc chắn muốn clear thông báo?", "Xác nhận."))
        {
            Notifications.Clear();
            var response = await _notificationService.ClearNotifications(_currentUserId);
            if (response.IsSuccess)
                await ShowToast(response?.Message ?? "Notification clear successfully");
            else
                await ShowToast("Failed to clear notification");
            await Task.Delay(100);
        }
    }

    [RelayCommand]
    async Task OnNotificationItemClickedAsync(object param)
    {
        if (NotificationItemClickedCommand.IsRunning || IsBusy)
            return;
        if (param is not NotificationResponse notification)
            return;

        var request = new UserChatModel()
        {
            UserId = notification.SenderId ?? "",
            MatchId = notification.RelatedId ?? ""
        };
        if (notification.Type == 2 && !string.IsNullOrEmpty(notification.RelatedId))
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await NavigationService.PushToPageAsync<MessageChatPage>(param: request);
            });
        }
    }

    [RelayCommand]
    async Task OnBackAsync(object param)
    {
        if (!IsBusy)
        {
            IsBusy = true;
            await NavigationService.PopPageAsync();
            IsBusy = false;
        }
    }
}