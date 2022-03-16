using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shashlik.RC.Server.Services.Identity.Dtos;

namespace Shashlik.RC.Server.Secret.GrantTypes;

public interface IGrantType : IServiceStrategy
{
    Task<UserDetailDto?> Get(JObject request);
}