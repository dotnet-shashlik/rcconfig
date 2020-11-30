using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shashlik.RC.Data.Entities;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Utils
{
    public static class AccountUtils
    {
        private static readonly object Lck = new object();

        public static void LoginFailed(this DbContext dbContext, string account)
        {
            lock (Lck)
            {
                var entity = dbContext.Set<AccountLocks>().FirstOrDefault(r => r.Id == account);

                if (entity is null)
                {
                    entity = new AccountLocks {Id = account};
                    dbContext.Add(entity);
                }

                if (entity.LoginFailedCount >= 5)
                {
                    entity.LoginFailedCount = 0;
                    entity.LockEnd = DateTime.Now.AddMinutes(5).GetLongDate();
                }
                else
                {
                    entity.LoginFailedCount += 1;
                }

                dbContext.SaveChanges();
            }
        }

        public static void ResetLockEnd(this DbContext dbContext, string account)
        {
            lock (Lck)
            {
                var entity = dbContext.Set<AccountLocks>().FirstOrDefault(r => r.Id == account);
                if (entity is null)
                {
                    entity = new AccountLocks {Id = account};
                    dbContext.Add(entity);
                }

                entity.LoginFailedCount = 0;
                entity.LockEnd = 0;
                dbContext.SaveChanges();
            }
        }

        public static bool IsLockout(this DbContext dbContext, string account)
        {
            var entity = dbContext.Set<AccountLocks>().FirstOrDefault(r => r.Id == account);

            if (entity is null)
            {
                entity = new AccountLocks {Id = account};
                dbContext.Add(entity);
            }

            return entity.LockEnd >= DateTime.Now.GetLongDate();
        }
    }
}