namespace CoachAssist.Shared.Services;

public class Toast
{
    public string Message { get; set; } = "";
    public string Kind { get; set; } = "success";
    public bool Visible { get; set; }

    public event Action? OnChange;

    public async Task Show(string message, string kind = "success", int ms = 2000)
    {
        Message = message;
        Kind = kind;
        Visible = true;
        OnChange?.Invoke();
        await Task.Delay(ms);
        Visible = false;
        OnChange?.Invoke();
    }
}
