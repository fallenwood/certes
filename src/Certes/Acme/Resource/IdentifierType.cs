namespace Certes.Acme.Resource;

using System.Runtime.Serialization;

/// <summary>
/// Represents type of <see cref="Identifier"/>.
/// </summary>
//[JsonConverter(typeof(StringEnumConverter))]
public enum IdentifierType
{
    /// <summary>
    /// The DNS type.
    /// </summary>
    [EnumMember(Value = "dns")]
    Dns,
}
