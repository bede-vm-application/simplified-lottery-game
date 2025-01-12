namespace Bede.Lottery.Services
{
    using Models;

    public interface IPlayerService
    {
        Task<DrawModel> GetPlayerInputAsync();
    }
}
