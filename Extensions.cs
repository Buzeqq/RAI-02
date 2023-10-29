using System.Text.Json;
using RAI_02.Dto;

namespace RAI_02;

public static class Extensions
{
    public static UserSessionInfomation? GetSessionInfomation(this ISession session)
    {
        try
        {
            var sessionString = session.GetString("session") ?? string.Empty;
            return JsonSerializer.Deserialize<UserSessionInfomation>(sessionString);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static void SetSessionInformation(this ISession session, UserSessionInfomation sessionInfomation)
    {
        var serialized = JsonSerializer.Serialize(sessionInfomation);
        session.SetString("session", serialized);
    }
}