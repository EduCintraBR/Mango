using Mango.Services.RewardsAPI.Message;
using Mango.Services.RewardsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Mango.Services.RewardsAPI.Services
{
    public class RewardsService : IRewardsService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public RewardsService(DbContextOptions<AppDbContext> dbOptions)
        {
            this._dbOptions = dbOptions;
        }

        public async Task<bool> UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    RewardsDate = DateTime.Now,
                    UserId = rewardsMessage.UserId
                };

                await using var _db = new AppDbContext(this._dbOptions);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
