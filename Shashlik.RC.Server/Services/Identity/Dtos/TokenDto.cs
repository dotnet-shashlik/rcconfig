namespace Shashlik.RC.Server.Services.Identity.Dtos;

public class TokenDto
{
    public string access_token { get; set; }
    public long expires_in { get; set; }
}