using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    [Required]
    public string Username { get; set; }
    public bool IsOnline { get; set; }
}
