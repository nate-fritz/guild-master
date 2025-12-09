using System;
using System.Threading.Tasks;

namespace GuildMaster.Services
{
    public interface IStorageService
    {
        Task<string?> ReadTextAsync(string key);
        Task WriteTextAsync(string key, string content);
        Task<bool> ExistsAsync(string key);
        Task<DateTime?> GetLastWriteTimeAsync(string key);
        Task CopyAsync(string sourceKey, string destKey);
    }
}
