using System.IO;
using Android.Content;
using Android.Support.V4.Content;
using PathwayGames.Droid.Services.Share;
using PathwayGames.Services.Share;
using Xamarin.Forms;

[assembly: Android.App.Permission(Name = "android.permission.READ_EXTERNAL_STORAGE")]
[assembly: Dependency(typeof(Share))]
namespace PathwayGames.Droid.Services.Share
{
    public class Share : IShare
    {
        public void ShareFile(string title, string message, string filePath)
        {
            var extension = Path.GetExtension(filePath.ToLower());  
            var contentType = string.Empty;

            // Map ContentTypes
            switch (extension)
            {
                case ".xlsx":
                    contentType = "application/excel";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                default:
                    contentType = "application/octetstream";
                    break;
            }

            // Use file provider to get file URI
            var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context,
                    Android.App.Application.Context.ApplicationContext.PackageName + ".fileprovider",
                    new Java.IO.File(filePath));

            //if (!filePath.StartsWith("file://"))
            //{
            //    filePath = string.Format("file://{0}", filePath);
            //}
            
            //var fileUri = Android.Net.Uri.Parse(filePath);

            var intent = new Intent();
            intent.SetAction(Intent.ActionSend);
            intent.SetFlags(ActivityFlags.ClearTop);
            intent.SetFlags(ActivityFlags.NewTask);
            intent.SetType(contentType);
            intent.PutExtra(Intent.ExtraStream, fileUri);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);

            var chooserIntent = Intent.CreateChooser(intent, title ?? string.Empty);
            chooserIntent.SetFlags(ActivityFlags.ClearTop);
            chooserIntent.SetFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(chooserIntent);
        }
    }
}