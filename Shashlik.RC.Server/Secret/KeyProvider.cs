using System.IO;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Helpers;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Server.Secret;

[Singleton]
public class KeyProvider
{
    private const string _file = "./sign.key";

    public string GetKey()
    {
        if (!File.Exists(_file))
        {
            var key = RandomHelper.RandomString(24);
            File.WriteAllText(_file, key);
            return key;
        }

        return File.ReadAllText(_file);
    }
}