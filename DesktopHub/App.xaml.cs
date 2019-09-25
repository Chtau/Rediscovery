using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace DesktopHub
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}