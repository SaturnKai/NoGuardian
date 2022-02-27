using System.Windows;

using Fiddler;

namespace NoGuardian
{
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            FiddlerApplication.Shutdown();
        }
    }
}