// ChatHub.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly UserService _userService;
    private static readonly Dictionary<string, string> _connections = new();

    public ChatHub(UserService userService) => _userService = userService;

    public async Task Register(string username)
    {
        _connections[Context.ConnectionId] = username;
        _userService.SetOnline(username, true);

        // Получить список всех, кто онлайн, кроме себя
        var alreadyOnline = _connections.Values
            .Where(u => u != username)
            .Distinct()
            .ToList();

        // Получить список, на кого подписан пользователь
        var subscriptions = _userService.GetSubscriptionsOf(username);

        // Оставить только тех онлайн-пользователей, на кого подписан
        var visibleOnline = alreadyOnline
            .Where(user => subscriptions.Contains(user))
            .ToList();

        await Clients.Caller.SendAsync("InitialOnlineUsers", visibleOnline);

        // Найти всех, кто подписан на username и онлайн
        var subscribers = _userService.GetSubscribersOf(username);
        var subscriberConnectionIds = _connections
            .Where(kvp => subscribers.Contains(kvp.Value))
            .Select(kvp => kvp.Key)
            .ToList();

        await Clients.Clients(subscriberConnectionIds).SendAsync("UserConnected", username);
    }

    public async Task SendMessage(string from, string to, string message)
    {
        // Взаимная подписка обязательна
        var fromSubs = _userService.GetSubscriptionsOf(from);
        var toSubs = _userService.GetSubscriptionsOf(to);

        if (fromSubs.Contains(to) && toSubs.Contains(from))
        {
            var targetConn = _connections.FirstOrDefault(kv => kv.Value == to).Key;
            if (targetConn != null)
            {
                await Clients.Client(targetConn).SendAsync("ReceiveMessage", from, message);
            }
        }
        else
        {
            // Можно логировать или сообщать отправителю об отказе
            Console.WriteLine($"Message blocked: {from} is not mutually subscribed with {to}");
        }
    }


    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        if (_connections.TryGetValue(Context.ConnectionId, out var username))
        {
            _userService.SetOnline(username, false);
            _connections.Remove(Context.ConnectionId);

            var subscribers = _userService.GetSubscribersOf(username);
            var subscriberConnectionIds = _connections
                .Where(kvp => subscribers.Contains(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            await Clients.Clients(subscriberConnectionIds).SendAsync("UserDisconnected", username);
        }

        await base.OnDisconnectedAsync(ex);
    }
}
