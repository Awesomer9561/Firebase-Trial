using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinFirebase.Helper;

namespace Firebase_Trial
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewUploads : ContentPage
    {
        FirebaseStorageHelper firebaseStorageHelper = new FirebaseStorageHelper();
        CollectionModel imageFile;

        ObservableCollection<CollectionModel> imageCollection { get; set; }
        public ViewUploads()
        {
            InitializeComponent();
            App.databaseLayer.Createtable();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            var imagedb = App.databaseLayer.DisplayImage().Result;
            if (imagedb != null)
            {
                imageCollection = new ObservableCollection<CollectionModel>(imagedb);
                UploadsCollection.ItemsSource = imageCollection;
            }
        }
        private void UploadsCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var previous = e.PreviousSelection;
            var current = e.CurrentSelection.FirstOrDefault() as CollectionModel;
            imageFile = current;

            fileIDLabel.Text = current.ID.ToString() + ".  ";
            fileNameLabel.Text = current.imgName;
        }

        private async void downloadFile(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(fileNameLabel.Text))
            {
                string path = await firebaseStorageHelper.GetFile(fileNameLabel.Text);


                string[] fileDetails = DownloadImage(path);
                string fileName = fileDetails[0], filePath = fileDetails[1];
                if (fileDetails != null)
                {
                    fileIDLabel.Text = "";
                    fileNameLabel.Text = "";

                    imageFile.imageUrl = filePath;
                    App.databaseLayer.SaveImage(imageFile);
                    await DisplayAlert("Success", filePath, "OK");
                    await App.Current.MainPage.Navigation.PushAsync(new ViewDownloadedImage(imageFile));
                }
            }
            else
            {
                await DisplayAlert("", "Please select a file", "ok");
            }
        }
        public string[] DownloadImage(string URL)
        {
            WebClient webClient = new WebClient();

            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Images", "temp");
            string fileName = URL.ToString().Split('/').Last();
            string filePath = Path.Combine(folderPath, fileName);

            webClient.DownloadDataCompleted += (s, e) =>
            {
                Directory.CreateDirectory(folderPath);

                File.WriteAllBytes(filePath, e.Result);
            };
            Uri uri = new Uri(URL);
            webClient.DownloadDataAsync(uri);

            string[] fileDetails = { fileNameLabel.Text, filePath };
            return fileDetails;
        }


        private async void deleteFile(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(fileNameLabel.Text))
            {
                await firebaseStorageHelper.DeleteFile(fileNameLabel.Text);
                App.databaseLayer.DeleteImage(imageFile);
                fileIDLabel.Text = "";
                fileNameLabel.Text = "";
                imageCollection.Clear();
                OnAppearing();

                await DisplayAlert("", "Deleted file", "OK");
            }
            else
            {
                await DisplayAlert("", "Please select a file", "ok");
            }
        }
    }
}