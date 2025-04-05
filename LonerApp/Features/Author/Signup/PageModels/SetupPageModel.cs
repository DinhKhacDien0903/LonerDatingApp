using CommunityToolkit.Mvvm.Input;
using FluentValidation;
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

        public SetupPageModel(INavigationService navigationService)
        : base(navigationService, true)
        {
        }

        public override async Task InitAsync(object? initData)
        {
            await base.InitAsync(initData);
            HasBackButton = true;
            IsVisibleNavigation = true;
        }

        public override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            var currentPage = AppShell.Current.CurrentPage;
            var currentPage1 = AppHelper.CurrentMainPage;
            if (currentPage is SetupGenderPage || currentPage is SetupShowGenderForMe)
            {
                Gender = new ObservableCollection<Gender>
                {
                    new () { ID = 0, Value = "Woman" },
                    new () { ID = 1, Value = "Man" },
                    new () { ID = 2, Value = "Other" }
                };
            }
            else if (currentPage is SetupInterestPage)
            {
                Interests = new ObservableCollection<Interest>
                {
                    new Interest { ID = 1, Value = "Âm nhạc" },
                    new Interest { ID = 2, Value = "Thể thao" },
                    new Interest { ID = 3, Value = "Du lịch" },
                    new Interest { ID = 4, Value = "Nấu ăn" },
                    new Interest { ID = 5, Value = "Lập trình" },
                    new Interest { ID = 6, Value = "Chụp ảnh" },
                    new Interest { ID = 7, Value = "Vẽ tranh" },
                    new Interest { ID = 8, Value = "Đọc sách" },
                    new Interest { ID = 9, Value = "Xem phim" },
                    new Interest { ID = 10, Value = "Chơi game" },
                    new Interest { ID = 11, Value = "Viết lách" },
                    new Interest { ID = 12, Value = "Học ngoại ngữ" },
                    new Interest { ID = 13, Value = "Đi bộ" },
                    new Interest { ID = 14, Value = "Yoga" },
                    new Interest { ID = 15, Value = "Thời trang" },
                    new Interest { ID = 16, Value = "Câu cá" },
                    new Interest { ID = 17, Value = "Cắm trại" },
                    new Interest { ID = 18, Value = "Khiêu vũ" },
                    new Interest { ID = 19, Value = "Làm vườn" },
                    new Interest { ID = 20, Value = "Chơi nhạc cụ" }
                };
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
            NameValue = NameValue.Trim();
            var validatorResult = _nameValidator.Validate(this);
            if (validatorResult.IsValid)
            {
                IsShowError = false;
                await NavigationService.PushToPageAsync<SetupDateOfBirthPage>(isPushModal: false);
            }
            else
            {
                IsShowError = true;
                ErrorTextValue = validatorResult.Errors[0].ErrorMessage;
            }
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnDateOfBirthContinueAsync(object param)
        {
            try
            {
                IsBusy = true;
                NameValue = NameValue.Trim();
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
                    //TODO: Set param date to API
                    await NavigationService.PushToPageAsync<SetupGenderPage>(isPushModal: false);
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
                await NavigationService.PushToPageAsync<SetupShowGenderForMe>(isPushModal: false);
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
                await NavigationService.PushToPageAsync<SetupUniversityPage>(isPushModal: false);
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
                await NavigationService.PushToPageAsync<SetupInterestPage>(isPushModal: false);
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
                await NavigationService.PushToPageAsync<SetupPhotosPage>(isPushModal: false);
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
                await NavigationService.PushToPageAsync<SetupDateOfBirthPage>(isPushModal: false);
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

            ImageSource oldImagePath = itemSelected.ImagePath;
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
                    item.ImagePath = ImageSource.FromFile("blank_image.png");
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
                if (_currentLocalImage != null)
                {
                    _currentLocalImage = await ProcessSelectedImageAsync(_currentLocalImage, itemSelected);
                }

                if (string.IsNullOrEmpty(_currentLocalImage))
                {
                    _currentLocalImage = olderLocalImagePath;
                }
            }
        }

        private async Task<string> ProcessSelectedImageAsync(string _currentLocalImage, AddPhotoModel itemSelected)
        {
            IsBusy = true;
            using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
            {
                _currentLocalImage = await CameraPluginExtensions.TrimImageAsync(stream, _currentLocalImage);
            }

            if (!string.IsNullOrEmpty(_currentLocalImage) && await UploadImageAsync(_currentLocalImage))
            {
                using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
                using (BinaryReader br = new BinaryReader(stream))
                {
                    byte[] imageBytes = br.ReadBytes((int)stream.Length);
                    itemSelected.ImagePath = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
            }
            else
            {
                _currentLocalImage = string.Empty;
            }

            IsBusy = false;
            return _currentLocalImage;
        }

        private async Task OnExcuteTakePhotoAsync(AddPhotoModel itemSelected)
        {
            IsBusy = true;
            ImageSource olderLocalImagePath = itemSelected.ImagePath;
            if (await this.AllowTakePhotoAsync())
            {
                var filename = Path.GetRandomFileName() + ".png";
                var _currentLocalImage = await CameraPluginExtensions.CancelableTakePhotoAsync(filename);
                if (!string.IsNullOrEmpty(_currentLocalImage))
                {
                    using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
                    {
                        _currentLocalImage = await CameraPluginExtensions.TrimImageAsync(stream, _currentLocalImage);
                    }
                }

                if (!string.IsNullOrEmpty(_currentLocalImage) && await UploadImageAsync(_currentLocalImage))
                {
                    using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        byte[] imageBytes = br.ReadBytes((int)stream.Length);
                        itemSelected.ImagePath = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    }
                }
                else
                {
                    itemSelected.ImagePath = olderLocalImagePath;
                }
            }

            IsBusy = false;
        }

        private async Task<bool> UploadImageAsync(string imagePath)
        {
            var isUpLoadImage = await AlertHelper.ShowConfirmationAlertAsync(new AlertConfigure
            {
                Title = I18nHelper.Get("SetupPhotoPage_SubmitPicture_Title"),
                Message = I18nHelper.Get("SetupPhotoPage_SubmitPicture_Message")
            });

            if (isUpLoadImage)
            {
                //TODO: handle upload image in server

                //_mailInquiry = await DataService.GetMailInquiryAsync();
                //EncryptImageModel encrypt;
                //imagePath = await ImageHelper.ImageToBase64Async(imagePath);
                //while (!(encrypt = await DataService.EncryptImageAsync([imagePath], _mailInquiry.InquiryNo)).IsSuccess)
                //{
                //    if (!await AlertHelper.ShowConfirmationAlertAsync(new AlertConfigure()
                //    {
                //        Title = I18nHelper.Get("PhotoRegistrationPage_ReUploadDialog_Title"),
                //        Message = I18nHelper.Get("PhotoRegistrationPage_ReUploadDialog_Message"),
                //        OK = I18nHelper.Get("PhotoRegistrationPage_ReUploadDialog_ReUpload")
                //    }))
                //    {
                //        return false;
                //    }
                //}

                //_imageUrl = encrypt.ImageUrl;
                return true;
            }

            return false;
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
                await NavigationService.PopPageAsync(isPopModal: false);
        }

        [RelayCommand]
        async Task OnSkipAsync(object param)
        {
            if (!IsBusy)
                await NavigationService.PushToPageAsync<SetupPhotosPage>();
        }
    }
}