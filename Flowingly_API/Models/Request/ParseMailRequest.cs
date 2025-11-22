using System.ComponentModel.DataAnnotations;

namespace Flowingly.API.Models.Request
{
    public class ParseMailRequest
    {
        [Required]
        public string Text { get; set; }
    }
}
