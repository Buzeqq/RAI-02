using System.Text.Json;
using System.Text.Json.Serialization;

namespace RAI_02.Models;

public class User
{
    public string Login { get; set; }
    public DateTime CreatedAt { get; set; }
    public IDictionary<string, User> Friends { get; set; }

    public User(Guid id, string login, IEnumerable<User>? friends = default, DateTime? createdAt = default)
    {
        if (string.IsNullOrEmpty(login))
        {
            throw new Exception("Not valid login");
        }
        
        Login = login;
        Friends = friends?.ToDictionary(u => u.Login) ?? new Dictionary<string, User>();
        CreatedAt = DateTime.UtcNow;
    }
    
    public User() {}

    public bool AddFriend(User newFriend)
    {
        if (newFriend.Login == Login)
        {
            return false;
        }

        if (Friends.ContainsKey(newFriend.Login))
        {
            return false;
        }
        
        Friends.Add(newFriend.Login, newFriend);
        return true;
    }


    public bool RemoveFriend(User user)
    {
        return Friends.Remove(user.Login);
    }

    public AddResult AddFriendRange(IEnumerable<User> friends)
    {
        var friendsSkipped = 0;
        var friendsAdded = 0;
        
        foreach (var friend in friends)
        {
            if (Friends.TryAdd(friend.Login, friend))
            {
                friendsAdded++;
            }
            else
            {
                friendsSkipped++;
            }
        }

        return new AddResult(friendsSkipped == 0, friendsAdded, friendsSkipped);
    }
}

public record AddResult(bool Succes, int FriendsAdded, int FriendsSkipped);

public class UserConverter : JsonConverter<User>
{
    public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var user = new User();
        var friends = new Dictionary<string, User>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                user.Friends = friends;
                return user;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString()!;

            reader.Read();

            switch (propertyName)
            {
                case "Login":
                    user.Login = reader.GetString()!;
                    break;
                case "CreatedAt":
                    user.CreatedAt = reader.GetDateTime();
                    break;
                case "Friends":
                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonException();
                    }

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }
                        
                        var friend = JsonSerializer.Deserialize<User>(ref reader, options)!;

                        friends[friend.Login] = friend;
                    }
                    break;
                default:
                    throw new JsonException();
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WritePropertyName(nameof(value.Login));
        JsonSerializer.Serialize(writer, value.Login, options);

        writer.WritePropertyName(nameof(value.CreatedAt));
        JsonSerializer.Serialize(writer, value.CreatedAt, options);
        
        writer.WritePropertyName(nameof(value.Friends));
        writer.WriteStartArray();

        foreach (var friend in value.Friends.Values)
        {
            JsonSerializer.Serialize(writer, friend, options);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}