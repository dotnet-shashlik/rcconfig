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
            if (BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash))
                return true;

            // 兼容旧版本MD5密码
            return HashHelper.MD5(plainPassword).ToUpperInvariant() == passwordHash.ToUpperInvariant();
        }
    }
}