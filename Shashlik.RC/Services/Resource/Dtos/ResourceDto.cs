#nullable disable
namespace Shashlik.RC.Services.Resource.Dtos
{
    public class ResourceDto
    {
        public string Id { get; set; }
        public string ResourceType => Id.Contains('/') ? "Application" : "Environment";
    }
}