﻿using System.Collections.Generic;

namespace Shashlik.RC.Client.Models;

public class ResourceModel
{
    public long Version { get; set; }

    public List<FileModel> Files { get; set; }
}