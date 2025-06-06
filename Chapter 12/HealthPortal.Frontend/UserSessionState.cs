namespace HealthPortal.Frontend;

public class UserSessionState
{
    public string? Username { get; private set; }
    public string? Role { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(Username);

    public event Action? OnChange;

    public void SetUser(string username, string role)
    {
        Username = username;
        Role = role;
        NotifyStateChanged();
    }

    public void ClearUser()
    {
        Username = null;
        Role = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
