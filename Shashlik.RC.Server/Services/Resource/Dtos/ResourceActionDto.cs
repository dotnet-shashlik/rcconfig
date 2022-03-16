#nullable disable
namespace Shashlik.RC.Server.Services.Resource.Dtos
{
    public class ResourceActionDto : ResourceDto
    {
        public PermissionAction Action { get; set; }
        public string ActionStr => Action.ToString();
    }
}