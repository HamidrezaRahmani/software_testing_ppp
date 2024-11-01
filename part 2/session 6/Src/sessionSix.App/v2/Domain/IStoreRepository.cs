namespace sessionSix.App.V2.Domain;

public interface IStoreRepository
{
    Store GetBy(string id);
}