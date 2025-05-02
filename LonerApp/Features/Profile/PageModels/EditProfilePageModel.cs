using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class EditProfilePageModel : BasePageModel
    {

        public bool IsNeedLoadUsersData = true;
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;        
        [ObservableProperty]
        private bool _isPreviewVisible;        
        [ObservableProperty]
        private bool _isEditVisible;
        [ObservableProperty]
        private ObservableCollection<AddPhotoModel> _addPhotos;
        [ObservableProperty]
        private string _aboutMeEditorValue;
        [ObservableProperty]
        private string _myGender;
        [ObservableProperty]
        private string _myInterest;
        [ObservableProperty]
        private string _myUniversity;
        [ObservableProperty]
        private string _workEditorValue;
        [ObservableProperty]
        //private UserModel _myProfile;
        private UserProfileDetailResponse _myProfile = new();
        public EditProfilePageModel(INavigationService navigationService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
        }

        public override async Task InitAsync(object? initData)
        {
            IsEditVisible = true;
            IsPreviewVisible = false;
            AddPhotos = new ObservableCollection<AddPhotoModel>(
                Enumerable.Range(1, 6).Select(_ => new AddPhotoModel()).ToList());
            await base.InitAsync(initData);
        }

        public override Task LoadDataAsync()
        {
            return base.LoadDataAsync();
        }

        [RelayCommand]
        async Task OnDonePressedAsync(object param)
        {
            if (DonePressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            var x = AboutMeEditorValue.Trim();
            //await NavigationService.PushToPageAsync<SettingPage>();
            await Task.Delay(100);
            IsBusy = false;
        }        

        [RelayCommand]
        async Task OnGotoSetupInterestAsync(object param)
        {
            if (DonePressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PushToPageAsync<SetupInterestPage>();
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnGotoSetupGenderAsync(object param)
        {
            if (DonePressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PushToPageAsync<SetupGenderPage>();
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnGotoSetupUniversityAsync(object param)
        {
            if (DonePressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PushToPageAsync<SetupUniversityPage>();
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnGotoDetailProfileAsync(object param)
        {
            if (GotoDetailProfileCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PushToPageAsync<DetailProfilePage>(param: MyProfile, isPushModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }

        public void ShowPreviewProfile()
        {
            if (MyInterest == null || AddPhotos == null || !MyInterest.Any() || !AddPhotos.Any())
                return;
            var interest = MyInterest.Split(',').Select(s => s.Trim()).ToList();
            MyProfile = new UserModel
            {
                Name = "John Doe 1",
                Age = 25,
                Status = "Single",
                ListImage = AddPhotos,
                Image = AddPhotos.FirstOrDefault(x => !x.IsDefaultImage).ImagePath,
                Interests = new ObservableCollection<string>(interest),
                University = MyUniversity,
                About = AboutMeEditorValue,
                Address = "Ha noi, Viet Nam",
                Gender = MyGender,
                Work = WorkEditorValue
            };
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
            if (!(await CheckRemoveOrUpdateImageAsync(itemSelected)))
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
                    return false;
                }
                else if ((string)choiceEdit == updatePhoto)
                {
                    return true;
                }

                return false;
            }

            return true;
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
            var olderLocalImagePath = itemSelected.ImagePath;
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
                        itemSelected.ImagePath = _currentLocalImage;
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
        async Task OnBackAsync(object param)
        {
            if (BackCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PopPageAsync(isPopModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }
    }
}
