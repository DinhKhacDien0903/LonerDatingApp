using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace LonerApp.PageModels;

public partial class ReportPageModel : BasePageModel
{
    [ObservableProperty]
    private bool _isVisibleNavigation;
    [ObservableProperty]
    private bool _hasBackButton;
    [ObservableProperty]
    private ObservableCollection<ReportModel> _reportTemplates = new();
    [ObservableProperty]
    private string _moreInformationEditorValue;
    private string Reason = string.Empty;
    [ObservableProperty]
    private bool _isEnabledContinueButton;
    [ObservableProperty]
    private bool _isSendingProcess;
    private readonly IReportService _reportService;
    private string _reportedId = string.Empty;
    private CancellationTokenSource cancellationToastToken = new CancellationTokenSource();
    public ReportPageModel(
        INavigationService navigationService,
        IReportService reportService)
        : base(navigationService, true)
    {
        IsVisibleNavigation = true;
        HasBackButton = true;
        _reportService = reportService;
    }

    public override Task InitAsync(object? initData)
    {
        if (initData is string Id)
            _reportedId = Id;
        return base.InitAsync(initData);
    }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await LoadDistrictLocationAsync();
    }
    public async Task LoadDistrictLocationAsync()
    {
        IsBusy = true;
        try
        {
            if (ReportTemplates != null && ReportTemplates.Any())
                return;
            using var stream = await FileSystem.OpenAppPackageFileAsync("Report.json");
            using var reader = new StreamReader(stream);

            var reportJson = await reader.ReadToEndAsync();
            var reports = JsonConvert.DeserializeObject<ObservableCollection<ReportModel>>(reportJson);
            List<ReportModel> reportList = new();
            if (reports != null)
            {
                foreach (var report in reports)
                {
                    reportList?.Add(report);
                }
            }

            ReportTemplates = new ObservableCollection<ReportModel>(reportList ?? []);
        }
        catch (Exception ex)
        {
            await AlertHelper.ShowErrorAlertAsync("Can't load district data");
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task OnBackAsync(object param)
    {
        if (BackCommand.IsRunning || IsBusy)
            return;
        IsBusy = true;
        var isAgree = await AlertHelper.ShowConfirmationAlertAsync(
            "Bạn chắc chắn muốn huỷ báo cáo người dùng này?",
            "Xác nhận");
        if (isAgree)
            await NavigationService.PopPageAsync(isPopModal: false);
        await Task.Delay(100);
        IsBusy = false;
    }

    [RelayCommand]
    async Task OnCancelAsync(object param)
    {
        if (CancelCommand.IsRunning || IsBusy)
            return;
        try
        {
            IsBusy = true;
            var isAgree = await AlertHelper.ShowConfirmationAlertAsync(
                "Bạn chắc chắn muốn huỷ báo cáo người dùng này?",
                "Xác nhận");
            if (isAgree)
                await NavigationService.PopPageAsync(isPopModal: false);

            await Task.Delay(100);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task OnItemReasonClickedkAsync(object param)
    {
        if (ItemReasonClickedkCommand.IsRunning || IsBusy)
            return;
        if (param is not Reason reasonParam)
            return;
        try
        {
            IsBusy = true;
            foreach (var item in ReportTemplates)
            {
                if (item == null)
                    continue;
                foreach (var reason in item.Reasons)
                {
                    reason.IsSelected = false;
                }
            }

            reasonParam.IsSelected = true;
            Reason = reasonParam.Label_vi ?? "";
            if (!IsEnabledContinueButton)
                IsEnabledContinueButton = true;
            await Task.Delay(100);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task OnContinueClickedAsync(object param)
    {
        if (ContinueClickedCommand.IsRunning || IsBusy)
            return;
        if (!IsEnabledContinueButton)
            return;
        try
        {
            IsBusy = true;
            IsSendingProcess = true;
            await Task.Delay(100);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task OnSendClickedAsync(object param)
    {
        if (SendClickedCommand.IsRunning || IsBusy)
            return;
        if (!IsEnabledContinueButton)
            return;
        try
        {
            IsBusy = true;
            var requestDto = new ReportRequestDto
            {
                ReporterId = UserSetting.Get(StorageKey.UserId),
                ReportedId = _reportedId,
                Reason = Reason,
                TypeBlocked = 2,
                MoreInformation = MoreInformationEditorValue
            };

            var isSuccess = await _reportService.ReportAsync(new ReportRequest
            {
                Request = requestDto
            });

            if (isSuccess?.IsSuccess ?? false)
                await ShowToast("Báo cáo thành công");
            else
                await ShowToast("Báo cáo không thành công");
            await NavigationService.PopToRootAsync();
            await Task.Delay(100);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ShowToast(string content)
    {
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(content, duration, fontSize);

        await toast.Show(cancellationToastToken.Token);
    }

}