using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Core
{
    public class CoreIO
    {
        private CoreIO()
        {
        }

        private static readonly Lazy<CoreIO> lazy = new Lazy<CoreIO>(() => new CoreIO());

        public static CoreIO Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private const string AppFolderName = "Rediscovery";

        public string DefaultDirectory => System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        public string GetExternalDirectoryDocument()
        {
            var folder = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath, AppFolderName);
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);
            return folder;
        }

        public string GetExternalDirectoryDownloads()
        {
            var folder = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, AppFolderName);
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);
            return folder;
        }

        public string GetExternalDirectoryPictures()
        {
            var folder = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath, AppFolderName);
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);
            return folder;
        }

        public void ClearExternalDirectories()
        {
            try
            {
                System.IO.Directory.Delete(GetExternalDirectoryPictures());
                System.IO.Directory.Delete(GetExternalDirectoryDownloads());
                System.IO.Directory.Delete(GetExternalDirectoryDocument());
            } catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
        }
    }
}