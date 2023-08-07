using Mango.Services.RewardsAPI.Message;

namespace Mango.Services.RewardsAPI.Services
{
    public interface IRewardsService
    {
        void SendEmail(string toAddress, string subject, string body);
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
