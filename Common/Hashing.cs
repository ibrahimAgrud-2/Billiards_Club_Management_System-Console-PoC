using System;
using System.Security.Cryptography;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Hashing
    {
        public static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
