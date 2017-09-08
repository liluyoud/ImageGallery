using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ImageGallery.Web.Services;
using Microsoft.AspNetCore.Mvc;
using ImageGallery.Core.ViewModels;
using Newtonsoft.Json;
using ImageGallery.Core.Models;
using System.Net.Http;

namespace ImageGallery.Web.Pages
{
    public class EditImageModel : PageModel
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        [BindProperty]
        public EditImageViewModel Edit { get; set; }

        public EditImageModel(IImageGalleryHttpClient imageGalleryHttpClient)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
        }

        public async Task OnGetAsync(Guid id)
        {
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.GetAsync($"api/images/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var imageAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedImage = JsonConvert.DeserializeObject<Image>(imageAsString);

                this.Edit = new EditImageViewModel();
                this.Edit.Id = deserializedImage.Id;
                this.Edit.Title = deserializedImage.Title;
            }
            else
            {
                throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // create an ImageForUpdate instance
            var imageForUpdate = new ImageForUpdate()
            { Title = Edit.Title };

            // serialize it
            var serializedImageForUpdate = JsonConvert.SerializeObject(imageForUpdate);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PutAsync(
                $"api/images/{id}",
                new StringContent(serializedImageForUpdate, System.Text.Encoding.Unicode, "application/json"))
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
