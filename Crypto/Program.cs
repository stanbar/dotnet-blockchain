// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Crypto;

{ // should pass
    var sha256 = SHA256.Create();
    var hash1 = sha256.ComputeHash(new byte[] { 0b0});
    Console.WriteLine(BitConverter.ToString(hash1).Replace("-", "").ToLowerInvariant());
    var hash2 = sha256.ComputeHash(new byte[] { 0b1});
    Console.WriteLine(BitConverter.ToString(hash2).Replace("-", "").ToLowerInvariant());
    Debug.Assert(hash1 != hash2);
}
{ // should pass
    var S = new string[] { "Alice", "Bob", "Carol", "David" };
    var comm = NaiveCommitment.Commit(S);
    Console.WriteLine(comm.Length);
    var accept = NaiveCommitment.Verify(comm, 1, "Bob", S);
    Debug.Assert(accept == true);
}
{ // should fail
    var S = new string[] { "Alice", "Bob", "Carol", "David" };
    var comm = NaiveCommitment.Commit(S);
    var accept = NaiveCommitment.Verify(comm, 2, "Bob", S);
    Debug.Assert(accept == false);
}
{
    List<string> data = new() { "Alice", "Bob", "Carol", "David" };
    MerkleTree tree = new(data);

    {
        var targetData = "Bob";
        var proof = tree.GenerateProof(targetData);
        var isValidProof = MerkleTree.VerifyProof(targetData, proof, tree.Root.Hash);
        Debug.Assert(isValidProof == true);
    }
    {
        // Generate a proof for the element "Bob".
        var targetData = "Bob";
        var proof = tree.GenerateProof(targetData);
        var verifyData = "Alice";
        var isValidProof = MerkleTree.VerifyProof(verifyData, proof, tree.Root.Hash);
        Debug.Assert(isValidProof == false);

    }
}
{ // Proof of work
    var proofOfWork = ProofOfWork.Mine("Hello World", 4);
    Console.WriteLine(proofOfWork);
}
{ // Signatures pass
    var (privateKey, publicKey) = Signatures.Gen();
    var data = Encoding.UTF8.GetBytes("Hello World");
    var signature = Signatures.Sign(privateKey, data);
    var verified = Signatures.Verify(publicKey, data, signature);
    Debug.Assert(verified == true);
}

{ // Signatures fail
    var (privateKey, publicKey) = Signatures.Gen();
    var data = Encoding.UTF8.GetBytes("Hello World");
    var signature = Signatures.Sign(privateKey, data);
    data = Encoding.UTF8.GetBytes("Bye World");
    var verified = Signatures.Verify(publicKey, data, signature);
    Debug.Assert(verified == false);
}