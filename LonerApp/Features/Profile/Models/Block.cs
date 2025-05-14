namespace LonerApp.Models;

public class BlockRequest
{
    public string BlockerId{ get; set; } = string.Empty;
    public string BlockedId{ get; set; } = string.Empty;
    public byte TypeBlocked{ get; set; }// 0: block profile, 1: block chat, 2: report
    public bool IsUnChatBlocked{ get; set; }
}

public class BlockResponse
{
    public bool IsSuccess{ get; set; }
}