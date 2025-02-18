using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.OrderDtos
{
    [Owned]
    public class AddressDto
    {
        [Required(ErrorMessage = "Please enter a Street")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "Please enter a Suburb")]
        public string? Suburb { get; set; }

        [Required(ErrorMessage = "Please enter a Postcode")]
        public int Postcode { get; set; }

        [Required(ErrorMessage = "Please enter a City")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please enter a Country")]
        public string? Country { get; set; }
    }
}
