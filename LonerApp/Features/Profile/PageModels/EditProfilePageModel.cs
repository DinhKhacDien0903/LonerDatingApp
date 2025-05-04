using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;
using System.Collections.ObjectModel;
using System.Text;

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
        private UserProfileDetailResponse _myProfile = new();
        [ObservableProperty]
        private UserProfileDetailResponse _previewProfile = new();
        private IProfileService _profileService;
        private Cloudinary _cloudDinary;
        private CancellationTokenSource cancellationToastToken = new CancellationTokenSource();
        public EditProfilePageModel(
            INavigationService navigationService,
            IProfileService profileService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            _profileService = profileService;
            _cloudDinary = new Cloudinary(Environments.CLOUDINARY_URL);
            _cloudDinary.Api.Secure = true; ;
        }

        public override async Task InitAsync(object? initData)
        {
            if (initData is UserProfileDetailResponse user)
                MyProfile = user;
            IsEditVisible = true;
            IsPreviewVisible = false;
            AddPhotos = new ObservableCollection<AddPhotoModel>(
                Enumerable.Range(1, 7).Select(_ => new AddPhotoModel()).ToList());

            if (MyProfile != null)
            {
                AboutMeEditorValue = MyProfile?.About ?? "";
                MyGender = MyProfile.Gender ? "Nam" : "Nữ";
                MyUniversity = MyProfile?.University ?? "";
                WorkEditorValue = MyProfile?.Work ?? "";
                StringBuilder result = new();
                foreach (var item in MyProfile.Interests)
                {
                    result.Append($"{item}, ");
                }
                MyInterest = result.ToString();
                //AddPhotos = new ObservableCollection<AddPhotoModel>((MyProfile.Photos ?? []).Select(x => new AddPhotoModel { ImagePath = x }).ToList());
                int length = MyProfile.Photos?.Count() ?? 0;
                for (int i = 0; i < length; i++)
                {
                    AddPhotos[i].ImagePath = MyProfile.Photos.ElementAt(i);
                }
            }
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
            try
            {
                var interest = MyInterest.Split(',').Select(s => s.Trim()).ToList();
                if (MyInterest.Contains(""))
                {
                    var lastIndex = interest.Count - 1;
                    interest.RemoveAt(lastIndex);
                }
                var editProfile = new EditInforRequest
                {
                    UserId = MyProfile?.Id ?? "",
                    About = AboutMeEditorValue.Trim(),
                    University = MyUniversity.Trim(),
                    Work = WorkEditorValue.Trim(),
                    Gender = MyGender.Equals("Nam"),
                    Interests = interest,
                    Photos = [.. AddPhotos.Where(x => !x.IsDefaultImage).Select(x => x.ImagePath)]
                };

                var updateResult = await _profileService.UpdateUserInforAsync(new UpdateUserInforRequest
                {
                    EditRequest = editProfile
                });

                if (updateResult?.IsSuccess ?? false)
                    await ShowToast("Cập nhật thành công!");
                else
                    await ShowToast("Cập nhật thất bại!");
                await NavigationService.PopPageAsync();
                await Task.Delay(100);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                await AlertHelper.ShowErrorAlertAsync(new AlertConfigure
                {
                    Title = "Lỗi",
                    Message = "Có lỗi xảy ra trong quá trình cập nhật! Vui lòng thực hiện lại.",
                });
            }
            finally
            {
                IsBusy = false;
            }
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
            await NavigationService.PushToPageAsync<DetailProfilePage>(param: PreviewProfile, isPushModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }

        private async Task ShowToast(string content)
        {
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;

            var toast = Toast.Make(content, duration, fontSize);

            await toast.Show(cancellationToastToken.Token);
        }

        public void ShowPreviewProfile()
        {
            if (MyInterest == null || AddPhotos == null || !MyInterest.Any() || !AddPhotos.Any())
                return;

            var interest = MyInterest.Split(',').Select(s => s.Trim()).ToList();
            var lastIndex = interest.Count - 1;
            interest.RemoveAt(lastIndex);
            PreviewProfile = new UserProfileDetailResponse
            {
                UserName = MyProfile.UserName,
                Age = MyProfile.Age,
                Photos = AddPhotos.Where(x => !x.IsDefaultImage).Select(x => x.ImagePath),
                AvatarUrl = MyProfile.AvatarUrl,
                Interests = new ObservableCollection<string>(interest),
                University = MyUniversity,
                About = AboutMeEditorValue,
                Address = MyProfile.Address,
                Gender = MyProfile.Gender,
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

                //if (itemSelected.ImagePath != null && itemSelected.ImagePath != oldImagePath)
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
                    item.ImagePath = "blank_image.png";
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

        //private async Task<string> ProcessSelectedImageAsync(string _currentLocalImage, AddPhotoModel itemSelected)
        //{
        //    IsBusy = true;
        //    using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
        //    {
        //        _currentLocalImage = await CameraPluginExtensions.TrimImageAsync(stream, _currentLocalImage);
        //    }

        //    if (!string.IsNullOrEmpty(_currentLocalImage) && await UploadImageAsync(_currentLocalImage))
        //    {
        //        using (var stream = ServiceHelper.GetService<IDeviceService>().GetRotatedImageStream(_currentLocalImage))
        //        using (BinaryReader br = new BinaryReader(stream))
        //        {
        //            byte[] imageBytes = br.ReadBytes((int)stream.Length);
        //            //itemSelected.ImagePath = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        //        }
        //    }
        //    else
        //    {
        //        _currentLocalImage = string.Empty;
        //    }

        //    IsBusy = false;
        //    return _currentLocalImage;
        //}

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