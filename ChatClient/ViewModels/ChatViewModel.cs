using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;

namespace ChatClient.ViewModels
{
    public class ChatMessage
    {
        public string User { get; set; }
        public string Text { get; set; }
    }

    public class ChatViewModel : ReactiveObject
    {
        public ObservableCollection<string> OnlineUsers { get; } = new();
        public ObservableCollection<ChatMessage> Messages { get; } = new();
        public ReactiveCommand<Unit, Unit> RefreshSubscriptionsCommand { get; }

        public string Username => _username;

        private string _selectedUser;
        public string SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public ReactiveCommand<Unit, Unit> SendCommand { get; }

        private readonly HubConnection _connection;
        private readonly string _username;

        public ChatViewModel(string username)
        {
            RefreshSubscriptionsCommand = ReactiveCommand.CreateFromTask(RefreshSubscriptionsAsync);
            _username = username;
            SendCommand = ReactiveCommand.CreateFromTask(SendMessageAsync);

            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatHub")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<string>("UserConnected", user =>
            {
                if (!OnlineUsers.Contains(user))
                    OnlineUsers.Add(user);
            });

            _connection.On<string>("UserDisconnected", user =>
            {
                OnlineUsers.Remove(user);
            });

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Messages.Add(new ChatMessage { User = user, Text = $"{user}: {message}" });
            });

            _connection.On<List<string>>("InitialOnlineUsers", users =>
            {
                OnlineUsers.Clear(); 
                foreach (var user in users)
                    OnlineUsers.Add(user);
            });


            _ = ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            try
            {
                await _connection.StartAsync();
                Console.WriteLine("Connected to SignalR hub");
                await _connection.InvokeAsync("Register", _username);
                Console.WriteLine("Sent Register() to hub");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
        }

        private async Task SendMessageAsync()
        {
            if (!string.IsNullOrWhiteSpace(Message) && !string.IsNullOrWhiteSpace(SelectedUser))
            {
                try
                {
                    await _connection.InvokeAsync("SendMessage", _username, SelectedUser, Message);
                    Message = string.Empty;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Send error: {ex.Message}");
                }
            }
        }
        private async Task RefreshSubscriptionsAsync()
        {
            try
            {
                await _connection.InvokeAsync("Register", _username);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Refresh error: " + ex.Message);
            }
        }

    }
}
