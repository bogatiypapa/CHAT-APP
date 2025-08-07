using System.ComponentModel.DataAnnotations;

public class UserSubscription
{
    public int Id { get; set; }

    public string Subscriber { get; set; }
    public string Target { get; set; }
}