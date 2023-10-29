using RAI_02.Models;

namespace RAI_02.Services;

public interface ISeedUsersService
{
    IEnumerable<User> Seed();
}

public class SeedUsersService : ISeedUsersService
{
    public IEnumerable<User> Seed()
    {
        var john = new User(Guid.NewGuid(), "John Doe");
        var foo = new User(Guid.NewGuid(), "foo-bar", new []{ john });
        var robert = new User(Guid.NewGuid(), "Robert", new [] {john, foo});
        var user1 = new User(Guid.NewGuid(), "user-1", new []{ foo, robert});
        var student = new User(Guid.NewGuid(), "studnet", new []{john, user1, foo});
        var buzeqq = new User(Guid.NewGuid(), "buzeqq", new []{john, foo, robert, user1, student});
        
        yield return john;
        yield return foo;
        yield return robert;
        yield return user1;
        yield return student;
        yield return buzeqq;
    }
}