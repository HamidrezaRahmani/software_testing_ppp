namespace sessionSix.App.V2.Domain;

public class Order
{
    public string Id { get;private set; }
    public Store Store { get;private set; } 
    public Discount? Discount { get;private set; }
    public Customer Customer { get;private set; }
    public List<Product> Products { get;private set; }


    
    public void AddOrder(string id,Customer customer,Store store,Discount discount , List<Product> products)
    {
      
        if (customer.IsActive == false)
            throw new Exception("Customer is deActivated");

      
        if (store.IsActive == false)
            throw new Exception("Store is deActivated");

        
        if (discount is not null && discount.IsActive == false)
            throw new Exception("Invalid discount code");


        if (products.Any(p => p.Price <= 0))
            throw new Exception("AtLeast one product is required.");


        Id = id;
        Store = store;
        Discount = discount;
        Customer = customer;
        Products = products;



    }
    
}