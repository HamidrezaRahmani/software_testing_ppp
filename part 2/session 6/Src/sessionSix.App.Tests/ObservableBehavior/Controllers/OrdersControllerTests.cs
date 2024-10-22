using NSubstitute;
using sessionSix.App.ObservableBehavior.Controllers;
using sessionSix.App.ObservableBehavior.Domain;
using sessionSix.App.ObservableBehavior.Services;
using Xunit;

namespace sessionSix.App.Tests.ObservableBehavior.Controllers;

public class OrdersControllerTests
{
    private readonly IOrderService _orderService;
    private readonly OrdersController _sut;

    public OrdersControllerTests()
    {
        _orderService = Substitute.For<IOrderService>();
        _sut = new OrdersController(_orderService);
    }
    
    [Theory]
    [InlineData("5")]
    [InlineData("1")]
    [InlineData("9")]
    public void Order_is_created_successfully(string id)
    {
        //Arrange
        var request = new CreateOrderRequest() {  Id = id    }; 
        var response = new Order() { Id = id }; 
        // Act
     
        _orderService.CreateOrder(request).Returns(response);
        var result = _sut.CreateOrder(request);
        
        // Assert
        Assert.Equal(id, result); 
    }
    
    [Theory]
    [InlineData("5")]
    [InlineData("9")]
    [InlineData("2")]
    public void Order_is_modified_successfully(string id)
    {
        //Arrange
        var request = new ModifyOrderRequest() {  Id = id   }; 
        var response = new Order() { Id = id }; 
        // Act
     
        _orderService.UpdateOrder(request).Returns(response);
        var result = _sut.UpdateOrder(request);
        
        // Assert
        Assert.Equal(id, result); 
    }
}