namespace Academia.Infrastructure.Email;

public class EmailSettings
{
    public string SmtpHost { get; init; } = "smtp.gmail.com";
    public int SmtpPort { get; init; } = 587;
    public bool UseSsl { get; init; } = false;
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string FromEmail { get; init; } = default!;
    public string FromName { get; init; } = "IBSG Academia";
}
