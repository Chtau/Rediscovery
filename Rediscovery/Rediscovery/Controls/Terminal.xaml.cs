using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Terminal : Grid
    {
        public event EventHandler<string> SendCommand;

        public Terminal()
        {
            InitializeComponent();
        }

        public void AddLines(params string[] lines)
        {
            try
            {
                if (lines != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var line in lines)
                        {
                            StackLines.Children.Add(new Label
                            {
                                Text = line,
                                LineBreakMode = LineBreakMode.NoWrap,
                                MaxLines = 1
                            });
                        }
                        scrollView.ScrollToAsync(0, StackLines.Height + 50, false);
                    });
                }
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString() + Environment.NewLine);
            }
        }

        private void send_Clicked(object sender, EventArgs e)
        {
            SendCommand?.Invoke(this, CommandInput.Text);
            CommandInput.Text = null;
        }
    }
}