using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CashVault.Domain.Common;

namespace CashVault.Infrastructure.Server;

public class SignatureProvider
{
    private readonly RSA _privateKey;
    private readonly CashVault.Domain.Common.HashAlgorithmType _supportedHashAlg;
    private readonly SignatureAlgorithmType _supportedSignatureAlg;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false // Ensuring consistent serialization for signing
    };

    public SignatureProvider(string privateKeyPem, Domain.Common.HashAlgorithmType? supportedHashAlg = null, SignatureAlgorithmType? supportedSignatureAlg = null)
    {
        _privateKey = RSA.Create();
        _privateKey.ImportFromPem(privateKeyPem);
        _supportedHashAlg = supportedHashAlg ?? Domain.Common.HashAlgorithmType.Default;
        _supportedSignatureAlg = supportedSignatureAlg ?? SignatureAlgorithmType.RSA;
    }

    public string SignRequest(string payload, Domain.Common.HashAlgorithmType hashAlgorithm, SignatureAlgorithmType signatureAlgorithm)
    {
        if (hashAlgorithm.Code != _supportedHashAlg.Code || _supportedSignatureAlg.Code != signatureAlgorithm.Code)
        {
            throw new NotSupportedException($"Hash algorithm {hashAlgorithm.Code} or signature algorithm {signatureAlgorithm.Code} is not supported.");
        }

        byte[] dataBytes = Encoding.UTF8.GetBytes(payload);

        byte[] signature = _privateKey.SignData(dataBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signature);
    }

    public bool VerifyRequest(RSA publicKey, string payload, string signatureBase64)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(payload);

        var fromBase64 = Convert.FromBase64String(signatureBase64);

        bool isValid = publicKey.VerifyData(dataBytes, fromBase64, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);


        return isValid;
    }

    public static string CanonicalizeJson(Object data)
    {
        return JsonSerializer.Serialize(data, _jsonSerializerOptions);
    }

    public static HttpRequestMessage ConstructRequest(string url, string payload, string requestSignature, Domain.Common.HashAlgorithmType hashAlgorithm, SignatureAlgorithmType supportedSignatureAlg)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        request.Headers.Add("X-Request-Hash-Alg", hashAlgorithm.Code);
        request.Headers.Add("X-Request-Signing-Algorithm", supportedSignatureAlg.Code);
        request.Headers.Add("X-Request-Signature", requestSignature);

        return request;
    }
}