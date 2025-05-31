using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto
{
    public class SocialProfileDto
    {
        [RegularExpression(@"^https?:\/\/(www\.)?[a-zA-Z0-9\-.]+(\.[a-zA-Z]{2,})(\/[a-zA-Z0-9\-\._~:\/\?#\[\]@!$&'\(\)\*\+,;=]*)?$", ErrorMessage = "Invalid Facebook URL.")]
        public string? Facebook { get; set; }

        [RegularExpression(@"^https?:\/\/(www\.)?[a-zA-Z0-9\-.]+(\.[a-zA-Z]{2,})(\/[a-zA-Z0-9\-\._~:\/\?#\[\]@!$&'\(\)\*\+,;=]*)?$", ErrorMessage = "Invalid Youtube URL.")]
        public string? Youtube { get; set; }

        [RegularExpression(@"^https?:\/\/(www\.)?[a-zA-Z0-9\-.]+(\.[a-zA-Z]{2,})(\/[a-zA-Z0-9\-\._~:\/\?#\[\]@!$&'\(\)\*\+,;=]*)?$", ErrorMessage = "Invalid Twitter URL.")]
        public string? Twitter { get; set; }

        [RegularExpression(@"^https?:\/\/(www\.)?[a-zA-Z0-9\-.]+(\.[a-zA-Z]{2,})(\/[a-zA-Z0-9\-\._~:\/\?#\[\]@!$&'\(\)\*\+,;=]*)?$", ErrorMessage = "Invalid Instagram URL.")]
        public string? Instagram { get; set; }

        [RegularExpression(@"^https?:\/\/(www\.)?[a-zA-Z0-9\-.]+(\.[a-zA-Z]{2,})(\/[a-zA-Z0-9\-\._~:\/\?#\[\]@!$&'\(\)\*\+,;=]*)?$", ErrorMessage = "Invalid Linkedin URL.")]
        public string? Linkedin { get; set; }

        [RegularExpression(@"^https?:\/\/(www\.)?[a-zA-Z0-9\-.]+(\.[a-zA-Z]{2,})(\/[a-zA-Z0-9\-\._~:\/\?#\[\]@!$&'\(\)\*\+,;=]*)?$", ErrorMessage = "Invalid Pinterest URL.")]
        public string? Pinterest { get; set; }
    }
}
