using System.Threading.Tasks;

namespace MobiControlApi
{
    public interface IWebApi
    {
        Task<string> GetJsonAsync(string resourcePath);
        Task<bool> PostAsync(string resourcePath, string body, string ContentType);
        Task<bool> PostJsonAsync(string resourcePath, string body);
        Task<bool> PutAsync(string resourcePath, string body, string ContentType);
        Task<bool> PutJsonAsync(string resourcePath, string body);
    }
}