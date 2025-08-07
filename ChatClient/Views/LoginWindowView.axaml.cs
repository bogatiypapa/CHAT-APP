// LoginWindowView.axaml.cs
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ChatClient.Views;
using ReactiveUI;

namespace ChatClient.Views
{
    public partial class LoginWindowView : Window
    {
        public event Action<string>? LoginCompleted;
        public event Action<string> LoginConfirmed;


        public LoginWindowView()
        {
            InitializeComponent();
        }

        private async void OnLoginClick(object? sender, RoutedEventArgs e)
        {
            var usernameBox = this.FindControl<TextBox>("UsernameBox");
            var username = usernameBox?.Text;

            if (!string.IsNullOrWhiteSpace(username))
            {
                // Зарегистрировать пользователя на сервере (в БД, offline)
                using var http = new HttpClient();
                var url = $"http://localhost:5000/api/users/register?username={username}";

                try
                {
                    await http.PostAsync(url, null);

                    // Открыть окно выбора подписок
                    var subWindow = new SubscriptionWindow(username);
                    subWindow.SubscriptionsConfirmed += (finalUsername) =>
                    {
                        LoginCompleted?.Invoke(finalUsername);
                    };
                    subWindow.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    //await MessageBus.Show(this, $"Ошибка подключения к серверу:\n{ex.Message}", "Ошибка", MessageBox.MessageBoxButtons.Ok);
                }
            }
        }
    }
} 