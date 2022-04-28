using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient _httpClient;

        public CatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
        {
            var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
            return items;
        }
    }
}