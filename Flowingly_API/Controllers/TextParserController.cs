using Microsoft.AspNetCore.Mvc;
using Flowingly.API.Models.Request;
using Flowingly.API.LogicLayer.Interface;
using Flowingly.API.LogicLayer;
using Flowingly.API.Models.Response;
using Flowingly_API.Common;

namespace Flowingly_API.Controllers
{
    /// <summary>
    /// Controller for parsing email text and extracting structured data.
    /// </summary>
    [ApiController]
    [Route("api/parser")]
    public class TextParserController : ControllerBase
    {
        #region Fields

        private readonly ITextParserImplementation _textParserImplementation;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TextParserController"/> class.
        /// </summary>
        /// <param name="textParserImplementation">Text parser logic implementation.</param>
        public TextParserController(ITextParserImplementation textParserImplementation)
        {
            _textParserImplementation = textParserImplementation;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Parses the email text and returns extracted data.
        /// </summary>
        /// <param name="request">Request containing the email text.</param>
        /// <returns>Parsed mail response.</returns>
        [HttpPost]
        public async Task<ActionResult<ParseMailResponse>> Post([FromBody] ParseMailRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Text))
                {
                    return BadRequest(Constants.MailContentEmpty);
                }

                var response = await _textParserImplementation.ParseMailLogic(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log exception here if needed
                return StatusCode(500, $"{Constants.UnknownCostCentre}: {ex.Message}");
            }
        }

        #endregion
    }
}