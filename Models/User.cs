namespace RAI_02.Models;

public class User
{
    public string Login { get; init; }
    public DateTime CreatedAt { get; init; }
    public IEnumerable<User> Friends => _friends;
    private readonly List<User> _friends;

    public User(Guid id, string login, IEnumerable<User>? friends = default)
    {
        if (string.IsNullOrEmpty(login))
        {
            throw new Exception("Not valid login");
        }
        
        Login = login;
        _friends = friends?.ToList() ?? new List<User>();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddFriend(User newFriend)
    {
        if (newFriend.Login == Login)
        {
            throw new Exception("Can't be friends with yourself");
        }
        
        _friends.Add(newFriend);
    }
}