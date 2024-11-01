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
    public void Order_is_created_successfully(string orderId, string customerId, string storeId, string discountCode, string productId, int productPrice)
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            Id = orderId,
            CustomerId = customerId,
            StoreId = storeId,
            DiscountCode = discountCode,
            Products = new List<ProductRequestItem>
            {
                new ProductRequestItem { Id = productId }
            }
        };

        var customer = new Customer { Id = customerId, IsActive = true };
        var store = new Store { Id = storeId, IsActive = true };
        var discount = new Discount { Code = discountCode, IsActive = true };
        var product = new Product { Id = productId, Price = productPrice };
        var products = new List<Product>();
        products.Add(product);
        
        _customerRepository.GetBy(customerId).Returns(customer);
        _storeRepository.GetBy(storeId).Returns(store);
        _discountRepository.GetBy(discountCode).Returns(discount);
        _productRepository.GetBy(productId).Returns(product);
        
        // Act
        var actual = _sut.CreateOrder(request);
        var expectedOrder = new Order()
        { 
            Id = orderId,
            Customer = customer,
            Store = store,
            Discount = discount,
            Products = products,
        };

        
        // Assert
        actual.Should().BeEquivalentTo(expectedOrder);
        _orderRepository.Received(1).Add(Arg.Any<Order>());
    }
    
    
    
    [Theory]
    [MemberData(nameof(GetUpdateTestData))]
    public void Order_is_modified_successfully(string orderId, string customerId, string storeId, string discountCode, string productId, int productPrice)
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
        var customer = new Customer { Id = customerId, IsActive = true };
        var store = new Store { Id = storeId, IsActive = true };
        var discount = new Discount { Code = discountCode, IsActive = true };
        var product = new Product { Id = productId, Price = productPrice };
        var products = new List<Product>();
        products.Add(product);
        
        
        _orderRepository.GetBy(request.Id).Returns(existingOrder);
        _customerRepository.GetBy(request.CustomerId).Returns(customer);
        _storeRepository.GetBy(request.StoreId).Returns(store);
        _discountRepository.GetBy(request.DiscountCode).Returns(discount);
        _productRepository.GetBy(productId).Returns(product);
        
        // Act
        var actual = _sut.UpdateOrder(request);
        var expectedOrder = new Order()
        { 
            Id = orderId,
            Customer = customer,
            Store = store,
            Discount = discount,
            Products = products,
        };

        // Assert 
        actual.Should().BeEquivalentTo(expectedOrder);
        _orderRepository.Received(1).Add(Arg.Any<Order>());
    }
    
    
    [Fact]
    public void Order_is_created_only_for_active_store()
    {
         //Arrange
        var request = new ModifyOrderRequest
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            StoreId = Guid.NewGuid().ToString(),
            DiscountCode = Guid.NewGuid().ToString(),
            Products = new List<ProductRequestItem> 
            { 
                new ProductRequestItem
                  {
                    Id = Guid.NewGuid().ToString(),
                    Quantity = 10
                 },
                new ProductRequestItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Quantity = 11
                },
                
            }
        };

        var existingOrder = new Order { Id = request.Id };
        var customer = new Customer { Id = request.CustomerId, IsActive = true };
        var store = new Store { Id = request.StoreId, IsActive = false };
        var discount = new Discount { Code = request.DiscountCode, IsActive = true };
        var products = new List<Product>();
        foreach (var productRequestItem in request.Products)
        {
            var product = new Product
            {
                Id = productRequestItem.Id,
                Price = 100
            };
            products.Add(product);
            _productRepository.GetBy(productRequestItem.Id).Returns(product);
        }
        
        _orderRepository.GetBy(request.Id).Returns(existingOrder);
        _customerRepository.GetBy(request.CustomerId).Returns(customer);
        _storeRepository.GetBy(request.StoreId).Returns(store);
        _discountRepository.GetBy(request.DiscountCode).Returns(discount);
        // Act
        Action action = () => _sut.CreateOrder(request);
       // Assert
        action.Should().Throw<Exception>().WithMessage("Store is deActivated");
    
    }

    [Fact]
    public void Order_is_created_only_for_active_customer()
    {
        
        //Arrange
        var request = new ModifyOrderRequest
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            StoreId = Guid.NewGuid().ToString(),
            DiscountCode = Guid.NewGuid().ToString(),
            Products = new List<ProductRequestItem> 
            { 
                new ProductRequestItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Quantity = 10
                },
                new ProductRequestItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Quantity = 11
                },
                
            }
        };

        var existingOrder = new Order { Id = request.Id };
        var customer = new Customer { Id = request.CustomerId, IsActive = false };
        var store = new Store { Id = request.StoreId, IsActive = true };
        var discount = new Discount { Code = request.DiscountCode, IsActive = true };
        var products = new List<Product>();
        foreach (var productRequestItem in request.Products)
        {
            var product = new Product
            {
                Id = productRequestItem.Id,
                Price = 100
            };
            products.Add(product);
            _productRepository.GetBy(productRequestItem.Id).Returns(product);
        }
        
        _orderRepository.GetBy(request.Id).Returns(existingOrder);
        _customerRepository.GetBy(request.CustomerId).Returns(customer);
        _storeRepository.GetBy(request.StoreId).Returns(store);
        _discountRepository.GetBy(request.DiscountCode).Returns(discount);
        // Act
        Action action = () => _sut.CreateOrder(request);
        // Assert
        action.Should().Throw<Exception>().WithMessage("Customer is deActivated");

    }

    [Fact]
    public void Order_is_created_with_atLeast_one_product()
    {
        
        //Arrange
        var request = new ModifyOrderRequest
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            StoreId = Guid.NewGuid().ToString(),
            DiscountCode = Guid.NewGuid().ToString(),
            Products = new List<ProductRequestItem> { }
        };

        var existingOrder = new Order { Id = request.Id };
        var customer = new Customer { Id = request.CustomerId, IsActive = true };
        var store = new Store { Id = request.StoreId, IsActive = true };
        var discount = new Discount { Code = request.DiscountCode, IsActive = true };
 
        
        _orderRepository.GetBy(request.Id).Returns(existingOrder);
        _customerRepository.GetBy(request.CustomerId).Returns(customer);
        _storeRepository.GetBy(request.StoreId).Returns(store);
        _discountRepository.GetBy(request.DiscountCode).Returns(discount);
        // Act
        Action action = () => _sut.CreateOrder(request);
        // Assert
        action.Should().Throw<Exception>().WithMessage("AtLeast one product is required.");

    }

    [Fact]
    public void Order_is_created_having_only_active_discountCode()
    {
        //Arrange
        var request = new ModifyOrderRequest
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            StoreId = Guid.NewGuid().ToString(),
            DiscountCode = Guid.NewGuid().ToString(),
            Products = new List<ProductRequestItem> { }
        };

        var existingOrder = new Order { Id = request.Id };
        var customer = new Customer { Id = request.CustomerId, IsActive = true };
        var store = new Store { Id = request.StoreId, IsActive = true };
        var discount = new Discount { Code = request.DiscountCode, IsActive = false };
 
        
        _orderRepository.GetBy(request.Id).Returns(existingOrder);
        _customerRepository.GetBy(request.CustomerId).Returns(customer);
        _storeRepository.GetBy(request.StoreId).Returns(store);
        _discountRepository.GetBy(request.DiscountCode).Returns(discount);
        // Act
        Action action = () => _sut.CreateOrder(request);
        // Assert
       
        action.Should().Throw<Exception>().WithMessage("Invalid discount code");
        
    }
    
    
    
    
    
    public static IEnumerable<object[]> GetUpdateTestData()
    {
        yield return new object[] { "order123", "cust1", "store1", "DISC10", "prod1", 100 };
        yield return new object[] { "order456", "cust2", "store2", "DISC20", "prod2", 200 };
        yield return new object[] { "order789", "cust3", "store3", "DISC30", "prod3", 150 };
        yield return new object[] { "order321", "cust4", "store4", "DISC40", "prod4", 250};
        yield return new object[] { "order654", "cust5", "store5", "DISC50", "prod5", 300};
    } 
    
    public static IEnumerable<object[]> GetCreateTestData()
    {
        yield return new object[] { "order123", "cust1", "store1", "DISC10", "prod1", 100 };
        yield return new object[] { "order456", "cust2", "store2", "DISC20", "prod2", 200 };
        yield return new object[] { "order789", "cust3", "store3", "DISC30", "prod3", 150 };
        yield return new object[] { "order321", "cust4", "store4", "DISC40", "prod4", 250 };
        yield return new object[] { "order654", "cust5", "store5", "DISC50", "prod5", 300 };
    }
    
}
