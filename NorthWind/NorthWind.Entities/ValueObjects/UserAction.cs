namespace NorthWind.Entities.ValueObjects;

public class UserAction(string user, string description)
{
    public DateTime CreatedDateTime { get; } = DateTime.Now;
    public string User => user;
    public string Description => description;
}


