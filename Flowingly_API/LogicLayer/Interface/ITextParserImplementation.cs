using Flowingly.API.Models.Request;
using Flowingly.API.Models.Response;

namespace Flowingly.API.LogicLayer.Interface
{
    public interface ITextParserImplementation
    {
        Task<ParseMailResponse> ParseMailLogic(ParseMailRequest request);
    }
}
