using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService service) => _userService = service;

    [HttpGet]
    public IActionResult GetUsers() =>
        Ok(_userService.GetAllUsers().Select(u => $"{u.Username} ({(u.IsOnline ? "Online" : "Offline")})"));


    [HttpPost("subscribe")]
    public IActionResult Subscribe([FromQuery] string subscriber, [FromQuery] string target)
    {
        _userService.AddSubscription(subscriber, target);
        return Ok();
    }

    [HttpPost("register")]
    public IActionResult Register([FromQuery] string username)
    {
        _userService.SetOnline(username, false); // просто добавить в БД
        return Ok();
    }

    [HttpDelete("{username}")]
    public IActionResult DeleteUser(string username)
    {
        _userService.DeleteUser(username);
        return Ok();
    }

    [HttpDelete("subscription")]
    public IActionResult DeleteSubscription([FromQuery] string subscriber, [FromQuery] string target)
    {
        _userService.RemoveSubscription(subscriber, target);
        return Ok();
    }

    [HttpGet("subscriptions/{username}")]
    public IActionResult GetSubscriptions(string username)
    {
        var targets = _userService.GetSubscriptionsOf(username);
        return Ok(targets);
    }

    [HttpGet("subscribers/{username}")]
    public IActionResult GetSubscribers(string username)
    {
        var subscribers = _userService.GetSubscribersOf(username);
        return Ok(subscribers);
    }


}
