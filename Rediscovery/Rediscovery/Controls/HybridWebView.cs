using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Controls
{
    // TODO: add baseUrl for multi file handling on websites
    // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/webview?tabs=windows

    public class HybridWebView : WebView
    {
        Action<string> action;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public void SetFolderSource(string directory)
        {
            var source = new HtmlWebViewSource();
            source.BaseUrl = "file://" + directory + (!directory.EndsWith("/") ? "/" : "");
            // find start file
            string startFile = "";
            if (System.IO.File.Exists(System.IO.Path.Combine(directory, "Index.html")))
                startFile = System.IO.Path.Combine(directory, "Index.html");
            else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "index.html")))
                startFile = System.IO.Path.Combine(directory, "index.html");
            else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "default.html")))
                startFile = System.IO.Path.Combine(directory, "default.html");
            else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "Default.html")))
                startFile = System.IO.Path.Combine(directory, "Default.html");
            if (!string.IsNullOrWhiteSpace(startFile))
                source.Html = System.IO.File.ReadAllText(startFile);
            Source = source;
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }
    }
}
