using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ImageGallery.Core.Models;
using ImageGallery.Web.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics;

namespace ImageGallery.Web.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        public IEnumerable<Image> Images { get; private set; } = new List<Image>();

        public IndexModel(IImageGalleryHttpClient imageGalleryHttpClient)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
        }

        public async Task OnGetAsync()
        {
            await WriteOutIdentityInformation();

            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.GetAsync("api/images").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                Images = JsonConvert.DeserializeObject<IList<Image>>(imagesAsString).ToList();
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                RedirectToPage("AccessDenied");
            }
            else
            {
                throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
            }
        }

        public async Task<IActionResult> OnGetDeleteAsync(Guid id)
        {
            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.DeleteAsync($"api/images/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            else
            {
                throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
            }
        }

        public async Task WriteOutIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // write it out
            Debug.WriteLine($"Identity token: {identityToken}");

            // write out the user claims
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
    }
}
