using System.ComponentModel.DataAnnotations;

namespace iRocks.WebAPI.Models
{
    public class TokenRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class ParsedExternalAccessToken
    {
        public string user_id { get; set; }
        public string app_id { get; set; }
        public string user_secret { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }
        [Required]
        public string Email { get; set; }

    }
    public class ExternalLoginModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }

    }
}
