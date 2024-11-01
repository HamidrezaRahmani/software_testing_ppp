namespace sessionSix.App.V2.Domain;

public interface IProductRepository
{
    Product GetBy(string id);
}