using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CMSMock.Utils;

public class RequestVerifier
{
    private readonly RSA _publicKey;
    private readonly RSA _privateKey;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RequestVerifier(string publicKeyPem, string privateKeyPem)
    {
        _publicKey = RSA.Create();
        _publicKey.ImportFromPem(publicKeyPem);
        _privateKey = RSA.Create();
        _privateKey.ImportFromPem(privateKeyPem);
    }

    /// <summary>
    /// SHA-512 and RSA signature verification
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="signatureBase64"></param>
    /// <returns></returns>
    public bool VerifyRequest(JsonDocument payload, string signatureBase64)
    {
        var canonized = CanonicalizeJson(payload);
        byte[] dataBytes = Encoding.UTF8.GetBytes(canonized);

        var fromBase64 = Convert.FromBase64String(signatureBase64);

        bool isValid = _publicKey.VerifyData(dataBytes, fromBase64, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

        return isValid;
    }

    private string CanonicalizeJson(JsonDocument json)
    {
        return JsonSerializer.Serialize(json, _jsonOptions);
    }
}
