using System.Collections.Concurrent;
using RAI_02.Models;

namespace RAI_02.Repositories;

public interface IUserRepository
{
    void Add(User user);
    void Delete(User user);
    User? Get(string login);

    IEnumerable<User> GetAll();
    void Clear();
    UserValidationResult Validate(IEnumerable<User> users);
}

public record UserValidationResult(bool Success, IEnumerable<User> validUsers, IEnumerable<User> invalidUsers);

public class UserRepository : IUserRepository
{
    private static readonly IDictionary<string, User> Users = new ConcurrentDictionary<string, User>(new Dictionary<string, User>
    {
        { "admin", new User(Guid.NewGuid(), "admin") }
    });

    
    public void Add(User user)
    {
        if (Users.ContainsKey(user.Login))
        {
            throw new Exception("Login already exist");
        }
        
        Users.Add(user.Login, user);
    }

    public void Delete(User user)
    {
        Users.Remove(user.Login);
    }

    public User? Get(string login) => Users.TryGetValue(login, out var user) ? user : null;

    public IEnumerable<User> GetAll()
    {
        return Users.Values;
    }

    public void Clear()
    {
        foreach (var (login, _) in Users)
        {
            if (login == "admin") continue;
            Users.Remove(login);
        }
    }

    public UserValidationResult Validate(IEnumerable<User> users)
    {
        var enumerable = users.ToList();
        var validUsers = enumerable.Where(u => Users.ContainsKey(u.Login));
        var invalidUsers = enumerable.Where(u => !Users.ContainsKey(u.Login)).ToList();
        
        return new UserValidationResult(invalidUsers.Any(), validUsers, invalidUsers);
    }
}