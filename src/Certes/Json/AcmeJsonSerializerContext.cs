namespace Certes;

using System.Text.Json;
using System.Text.Json.Serialization;
using Certes.Acme;
using Certes.Acme.Resource;
using Certes.Crypto;
using Certes.Jws;

/// <summary>
/// 
/// </summary>
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(AcmeError))]
[JsonSerializable(typeof(JsonWebKey))]
[JsonSerializable(typeof(NewAccountHeader))]
[JsonSerializable(typeof(KeyChange))]
[JsonSerializable(typeof(JwsPayload))]
[JsonSerializable(typeof(Account))]
[JsonSerializable(typeof(Directory))]
[JsonSerializable(typeof(CertificateRevocation))]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(Authorization))]
[JsonSerializable(typeof(OrderList))]
[JsonSerializable(typeof(JwsSignHeader))]
[JsonSerializable(typeof(AsymmetricCipherKey))]
public partial class AcmeJsonSerializerContext : JsonSerializerContext
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly AcmeJsonSerializerContext Unindented = new (new System.Text.Json.JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // NullValueHandling = NullValueHandling.Ignore,
        // MissingMemberHandling = MissingMemberHandling.Ignore
    });

    /// <summary>
    /// 
    /// </summary>
    public static readonly AcmeJsonSerializerContext Indented = new(new System.Text.Json.JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // NullValueHandling = NullValueHandling.Ignore,
        // MissingMemberHandling = MissingMemberHandling.Ignore
    });
}
