// SubscriptionViewModel.cs
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

public class SubscriptionItem : ReactiveObject
{
    public string Username { get; set; } = string.Empty;

    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }
}

public class SubscriptionViewModel : ReactiveObject
{
    public ObservableCollection<SubscriptionItem> Users { get; } = new();
    public ReactiveCommand<Unit, Unit> LoadUsersCommand { get; }
    public ReactiveCommand<Unit, string> ConfirmCommand { get; }

    private readonly string _currentUsername;

    public SubscriptionViewModel(string currentUsername)
    {
        _currentUsername = currentUsername;

        LoadUsersCommand = ReactiveCommand.CreateFromTask(LoadUsersAsync);
        ConfirmCommand = ReactiveCommand.CreateFromTask(ConfirmAsync);
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            using var client = new HttpClient();
            var json = await client.GetStringAsync("http://localhost:5000/api/users");
            var usernames = JsonSerializer.Deserialize<List<string>>(json);

            Console.WriteLine("Loaded users:");
            foreach (var u in usernames)
            {
                Console.WriteLine(u);
            }

            Users.Clear();

            foreach (var entry in usernames)
            {
                var index = entry.IndexOf(" (");
                var name = index > 0 ? entry.Substring(0, index) : entry;

                if (name != _currentUsername)
                {
                    Users.Add(new SubscriptionItem { Username = name, IsChecked = false });
                    Console.WriteLine($"Added user to UI: {name}");

                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading users: " + ex.Message);
        }
    }

    private async Task<string> ConfirmAsync()
    {
        using var client = new HttpClient();

        foreach (var user in Users.Where(u => u.IsChecked))
        {
            var url = $"http://localhost:5000/api/users/subscribe?subscriber={_currentUsername}&target={user.Username}";
            await client.PostAsync(url, null);
        }

        return _currentUsername;
    }
}
