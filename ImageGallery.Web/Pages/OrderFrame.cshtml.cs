using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace ImageGallery.Web.Pages
{
    [Authorize(Policy = "CanOrderFrame")]
    public class OrderFrameModel : PageModel
    {
        public string Address { get; set; }

        public async Task OnGetAsync()
        {
            var discoveryClient = new DiscoveryClient("https://localhost:44379/");
            var metaDataResponse = await discoveryClient.GetAsync();
            var userInfoClient = new UserInfoClient(metaDataResponse.UserInfoEndpoint);
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var response = await userInfoClient.GetAsync(accessToken);
            if (response.IsError)
            {
                throw new Exception("Problem accessing UserInfo EndPoint", response.Exception);
            }

            Address = response.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
        }
    }
}