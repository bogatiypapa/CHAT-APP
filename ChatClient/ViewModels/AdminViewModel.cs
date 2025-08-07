using System.Net.Http;
using System.Net.Http.Json;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Reactive;

namespace ChatClient.ViewModels
{
    public class AdminViewModel : ReactiveObject
    {
        public ObservableCollection<string> Users { get; } = new();
        public ObservableCollection<string> Subscriptions { get; } = new();
        public ObservableCollection<string> Subscribers { get; } = new();

        private string _selectedUser;
        public string SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }

        private string _newSubscriber;
        public string NewSubscriber
        {
            get => _newSubscriber;
            set => this.RaiseAndSetIfChanged(ref _newSubscriber, value);
        }

        private string _newTarget;
        public string NewTarget
        {
            get => _newTarget;
            set => this.RaiseAndSetIfChanged(ref _newTarget, value);
        }

        public ReactiveCommand<Unit, Unit> LoadUsersCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteUserCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadSubscriptionsCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadSubscribersCommand { get; }
        public ReactiveCommand<Unit, Unit> AddSubscriptionCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteSubscriptionCommand { get; }

        public AdminViewModel()
        {
            LoadUsersCommand = ReactiveCommand.CreateFromTask(LoadUsersAsync);
            DeleteUserCommand = ReactiveCommand.CreateFromTask(DeleteUserAsync);
            LoadSubscriptionsCommand = ReactiveCommand.CreateFromTask(LoadSubscriptionsAsync);
            LoadSubscribersCommand = ReactiveCommand.CreateFromTask(LoadSubscribersAsync);
            AddSubscriptionCommand = ReactiveCommand.CreateFromTask(AddSubscriptionAsync);
            DeleteSubscriptionCommand = ReactiveCommand.CreateFromTask(DeleteSubscriptionAsync);

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                Users.Clear();
                using var client = new HttpClient();
                var users = await client.GetFromJsonAsync<List<string>>("http://localhost:5000/api/users");
                foreach (var user in users)
                    Users.Add(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading users: " + ex.Message);
            }
        }

        private async Task DeleteUserAsync()
        {
            if (!string.IsNullOrWhiteSpace(SelectedUser))
            {
                using var client = new HttpClient();
                var nameOnly = SelectedUser.Split(" (")[0];
                await client.DeleteAsync($"http://localhost:5000/api/users/{nameOnly}");
                await LoadUsersAsync();
            }
        }

        private async Task LoadSubscriptionsAsync()
        {
            Subscriptions.Clear();
            if (!string.IsNullOrWhiteSpace(SelectedUser))
            {
                var nameOnly = SelectedUser.Split(" (")[0];
                using var client = new HttpClient();
                var result = await client.GetFromJsonAsync<List<string>>($"http://localhost:5000/api/users/subscriptions/{nameOnly}");
                foreach (var s in result)
                    Subscriptions.Add(s);
            }
        }

        private async Task LoadSubscribersAsync()
        {
            Subscribers.Clear();
            if (!string.IsNullOrWhiteSpace(SelectedUser))
            {
                var nameOnly = SelectedUser.Split(" (")[0];
                using var client = new HttpClient();
                var result = await client.GetFromJsonAsync<List<string>>($"http://localhost:5000/api/users/subscribers/{nameOnly}");
                foreach (var s in result)
                    Subscribers.Add(s);
            }
        }

        private async Task AddSubscriptionAsync()
        {
            if (!string.IsNullOrWhiteSpace(NewSubscriber) && !string.IsNullOrWhiteSpace(NewTarget))
            {
                using var client = new HttpClient();
                var url = $"http://localhost:5000/api/users/subscribe?subscriber={NewSubscriber}&target={NewTarget}";
                await client.PostAsync(url, null);
            }
        }

        private async Task DeleteSubscriptionAsync()
        {
            if (!string.IsNullOrWhiteSpace(NewSubscriber) && !string.IsNullOrWhiteSpace(NewTarget))
            {
                using var client = new HttpClient();
                var url = $"http://localhost:5000/api/users/subscription?subscriber={NewSubscriber}&target={NewTarget}";
                await client.DeleteAsync(url);
            }
        }
    }
}
