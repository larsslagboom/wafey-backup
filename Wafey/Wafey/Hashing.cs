using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Wafey
{

    /*
     * 
     * 
     * Example code on how to use this to hash passwords.
     * In this example password 1 would be the password stored in the database and password2 would be the password input by the user.
     * 
     * 
     *       --password that user would input when registering--
     *       string password1 = "passsword123";
     *       
     *       --password would be hashed and salted only that one them in the registering process after that it will be stored in the database--
     *       HashSalt hashSalt = GenerateSaltedHash(64, password1);
     *       
     *       --password input by the user--
     *       string password2 = "passsword123";
     *       
     *       --Function to check if the passwords match--
     *       bool isPasswordMatched = VerifyPassword(password2, hashSalt.Hash, hashSalt.Salt);
     *
     *       --just a simple statement to do something with the password bool--
     *       if (isPasswordMatched)
     *       {
     *           //Login Successfull
     *           Console.WriteLine("Successfull");
     *       }
     *       else
     *       {
     *           //Login Failed
     *           Console.WriteLine("Failed");
     *       }
     * 
     */

    class Hashing
    {

        public HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            byte[] bytesHash = Encoding.ASCII.GetBytes(hashPassword);
            hashPassword = HexEncode(bytesHash);

            byte[] bytesSalt = Encoding.ASCII.GetBytes(salt);
            salt = HexEncode(bytesSalt);

            HashSalt hashSalt = new HashSalt { Hash = hashPassword, Salt = salt };
            return hashSalt;
        }

        public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {

            byte[] bytesHash = HexDecode(storedHash);
            storedHash = Encoding.ASCII.GetString(bytesHash);

            byte[] bytesSalt = HexDecode(storedSalt);
            storedSalt = Encoding.ASCII.GetString(bytesSalt);

            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

        public static string HexEncode(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }


        public static byte[] HexDecode(string hexEncoded)
        {
            int numChars = hexEncoded.Length;
            byte[] retVal = new byte[numChars / 2];
            for (int i = 0; i < numChars; i += 2) { retVal[i / 2] = Convert.ToByte(hexEncoded.Substring(i, 2), 16); }
            return retVal;
        }

    }
}
