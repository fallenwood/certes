﻿using System;
using System.IO;
using Certes.Jws;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

namespace Certes.Crypto;

/// <summary>
/// 
/// </summary>
public class AsymmetricCipherKey : IKey
{
    /// <summary>
    /// 
    /// </summary>
    public JsonWebKey JsonWebKey
    {
        get
        {
            if (Algorithm == KeyAlgorithm.RS256)
            {
                var rsaKey = (RsaKeyParameters)KeyPair.Public;
                return new RsaJsonWebKey
                {
                    Exponent = JwsConvert.ToBase64String(rsaKey.Exponent.ToByteArrayUnsigned()),
                    KeyType = "RSA",
                    Modulus = JwsConvert.ToBase64String(rsaKey.Modulus.ToByteArrayUnsigned())
                };
            }
            else
            {
                var ecKey = (ECPublicKeyParameters)KeyPair.Public;
                var curve =
                    Algorithm == KeyAlgorithm.ES256 ? "P-256" :
                    Algorithm == KeyAlgorithm.ES384 ? "P-384" : "P-521";

                // https://tools.ietf.org/html/rfc7518#section-6.2.1.2
                // get the byte representation of the x & y coords on the Elliptic Curve,
                // with padding bytes to the required field length

                var xBytes = ecKey.Q.AffineXCoord.GetEncoded();
                var yBytes = ecKey.Q.AffineYCoord.GetEncoded();

                return new EcJsonWebKey
                {
                    KeyType = "EC",
                    Curve = curve,
                    X = JwsConvert.ToBase64String(xBytes),
                    Y = JwsConvert.ToBase64String(yBytes)
                };
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public AsymmetricCipherKeyPair KeyPair { get; }

    /// <summary>
    /// 
    /// </summary>
    public KeyAlgorithm Algorithm { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="algorithm"></param>
    /// <param name="keyPair"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AsymmetricCipherKey(KeyAlgorithm algorithm, AsymmetricCipherKeyPair keyPair)
    {
        KeyPair = keyPair ?? throw new ArgumentNullException(nameof(keyPair));
        Algorithm = algorithm;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public byte[] ToDer()
    {
        var privateKey = PrivateKeyInfoFactory.CreatePrivateKeyInfo(KeyPair.Private);
        return privateKey.GetDerEncoded();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string ToPem()
    {
        using (var sr = new StringWriter())
        {
            var pemWriter = new PemWriter(sr);
            pemWriter.WriteObject(KeyPair);
            return sr.ToString();
        }
    }
}
