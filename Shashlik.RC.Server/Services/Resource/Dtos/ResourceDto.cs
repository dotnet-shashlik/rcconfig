﻿using Shashlik.RC.Server.Data;

#nullable disable
namespace Shashlik.RC.Server.Services.Resource.Dtos
{
    public class ResourceDto : IResource
    {
        public string Id { get; set; }
        public string ResourceType => Id.Contains('/') ? "Environment" : "Application";
        public string ResourceId => Id;
    }
}