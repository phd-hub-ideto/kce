namespace KhumaloCraft.Domain.Craftworks;

public interface ICraftworkRepository
{
    IQueryable<Craftwork> Query();

    void Upsert(Craftwork craftwork);

    void UpsertCraftworkQuantity(int craftworkId, int quantity);
}