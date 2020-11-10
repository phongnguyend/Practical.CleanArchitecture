using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Blazor.Modules.Users.Models
{
    public class SetPasswordModel : IValidatableObject
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        //[Compare("Password")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ConfirmPassword != Password)
            {
                yield return new ValidationResult("Password doesn't match.", new[] { "ConfirmPassword" });
            }
        }
    }
}
