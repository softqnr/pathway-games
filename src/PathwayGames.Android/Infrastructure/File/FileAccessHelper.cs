using PathwayGames.Droid.Infrastructure.File;
using PathwayGames.Infrastructure.File;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileAccessHelper))]
namespace PathwayGames.Droid.Infrastructure.File
{
    public class FileAccessHelper : IFileAccessHelper
    {
        public string GetDBPathAndCreateIfNotExists(string databaseName)
        {
            var docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbFile = Path.Combine(docFolder, databaseName);

            if (!System.IO.File.Exists(dbFile))
            {
                // Copy from assets
                CopyAssetFileTo(databaseName, dbFile);
            }

            return dbFile;
        }

        public void CopyAssetFileTo(string assetFile, string destinationFileName)
        {
            using (var fileStream = new FileStream(destinationFileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Android.App.Application.Context.Assets.Open(assetFile).CopyTo(fileStream);
            }
        }
    }
}