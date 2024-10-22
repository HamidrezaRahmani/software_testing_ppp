namespace sessionSix.App.ObservableBehavior.Domain;

public class Order
{
    public string Id { get; set; }
    public Store Store { get; set; }
    public Discount? Discount { get; set; }
    public Customer Customer { get; set; }
    public List<Product> Products { get; set; }


    
    public Order AddOrder(string id,Customer customer,Store store,Discount discount , List<Product> products)
    {
      
        if (customer.IsActive == false)
            throw new Exception("Customer is deActivated");

      
        if (store.IsActive == false)
            throw new Exception("Store is deActivated");

        
        if (discount is not null && discount.IsActive == false)
            throw new Exception("Invalid discount code");


        if (products.Any(p => p.Price <= 0))
            throw new Exception("AtLeast one product is required.");

        return new Order()
        {
            Id = id,
            Store = store,
            Discount = discount,
            Customer = customer,
            Products = products,
        };


    }
    
}