using System.Security.Cryptography;

namespace Crypto
{
    // Static class implementing Elliptic Curve Digital Signature operations.
    public static class Signatures
    {
        // Method to generate a new public/private key pair.
        public static (byte[], byte[]) Gen()
        {
            using var ecdsa = ECDsa.Create();  // Create a new instance of ECDSA (Elliptic Curve Digital Signature Algorithm).
            // Export the private key and public key as byte arrays.
            return (ecdsa.ExportECPrivateKey(), ecdsa.ExportSubjectPublicKeyInfo());
        }

        // Method to sign data using the provided private key.
        public static byte[] Sign(byte[] privateKey, byte[] data)
        {
            using var ecdsa = ECDsa.Create();  // Create a new instance of ECDSA.
            // Import the provided private key into the ECDSA instance.
            ecdsa.ImportECPrivateKey(privateKey, out _);
            // Sign the provided data using SHA256 as the hash algorithm.
            return ecdsa.SignData(data, HashAlgorithmName.SHA256);
        }

        // Method to verify if a signature is valid given the data and the public key.
        public static bool Verify(byte[] publicKeyBytes, byte[] data, byte[] signature)
        {
            using var publicKey = ECDsa.Create();  // Create a new instance of ECDSA.
            // Import the provided public key into the ECDSA instance.
            publicKey.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
            // Verify the provided signature using SHA256 as the hash algorithm.
            return publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256);
        }
    }
}