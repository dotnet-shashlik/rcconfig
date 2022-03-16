using Shashlik.RC.Server.Services.Resource;

#nullable disable
namespace Shashlik.RC.Server.Services.Permission.Dtos
{
    public class PermissionDto
    {
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public PermissionAction Action { get; set; }
        public string ActionStr => Action.ToString();
    }
}