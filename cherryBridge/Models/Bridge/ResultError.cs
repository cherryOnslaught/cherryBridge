namespace cherryBridge.Models.Bridge
{
  public class ResultError
  {
    public int ErrorCode { get; set; }
    public required string ErrorTag { get; set; }
    public string? Error { get; set; }
  }
}
