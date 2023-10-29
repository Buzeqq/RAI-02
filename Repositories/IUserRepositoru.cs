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
}

public class UserRepository : IUserRepository
{
    private static readonly IDictionary<string, User> Users = new ConcurrentDictionary<string, User>();

    
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
        Users.Clear();
    }
}