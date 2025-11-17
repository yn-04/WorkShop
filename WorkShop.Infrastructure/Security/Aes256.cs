using System.Security.Cryptography;
using System.Text;

namespace Dashboard.Infrastructure.Security
{
    /// <summary>
    /// ใช้เข้ารหัสข้อมูล แบบ Advanced Encryption Standard (AES) ระดับ 256 bits ซึ่งเป็น symmetric key encryption algorithm ที่สามารถเข้ารหัสแล้วสามารถถอดค่าคืนได้
    /// </summary>
    public sealed class Aes256
    {
        private const int DefaultKeySize = 256;
        private const int DefaultBlockSize = 128;
        private readonly Encoding encoding = Encoding.UTF8;
        private readonly byte[] salt;

        /// <summary>
        /// Create instance ด้วย default Salt, สำหรับการใช้งานทั่วไป ที่ไม่ต้องการลงลึกเรื่อง security
        /// </summary>
        public static Aes256 DefaultInstance
        {
            get
            {
                byte[] defaultSaltBytes = { 12, 211, 36, 78, 121, 255, 128, 5, 99, 192 };

                return new Aes256(defaultSaltBytes);
            }
        }

        public Aes256(byte[] salt)
        {
            this.salt = salt;
        }

        public string Encrypt(string text, string key)
        {
            byte[] textInBytes = encoding.GetBytes(text);
            byte[] passwordBytes = encoding.GetBytes(key);

            byte[] bytesEncrypted = Encrypt(textInBytes, passwordBytes);
            return Convert.ToBase64String(bytesEncrypted);
        }

        public string Decrypt(string text, string key)
        {
            byte[] textInBytes = Convert.FromBase64String(text);
            byte[] passwordBytes = encoding.GetBytes(key);

            byte[] bytesDecrypted = Decrypt(textInBytes, passwordBytes);
            return encoding.GetString(bytesDecrypted);
        }

        public byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged algorithm = new RijndaelManaged())
                {
                    algorithm.KeySize = DefaultKeySize;
                    algorithm.BlockSize = DefaultBlockSize;

                    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                    algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
                    algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

                    algorithm.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    result = ms.ToArray();
                }
            }

            return result;
        }

        public byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged algorithm = new RijndaelManaged())
                {
                    algorithm.KeySize = DefaultKeySize;
                    algorithm.BlockSize = DefaultBlockSize;

                    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                    algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
                    algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

                    algorithm.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    result = ms.ToArray();
                }
            }

            return result;
        }
    }
}