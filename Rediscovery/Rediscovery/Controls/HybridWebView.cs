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
        private SharedBase.Logging.ILogger _logger => DependencyService.Get<SharedBase.Logging.ILogger>() ?? new Services.Logger();
        private IHtmlUIService htmlUIService => DependencyService.Get<IHtmlUIService>() ?? new HtmlUIService();

        public event EventHandler SourceFolderSet;
        public event EventHandler SourceFolderNoHtml;

        Action<string> action;
        Action<string> logger;
        Action domReady;
        Action<string> error;

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
                        _logger.LogWarning(msg);
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
                _logger.LogTrace("Invoke JS model change with data:" + data);
                this.Eval($"featureReceive({data})");
            });
        }

        public void SetProfileChanged(string data)
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                _logger.LogTrace("Invoke JS profile change with data:" + data);
                this.Eval($"profileChanged({data})");
            });
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void RegisterLogger(Action<string> callback)
        {
            logger = callback;
        }

        public void RegisterErrorCallback(Action<string> callback)
        {
            error = callback;
        }

        public void RegisterDOMReady(Action callback)
        {
            domReady = callback;
        }

        public void Cleanup()
        {
            action = null;
            domReady = null;
            logger = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }

        public void InvokeError(string data)
        {
            if (error == null || data == null)
            {
                return;
            }
            error.Invoke(data);
        }

        public void InvokeDOMReady()
        {
            if (domReady == null)
            {
                return;
            }
            domReady.Invoke();
        }

        public void InvokeLogger(string data)
        {
            if (logger == null || data == null)
            {
                return;
            }
            logger.Invoke(data);
        }
    }
}
