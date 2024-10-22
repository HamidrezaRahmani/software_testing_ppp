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
    
    
    [Fact]
    public void Order_is_created_successfully()
    {
        var request = new ModifyOrderRequest
        {
            Id = "order123",
            CustomerId = "cust1",
            StoreId = "store1",
            DiscountCode = "DISC10",
            Products = new List<ProductRequestItem> { new ProductRequestItem { Id = "prod1" } }
        };
        
        var customer = new Customer { Id = "cust1", IsActive = true };
        var store = new Store { Id = "store1", IsActive = true };
        var discount = new Discount { Code = "DISC10", IsActive = true };
        var product = new Product { Id = "prod1", Price = 100 };

        _customerRepository.GetBy(request.CustomerId).Returns(customer);
        _storeRepository.GetBy(request.StoreId).Returns(store);
        _discountRepository.GetBy(request.DiscountCode).Returns(discount);
        _productRepository.GetBy("prod1").Returns(product);
        //act
        var actual = _sut.CreateOrder(request);
        
        //Assert
        Assert.Equal("order123", actual.Id);
        Assert.Equal(store, actual.Store);
        Assert.Equal(discount, actual.Discount);
        Assert.Equal(customer, actual.Customer);
        actual.Products.Should().ContainEquivalentOf(product);
        
    }
    
    [Fact]
    public void Order_is_modified_successfully()
    {
        var request = new ModifyOrderRequest
        {
            Id = "order123",
            CustomerId = "cust1",
            StoreId = "store1",
            DiscountCode = "DISC10",
            Products = new List<ProductRequestItem> { new ProductRequestItem { Id = "prod1" } }
        };

        var existingOrder = new Order { Id = "order123" };
        var customer = new Customer { Id = "cust1", IsActive = true };
        var store = new Store { Id = "store1", IsActive = true };
        var discount = new Discount { Code = "DISC10", IsActive = true };
        var product = new Product { Id = "prod1", Price = 100 };

        _orderRepository.GetBy(request.Id).Returns(existingOrder);
        _customerRepository.GetBy(request.CustomerId).Returns(customer);
        _storeRepository.GetBy(request.StoreId).Returns(store);
        _discountRepository.GetBy(request.DiscountCode).Returns(discount);
        _productRepository.GetBy("prod1").Returns(product);

        // Act
        var result = _sut.UpdateOrder(request);

        // Assert (using FluentAssertions)
        result.Should().NotBeNull();
        result.Id.Should().Be("order123");
        result.Store.Should().Be(store);
        result.Discount.Should().Be(discount);
        result.Customer.Should().Be(customer);
        result.Products.Should().ContainEquivalentOf(product);

    }
}