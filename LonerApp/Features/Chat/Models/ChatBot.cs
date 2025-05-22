namespace LonerApp.Models;

public class PromptRequest
{
    public MessageModel Request { get; set; } = new();
}

public class PromptResponse
{
    public string Response { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
}