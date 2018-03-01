using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace CarteirinhaVacinacao.Classes
{
    public static partial class Utilites
    {
        public static string HashPass(string Pass, string Salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Salt + Pass + Salt + Salt));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return (hash);
            }
        }
    }
}
