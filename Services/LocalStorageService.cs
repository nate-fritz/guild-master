using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace GuildMaster.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string?> ReadTextAsync(string key)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            }
            catch
            {
                return null;
            }
        }

        public async Task WriteTextAsync(string key, string content)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, content);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var value = await ReadTextAsync(key);
            return value != null;
        }

        public async Task<DateTime?> GetLastWriteTimeAsync(string key)
        {
            // LocalStorage doesn't track write times, so we'll store it in metadata
            var metadataKey = $"{key}_metadata";
            var metadata = await ReadTextAsync(metadataKey);

            if (metadata != null)
            {
                try
                {
                    var meta = JsonSerializer.Deserialize<SaveMetadata>(metadata);
                    return meta?.LastWriteTime;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public async Task CopyAsync(string sourceKey, string destKey)
        {
            var content = await ReadTextAsync(sourceKey);
            if (content != null)
            {
                await WriteTextAsync(destKey, content);

                // Also copy metadata
                var sourceMetadata = await ReadTextAsync($"{sourceKey}_metadata");
                if (sourceMetadata != null)
                {
                    await WriteTextAsync($"{destKey}_metadata", sourceMetadata);
                }
            }
        }

        private class SaveMetadata
        {
            public DateTime LastWriteTime { get; set; }
        }
    }
}
