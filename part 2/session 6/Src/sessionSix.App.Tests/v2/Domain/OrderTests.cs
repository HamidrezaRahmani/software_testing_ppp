using FluentAssertions;
using sessionSix.App.V2.Domain;
using Xunit;

namespace sessionSix.App.Tests.v2.Domain;

public class OrderTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Order_is_Created_successfully(string id, string customerId, string storeId, string discountCode, string productId, int productPrice, bool customerIsActive, bool storeIsActive, bool discountIsActive)
    {
        // Arrange
        
        var customer = new Customer
        {
            Id = customerId, IsActive = customerIsActive
        };
        var store = new Store
        {
            Id = storeId, IsActive = storeIsActive
        };
        var discount = new Discount
        {
            Code = discountCode, IsActive = discountIsActive
        };
        var products = new List<Product>
        {
            new Product { Id = productId, Price = productPrice }
        };
        
        var sut = new Order();
        // Act
        sut.AddOrder(id,customer,store,discount,products);

        // Assert
      
        sut.Id.Should().Be(id);
        sut.Store.Should().Be(store);
        sut.Discount.Should().Be(discount);
        sut.Customer.Should().Be(customer);
        sut.Products.Should().BeEquivalentTo(products);
    }
    
    
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { "order123", "cust1", "store1", "DISC10", "prod1", 100, true, true, true };
        yield return new object[] { "order456", "cust2", "store2", "DISC20", "prod2", 200, true, true, true };
        yield return new object[] { "order789", "cust3", "store3", "DISC30", "prod3", 150, true, true, true };
        yield return new object[] { "order321", "cust4", "store4", "DISC40", "prod4", 250, true, true, true };
        yield return new object[] { "order654", "cust5", "store5", "DISC50", "prod5", 300, true, true, true };
    } 
    
}