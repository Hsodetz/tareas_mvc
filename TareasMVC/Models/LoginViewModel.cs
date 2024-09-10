using System;
using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Models
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Error.Required")]
        [EmailAddress(ErrorMessage = "Error.Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Error.Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Recuérdame")]
        public bool Remember { get; set; }
    }
}

