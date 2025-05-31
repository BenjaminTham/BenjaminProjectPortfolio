using System;
using System.Security.Cryptography;
using System.Text;

public class TokenService1
{
    // Secret key for HMAC encryption
    private readonly string _secretKey;

    // Constructor to initialize the secret key
    public TokenService1(string secretKey)
    {
        _secretKey = secretKey;
    }
    public string SecretKey => _secretKey;

    public string GenerateToken(string email)
    {
        // Get the current Unix timestamp in seconds
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // Generate a unique nonce
        var nonce = GenerateNonce();
        // Concatenate email, timestamp, and nonce to form the data string
        var data = $"{email}:{timestamp}:{nonce}";

        // Create the HMAC signature
        var signature = CreateHMACSignature(data, _secretKey);

        // Concatenate the data and signature
        var tokenWithSignature = $"{data}:{signature}";

        // Encrypt the data string using the secret key
        var encryptedData = EncryptData(tokenWithSignature, _secretKey);
        // Return the encrypted data as a Base64 string
        return Convert.ToBase64String(encryptedData);
    }

    // Method to create HMAC signature
    private string CreateHMACSignature(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }


    // Method to encrypt data using AES
    public byte[] EncryptData(string data, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.GenerateIV();
        var iv = aes.IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, iv);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(data);

        var encryptedData = ms.ToArray();
        var result = new byte[iv.Length + encryptedData.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encryptedData, 0, result, iv.Length, encryptedData.Length);

        return result;
    }


    public bool ValidateToken(string token, string email)
    {
        try
        {
            // Decode the token from Base64
            var encryptedData = Convert.FromBase64String(token);
            // Decrypt the token to get the original data string
            var decryptedData = DecryptData(encryptedData, _secretKey);
            // Split the data string into its components (email, timestamp, nonce, signature)
            var parts = decryptedData.Split(':');

            // Check if the token has the correct number of parts
            if (parts.Length != 4) return false;

            // Extract the email, timestamp, and signature from the token
            var tokenEmail = parts[0];
            var timestamp = long.Parse(parts[1]);
            var nonce = parts[2];
            var signature = parts[3];

            // Validate that the email in the token matches the provided email
            if (tokenEmail != email) return false;

            // Check if the token has expired
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var tokenAge = currentTimestamp - timestamp;
            var expirationTimeInSeconds = 3600; // 1 hour token expiration

            // Validate the token's age
            if (tokenAge > expirationTimeInSeconds) return false;

            // Recreate the data string
            var data = $"{tokenEmail}:{timestamp}:{nonce}";

            // Verify the HMAC signature
            var expectedSignature = CreateHMACSignature(data, _secretKey);
            return signature == expectedSignature;
        }
        catch
        {
            // Return false if any exception occurs
            return false;
        }
    }


    // Method to decrypt data using AES
    public string DecryptData(byte[] encryptedData, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);

        var iv = new byte[aes.BlockSize / 8];
        var cipherText = new byte[encryptedData.Length - iv.Length];

        Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(encryptedData, iv.Length, cipherText, 0, cipherText.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }

    // Method to generate a random nonce
    private string GenerateNonce()
    {
        using var rng = new RNGCryptoServiceProvider();
        var nonce = new byte[16];
        rng.GetBytes(nonce);
        return Convert.ToBase64String(nonce);
    }
}
