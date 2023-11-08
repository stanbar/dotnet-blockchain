namespace Crypto;

public class MerkleNode
{
    public string Hash { get; private set; }
    public MerkleNode? Left { get; private set; }
    public MerkleNode? Right { get; private set; }

    public MerkleNode(string data)
    {
        Hash = Crypto.Hash.Sha256String(data);
    }

    public MerkleNode(MerkleNode left, MerkleNode right)
    {
        Left = left;
        Right = right;
        // Ensure the smaller hash is always first
        var orderedHashes = new List<string> { left.Hash, right.Hash }.OrderBy(h => h).ToList();
        Hash = Crypto.Hash.Sha256String(orderedHashes[0] + orderedHashes[1]);
    }
}

public class MerkleTree
{
    public MerkleNode Root { get; private set; }

    public MerkleTree(List<string> data)
    {
        List<MerkleNode> nodes = data.Select(d => new MerkleNode(d)).ToList();

        while (nodes.Count > 1)
        {
            List<MerkleNode> newLevel = new();

            for (int i = 0; i < nodes.Count; i += 2)
            {
                if (i + 1 < nodes.Count)
                {
                    newLevel.Add(new MerkleNode(nodes[i], nodes[i + 1]));
                }
                else
                {
                    newLevel.Add(new MerkleNode(nodes[i], nodes[i])); // Duplicate the last node if the count is odd
                }
            }

            nodes = newLevel;
        }

        Root = nodes.FirstOrDefault()!;
    }

    public List<string> GenerateProof(string data)
    {
        List<string> proof = new();
        GenerateProof(Root, Hash.Sha256String(data), proof);
        return proof;
    }

    private bool GenerateProof(MerkleNode node, string targetHash, List<string> proof)
    {
        if (node == null)
            return false;

        if (node.Left == null && node.Right == null)
        {
            return node.Hash == targetHash;
        }

        if (GenerateProof(node.Left!, targetHash, proof))
        {
            proof.Add(node.Right!.Hash);
            return true;
        }
        else if (GenerateProof(node.Right!, targetHash, proof))
        {
            proof.Add(node.Left!.Hash);
            return true;
        }

        return false;
    }

    public static bool VerifyProof(string data, List<string> proof, string rootHash)
    {
        string currentHash = Hash.Sha256String(data);

        foreach (var hash in proof)
        {
            // Order the current hash and proof hash lexicographically
            var orderedHashes = new List<string> { currentHash, hash }.OrderBy(h => h).ToList();
            currentHash = Hash.Sha256String(orderedHashes[0] + orderedHashes[1]);
        }

        return currentHash == rootHash;
    }


}