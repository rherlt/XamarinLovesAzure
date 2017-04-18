using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DropIt.Web.Client.Extensions
{
    public static class DataContractExtensions
    {
        public static async Task<T> ToDataContractAsync<T>(this HttpResponseMessage message)
        {
            var responseString = await message.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<T>(responseString);
            return obj;
        }
    }
}
