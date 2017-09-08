using System;

namespace ImageGallery.Core.Models
{
    public class Image
    {      
        public Guid Id { get; set; }
 
        public string Title { get; set; }
 
        public string FileName { get; set; }    
    }
}
