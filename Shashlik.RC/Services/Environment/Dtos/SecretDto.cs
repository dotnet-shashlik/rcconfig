using Shashlik.AutoMapper;
using Shashlik.RC.Data.Entities;

#nullable disable
namespace Shashlik.RC.Services.Environment.Dtos
{
    public class SecretDto : IMapFrom<Secrets>
    {
        public int Id { get; set; }

        public string SecretId { get; set; }

        public string SecretKey { get; set; }

        public bool Enabled { get; set; }

        public long CreateTime { get; set; }
    }
}