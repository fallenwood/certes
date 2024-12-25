namespace Certes.Acme.Models;

using System.Text.Json.Serialization;
using Certes.Acme.Resource;
using Certes.Jws;

/// <summary>
/// 
/// </summary>
[JsonSerializable(typeof(AcmeHeader))]
[JsonSerializable(typeof(JsonWebKey))]
[JsonSerializable(typeof(Account))]
[JsonSerializable(typeof(JwsPayload))]
[JsonSerializable(typeof(Directory))]
[JsonSerializable(typeof(ProtectedHeader))]
[JsonSerializable(typeof(KeyChange))]
[JsonSerializable(typeof(CertificateRevocation))]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(AcmeError))]
[JsonSerializable(typeof(string))]
public partial class AcmeJsonContext : JsonSerializerContext
{
}

/// <summary>
/// 
/// </summary>
public class KeyChange
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("account")]
    public System.Uri Account { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("account")]
    public JsonWebKey OldKey { get; set; }
}
