using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StockAnalysis
{
    ///<summary>
    /// Get history of a specific stock from IEX Cloud
    ///</summary>
    public class IEXCloudService
    {
        public HttpClient Client { get; }

        public IEXCloudService(HttpClient client)
        {
            client.BaseAddress = new Uri("https://sandbox.iexapis.com/stable/stock/");
            Client = client;
        }

        public async Task<List<StockHistoryDay>> GetStockHistory(string token)
        {
            string query = "MSFT/chart/6m?token=T" + token;
            var response = await Client.GetAsync(query);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return await JsonSerializer.DeserializeAsync<List<StockHistoryDay>>(responseStream, options);
        }
    }
}