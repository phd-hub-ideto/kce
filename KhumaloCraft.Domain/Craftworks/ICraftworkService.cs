namespace KhumaloCraft.Domain.Craftworks;

public interface ICraftworkService
{
    IEnumerable<Craftwork> GetAllCraftworks();
    IEnumerable<Craftwork> FetchAllCraftworks();
    Craftwork GetCraftworkById(int id);
    bool TryGetCraftworkById(int id, out Craftwork craftwork);
    Craftwork FetchByCraftworkId(int id);
    bool TryFetchByCraftworkId(int id, out Craftwork craftwork);
    void AddCraftwork(Craftwork craftwork);
    void UpdateCraftwork(Craftwork craftwork);
}