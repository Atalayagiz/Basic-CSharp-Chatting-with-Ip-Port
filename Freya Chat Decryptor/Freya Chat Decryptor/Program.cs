using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Freya_Chat_Decryptor
{
    class Program   
    {

        #region Decryption

        public static string Decrypt(string cipherText)
        {
            string hashx = "VOsSXlJofrPHawTjlvedDPUfE67paLvidifDrJPQtZyAOeyp6tWwZg5kGuoCb0ThkVh7XWPR3NQDClI7UB3Tg3O0jOK4fCWB2Fstv843u2E36pdNaDZVl6EHNgWHJpHpaRIo4ZX9fVi6jPiOUh2V5YQeU6wrhs5JeCLRhy3vCnEqhtJIpOx9OgqrPuQtqca9EFaGzZMzcDccdqEaQp38WZrCapfaoX2JkU7EGoPKv1jq5E7jgqFJ03ZhdQQPhEmK";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(hashx, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion








        static void Main(string[] args)
        {

            Console.Clear();

            int checker = 0;

            Console.WriteLine("Şifrelenmiş yazıyı girin: ");

            while (checker == 0)
            {


                string ttd = Console.ReadLine();

                string dt = Decrypt(ttd);

                Console.WriteLine(dt);

            }




        }
    }
}
