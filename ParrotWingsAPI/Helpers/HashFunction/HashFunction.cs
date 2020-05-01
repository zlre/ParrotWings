namespace ParrotWingsAPI
{
    using System;
    using System.Security.Cryptography;

    public class HashFunction : IHashFunction
    {
        public string Hash(byte[] salt, string password)
        {
            Rfc2898DeriveBytes saltedHash = new Rfc2898DeriveBytes(password, salt, 1000);

            return Convert.ToBase64String(saltedHash.GetBytes(16));
        }
    }
}
