using AspNetCore.Reporting;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Web.Mvc.Helpers;

public static class EncryptionHelper
{
    private static readonly string Key = "YourSecretEncryptionKey"; // Must be 16, 24, or 32 characters for AES-128, AES-192, or AES-256

    public static string EncryptString(string plainText)
    {
        using var aes = Aes.Create();
        var key = Encoding.UTF8.GetBytes(Key);
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(plainText);

        var iv = aes.IV;
        var encryptedContent = ms.ToArray();
        var result = new byte[iv.Length + encryptedContent.Length];

        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);

        return Convert.ToBase64String(result);
    }

    public static string DecryptString(string encryptedText)
    {
        var fullCipher = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        var iv = new byte[aes.BlockSize / 8];
        var cipher = new byte[fullCipher.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        var key = Encoding.UTF8.GetBytes(Key);
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipher);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}
