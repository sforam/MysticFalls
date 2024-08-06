using MysticFalls.Models;

namespace MysticFalls.Services.IServices
{
    public interface IBaseService
    {
            APIResponse responseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
