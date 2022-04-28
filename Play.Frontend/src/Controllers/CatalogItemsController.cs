using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Play.Frontend.Models;
using Play.Frontend.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Play.Frontend.Controllers
{
    public class CatalogItemsController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        private object _baseUrl;

        public CatalogItemsController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = config.GetSection("ServicesSettings:PlayCatalog").Get<ServiceSettingsBase>().ToString();
        }

        // GET: CatalogItems
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}/items");

            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadAsAsync<IEnumerable<ItemDto>>();

                return View(items);
            }

            return NotFound();
        }

        // GET: CatalogItems/Details/{id}
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}/items/{id}");

            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadAsAsync<ItemDto>();

                if (item == null) return NotFound();

                return View(item);
            }

            return NotFound(id);
        }

        // GET: CatalogItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CatalogItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price")] CreateItemDto createItemDto)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.PostAsJsonAsync($"{_baseUrl}/items", createItemDto);

                return RedirectToAction(nameof(Index));
            }
            return View(createItemDto);
        }

        // GET: CatalogItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}/items/{id}");

            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadAsAsync<ItemDto>();

                if (item == null) return NotFound();

                return View(item);
            }
            return NotFound();
        }

        // POST: CatalogItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,Price")] UpdateItemDto updateItemDto)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.PutAsJsonAsync($"{_baseUrl}/items/{id}", updateItemDto);

                return RedirectToAction(nameof(Index));
            }
            return View(updateItemDto);
        }

        // GET: CatalogItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}/items/{id}");

            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadAsAsync<ItemDto>();

                if (item == null) return NotFound();

                return View(item);
            }
            return NotFound();
        }

        // POST: CatalogItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_baseUrl}/items/{id}");

            return RedirectToAction(nameof(Index));
        }


    }
}
