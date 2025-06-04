using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using LonerApp.Features.Author.Services;
using LonerApp.Features.Author.Signup.Models;
using LonerApp.Models;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public class SetupNameValidator : AbstractValidator<SetupPageModel>
    {
        public SetupNameValidator()
        {
            RuleFor(vm => vm.NameValue)
                .FiledNotEmpty();
        }
    }

    public class DateValidator : AbstractValidator<SetupPageModel>
    {
        public DateValidator()
        {
            RuleFor(vm => vm.DateValue)
                .Date();
        }

    }

    public class MonthValidator : AbstractValidator<SetupPageModel>
    {
        public MonthValidator()
        {
            RuleFor(vm => vm.MonthValue)
                .Month();
        }

    }

    public class YearValidator : AbstractValidator<SetupPageModel>
    {
        public YearValidator()
        {
            RuleFor(vm => vm.YearValue)
                .Year();
        }

    }

    public partial class SetupPageModel : BasePageModel
    {
        [ObservableProperty]
        private bool _isOpened;
        [ObservableProperty]
        private string _entryValue = string.Empty;
        [ObservableProperty]
        private string _errorTextValue = string.Empty;
        [ObservableProperty]
        private string _nameValue = string.Empty;
        [ObservableProperty]
        private string _dateValue = string.Empty;
        [ObservableProperty]
        private string _monthValue = string.Empty;
        [ObservableProperty]
        private string _yearValue = string.Empty;
        [ObservableProperty]
        private string _universityValue = string.Empty;
        [ObservableProperty]
        private bool _isShowError;
        [ObservableProperty]
        private bool _isContinue;
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        private bool _isShowGenderInProfile;
        [ObservableProperty]
        private ObservableCollection<Gender> _gender;
        [ObservableProperty]
        private ObservableCollection<Interest> _interests;
        [ObservableProperty]
        private ObservableCollection<AddPhotoModel> _addPhotos;
        private List<Interest> SelectedInterests = new();
        private SetupNameValidator _nameValidator = new();
        private DateValidator _dateValidator = new();
        private MonthValidator _monthValidator = new();
        private YearValidator _yearValidator = new();
        private string _currentLocalImage;
        EditProfilePageModel? _editProfilePageModel;
        private readonly INavigationOtherShellService _navigationOtherShell;
        private readonly ISetupService _setupService;
        ContentPage? _previousPage;
        SettingPageModel? _settingPageModel;
        private Cloudinary _cloudDinary;
        private CancellationTokenSource cancellationToastToken = new CancellationTokenSource();

        public SetupPageModel(INavigationService navigationService,
            INavigationOtherShellService navigationOtherShell,
            ISetupService setupService)
            : base(navigationService, true)
        {
            _navigationOtherShell = navigationOtherShell;
            _setupService = setupService;
            _cloudDinary = new Cloudinary(Environments.CLOUDINARY_URL);
            _cloudDinary.Api.Secure = true; ;
        }

        public override async Task InitAsync(object? initData)
        {
            _previousPage = AppShell.Current?.CurrentPage as ContentPage;
            if (_previousPage != null)
            {
                _editProfilePageModel = _previousPage.BindingContext as EditProfilePageModel;
                _settingPageModel = _previousPage.BindingContext as SettingPageModel;
            }
            HasBackButton = true;
            IsVisibleNavigation = true;
            await base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            var currentPage = AppShell.Current?.CurrentPage;
            currentPage ??= (AppHelper.CurrentMainPage?.GetCurrentPage() as NavigationPage)?.CurrentPage;
            if (currentPage is SetupGenderPage || currentPage is SetupShowGenderForMe)
            {
                Gender = new ObservableCollection<Gender>
                {
                    new () { ID = 0, Value = "Nam" },
                    new () { ID = 1, Value = "Nữ" },
                    new () { ID = 2, Value = "Khác" }
                };
            }
            else if (currentPage is SetupInterestPage)
            {
                Interests = new ObservableCollection<Interest>();
                foreach (InterestEnum interestEnum in Enum.GetValues(typeof(InterestEnum)))
                {
                    Interests.Add(new Interest
                    {
                        ID = (int)interestEnum,
                        Value = interestEnum.GetDisplayName()
                    });
                }
            }
            else if (currentPage is SetupPhotosPage)
            {
                AddPhotos = new ObservableCollection<AddPhotoModel>(
                    Enumerable.Range(1, 4).Select(_ => new AddPhotoModel()).ToList());
            }
        }

        [RelayCommand]
        async Task OnNameContinueAsync(object param)
        {
            IsBusy = true;
            try
            {
                NameValue = NameValue.Trim();
                var validatorResult = _nameValidator.Validate(this);
                if (validatorResult.IsValid)
                {
                    IsShowError = false;
                    SetUpNameRequest request = new()
                    {
                        UserName = NameValue
                    };
                    var result = await _setupService.SetupNameAsync(request);
                    if (result != null && result.IsSuccess)
                    {
                        await ShowToast("Cập nhật thành công!");
                        await Task.Delay(100);
                        await _navigationOtherShell.NavigateToAsync<SetupDateOfBirthPage>();
                    }
                    else
                        await ShowToast(result?.Message ?? "Cập nhật thất bại!");
                }
                else
                {
                    IsShowError = true;
                    ErrorTextValue = validatorResult.Errors[0].ErrorMessage;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnDateOfBirthContinueAsync(object param)
        {
            try
            {
                IsBusy = true;
                var validationResults = new[]
                {
                    _dateValidator.Validate(this),
                    _monthValidator.Validate(this),
                    _yearValidator.Validate(this)
                };

                bool isAllValid = validationResults.All(result => result.IsValid);

                if (isAllValid)
                {
                    IsShowError = false;
                    SetUpDOBRequest request = new()
                    {
                        Dob = DateTime.ParseExact(
                            $"{DateValue.Trim()} {MonthValue.Trim()} {YearValue.Trim()}",
                            "dd MM yyyy", System.Globalization.CultureInfo.InvariantCulture)
                    };
                    var result = await _setupService.SetupDateOfBirthAsync(request);
                    if (result != null && result.IsSuccess)
                    {
                        await ShowToast("Cập nhật thành công!");
                        await Task.Delay(100);
                        await _navigationOtherShell.NavigateToAsync<SetupGenderPage>();
                    }
                    else
                        await ShowToast(result?.Message ?? "Cập nhật thất bại!");
                }
                else
                {
                    IsShowError = true;
                    var firstInvalidResult = validationResults.FirstOrDefault(result => !result.IsValid);
                    ErrorTextValue = firstInvalidResult?.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed";
                }
            }
            catch (Exception ex)
            {
                IsShowError = true;
                ErrorTextValue = I18nHelper.Get("Common_Error_DateOfBirth");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnGenderContinueAsync(object param)
        {
            try
            {
                IsBusy = true;
                var genderChoosed = Gender.Where(x => x.IsSelected).FirstOrDefault();
                if (_previousPage is EditProfilePage editPage)
                {
                    if (!string.IsNullOrEmpty(genderChoosed.Value))
                        _editProfilePageModel.MyGender = genderChoosed.Value;
                    await NavigationService.PopPageAsync(isPopModal: false);
                }
                else
                {
                    SetUpGenderRequest request = new()
                    {
                        Gender = genderChoosed.Value == "Nam" ? true : false
                    };
                    var result = await _setupService.SetupGenderAsync(request);
                    if (result != null && result.IsSuccess)
                    {
                        await ShowToast("Cập nhật thành công!");
                        await Task.Delay(100);
                        await _navigationOtherShell.NavigateToAsync<SetupShowGenderForMe>();
                    }
                    else
                        await ShowToast(result?.Message ?? "Cập nhật thất bại!");
                }
            }
            catch (Exception ex)
            {
                IsShowError = true;
                ErrorTextValue = I18nHelper.Get("Common_Error_DateOfBirth");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnGenderShowMeContinueAsync(object param)
        {
            try
            {
                IsBusy = true;
                if (_settingPageModel != null)
                {
                    _settingPageModel.ShowGenderValue = Gender.Where(x => x.IsSelected).FirstOrDefault()?.Value ?? "Nam";
                    await NavigationService.PopPageAsync();
                    return;
                }

                SetUpShowGenderRequest request = new()
                {
                    ShowGender = Gender.Where(x => x.IsSelected).FirstOrDefault()?.Value == "Nam"
                };
                var result = await _setupService.SetupGenderShowMeAsync(request);
                if (result != null && result.IsSuccess)
                {
                    await ShowToast("Cập nhật thành công!");
                    await Task.Delay(100);
                    await _navigationOtherShell.NavigateToAsync<SetupUniversityPage>();
                }
                else
                    await ShowToast(result?.Message ?? "Cập nhật thất bại!");
            }
            catch (Exception ex)
            {
                IsShowError = true;
                ErrorTextValue = I18nHelper.Get("Common_Error_DateOfBirth");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnUniversityContinueAsync(object param)
        {
            try
            {
                IsBusy = true;
                if (_previousPage is EditProfilePage editPage)
                {
                    if (!string.IsNullOrEmpty(UniversityValue))
                        _editProfilePageModel.MyUniversity = UniversityValue.Trim();
                    await NavigationService.PopPageAsync(isPopModal: false);
                }
                else
                {
                    SetUpUniversityRequest request = new()
                    {
                        University = UniversityValue.Trim()
                    };
                    var result = await _setupService.SetupUniversityAsync(request);
                    if (result != null && result.IsSuccess)
                    {
                        await ShowToast("Cập nhật thành công!");
                        await Task.Delay(100);
                        await _navigationOtherShell.NavigateToAsync<SetupInterestPage>();
                    }
                    else
                        await ShowToast(result?.Message ?? "Cập nhật thất bại!");
                }
            }
            catch (Exception ex)
            {
                IsShowError = true;
                ErrorTextValue = I18nHelper.Get("Common_Error_DateOfBirth");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnInterestContinueAsync(object param)
        {
            try
            {
                IsBusy = true;
                if (_previousPage is EditProfilePage editPage)
                {
                    var interestChoosed = Interests.Where(x => x.IsSelected).ToList();
                    if (interestChoosed.Any())
                        _editProfilePageModel.MyInterest = string.Join(", ", interestChoosed.Select(x => x.Value));
                    await NavigationService.PopPageAsync(isPopModal: false);
                }
                else
                {
                    SetUpInterestRequest request = new()
                    {
                        Interests = Interests.Where(x => x.IsSelected).Select(x => x.Value).ToList()
                    };
                    var result = await _setupService.SetupInterestsAsync(request);
                    if (result != null && result.IsSuccess)
                    {
                        await ShowToast("Cập nhật thành công!");
                        await Task.Delay(100);
                        await _navigationOtherShell.NavigateToAsync<SetupPhotosPage>();
                    }
                    else
                        await ShowToast(result?.Message ?? "Cập nhật thất bại!");
                }
            }
            catch (Exception ex)
            {
                IsShowError = true;
                ErrorTextValue = I18nHelper.Get("Common_Error_DateOfBirth");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task AddPhotosContinueAsync(object param)
        {
            try
            {
                IsBusy = true;
                SetUpPhotosRequest request = new()
                {
                    Photos = AddPhotos.Where(x => !x.IsDefaultImage).Select(x => x.ImagePath).ToList()
                };
                var result = await _setupService.SetupPhotosAsync(request);
                if (result != null && result.IsSuccess)
                {
                    await ShowToast("Cập nhật thành công!");
                    await Task.Delay(100);
                    await _navigationOtherShell.NavigateToAsync<SetupPhotosPage>();
                }
                else
                    await ShowToast(result?.Message ?? "Cập nhật thất bại!");
                await Task.Delay(100);
                App.RefreshApp();
                await Task.Delay(200);
            }
            catch (Exception ex)
            {
                IsShowError = true;
                ErrorTextValue = I18nHelper.Get("Common_Error_DateOfBirth");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        void OnGenderItemClicked(object param)
        {
            if (GenderContinueCommand.IsRunning)
            {
                return;
            }

            if (param is Gender genderSelected)
            {
                foreach (var item in Gender)
                {
                    item.IsSelected = false;
                }

                genderSelected.IsSelected = true;
            }

            IsContinue = true;
        }

        #region Handle Add Image
        [RelayCommand]
        async Task OnAddPhotoItemClickedAsync(object param)
        {
            if (AddPhotoItemClickedCommand.IsRunning || param is not AddPhotoModel itemSelected)
            {
                return;
            }
            IsBusy = true;
            if (await CheckRemoveOrUpdateImageAsync(itemSelected))
            {
                IsBusy = false;
                return;
            }

            string oldImagePath = itemSelected.ImagePath;
            string takePhoto = I18nHelper.Get("SetupPhotoPage_ImageDialog_TakePhoto");
            string choosePhoto = I18nHelper.Get("SetupPhotoPage_ImageDialog_ChoosePhoto");
            string[] buttons = { takePhoto, choosePhoto };
            var choice = await AppHelper.CurrentMainPage.DisplayActionSheet(
                                I18nHelper.Get("SetupPhotoPage_ImageDialog_Title"), I18nHelper.Get("Common_Cancel"), null, buttons);
            try
            {
                if (((string)choice).Equals(takePhoto))
                {
                    await OnExcuteTakePhotoAsync(itemSelected);
                }
                else if (((string)choice).Equals(choosePhoto))
                {
                    await OnExcuteChoosePhotoAsync(itemSelected);
                }

                if (itemSelected.ImagePath != null && itemSelected.ImagePath != oldImagePath)
                {
                    itemSelected.IconPath = "\uf6fe";
                }
            }
            finally
            {
                IsBusy = false;
            }

            if (!IsContinue && AddPhotos.Count(x => !x.IsDefaultImage) >= 2)
            {
                IsContinue = true;
            }
        }

        private async Task<bool> CheckRemoveOrUpdateImageAsync(AddPhotoModel item)
        {
            if (item.IconPath.Equals("\uf6fe"))
            {
                string removePhoto = I18nHelper.Get("SetupPhotoPage_ImageDialog_RemovePhoto");
                string updatePhoto = I18nHelper.Get("SetupPhotoPage_ImageDialog_UpdatePhoto");
                string[] choices = { removePhoto, updatePhoto };
                var choiceEdit = await AppHelper.CurrentMainPage.DisplayActionSheet(
                    I18nHelper.Get("SetupPhotoPage_ImageDialog_Title"), I18nHelper.Get("Common_Cancel"), null, choices);
                if ((string)choiceEdit == removePhoto)
                {
                    item.ImagePath = "blank_image.png";
                    item.IconPath = "\uf417";
                    return true;
                }
                else if ((string)choiceEdit == updatePhoto)
                {
                    return false;
                }
            }

            return false;
        }

        private async Task OnExcuteChoosePhotoAsync(AddPhotoModel itemSelected)
        {
            string olderLocalImagePath = itemSelected.ImagePath.ToString();
            var filename = Path.GetRandomFileName() + ".png";
            if (await this.AllowTakePhotoAsync())
            {
                var _currentLocalImage = await CameraPluginExtensions.CancelableChoosePhotoAsync(filename);
                IsBusy = true;

                var urlImageResult = await UploadImageAsync(_currentLocalImage);
                if (!string.IsNullOrEmpty(_currentLocalImage) && !string.IsNullOrEmpty(urlImageResult))
                {
                    itemSelected.ImagePath = urlImageResult;
                }
                else
                {
                    itemSelected.ImagePath = "blank_image.png";
                }

                IsBusy = false;
            }
        }

        // private async Task<string> ProcessSelectedImageAsync(string _currentLocalImage, AddPhotoModel itemSelected)
        // {
        //     IsBusy = true;
        //     using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
        //     {
        //         _currentLocalImage = await CameraPluginExtensions.TrimImageAsync(stream, _currentLocalImage);
        //     }

        //     if (!string.IsNullOrEmpty(_currentLocalImage) && await UploadImageAsync(_currentLocalImage))
        //     {
        //         using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
        //         using (BinaryReader br = new BinaryReader(stream))
        //         {
        //             byte[] imageBytes = br.ReadBytes((int)stream.Length);
        //             //itemSelected.ImagePath = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        //         }
        //     }
        //     else
        //     {
        //         _currentLocalImage = string.Empty;
        //     }

        //     IsBusy = false;
        //     return _currentLocalImage;
        // }

        private async Task OnExcuteTakePhotoAsync(AddPhotoModel itemSelected)
        {
            IsBusy = true;
            var olderLocalImagePath = itemSelected.ImagePath;
            if (await this.AllowTakePhotoAsync())
            {
                var filename = Path.GetRandomFileName() + ".png";
                var _currentLocalImage = await CameraPluginExtensions.CancelableTakePhotoAsync(filename);
                var urlImageResult = await UploadImageAsync(_currentLocalImage);
                if (!string.IsNullOrEmpty(_currentLocalImage) && !string.IsNullOrEmpty(urlImageResult))
                {
                    itemSelected.ImagePath = urlImageResult;
                }
                else
                {
                    itemSelected.ImagePath = "blank_image.png";
                }
            }

            IsBusy = false;
        }

        private async Task<string> UploadImageAsync(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new CloudinaryDotNet.FileDescription(imagePath),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true
                };
                var uploadResult = (await _cloudDinary.UploadAsync(uploadParams)).JsonObj;
                return (uploadResult["secure_url"]?.ToString() ?? string.Empty);
            }

            return string.Empty;
        }
        #endregion

        [RelayCommand]
        void OnInterestItemClicked(object param)
        {
            if (GenderContinueCommand.IsRunning || param is not Interest interestSelected)
            {
                return;
            }

            interestSelected.IsSelected = !interestSelected.IsSelected;
            if (interestSelected.IsSelected)
                SelectedInterests.Add(interestSelected);
            else
                SelectedInterests.Remove(interestSelected);

            IsContinue = SelectedInterests.Count >= 5;
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (!IsBusy)
            {
                if (AppShell.Current != null)
                    await NavigationService.PopPageAsync(isPopModal: false);
                else
                    await _navigationOtherShell.GoBackAsync();
            }
        }

        private async Task ShowToast(string content)
        {
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;

            var toast = Toast.Make(content, duration, fontSize);

            await toast.Show(cancellationToastToken.Token);
        }

        [RelayCommand]
        async Task OnSkipAsync(object param)
        {
            if (!IsBusy)
                // await NavigationService.PushToPageAsync<SetupPhotosPage>();
                await _navigationOtherShell.NavigateToAsync<SetupPhotosPage>();
        }
    }
}