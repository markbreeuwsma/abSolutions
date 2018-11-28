using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication_MVC.Models
{
    public interface ICrudRepositoryAsync<T, K> where T : class
    {
        Task<bool> ExistAsync(K id);
        Task<T> ReadAsync(K id);
        Task<IEnumerable<T>> ReadAllAsync();

        Task<T> RetrieveOriginalValuesAsync(T t);

        bool CreateValid(T t);
        Task<T> CreateAsync(T t);

        bool UpdateValid(T t);
        Task<T> UpdateAsync(T t);

        bool DeleteValid(T t);
        Task<T> DeleteAsync(T t);
    }
}
