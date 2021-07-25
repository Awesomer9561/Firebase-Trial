using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (path != null)
                {
                    fileIDLabel.Text = "";
                    fileNameLabel.Text = "";
                    await DisplayAlert("Success", path, "OK");
                }
            }
            else
            {
                await DisplayAlert("", "Please select a file", "ok");
            }
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