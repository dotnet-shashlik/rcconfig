using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jinkong.RC.Config
{
    class InternalService
    {
        public static IServiceCollection Services { get; } = new ServiceCollection();
    }
}
