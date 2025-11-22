using Flowingly.API.LogicLayer;
using Flowingly.API.LogicLayer.Interface;
using Flowingly.API.Models.Request;
using Microsoft.Extensions.Configuration;
using Moq;

public class TextParserImplementationTests
{
    [Fact]
    public async Task ParseMailLogic_ReturnsError_WhenTextIsNullOrEmpty()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var parser = new TextParserImplementation(mockConfiguration.Object);

        var request = new ParseMailRequest { Text = null };
        var result = await parser.ParseMailLogic(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("The mail content is empty", result.Message);
    }

    [Fact]
    public async Task ParseMailLogic_ReturnsError_WhenTextHasInvalidTags()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var parser = new TextParserImplementation(mockConfiguration.Object);

        var request = new ParseMailRequest { Text = "<total><invalid></total>" };
        var result = await parser.ParseMailLogic(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("The text contains unmatched or improperly nested XML-like tags.", result.Message);
    }

    [Fact]
    public async Task ParseMailLogic_ReturnsError_WhenTotalTagMissing()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var parser = new TextParserImplementation(mockConfiguration.Object);

        var request = new ParseMailRequest { Text = "<costcentre>ABC</costcentre>" };
        var result = await parser.ParseMailLogic(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("The <total> tag is missing in the mail content.", result.Message);
    }

    [Fact]
    public async Task ParseMailLogic_ReturnsSuccess_WhenTextIsValid()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var parser = new TextParserImplementation(mockConfiguration.Object);

        var request = new ParseMailRequest { Text = "<total>100</total><costcentre>ABC</costcentre>" };
        var result = await parser.ParseMailLogic(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Parsing successful", result.Message);
        Assert.Equal("ABC", result.CostCentre);
        Assert.Equal(100, result.TotalExcludingTax);
      
    }
}
