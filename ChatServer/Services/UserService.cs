using System;
using System.Collections.Generic;
using System.Linq;

public class UserService
{
    private readonly ChatDbContext _db;

    public UserService(ChatDbContext db) => _db = db;

    public User? GetUser(string username) =>
        _db.Users.FirstOrDefault(u => u.Username == username);

    public void SetOnline(string username, bool isOnline)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        var user = GetUser(username);
        if (user == null)
        {
            user = new User { Username = username };
            _db.Users.Add(user);
        }
        user.IsOnline = isOnline;
        _db.SaveChanges();
    }

    public List<User> GetAllUsers() => _db.Users.ToList();

    public void AddSubscription(string subscriber, string target)
    {
        if (!_db.Subscriptions.Any(s => s.Subscriber == subscriber && s.Target == target))
        {
            _db.Subscriptions.Add(new UserSubscription { Subscriber = subscriber, Target = target });
            _db.SaveChanges();
        }
    }

    public List<string> GetSubscriptionsOf(string subscriber)
    {
        return _db.Subscriptions
                .Where(s => s.Subscriber == subscriber)
                .Select(s => s.Target)
                .ToList();
    }
    public List<string> GetSubscribersOf(string targetUser)
    {
        return _db.Subscriptions
                  .Where(s => s.Target == targetUser)
                  .Select(s => s.Subscriber)
                  .ToList();
    }
    public void DeleteUser(string username)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            // Удаляем подписки, где он подписчик или цель
            var relatedSubs = _db.Subscriptions
                .Where(s => s.Subscriber == username || s.Target == username);
            _db.Subscriptions.RemoveRange(relatedSubs);

            _db.Users.Remove(user);
            _db.SaveChanges();
        }
    }

    public void RemoveSubscription(string subscriber, string target)
    {
        var sub = _db.Subscriptions.FirstOrDefault(s => s.Subscriber == subscriber && s.Target == target);
        if (sub != null)
        {
            _db.Subscriptions.Remove(sub);
            _db.SaveChanges();
        }
    }



}
