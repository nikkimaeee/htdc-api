using System.Security.Cryptography;
using System.Text;
using htdc_api.Interface;

namespace htdc_api.Services;

public class MD5CryptoService: IMD5CryptoService
{
    static string key { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";
    public string Encrypt(string text)
    {
        using (var md5 = new MD5CryptoServiceProvider())
        {
            using (var tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                using (var transform = tdes.CreateEncryptor())
                {
                    byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                    byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                    return Convert.ToBase64String(bytes, 0, bytes.Length);
                }
            }
        }
    }
    
    public string Decrypt(string cipher)
    {
        using (var md5 = new MD5CryptoServiceProvider())
        {
            using (var tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                using (var transform = tdes.CreateDecryptor())
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipher);
                    byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return UTF8Encoding.UTF8.GetString(bytes);
                }
            }
        }
    }
}