using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeOwnerRentEstimates.Models
{
    public class HomeOwner
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="password should have min 6 characters")]
        [MaxLength(10, ErrorMessage = "password should have max 10 characters")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Email ID")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Not a valid Email ID")]
        public string Email { get; set; }

       
        public string StreetAddress { get; set; }

     
        public string City { get; set; }

       
        public string State { get; set; }

        
        public string Country { get; set; }

      
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }

        [Display(Name = "Rent Amount($)")]
        [DataType(DataType.Currency)]
        public string rentAmount { get; set; }

        [Display(Name = "Max Rent Amount($)")]
        [DataType(DataType.Currency)]
        public string maxRentAmount { get; set; }

        [Display(Name = "Min Rent Amount($)")]
        [DataType(DataType.Currency)]
        public string minRentAmount { get; set; }

        public string IPAddress { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Enter Your Rent Expectation($)")]
        [NotMapped]
        public string ownerRentAmount { get; set; }

        

        //public int AddressId { get; set; }

        //public int RentId { get; set; }

        //[ForeignKey("AddressId")]
        //public Address Address { get; set; }

        //[ForeignKey("RentId")]
        //public RentEstimates RentEstimates { get; set; }
    }
}