using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinFirebase.Helper;

namespace Firebase_Trial
{
    public partial class MainPage : ContentPage
    {
        FirebaseStorageHelper firebaseStorageHelper = new FirebaseStorageHelper();
        string fileName;

        MediaFile file;
        public MainPage()
        {
            InitializeComponent();
        }
        private async void BtnUpload_Clicked(object sender, EventArgs e)
        {
            if (file != null)
            {
                selectanImgLabel.IsVisible = false;
                imageViewer.IsVisible = false;
                uploadingIndicator.IsRunning = true;

                fileName = CustomFileName(Path.GetFileName(file.Path));
                ImgNameLabel.Text = fileName;
                await firebaseStorageHelper.UploadFile(file.GetStream(), fileName);

                //Saving to local database
                SaveImage();
                uploadingIndicator.IsRunning = false;
                await DisplayAlert("", "Uploaded " + fileName + " successfully", "ok");
                resetFrame();
            }
            else
            {
                await DisplayAlert("", "Please select a file", "ok");
            }
        }

        private void SaveImage()
        {
            CollectionModel imageFile = new CollectionModel()
            {
                imageUrl = file.GetStream().ToString(),
                imgName = fileName,
                Timestamp = DateTime.Now.ToString("d MMM yyyy  h:mm tt")
            };
            App.databaseLayer.SaveImage(imageFile);
        }

        private void resetFrame()
        {
            imageViewer.Source = "";
            ImgNameLabel.Text = "";
            imageViewer.IsVisible = true;
            selectanImgLabel.IsVisible = true;
            fileNameEntry.Text = "";
            file = null;
        }

        private async void BtnDelete_Clicked(object sender, EventArgs e)
        {
            await firebaseStorageHelper.DeleteFile(fileNameEntry.Text);

            await DisplayAlert("Success", "Deleted", "OK");
        }

        private async void BtnDownload_Clicked(object sender, EventArgs e)
        {
            string path = await firebaseStorageHelper.GetFile(fileNameEntry.Text);
            if (path != null)
            {
                await DisplayAlert("Success", path, "OK");
            }

        }

        private async void BtnPick_Clicked(object sender, EventArgs e)
        {
            await CheckPermissions();
        }

        private async Task<PermissionStatus> CheckPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (status == PermissionStatus.Granted)
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // or general UTType values
                    { DevicePlatform.Android, new[] { "*/*" } }
                });

                var options = new PickOptions
                {
                    PickerTitle = "Please select a comic file",
                    FileTypes = customFileType,
                };
                PickImage();
                return status;
            }

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.StorageRead>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.StorageRead>();

            return status;
        }
        private async void PickImage()
        {
            await CrossMedia.Current.Initialize();
            try
            {
                file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });
                if (file == null)
                    return;
                ImgNameLabel.Text = Path.GetFileName(file.Path); //Setting the filename
                imageViewer.Source = ImageSource.FromStream(() => //Setting the image
                {
                    var imageStream = file.GetStream();
                    return imageStream;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private string CustomFileName(string OrginalFileName)
        {
            if (string.IsNullOrEmpty(fileNameEntry.Text)) //If empty
                return OrginalFileName;
            else                                          //If not empty
                return fileNameEntry.Text;
        }

        private void ViewFiles(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ViewUploads());
        }
    }
}
