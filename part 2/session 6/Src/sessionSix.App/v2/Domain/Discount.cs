namespace sessionSix.App.V2.Domain;

public class Discount
{
    public string Id { get; set; }
    public string Code { get; set; }
    public int Amount { get; set; }
    public bool IsActive { get; set; }
}