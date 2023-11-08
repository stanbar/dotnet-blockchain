using System.Diagnostics;
using System.Text;
using Crypto;

{
    var hash1 = Hash.Sha256String("1");

    Debug.Assert(Hash.Sha256String("1") == Hash.Sha256String("1"));

    var hash2 = Hash.Sha256String("2");

    Debug.Assert(hash1 != hash2);
}
{
    var S = new List<string> { "Alice", "Bob", "Carol" };
    var commit = NaiveCommitment.Commit(S);
    Debug.Assert(NaiveCommitment.Verify(commit, 0, "Alice", S));
    Debug.Assert(NaiveCommitment.Verify(commit, 1, "Bob", S));
}
{

    var S = new List<string> { "Alice", "Bob", "Carol" };
    var commit = NaiveCommitment.Commit(S);
    Debug.Assert(NaiveCommitment.Verify(commit, 0, "Alice", S));
    Debug.Assert(NaiveCommitment.Verify(commit, 1, "Bob", S));
    Debug.Assert(NaiveCommitment.Verify(commit, 2, "Carol", S));
}
{
    var S = new List<string> { "Alice", "Bob", "Carol" };
    var commit = NaiveCommitment.Commit(S);
    Debug.Assert(NaiveCommitment.Verify(commit, 0, "Alice", S));
    Debug.Assert(NaiveCommitment.Verify(commit, 1, "Bob", S));
    Debug.Assert(NaiveCommitment.Verify(commit, 2, "Carol", S));
    Debug.Assert(!NaiveCommitment.Verify(commit, 0, "Bob", S));
    Debug.Assert(!NaiveCommitment.Verify(commit, 1, "Carol", S));
    Debug.Assert(!NaiveCommitment.Verify(commit, 2, "Alice", S));
}
{
    List<string> S = new List<string> { "Alice", "Bob", "Carol" };
    MerkleTree tree = new MerkleTree(S);
    {
        var targetData = "Bob";
        var proof = tree.GenerateProof(targetData);
        Debug.Assert(MerkleTree.VerifyProof(targetData, proof, tree.Root.Hash));
        Debug.Assert(MerkleTree.VerifyProof("Alice", proof, tree.Root.Hash) == false);
    }
}
{
    var (proofOfWork,hash) = ProofOfWork.Mine("Hello World", 4);
    Console.WriteLine($"Proof of work: {proofOfWork} Hash: {hash}");
}
{
    var (privateKey, publicKey) = Signatures.Gen();
    var data = Encoding.UTF8.GetBytes("Hello World");
    var signature = Signatures.Sign(privateKey, data);
    var verified = Signatures.Verify(publicKey, data, signature);
    Debug.Assert(verified == true);
}