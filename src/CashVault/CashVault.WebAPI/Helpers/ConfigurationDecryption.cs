using System.Security.Cryptography;
using System.Text;

namespace CashVault.WebAPI.Helpers;

public static class ConfigurationDecryption
{
    /// <summary>
    /// Loads encryption keys from /etc/monroe/.env file or falls back to environment variables
    /// </summary>
    public static (string? key, string? iv) LoadEncryptionKeys()
    {
        const string envFilePath = "/etc/monroe/.env";
        
        if (!File.Exists(envFilePath))
        {
            // Fallback to environment variables
            return (
                Environment.GetEnvironmentVariable("MONROE_AES_KEY"),
                Environment.GetEnvironmentVariable("MONROE_AES_IV")
            );
        }
        
        string? key = null;
        string? iv = null;
        
        try
        {
            foreach (var line in File.ReadAllLines(envFilePath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                    continue;
                    
                var parts = trimmed.Split('=', 2);
                if (parts.Length != 2)
                    continue;
                    
                var name = parts[0].Trim();
                var value = parts[1].Trim();
                
                if (name == "MONROE_AES_KEY")
                    key = value;
                else if (name == "MONROE_AES_IV")
                    iv = value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to read {envFilePath}: {ex.Message}");
            // Fallback to environment variables on error
            return (
                Environment.GetEnvironmentVariable("MONROE_AES_KEY"),
                Environment.GetEnvironmentVariable("MONROE_AES_IV")
            );
        }
        
        return (key, iv);
    }
    
    public static string DecryptAppSettings(string encryptedFilePath, string keyBase64, string ivBase64)
    {
        try
        {
            if (!File.Exists(encryptedFilePath))
            {
                throw new FileNotFoundException($"Encrypted configuration file not found: {encryptedFilePath}");
            }
            
            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);
            
            // Validate key size (AES supports 128, 192, or 256 bits)
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            {
                throw new InvalidOperationException($"Invalid AES key size: {key.Length} bytes. Must be 16, 24, or 32 bytes.");
            }
            
            // Validate IV size (must be 16 bytes for AES)
            if (iv.Length != 16)
            {
                throw new InvalidOperationException($"Invalid AES IV size: {iv.Length} bytes. Must be 16 bytes.");
            }
            
            byte[] cipherText = File.ReadAllBytes(encryptedFilePath);
            
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            using MemoryStream msDecrypt = new(cipherText);
            using CryptoStream csDecrypt = new(msDecrypt, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            
            return srDecrypt.ReadToEnd();
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Failed to decrypt configuration. Verify MONROE_AES_KEY and MONROE_AES_IV are correct.", ex);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Invalid base64 format for MONROE_AES_KEY or MONROE_AES_IV.", ex);
        }
    }
}
