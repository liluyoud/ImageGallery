using System.ComponentModel.DataAnnotations;

namespace ImageGallery.Core.Models
{ 
    public class ImageForUpdate
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }      
    }
}
