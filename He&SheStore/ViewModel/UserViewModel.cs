﻿#nullable disable
using System.ComponentModel.DataAnnotations;

namespace He_SheStore.ViewModel
{
    public class UserViewModel
    {
        public string userId { get; set; }
        [Required]
        [Display(Name = "First Name ")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name ")]
        public string LastName { get; set; }   
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]

        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        public string UserRole { get; set; }
    }
}
