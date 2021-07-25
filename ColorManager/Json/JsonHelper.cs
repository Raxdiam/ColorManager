using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ColorManager.Json
{
    public static class JsonHelper
    {
        public static async Task<T> DeserializeAsync<T>(string value) => 
            await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(value)).ConfigureAwait(true);

        public static async Task<T> DeserializeAsync<T>(string value, JsonSerializerSettings settings) =>
            await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(value, settings)).ConfigureAwait(true);

        public static async Task<string> SerializeAsync(object value) =>
            await Task.Factory.StartNew(() => JsonConvert.SerializeObject(value)).ConfigureAwait(true);

        public static async Task<string> SerializeAsync(object value, Formatting formatting) =>
            await Task.Factory.StartNew(() => JsonConvert.SerializeObject(value, formatting)).ConfigureAwait(true);

        public static async Task<string> SerializeAsync(object value, JsonSerializerSettings settings) =>
            await Task.Factory.StartNew(() => JsonConvert.SerializeObject(value, settings)).ConfigureAwait(true);

        public static async Task<string> SerializeAsync(object value, Formatting formatting, JsonSerializerSettings settings) =>
            await Task.Factory.StartNew(() => JsonConvert.SerializeObject(value, formatting, settings)).ConfigureAwait(true);
    }
}
