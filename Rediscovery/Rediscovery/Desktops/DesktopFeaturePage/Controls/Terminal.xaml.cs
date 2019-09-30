using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Desktops.DesktopFeaturePage.Controls
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
            /*if (lines != null)
            {
                foreach (var line in lines)
                {
                    StackLines.Children.Add(new Label 
                    { 
                        Text = line, 
                        LineBreakMode = LineBreakMode.NoWrap, 
                        HorizontalOptions = LayoutOptions.FillAndExpand, 
                        MinimumWidthRequest = 750, 
                        WidthRequest = 750,
                        MaxLines = 1
                    });
                }
            }*/
        }

        private void send_Clicked(object sender, EventArgs e)
        {
            SendCommand?.Invoke(this, CommandInput.Text);
        }
    }
}