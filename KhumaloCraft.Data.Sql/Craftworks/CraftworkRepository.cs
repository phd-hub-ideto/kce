using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Data.Sql.Queries;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Structs;
using KhumaloCraft.Sync;

namespace KhumaloCraft.Data.Sql.Craftworks;

[Singleton(Contract = typeof(ICraftworkRepository))]
public sealed class CraftworkRepository(
    IPrincipalResolver principalResolver,
    IImageReferenceManager imageReferenceManager) : ICraftworkRepository
{
    public IQueryable<Craftwork> Query()
    {
        return QueryContainerFactory.Create(scope =>
            from craftwork in scope.KhumaloCraft.Craftwork
            join craftworkQuantity in scope.KhumaloCraft.CraftworkQuantity on craftwork.Id equals craftworkQuantity.CraftworkId
            select new Craftwork
            {
                Id = craftwork.Id,
                CreatedDate = craftwork.CreatedDate,
                Title = craftwork.Title,
                Description = craftwork.Description,
                Category = (CraftworkCategory)craftwork.CraftworkCategoryId,
                Price = new Money(craftwork.Price),
                IsActive = craftwork.IsActive,
                UpdatedDate = craftwork.UpdatedDate,
                LastEditedByUserId = craftwork.LastEditedByUserId,
                PrimaryImageReferenceId = craftwork.PrimaryImageReferenceId,
                Quantity = craftworkQuantity.Count
            }
        );
    }

    public void Upsert(Craftwork craftwork)
    {
        using var scope = DalScope.Begin();

        DalCraftwork dalCraftwork;

        if (craftwork.IsNew)
        {
            dalCraftwork = new DalCraftwork
            {
                CreatedDate = scope.ServerDate()
            };
        }
        else
        {
            dalCraftwork = scope.KhumaloCraft.Craftwork.Single(c => c.Id == craftwork.Id.Value);
        }

        dalCraftwork.Title = craftwork.Title;
        dalCraftwork.Description = craftwork.Description;
        dalCraftwork.CraftworkCategoryId = (int)craftwork.Category;
        dalCraftwork.Price = craftwork.Price.Amount;
        dalCraftwork.IsActive = craftwork.IsActive;
        dalCraftwork.UpdatedDate = scope.ServerDate();
        dalCraftwork.LastEditedByUserId = principalResolver.GetRequiredUserId();
        dalCraftwork.PrimaryImageReferenceId = craftwork.PrimaryImageReferenceId;

        scope.KhumaloCraft.Craftwork.Update(dalCraftwork);

        scope.Commit();

        UpsertCraftworkQuantity(dalCraftwork.Id, craftwork.Quantity);

        craftwork.Id = dalCraftwork.Id;

        SaveImages(craftwork.Id.Value, craftwork.CraftworkImages);
    }

    public void UpsertCraftworkQuantity(int craftworkId, int quantity)
    {
        using var scope = DalScope.Begin();

        var dalCraftworkQuantity = scope.KhumaloCraft.CraftworkQuantity
                                        .SingleOrDefault(c => c.CraftworkId == craftworkId);

        if (dalCraftworkQuantity == null)
        {
            dalCraftworkQuantity = new DalCraftworkQuantity
            {
                CraftworkId = craftworkId
            };
        }

        dalCraftworkQuantity.Count = quantity;

        scope.KhumaloCraft.CraftworkQuantity.Update(dalCraftworkQuantity);

        scope.Commit();
    }

    private void SaveImages(int craftworkId, List<CraftworkImage> craftworkImages)
    {
        using var scope = DalScope.Begin();

        var currentImages = scope.KhumaloCraft.CraftworkImage
                                .Where(i => i.CraftworkId == craftworkId)
                                .Select(i => i.ImageReferenceId)
                                .ToList();

        SimpleCollectionSync
            .Sync(craftworkImages.Select(i => i.ImageReferenceId).ToList(), currentImages, out _, out var removes, out var adds);

        bool hasRemoves = removes.Any(), hasAdds = adds.Any();

        if (hasRemoves)
        {
            scope.KhumaloCraft.CraftworkImage.RemoveAll(i => removes.Contains(i.ImageReferenceId));
        }

        if (hasAdds)
        {
            var dalCraftworkImages = new List<DalCraftworkImage>();

            foreach (var imageReferenceId in adds)
            {
                var dalCraftworkImage = new DalCraftworkImage()
                {
                    CraftworkId = craftworkId,
                    ImageReferenceId = imageReferenceId
                };

                dalCraftworkImages.Add(dalCraftworkImage);
            }

            scope.KhumaloCraft.CraftworkImage.AddRange(dalCraftworkImages);
        }

        if (hasAdds || hasRemoves)
        {
            imageReferenceManager.UpdateReferenceCollection(currentImages, adds);

            scope.Commit();

        }
    }
}
