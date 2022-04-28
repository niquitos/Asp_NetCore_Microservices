using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemUpdateConsumer : IConsumer<CatalogItemUpdated>
    {
        private readonly IRepository<CatalogItem> _repository;

        public CatalogItemUpdateConsumer(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {
            var message = context.Message;

            var item = await _repository.GetAsync(message.ItemId);

            if (item == null)
            {
                item = new CatalogItem
                {
                    Id = message.ItemId,
                    Description = message.Description,
                    Name = message.Name
                };

                await _repository.CreateAsync(item);
            }
            else
            {
                item.Name = message.Name;
                item.Description=message.Description;

                await _repository.UpdateAsync(item);
            }


        }
    }
}