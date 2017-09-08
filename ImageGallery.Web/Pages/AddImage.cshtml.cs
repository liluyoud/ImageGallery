using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using ImageGallery.Web.Services;
using ImageGallery.Core.Models;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using ImageGallery.Core.ViewModels;

namespace ImageGallery.Web.Pages
{
    public class AddImageModel : PageModel
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        [BindProperty]
        public AddImageViewModel Input { get; set; }

        public AddImageModel(IImageGalleryHttpClient imageGalleryHttpClient)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // create an ImageForCreation instance
            var imageForCreation = new ImageForCreation()
            { Title = this.Input.Title };

            // take the first (only) file in the Files list
            var imageFile = this.Input.Files.First();

            if (imageFile.Length > 0)
            {
                using (var fileStream = imageFile.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    imageForCreation.Bytes = ms.ToArray();
                }
            }

            // serialize it
            var serializedImageForCreation = JsonConvert.SerializeObject(imageForCreation);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PostAsync(
                $"api/images",
                new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            else
            {
                throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
            }
        }
    }
}
