using Shashlik.AutoMapper;
using Shashlik.RC.Data.Entities;

#nullable disable
namespace Shashlik.RC.Services.Secret.Dtos
{
    public class SecretDto : IMapFrom<Secrets>
    {
        public string UserId { get; set; }

        public string SecretId { get; set; }
        public string SecretKey { get; set; }

        public long CreateTime { get; set; }
    }
}