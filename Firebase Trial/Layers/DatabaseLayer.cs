using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Firebase_Trial
{
    public class DatabaseLayer
    {
        static Lazy<SQLiteAsyncConnection> lazy = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DBPath, Constants.Flags);
        });
        //Instantiating the Database
        public SQLiteAsyncConnection SQLiteAsync = lazy.Value;


        //Create table
        public void Createtable()
        {
            if (!SQLiteAsync.TableMappings.Any(m => m.TableName == typeof(CollectionModel).Name))
            {
                SQLiteAsync.CreateTableAsync<CollectionModel>();
            }
        }

        //Read table
        public Task<List<CollectionModel>> DisplayImage()
        {
            return SQLiteAsync.Table<CollectionModel>().ToListAsync();
        }

        //Update/Save image
        public void SaveImage(CollectionModel images)
        {
            if (images.ID == 0)
                SQLiteAsync.InsertAsync(images);
            else
                SQLiteAsync.UpdateAsync(images);
        }

        //Delete image
        public void DeleteImage(CollectionModel images)
        {
            SQLiteAsync.DeleteAsync(images);
        }
        
    }

    //Constants class
    public static class Constants
    {
        public const string DatabaseFilename = "UploadedImages.db3";
        public const SQLite.SQLiteOpenFlags Flags =
       // open the database in read/write mode
       SQLite.SQLiteOpenFlags.ReadWrite |
       // create the database if it doesn't exist
       SQLite.SQLiteOpenFlags.Create |
       // enable multi-threaded database access
       SQLite.SQLiteOpenFlags.SharedCache;

        public static string DBPath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
    }
}
