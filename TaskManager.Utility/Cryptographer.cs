using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Azersun.Audit.Utilities;
public static class Cryptographer
{
    private static IConfigurationSection crypto = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Cryptographer");
    private static byte[] EncryptionKey { get; set; }
    private static byte[] Global_IV { get; set; }

    public static void TripleDESImplementation(string encryptionKey, string IV)
    {
        if (string.IsNullOrEmpty(encryptionKey))
        {
            throw new ArgumentNullException("'encryptionKey' parameter cannot be null.", "encryptionKey");
        }
        if (string.IsNullOrEmpty(IV))
        {
            throw new ArgumentException("'IV' parameter cannot be null or empty.", "IV");
        }
        EncryptionKey = Encoding.UTF8.GetBytes(encryptionKey);
        Trace.Assert(EncryptionKey.Length == 24, "Encryption key must be exactly 24 characters of ASCII text (24 bytes)");
        Global_IV = Encoding.UTF8.GetBytes(IV);
        Trace.Assert(Global_IV.Length == 8, "Init. vector must be exactly 8 characters of ASCII text (8 bytes)");
    }

    public static string Encrypt(this string textToEncrypt)
    {
        try
        {
            TripleDESImplementation(crypto["Key1"].ToString(), crypto["Key2"].ToString());

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = EncryptionKey;
            tdes.IV = Global_IV;
            byte[] buffer = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(tdes.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        }
        catch (Exception er)
        {
            return er.Message;
        }
    }

    public static string Decrypt(this string textToDecrypt)
    {
        try
        {
            TripleDESImplementation(crypto["Key1"].ToString(), crypto["Key2"].ToString());
            byte[] buffer = Convert.FromBase64String(textToDecrypt);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = EncryptionKey;
            des.IV = Global_IV;
            return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        }
        catch (Exception er)
        {
            return er.Message;
        }
    }

    public static string SHA1(this string Value)
    {
        using (SHA1Managed sha1 = new SHA1Managed())
        {
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(Value));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }

    public static string SHA1Special(this string Value)
    {
        SHA1 ShaEncrp = new SHA1CryptoServiceProvider();
        Value = string.Format("{0}{1}{0}", crypto["Key3"].ToString(), Value);
        byte[] buffer = Encoding.UTF8.GetBytes(Value);
        byte[] result = ShaEncrp.ComputeHash(buffer);
        return Convert.ToBase64String(result);
    }
}
