using Flowingly.API.LogicLayer.Interface;
using Flowingly.API.Models.Request;
using Flowingly.API.Models.Response;
using Flowingly_API.Common;
using Flowingly_API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class TextParserControllerTests
{
    [Fact]
    public async Task Post_ReturnsBadRequest_WhenRequestIsNull()
    {
        var mockParser = new Mock<ITextParserImplementation>();
        var controller = new TextParserController(mockParser.Object);

        var result = await controller.Post(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(Constants.MailContentEmpty, badRequest.Value);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenTextIsEmpty()
    {
        var mockParser = new Mock<ITextParserImplementation>();
        var controller = new TextParserController(mockParser.Object);

        var request = new ParseMailRequest { Text = "   " };
        var result = await controller.Post(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(Constants.MailContentEmpty, badRequest.Value);
    }

    [Fact]
    public async Task Post_ReturnsOk_WhenParseMailLogicSucceeds()
    {
        var mockParser = new Mock<ITextParserImplementation>();
        var response = new ParseMailResponse { IsSuccess = true, Message = "Success" };
        mockParser.Setup(x => x.ParseMailLogic(It.IsAny<ParseMailRequest>())).ReturnsAsync(response);

        var controller = new TextParserController(mockParser.Object);
        var request = new ParseMailRequest { Text = "valid text" };

        var result = await controller.Post(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Post_ReturnsServerError_WhenExceptionThrown()
    {
        var mockParser = new Mock<ITextParserImplementation>();
        mockParser.Setup(x => x.ParseMailLogic(It.IsAny<ParseMailRequest>())).ThrowsAsync(new System.Exception("fail"));

        var controller = new TextParserController(mockParser.Object);
        var request = new ParseMailRequest { Text = "valid text" };

        var result = await controller.Post(request);

        var serverError = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, serverError.StatusCode);
        Assert.Contains(Constants.UnknownCostCentre, serverError.Value.ToString());
    }
}
