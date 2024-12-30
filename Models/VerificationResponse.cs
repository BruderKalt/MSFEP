namespace MSFEP.Models;

public class VerificationResponse
{
    public string requestId { get; set; }
    public string url { get; set; }
    public long expiry { get; set; }
    public string qrCode { get; set; }
}
