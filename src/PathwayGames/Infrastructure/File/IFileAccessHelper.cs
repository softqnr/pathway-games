namespace PathwayGames.Infrastructure.File
{
    public interface IFileAccessHelper
    {
        string GetDBPathAndCreateIfNotExists(string databaseFilename);

        string GetMLPathAndCreateIfNotExists(string filename);
    }
}
