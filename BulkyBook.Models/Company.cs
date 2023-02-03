using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name="Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        [Display(Name="Post Code")]
        public string PostalCode { get; set; }
        [Display(Name="Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
