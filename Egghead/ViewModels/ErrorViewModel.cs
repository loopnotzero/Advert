using Egghead.Common;

namespace Egghead.ViewModels
{
    public class ErrorViewModel
    {
        public string TagName { get; set; }

        public string ErrorMessage { get; set; }
               
        public ErrorStatusCode ErrorStatusCode { get; set; }     
    }
}