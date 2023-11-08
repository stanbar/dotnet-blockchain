using System.Security.Cryptography;

namespace Crypto
{
    public static class Signatures
    {
        public static (byte[], byte[]) Gen()
        {
            using var ecdsa = ECDsa.Create();
            return (ecdsa.ExportECPrivateKey(), ecdsa.ExportSubjectPublicKeyInfo());
        }

        public static byte[] Sign(byte[] privateKey, byte[] data)
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportECPrivateKey(privateKey, out _);
            return ecdsa.SignData(data, HashAlgorithmName.SHA256);
        }

        public static bool Verify(byte[] publicKeyBytes, byte[] data, byte[] signature)
        {
            using var publicKey = ECDsa.Create();
            publicKey.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
            return publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256);
        }
    }
}