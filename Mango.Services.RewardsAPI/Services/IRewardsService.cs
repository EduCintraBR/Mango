using Mango.Services.RewardsAPI.Message;

namespace Mango.Services.RewardsAPI.Services
{
    public interface IRewardsService
    {
        Task<bool> UpdateRewards(RewardsMessage rewardsMessage);
    }
}
