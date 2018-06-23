using Egghead.Common;

namespace Egghead.Models
{
    public class ErrorViewModel
    {
        public string TagName { get; set; }       
        public string RedirectUrl { get; set; }
        public string ErrorMessage { get; set; }                   
        public ErrorStatusCode ErrorStatusCode { get; set; }     
    }
}