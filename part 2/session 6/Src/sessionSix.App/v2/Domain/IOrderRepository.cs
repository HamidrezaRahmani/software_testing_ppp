namespace sessionSix.App.V2.Domain;


public interface IOrderRepository
{
    Order GetBy(string id);
    void Add(Order order);
}