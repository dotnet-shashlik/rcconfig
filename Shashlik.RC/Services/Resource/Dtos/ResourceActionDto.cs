#nullable disable
namespace Shashlik.RC.Services.Resource.Dtos
{
    public class ResourceActionDto : ResourceDto
    {
        public PermissionAction Action { get; set; }
        public string ActionStr => Action.ToString();
    }
}