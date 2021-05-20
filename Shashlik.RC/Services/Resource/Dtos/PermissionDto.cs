using Shashlik.RC.Services.Resource;

#nullable disable
namespace Shashlik.RC.Services.Permission.Dtos
{
    public class PermissionDto
    {
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public PermissionAction Action { get; set; }
        public string ActionStr => Action.ToString();
    }
}