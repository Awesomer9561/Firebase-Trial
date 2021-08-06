using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinFirebase.Helper;

namespace Firebase_Trial
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewDownloadedImage : ContentPage
    {
        FirebaseStorageHelper firebaseStorageHelper = new FirebaseStorageHelper();
        CollectionModel imageFile;
        public ViewDownloadedImage(CollectionModel ImageFile)
        {
            InitializeComponent();
            imageFile = ImageFile;
            imageViewer.Source = imageFile.imageUrl;
            ImageName.Text = imageFile.imgName;
        }

        private async void deleteFile(object sender, EventArgs e)
        {
            await firebaseStorageHelper.DeleteFile(ImageName.Text);
            App.databaseLayer.DeleteImage(imageFile);

            await DisplayAlert("", "Deleted file", "OK");
            App.Current.MainPage.Navigation.PopAsync();
        }

        private async void getLocation(object sender, EventArgs e)
        {
            await DisplayAlert("File cloud location", imageFile.imageUrl, "OK");

        }
    }
}