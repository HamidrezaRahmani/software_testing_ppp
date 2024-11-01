namespace sessionSix.App.V2.Domain;

public interface IDiscountRepository
{
    Discount? GetBy(string? id);
}