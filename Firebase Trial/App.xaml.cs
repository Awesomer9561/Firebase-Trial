using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Firebase_Trial
{
    public partial class App : Application
    {
        public static DatabaseLayer database;
        public static DatabaseLayer databaseLayer
        {
            get
            {
                if (database == null)
                {
                    database = new DatabaseLayer();
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
            //MainPage = new NavigationPage(new ViewUploads());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
