using System.Net.Http;
using System.Threading.Tasks;

namespace ImageGallery.Web.Services
{
    public interface IImageGalleryHttpClient
    {
        Task<HttpClient> GetClient();
    }
}
