using Rediscovery.Features.DesktopFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rediscovery.Controls
{
    public class HybridWebView : WebView
    {
        private IHtmlUIService htmlUIService => DependencyService.Get<IHtmlUIService>() ?? new HtmlUIService();

        public event EventHandler SourceFolderSet;
        public event EventHandler SourceFolderNoHtml;

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

        public void SetDefaultHtml()
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                var source = new HtmlWebViewSource
                {
                    Html = htmlUIService.NoUIHtmlDefault()
                };
                Source = source;
            });
        }

        public void SetFolderSource(string directory)
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                var source = new HtmlWebViewSource();
                if (!string.IsNullOrWhiteSpace(directory) && System.IO.Directory.Exists(directory))
                {
                    source.BaseUrl = "file://" + directory + (!directory.EndsWith("/") ? "/" : "");
                    // find start file
                    string startFile = htmlUIService.GetIndexFile(directory);
                    if (!string.IsNullOrWhiteSpace(startFile))
                    {
                        source.Html = System.IO.File.ReadAllText(startFile);
                        SourceFolderSet?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        string msg = "No HTML file for the UI!";
                        System.Diagnostics.Debug.Print(msg);
                        SourceFolderSet?.Invoke(this, EventArgs.Empty);
                        SourceFolderNoHtml?.Invoke(this, EventArgs.Empty);
                        throw new System.IO.FileNotFoundException(msg);
                    }
                }

                Source = source;
            });
        }

        public void SetModel(string data)
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                System.Diagnostics.Debug.Print("Invoke JS model change with data:" + data);
                //this.Eval($"internalChangeModel({data})");
                //this.Eval($"log({data})");
                this.Eval($"featureReceive({data})");
            });
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
