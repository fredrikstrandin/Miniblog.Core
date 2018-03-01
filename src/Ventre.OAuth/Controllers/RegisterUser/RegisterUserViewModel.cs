using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.OAuth.Controllers
{
    public class RegisterUserViewModel
    {
        // credentials       
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        // claims 
        [Required]
        [MaxLength(100)]
        public string Firstname { get; set; }

        [Required]
        [MaxLength(100)]
        public string Lastname { get; set; }

        [Required]
        [MaxLength(249)]
        public string Email { get; set; }

        public string ProfileImage { get; set; }

        [Required]
        [MaxLength(2)]
        public string Country { get; set; }

        public SelectList CountryCodes { get; set; } =
            new SelectList(
                new[] 
                {
                    new { Id = "SE", Value = "Sweden" },
                    new { Id = "BE", Value = "Belgium" },
                    new { Id = "US", Value = "United States of America" },
                    new { Id = "IN", Value = "India" },
                }, "Id", "Value");

        public string ReturnUrl { get; set; }

        public string Provider { get; set; }

        public string ProviderUserId { get; set; }

        public bool IsProvisioningFromExternal
        {
            get
            {
                return (Provider != null);
            }
        }
    }
}

