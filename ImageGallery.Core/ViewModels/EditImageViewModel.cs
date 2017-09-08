using System;
using System.ComponentModel.DataAnnotations;

namespace ImageGallery.Core.ViewModels
{
    public class EditImageViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public Guid Id { get; set; }  
    }
}
