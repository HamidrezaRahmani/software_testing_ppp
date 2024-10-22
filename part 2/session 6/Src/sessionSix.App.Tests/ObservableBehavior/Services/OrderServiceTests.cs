using NSubstitute;
using sessionSix.App.ObservableBehavior.Domain;
using sessionSix.App.ObservableBehavior.Services;
using Xunit;
using FluentAssertions;

namespace sessionSix.App.Tests.ObservableBehavior.Services;

public class OrderServiceTests
{
    
    private readonly IStoreRepository _storeRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly OrderService _sut;
    
    
    public OrderServiceTests()
    {
        _storeRepository = Substitute.For<IStoreRepository>();
        _discountRepository = Substitute.For<IDiscountRepository>();
        _customerRepository = Substitute.For<ICustomerRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _orderRepository = Substitute.For<IOrderRepository>();

        _sut = new OrderService(
            _storeRepository,
            _discountRepository,
            _customerRepository,
            _productRepository,
            _orderRepository);
    }
    
    [Theory]
    [MemberData(nameof(GetCreateTestData))]
    public void Order_is_created_successfully(string orderId, string customerId, string storeId, string discountCode, string productId, int productPrice, bool customerIsActive, bool storeIsActive, bool discountIsActive)
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            Id = orderId,
            CustomerId = customerId,
            StoreId = storeId,
            DiscountCode = discountCode,
            Products = new List<ProductRequestItem> { new ProductRequestItem { Id = productId } }
        };

        var customer = new Customer { Id = customerId, IsActive = customerIsActive };
        var store = new Store { Id = storeId, IsActive = storeIsActive };
        var discount = new Discount { Code = discountCode, IsActive = discountIsActive };
        var product = new Product { Id = productId, Price = productPrice };

        _customerRepository.GetBy(customerId).Returns(customer);
        _storeRepository.GetBy(storeId).Returns(store);
        _discountRepository.GetBy(discountCode).Returns(discount);
        _productRepository.GetBy(productId).Returns(product);
        
        // Act
        var actual = _sut.CreateOrder(request);
        
        // Assert
        actual.Id.Should().Be(orderId);
        actual.Store.Should().Be(store);
        actual.Discount.Should().Be(discount);
        actual.Customer.Should().Be(customer);
        actual.Products.Should().ContainEquivalentOf(product);
    }
    
    
    
    [Theory]
    [MemberData(nameof(GetUpdateTestData))]
    public void Order_is_modified_successfully(string orderId, string customerId, string storeId, string discountCode, string productId, int productPrice, bool customerIsActive, bool storeIsActive, bool discountIsActive)
    {
        // Arrange
        var request = new ModifyOrderRequest
        {
            Id = orderId,
            CustomerId = customerId,
            StoreId = storeId,
            DiscountCode = discountCode,
            Products = new List<ProductRequestItem> { new ProductRequestItem { Id = productId } }
        };

        var existingOrder = new Order { Id = orderId };
        var customer = new Customer { Id = customerId, IsActive = customerIsActive };
        var store = new Store { Id = storeId, IsActive = storeIsActive };
        var discount = new Discount { Code = discountCode, IsActive = discountIsActive };
        var product = new Product { Id = productId, Price = productPrice };

        _orderRepository.GetBy(request.Id).Returns(existingOrder);
        _customerRepository.GetBy(request.CustomerId).Returns(customer);
        _storeRepository.GetBy(request.StoreId).Returns(store);
        _discountRepository.GetBy(request.DiscountCode).Returns(discount);
        _productRepository.GetBy(productId).Returns(product);
        
        // Act
        var actual = _sut.UpdateOrder(request);

        // Assert 
        actual.Id.Should().Be(orderId);
        actual.Store.Should().Be(store);
        actual.Discount.Should().Be(discount);
        actual.Customer.Should().Be(customer);
        actual.Products.Should().ContainEquivalentOf(product);
    }
    
    public static IEnumerable<object[]> GetUpdateTestData()
    {
        yield return new object[] { "order123", "cust1", "store1", "DISC10", "prod1", 100, true, true, true };
        yield return new object[] { "order456", "cust2", "store2", "DISC20", "prod2", 200, true, true, true };
        yield return new object[] { "order789", "cust3", "store3", "DISC30", "prod3", 150, true, true, true };
        yield return new object[] { "order321", "cust4", "store4", "DISC40", "prod4", 250, true, true, true };
        yield return new object[] { "order654", "cust5", "store5", "DISC50", "prod5", 300, true, true, true };
    } 
    
    public static IEnumerable<object[]> GetCreateTestData()
    {
        yield return new object[] { "order123", "cust1", "store1", "DISC10", "prod1", 100, true, true, true };
        yield return new object[] { "order456", "cust2", "store2", "DISC20", "prod2", 200, true, true, true };
        yield return new object[] { "order789", "cust3", "store3", "DISC30", "prod3", 150, true, true, true };
        yield return new object[] { "order321", "cust4", "store4", "DISC40", "prod4", 250, true, true, true };
        yield return new object[] { "order654", "cust5", "store5", "DISC50", "prod5", 300, true, true, true };
    }
    
}
