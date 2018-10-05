using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dpearson4_MVC_Music.Data;

namespace dpearson4_MVC_Music.Models
{
    public class Album : IValidatableObject
    {
        public Album()
        {
            Songs = new HashSet<Song>();
            Price = 19.99m; //Default: Avoid validation error is you create
                            //the object without supplying a value.
        }
        public int ID { get; set; }


        [Display(Name = "Album")]
        [Required(ErrorMessage = "You cannot leave the Album name blank.")]
        [StringLength(50, ErrorMessage = "Album name cannot be more than 50 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Year Produced")]
        [Required(ErrorMessage = "You cannot leave the Year Produced blank.")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "The Year Produced must be entered as exactly 4 numeric digits.")]
        [StringLength(4)]//DS Note: we only include this to limit the size of the database field to 10
        public string YearProduced { get; set; }

        [Required(ErrorMessage = "You must enter a price for the Album.")]
        [Range(1.0, 200000.00, ErrorMessage = "Price must be between $1.00 and $200,000.00")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Genre")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select the primary Genre of the Album.")]
        public int GenreID { get; set; }
        public Genre Genre { get; set; }

        public ICollection<Song> Songs { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Convert.ToInt16(YearProduced) > DateTime.Now.Year + 1)
            {
                yield return new ValidationResult(
                    $"Year produced cannot be more than 1 year in the future.",
                    new[] { "YearProduced" });
            }
            
        }

        [Display(Name = "Summary")]
        public string Summary
        {
            get
            {
                return $"Title: {Name}\n" +
                    $"Year Produced: {YearProduced}";
            }
        }

        


    }
}
