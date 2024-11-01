namespace sessionSix.App.V2.Domain;

public interface ICustomerRepository
{
    Customer GetBy(string id);
}