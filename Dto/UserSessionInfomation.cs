namespace RAI_02.Dto;

public class UserSessionInfomation
{
    public string Login { get; set; }
    public bool isAdmin => Login.ToLower() == "admin";
}