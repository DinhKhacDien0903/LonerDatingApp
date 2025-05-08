using System.Collections.ObjectModel;
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
    private readonly INotificationManagerService _notificationService;
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
        _currentUserId = "00289d2b-73dd-4a70-9306-654e0495f47a";
        if (string.IsNullOrEmpty(_currentUserId))
            return;
        var response = await _notificationService.GetNotificationAsync(_currentPage, PageSize, _currentUserId);
        Notifications = [.. response?.Notifications?.Items ?? []];
        await base.LoadDataAsync();
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