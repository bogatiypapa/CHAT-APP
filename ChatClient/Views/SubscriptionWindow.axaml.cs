using Avalonia.Controls;
using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace ChatClient.Views
{
    public partial class SubscriptionWindow : Window
    {
        public event Action<string>? SubscriptionsConfirmed;

        public SubscriptionWindow(string username)
        {
            InitializeComponent();

            var vm = new SubscriptionViewModel(username);
            this.DataContext = vm;
            Console.WriteLine("DataContext type: " + this.DataContext?.GetType().Name);

            vm.ConfirmCommand.Subscribe(async u =>
            {
                SubscriptionsConfirmed?.Invoke(u);

                try
                {
                    // Повторная регистрация после добавления подписок
                    var connection = new HubConnectionBuilder()
                        .WithUrl("http://localhost:5000/chatHub")
                        .WithAutomaticReconnect()
                        .Build();

                    await connection.StartAsync();
                    await connection.InvokeAsync("Register", u);
                    await connection.StopAsync(); // сразу можно закрыть соединение
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error refreshing subscriptions: " + ex.Message);
                }

                this.Close();
            });


            // Подключаем логирование результата
            vm.LoadUsersCommand
              .Execute()
              .Subscribe(_ =>
              {
                  Console.WriteLine("Loaded into UI: " + vm.Users.Count);
              },
              ex =>
              {
                  Console.WriteLine("Error in LoadUsersCommand: " + ex.Message);
              });
        }
    }
}
