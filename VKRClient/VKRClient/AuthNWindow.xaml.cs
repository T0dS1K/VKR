using System.Net;
using System.Windows;
using VKRClient.Models;
using System.Net.Http.Json;

namespace VKRClient
{
    public partial class AuthNWindow : Window
    {
        public AuthNWindow()
        {
            InitializeComponent();
        }

        private async void SingIn_Click(object sender, EventArgs e)
        {
            ReportBox.Content = string.Empty;

            if (LoginBox.Text.Length >= 1 && PassBox.Password.Length >= 1)
            {
                try
                {
                    var Data = new Data
                    {
                        Login = LoginBox.Text,
                        Password = PassBox.Password
                    };

                    var Response = await App.Client.PostAsJsonAsync("AuthN/SignIn", Data);
                    
                    if (Response.StatusCode != HttpStatusCode.BadRequest)
                    {
                        JWTManager.SaveTokens(await Response.Content.ReadAsStringAsync());
                        App.OpenMainWindow();
                        Close();
                    }
                    else
                    {
                        ReportBox.Content = "НЕВЕРНЫЙ ЛОГИН ИЛИ ПАРОЛЬ";
                    }
                }
                catch
                {
                    ReportBox.Content = "СОЕДИНЕНИЕ НЕ УСТАНОВЛЕНО";
                }
            }
            else
            {
                ReportBox.Content = "СЛИШКОМ КОРОТКИЙ ЛОГИН ИЛИ ПАРОЛЬ";
            }

            ReportBox.Content = "ЧТО-ТО ПОШЛО НЕ ТАК";
        }
    }
}
