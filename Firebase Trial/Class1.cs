using System.IO;
using System.Threading.Tasks;
using Firebase.Storage;

namespace XamarinFirebase.Helper
{

    public class FirebaseStorageHelper
    {
        FirebaseStorage firebaseStorage = new FirebaseStorage("fir-demo-71fa5.appspot.com");

        public async Task<string> UploadFile(Stream fileStream, string fileName)
        {
            var imageUrl = await firebaseStorage
                .Child("MyDummyFolder")
                .Child(fileName)
                .PutAsync(fileStream);
            return imageUrl;
        }

        public async Task<string> GetFile(string fileName)
        {
            return await firebaseStorage
                .Child("MyDummyFolder")
                .Child(fileName)
                .GetDownloadUrlAsync();
        }
        public async Task DeleteFile(string fileName)
        {
            await firebaseStorage
                 .Child("MyDummyFolder")
                 .Child(fileName)
                 .DeleteAsync();

        }
    }
}