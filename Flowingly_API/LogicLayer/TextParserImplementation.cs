using Flowingly.API.LogicLayer.Interface;
using Flowingly.API.Models.Request;
using Flowingly.API.Models.Response;
using System.Text.Json;
using System.Text.RegularExpressions;
using Flowingly_API.Common;

namespace Flowingly.API.LogicLayer
{
    /// <summary>
    /// Provides logic for parsing email text, extracting XML-like tags, and calculating tax details.
    /// </summary>
    public class TextParserImplementation : ITextParserImplementation
    {
        private readonly IConfiguration _configuration;

        public TextParserImplementation(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        #region Public Methods

        /// <summary>
        /// Parses the email text, validates tags, extracts values, and calculates tax details.
        /// </summary>
        /// <param name="request">The request containing the email text.</param>
        /// <returns>A response object with parsing results and extracted values.</returns>
        public async Task<ParseMailResponse> ParseMailLogic(ParseMailRequest request)
        {
            var response = new ParseMailResponse();
            try
            {
                // check if the mail content is empty
                if (string.IsNullOrWhiteSpace(request?.Text))
                {
                    response.IsSuccess = false;
                    response.Message = Constants.MailContentEmpty;
                    return response;
                }

                //check if the tags are valid
                var (isSuccess, tag) = await Task.Run(() => CheckForValidTags(request.Text));
                if (!isSuccess)
                {
                    response.IsSuccess = false;
                    if (!string.IsNullOrEmpty(tag))
                        response.Message = Constants.UnmatchedTag.Replace("$", tag);
                    else 
                        response.Message = Constants.InvalidTags;
                    return response;
                }

                //check if total tag is missing or not.
                isSuccess = await Task.Run(() => IsTotalTagMissing(request.Text));
                if (!isSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = Constants.TotalTagMissing;
                    return response;
                }

                //get the total value from the text
                var totalMatch = await Task.Run(() => Regex.Match(request.Text, @"<total>(.*?)</total>", RegexOptions.Singleline));
                var totalIncludingTax = decimal.Parse(totalMatch.Groups[1].Value.Replace(",", ""));

                // Get tax rate from configuration
                var taxRateValue = _configuration["TaxRate"];
                decimal taxRate = decimal.Parse(taxRateValue); // Convert percentage to decimal


                //calculate sales tax and total excluding tax
                var (salesTax, totalExcludingTax) = CalculateTaxAndExcludingTotal(totalIncludingTax, taxRate);
                response.SalesTax = salesTax;
                response.TotalExcludingTax = totalExcludingTax;

                response.CostCentre = await Task.Run(() => GetCostCentreOrDefault(request.Text));

                response.IsSuccess = true;
                response.Message = Constants.ParsingSuccessful;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"{Constants.ExceptionPrefix}{ex.Message}";
                throw;
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Checks for unmatched or improperly nested XML-like tags in the text.
        /// </summary>
        /// <param name="text">The input text to validate.</param>
        /// <returns>True if tags are valid; otherwise, false.</returns>
        private (bool isSuccess, string tag) CheckForValidTags(string text)
        {
            try
            {
                bool isSuccess = true;
                var openingTags = Regex.Matches(text, @"<([a-zA-Z0-9_]+)[^>]*>");
                var closingTags = Regex.Matches(text, @"</([a-zA-Z0-9_]+)>");

                var openTagNames = openingTags.Cast<Match>().Select(m => m.Groups[1].Value).ToList();
                var closeTagNames = closingTags.Cast<Match>().Select(m => m.Groups[1].Value).ToList();

                if (openTagNames.Count == 0 || closeTagNames.Count == 0)
                {
                    isSuccess = false;
                    return (isSuccess, "");
                }

                foreach (var tag in openTagNames)
                {
                    if (openTagNames.Count(t => t == tag) != closeTagNames.Count(t => t == tag))
                    {
                        isSuccess = false;
                        return (isSuccess, tag);
                    }
                }
                return (isSuccess, "");


            }
            catch (Exception ex)
            {
                throw new Exception($"{Constants.ErrorCheckForValidTags}{ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the &lt;total&gt; tag is missing in the text.
        /// </summary>
        /// <param name="text">The input text to check.</param>
        /// <returns>True if the &lt;total&gt; tag exists; otherwise, false.</returns>
        private bool IsTotalTagMissing(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return false;

                var match = Regex.Match(text, @"<total>.*?</total>", RegexOptions.Singleline);
                return match.Success;
            }
            catch (Exception ex)
            {
                throw new Exception($"{Constants.ErrorIsTotalTagMissing}{ex.Message}", ex);
            }
        }

        /// <summary>
        /// Extracts the value of &lt;cost_centre&gt; from the text, or returns "UNKNOWN" if missing.
        /// </summary>
        /// <param name="text">The input text to parse.</param>
        /// <returns>The cost centre value or "UNKNOWN".</returns>
        private string GetCostCentreOrDefault(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return Constants.UnknownCostCentre;

                var match = Regex.Match(text, @"<cost_centre>(.*?)</cost_centre>", RegexOptions.Singleline);
                if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
                    return match.Groups[1].Value.Trim();

                return Constants.UnknownCostCentre;
            }
            catch (Exception ex)
            {
                throw new Exception($"{Constants.ErrorGetCostCentreOrDefault}{ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates the sales tax and total excluding tax from the total including tax.
        /// </summary>
        /// <param name="totalIncludingTax">Total amount including tax.</param>
        /// <param name="taxRate">Tax rate as a decimal (e.g., 0.10 for 10%).</param>
        /// <returns>Tuple containing sales tax and total excluding tax.</returns>
        private (decimal salesTax, decimal totalExcludingTax) CalculateTaxAndExcludingTotal(decimal totalIncludingTax, decimal taxRate)
        {
            try
            {
                decimal salesTax = decimal.Round(totalIncludingTax / taxRate, 2);
                decimal totalExcludingTax = decimal.Round(totalIncludingTax - salesTax, 2);

                return (salesTax, totalExcludingTax);
            }
            catch (Exception ex)
            {
                throw new Exception($"{Constants.ErrorCalculateTaxAndExcludingTotal}{ex.Message}", ex);
            }
        }

        #endregion
    }
}
