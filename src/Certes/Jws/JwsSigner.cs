namespace Certes.Jws;

using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// 
/// </summary>
public record JwsSignHeader
{
    /// <summary>
    /// 
    /// </summary>
    public string alg { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public JsonWebKey jwk { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Uri kid { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string nonce { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Uri url { get; set; }
}

/// <summary>
/// Represents an signer for JSON Web Signature.
/// </summary>
internal class JwsSigner
{
    private readonly IKey keyPair;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwsSigner"/> class.
    /// </summary>
    /// <param name="keyPair">The keyPair.</param>
    public JwsSigner(IKey keyPair)
    {
        this.keyPair = keyPair;
    }

    /// <summary>
    /// Signs the specified payload.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="nonce">The nonce.</param>
    /// <param name="jsonTypeInfo"></param>
    /// <returns>The signed payload.</returns>
    public JwsPayload Sign(object payload, string nonce, JsonTypeInfo jsonTypeInfo)
        => Sign(payload, jsonTypeInfo, null, null, nonce);

    /// <summary>
    /// Encodes this instance.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="url">The URL.</param>
    /// <param name="nonce">The nonce.</param>
    /// <returns>The signed payload.</returns>
    public JwsPayload Sign(
        object payload,
        JsonTypeInfo jsonTypeInfo,
        Uri keyId = null,
        Uri url = null,
        string nonce = null)
    {
        var protectedHeader = (keyId) == null ?
            new JwsSignHeader
            {
                alg = keyPair.Algorithm.ToJwsAlgorithm(),
                jwk = keyPair.JsonWebKey,
                nonce = nonce,
                url = url,
            } :
            new JwsSignHeader
            {
                alg = keyPair.Algorithm.ToJwsAlgorithm(),
                kid = keyId,
                nonce = nonce,
                url = url,
            };

        var entityJson = payload switch
        {
            null => string.Empty,
            // settings
            _ => JsonSerializer.Serialize(payload, jsonTypeInfo),
        };

        var protectedHeaderJson = JsonSerializer.Serialize(protectedHeader, AcmeJsonSerializerContext.Unindented.JwsSignHeader);

        var payloadEncoded = JwsConvert.ToBase64String(Encoding.UTF8.GetBytes(entityJson));
        var protectedHeaderEncoded = JwsConvert.ToBase64String(Encoding.UTF8.GetBytes(protectedHeaderJson));

        var signature = $"{protectedHeaderEncoded}.{payloadEncoded}";
        var signatureBytes = Encoding.UTF8.GetBytes(signature);
        var signedSignatureBytes = keyPair.GetSigner().SignData(signatureBytes);
        var signedSignatureEncoded = JwsConvert.ToBase64String(signedSignatureBytes);

        var body = new JwsPayload
        {
            Protected = protectedHeaderEncoded,
            Payload = payloadEncoded,
            Signature = signedSignatureEncoded
        };

        return body;
    }
}
