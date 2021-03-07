using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
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
        private const string DeviceFeatureThumbnail = "featurethumbnails";

        public string DefaultDirectory => System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        public string DeviceDirectory(Guid deviceId)
        {
            var idFolder = deviceId.ToSafeString();
            var path = Path.Combine(DefaultDirectory, idFolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public string DeviceFeatureThumbnailDirectory(Guid deviceId)
        {
            var path = Path.Combine(DeviceDirectory(deviceId), DeviceFeatureThumbnail);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public string DeviceFeatureDirectory(Guid deviceId, Guid featureId)
        {
            var idFolder = featureId.ToSafeString();
            var path = Path.Combine(DeviceDirectory(deviceId), idFolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public Android.Net.Uri AddPublicFile(string path, string title = null, string description = null)
        {
            try
            {
                string name = null;
                var fileInfo = new System.IO.FileInfo(path);
                if (string.IsNullOrWhiteSpace(title))
                    name = System.IO.Path.GetFileNameWithoutExtension(path);
                else
                    name = title;
                var resolver = Application.Context.ContentResolver;
                var values = new ContentValues();
                Android.Net.Uri newUri = null;

                var mime = GetMimeType(path);
                if (mime.ToLower().StartsWith("image"))
                {
                    newUri = resolver.Insert(MediaStore.Images.Media.ExternalContentUri, values);
                    values.Put(MediaStore.Images.ImageColumns.Title, name);
                    values.Put(MediaStore.Images.ImageColumns.DisplayName, name);
                    values.Put(Android.Provider.MediaStore.Images.ImageColumns.Size, fileInfo.Length);
                    if (!string.IsNullOrWhiteSpace(description))
                        values.Put(MediaStore.Images.ImageColumns.Description, description);
                    values.Put(MediaStore.Images.ImageColumns.MimeType, mime);

                    // Add the date meta data to ensure the image is added at the front of the gallery
                    values.Put(MediaStore.Images.ImageColumns.DateAdded, Java.Lang.JavaSystem.CurrentTimeMillis());
                    values.Put(MediaStore.Images.ImageColumns.DateTaken, Java.Lang.JavaSystem.CurrentTimeMillis());
                } else if (mime.ToLower().StartsWith("video"))
                {
                    newUri = resolver.Insert(MediaStore.Video.Media.ExternalContentUri, values);
                    values.Put(MediaStore.Video.VideoColumns.Title, name);
                    values.Put(MediaStore.Video.VideoColumns.DisplayName, name);
                    values.Put(Android.Provider.MediaStore.MediaColumns.Size, fileInfo.Length);
                    if (!string.IsNullOrWhiteSpace(description))
                        values.Put(MediaStore.Video.VideoColumns.Description, description);
                    values.Put(MediaStore.Video.VideoColumns.MimeType, mime);

                    // Add the date meta data to ensure the image is added at the front of the gallery
                    values.Put(MediaStore.Video.VideoColumns.DateAdded, Java.Lang.JavaSystem.CurrentTimeMillis());
                    values.Put(MediaStore.Video.VideoColumns.DateTaken, Java.Lang.JavaSystem.CurrentTimeMillis());
                } else
                {
                    newUri = resolver.Insert(MediaStore.Files.GetContentUri("external"), values);
                    values.Put(MediaStore.Video.VideoColumns.Title, name);
                    values.Put(MediaStore.Video.VideoColumns.DisplayName, name);
                    values.Put(Android.Provider.MediaStore.MediaColumns.Size, fileInfo.Length);
                    if (!string.IsNullOrWhiteSpace(description))
                        values.Put(MediaStore.Video.VideoColumns.Description, description);
                    values.Put(MediaStore.Video.VideoColumns.MimeType, mime);

                    // Add the date meta data to ensure the image is added at the front of the gallery
                    values.Put(MediaStore.Video.VideoColumns.DateAdded, Java.Lang.JavaSystem.CurrentTimeMillis());
                    values.Put(MediaStore.Video.VideoColumns.DateTaken, Java.Lang.JavaSystem.CurrentTimeMillis());
                }
                
                // uri for content copy
                var outputStream = resolver.OpenOutputStream(newUri);
                var inputstream = new System.IO.FileStream(path, System.IO.FileMode.Open);
                inputstream.CopyTo(outputStream);
                return newUri;
            } catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
            return null;
        }

        public string GetMimeType(Android.Net.Uri uri)
        {
            string mimeType = null;
            if (uri.Scheme.Equals(ContentResolver.SchemeContent))
            {
                ContentResolver cr = Application.Context.ContentResolver;
                mimeType = cr.GetType(uri);
            }
            else
            {
                string fileExtension = MimeTypeMap.GetFileExtensionFromUrl(uri.ToString());
                mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(fileExtension.ToLower());
            }
            return mimeType;
        }

        public string GetMimeType(string file)
        {
            string fileExtension = System.IO.Path.GetExtension(file);
            return MimeTypeMap.Singleton.GetMimeTypeFromExtension(fileExtension.Replace(".", "").ToLower());
        }

        /*private bool CopyToDownloadsAndroidQ(string localPath)
        {
            try
            {
                string name = System.IO.Path.GetFileName(localPath);
                string ext = GetMimeType(System.IO.Path.GetExtension(name));
                var contentValues = new ContentValues();
                contentValues.Put(MediaStore.MediaColumns.DisplayName, name);
                contentValues.Put(MediaStore.MediaColumns.MimeType, ext);
                //contentValues.Put(MediaStore.MediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);
                contentValues.Put(MediaStore.DownloadColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);

                var resolver = Android.App.Application.Context.ContentResolver;

                Stream stream = null;
                Android.Net.Uri uri = null;
                try
                {
                    var contentUri = MediaStore.Files.GetContentUri("external");

                    try
                    {
                        var cursor = resolver.Query(
                        contentUri,
                        new[] { MediaStore.MediaColumns.Id, MediaStore.MediaColumns.DisplayName, MediaStore.MediaColumns.MimeType, MediaStore.DownloadColumns.RelativePath },
                        $"{MediaStore.MediaColumns.DisplayName} = ? AND {MediaStore.MediaColumns.MimeType} = ? AND {MediaStore.DownloadColumns.RelativePath} = ?",
                        new[] { localPath, ext, Android.OS.Environment.DirectoryDownloads },
                        null
                        );
                        if (cursor != null && cursor.Count >= 1)
                        {
                            cursor.MoveToFirst();

                            var id = cursor.GetLong(cursor.GetColumnIndexOrThrow(MediaStore.MediaColumns.Id));
                            var displayName = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.MediaColumns.DisplayName));
                            var relativePath = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.MediaColumns.MimeType));
                            var lastModifiedDate = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.DownloadColumns.RelativePath));

                            uri = ContentUris.WithAppendedId(contentUri, id);
                        }
                        else
                        {
                            uri = resolver.Insert(contentUri, contentValues);
                        }
                        cursor?.Close();
                    }
                    catch (Exception ex)
                    {
                        // ignore error here => problably a error with cursor & andoird sqlite db because of older android versions
                        System.Diagnostics.Debug.Print(ex.ToString());
                        uri = resolver.Insert(contentUri, contentValues);
                    }
                    if (uri == null)
                    {
                        throw new Exception("Retrieving of creating MediaStore record failed.");
                    }

                    byte[] buffer = new byte[1024];
                    stream = resolver.OpenOutputStream(uri);

                    using (var inputstream = new FileStream(localPath, FileMode.Open))
                    {
                        inputstream.CopyTo(stream);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                    if (uri != null)
                    {
                        // Don't leave an orphan entry in the MediaStore
                        resolver.Delete(uri, null, null);
                    }

                    throw;
                }
                finally
                {
                    stream?.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }*/

#if APILower29
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
#endif
    }
}