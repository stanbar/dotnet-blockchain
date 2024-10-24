namespace Crypto
{
    // Represents a node in a Merkle Tree.
    public class MerkleNode
    {
        public string Hash { get; } // The hash value for this node.
        public MerkleNode? Left { get; private set; } // Reference to the left child node (nullable).
        public MerkleNode? Right { get; private set; } // Reference to the right child node (nullable).

        // Constructor for creating a leaf node from data.
        public MerkleNode(string data)
        {
            Hash = Crypto.Hash.Sha256String(data);   // Hash the provided data to create the node's hash.
        }

        public MerkleNode(MerkleNode left, MerkleNode right)
        {
            Left = left;
            Right = right;
            // Ensure that the smaller hash always comes first for consistency.
            var orderedHashes = new List<string> { left.Hash, right.Hash }.OrderBy(h => h).ToList();
            // Concatenate the hashes and compute the new hash for the parent node.
            Hash = Crypto.Hash.Sha256String(orderedHashes[0] + orderedHashes[1]);
        }
    }

    // Represents a Merkle Tree that can be constructed from data.
    public class MerkleTree
    {
        public MerkleNode Root { get; } // The root node of the Merkle Tree.

        // Constructor that takes a list of strings and builds the Merkle Tree.
        public MerkleTree(List<string> data)
        {
            // Convert each data element into a leaf node.
            var nodes = data.Select(d => new MerkleNode(d)).ToList();

            // Build the tree level by level until only one node (the root) is left.
            while (nodes.Count > 1)
            {
                var newLevel = new List<MerkleNode>();

                // Iterate through the current level nodes in pairs.
                for (int i = 0; i < nodes.Count; i += 2)
                {
                    if (i + 1 < nodes.Count)
                    {
                        // Pair nodes together to create new parent nodes.
                        newLevel.Add(new MerkleNode(nodes[i], nodes[i + 1]));
                    }
                    else
                    {
                        // If there's an odd number of nodes, duplicate the last one.
                        newLevel.Add(new MerkleNode(nodes[i], nodes[i]));
                    }
                }

                nodes = newLevel;   // Move up to the new level.
            }
            // The last remaining node is the root of the tree.
            Root = nodes.First();
        }

        // Generate a proof for a given piece of data (Merkle proof).
        public List<string> GenerateProof(string data)
        {
            var proof = new List<string>();
            // Start generating the proof from the root node.
            GenerateProof(Root, Hash.Sha256String(data), proof);
            return proof; // Return the generated proof.
        }

        // Recursively generate a proof for a specific hash, adding sibling hashes to the proof.
        private bool GenerateProof(MerkleNode node, string targetHash, List<string> proof)
        {
            if (node == null)
                return false;

            // If this is a leaf node, check if it matches the target hash.
            if (node.Left == null && node.Right == null)
            {
                return node.Hash == targetHash;
            }

            // Recurse to the left node and add the right node's hash to the proof if a match is found.
            if (GenerateProof(node.Left!, targetHash, proof))
            {
                proof.Add(node.Right!.Hash);
                return true;
            }
            // Recurse to the right node and add the left node's hash to the proof if a match is found.
            else if (GenerateProof(node.Right!, targetHash, proof))
            {
                proof.Add(node.Left!.Hash);
                return true;
            }

            return false;
        }

        // Verifies that the given data and proof lead to the correct root hash.
        public static bool VerifyProof(string data, List<string> proof, string rootHash)
        {
            // Start with the hash of the data.
            var currentHash = Hash.Sha256String(data);

            // Iteratively apply each hash from the proof to derive the root hash.
            foreach (var hash in proof)
            {
                // Always concatenate the smaller hash first to maintain consistency.
                var orderedHashes = new List<string> { currentHash, hash }.OrderBy(h => h).ToList();
                // Compute the next hash in the verification process.
                currentHash = Hash.Sha256String(orderedHashes[0] + orderedHashes[1]);
            }

            // Check if the final computed hash matches the root hash.
            return currentHash == rootHash;
        }
    }
}