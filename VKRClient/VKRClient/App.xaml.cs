using System.Windows;
using System.Net.Http;

namespace VKRClient
{
    public partial class App : Application
    {
        public static HttpClient Client { get; private set; }

        static App()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://192.168.0.225:8000/")
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeWindow();
        }

        private async void InitializeWindow()
        {
            if (await JWTManager.SetBearer())
            {
                OpenMainWindow();
            }
            else
            {
                OpenAuthWindow();
            }
        }

        public static void OpenMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Current.MainWindow = mainWindow;
        }

        public static void OpenAuthWindow()
        {
            AuthNWindow authNWindow = new AuthNWindow();
            authNWindow.Show();
            authNWindow.Closed += delegate
            {
                if (Current.MainWindow == null)
                {
                    Current.Shutdown();
                }
            };
        }
    }
}