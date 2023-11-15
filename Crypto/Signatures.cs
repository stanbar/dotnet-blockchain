using System.Security.Cryptography;

namespace Crypto;
class Signatures {
    public static (byte[], byte[]) Gen()
    {
        using ECDsa ecdsa = ECDsa.Create();
        return (ecdsa.ExportECPrivateKey(), ecdsa.ExportSubjectPublicKeyInfo());

    }
    public static byte[] Sign(byte[] privateKey, byte[] data)
    {
        using ECDsa ecdsa = ECDsa.Create();
        ecdsa.ImportECPrivateKey(privateKey, out _);
        // Sign the data using the provided private key
        return ecdsa.SignData(data, HashAlgorithmName.SHA256);
    }

    public static bool Verify(byte[] publicKeyBytes, byte[] data, byte[] signature)
    {
        // Create an ECDsa instance from the provided public key bytes
        using ECDsa publicKey = ECDsa.Create();
        publicKey.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        // Verify the signature using the public key
        return publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256);
    }
}