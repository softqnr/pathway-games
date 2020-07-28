using Foundation;
using PathwayGames.Infrastructure.File;
using PathwayGames.iOS.Infrastructure.File;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileAccessHelper))]
namespace PathwayGames.iOS.Infrastructure.File
{
    public class FileAccessHelper : IFileAccessHelper
    {
        public string GetDBPathAndCreateIfNotExists(string databaseFilename)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, databaseFilename);
            if (!System.IO.File.Exists(path))
            {
                var existingDb = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(databaseFilename)
                    , Path.GetExtension(databaseFilename));
                System.IO.File.Copy(existingDb, path);
            }
            return path;
        }

        public string GetMLPathAndCreateIfNotExists(string filename)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(documentsPath, filename);
            if (!System.IO.File.Exists(path))
            {
                var existingMLFile = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(filename)
                    , Path.GetExtension(filename));
                System.IO.File.Copy(existingMLFile, path);
            }
            return path;
        }
    }
}