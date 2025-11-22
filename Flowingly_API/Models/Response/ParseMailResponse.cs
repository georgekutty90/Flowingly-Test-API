namespace Flowingly.API.Models.Response
{
    public class ParseMailResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public decimal SalesTax { get; set; }
        public decimal TotalExcludingTax { get; set; }
        public string CostCentre { get; set; }
    }
}
