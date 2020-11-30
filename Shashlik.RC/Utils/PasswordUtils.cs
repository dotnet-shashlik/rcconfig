using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Utils
{
    public class PasswordUtils
    {
        public static string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        public static bool Verify(string passwordHash, string plainPassword)
        {
            if (passwordHash.Length == 32)
            {
                // 兼容旧版本MD5密码
                return HashHelper.MD5(plainPassword).ToUpperInvariant().Equals(passwordHash.ToUpperInvariant());
            }

            return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
        }
    }
}