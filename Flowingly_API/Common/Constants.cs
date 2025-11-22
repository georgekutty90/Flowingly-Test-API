namespace Flowingly_API.Common
{
    public static class Constants
    {
        public const string GenericErrorMessage = "An exception occurred in Flowingly Api";
        public const string MailContentEmpty = "The mail content is empty";
        public const string InvalidTags = "The text contains no tags or unmatched or improperly nested XML-like tags.";
        public const string UnmatchedTag = "The <$> tag is unmatched or improperly nested XML-like tags.";
        public const string TotalTagMissing = "The <total> tag is missing in the mail content.";
        public const string ParsingSuccessful = "Parsing successful";
        public const string UnknownCostCentre = "UNKNOWN";
        public const string ExceptionPrefix = "Exception occurred: ";
        public const string ErrorCheckForValidTags = "Error in CheckForValidTags: ";
        public const string ErrorIsTotalTagMissing = "Error in IsTotalTagMissing: ";
        public const string ErrorGetCostCentreOrDefault = "Error in GetCostCentreOrDefault: ";
        public const string ErrorParseTagsToJson = "Error in ParseTagsToJson: ";
        public const string ErrorCalculateTaxAndExcludingTotal = "Error in CalculateTaxAndExcludingTotal: ";
        public const string ApplicationKey = "ApplicationKey";
    }
}
