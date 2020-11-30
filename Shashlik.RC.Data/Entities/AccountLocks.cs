using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    public class AccountLocks : IEntity<string>
    {
        public string Id { get; set; }

        /// <summary>
        /// 锁定结束时间
        /// </summary>
        public long LockEnd { get; set; }

        /// <summary>
        /// 登录失败次数
        /// </summary>
        public int LoginFailedCount { get; set; }
    }
}