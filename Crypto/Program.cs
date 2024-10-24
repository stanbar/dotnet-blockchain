using System.Diagnostics;
using System.Text;
using Crypto;

{ 
    // **Section 1: Hashing Demonstration and Tests**
    // Generate a SHA256 hash for the string "Haslo123".
    var hash1 = Hash.Sha256String("Haslo123");

    // Test 1: Check that hashing the same value ("1") twice produces the same hash.
    Debug.Assert(Hash.Sha256String("1") == Hash.Sha256String("1")); // Ensures that SHA256 is deterministic.

    // Generate another hash for the string "2".
    var hash2 = Hash.Sha256String("2");

    // Test 2: Check that hashing two different values produces different results.
    Debug.Assert(hash1 != hash2); // Verifies that different inputs lead to different outputs.

    // Output the two hashes for visual inspection.
    Console.WriteLine($"hash1: {hash1}");  // Expected to be the SHA256 of "Haslo123".
    Console.WriteLine($"hash2: {hash2}");  // Expected to be the SHA256 of "2".
}
{
    // **Section 2: Naive Commitment - Basic Test**
    // Create a list of strings to represent participants or values.
    var S = new List<string> { "Alice", "Bob", "Carol" };

    // Create a commitment for the list of strings.
    var commit = NaiveCommitment.Commit(S);

    // Test 3: Verify the commitment for "Alice" at index 0.
    Debug.Assert(NaiveCommitment.Verify(commit, 0, "Alice", S));

    // Test 4: Verify the commitment for "Bob" at index 1.
    Debug.Assert(NaiveCommitment.Verify(commit, 1, "Bob", S));

    // Proves that each value in the original list is properly committed and can be verified.
}
{
    // **Section 3: Naive Commitment - Full List Verification**
    // Another commitment is created for the same list.
    var S = new List<string> { "Alice", "Bob", "Carol" };
    var commit = NaiveCommitment.Commit(S);

    // Test 5: Verify each element individually by index.
    Debug.Assert(NaiveCommitment.Verify(commit, 0, "Alice", S));  // Verifies "Alice" is correctly committed at index 0.
    Debug.Assert(NaiveCommitment.Verify(commit, 1, "Bob", S));  // Verifies "Bob" at index 1.
    Debug.Assert(NaiveCommitment.Verify(commit, 2, "Carol", S));  // Verifies "Carol" at index 2.

    // Proves that the commitment correctly represents the entire list of elements.
}
{
    // **Section 4: Naive Commitment - Correctness and Failure Cases**
    // Commitment is created again to verify correctness with negative cases.
    var S = new List<string> { "Alice", "Bob", "Carol" };
    var commit = NaiveCommitment.Commit(S);

    // Test 6-8: Verify each element is correctly committed.
    Debug.Assert(NaiveCommitment.Verify(commit, 0, "Alice", S));  // Positive test: "Alice" at index 0.
    Debug.Assert(NaiveCommitment.Verify(commit, 1, "Bob", S));  // Positive test: "Bob" at index 1.
    Debug.Assert(NaiveCommitment.Verify(commit, 2, "Carol", S));  // Positive test: "Carol" at index 2.

    // Negative Tests: Ensure incorrect verifications fail.
    Debug.Assert(!NaiveCommitment.Verify(commit, 0, "Bob", S));  // Negative test: "Bob" is not at index 0.
    Debug.Assert(!NaiveCommitment.Verify(commit, 1, "Carol", S));  // Negative test: "Carol" is not at index 1.
    Debug.Assert(!NaiveCommitment.Verify(commit, 2, "Alice", S));  // Negative test: "Alice" is not at index 2.

    // Proves that verification works both for positive and negative cases, ensuring data integrity.
}
{
    // **Section 5: Merkle Tree Proof Generation and Verification**
    // Create a list of strings to be used in a Merkle Tree.
    List<string> S = new() { "Alice", "Bob", "Carol" };

    // Construct a Merkle Tree from the list.
    MerkleTree tree = new(S);

    {
        // Generate a proof for the element "Bob".
        var targetData = "Bob";
        var proof = tree.GenerateProof(targetData);

        // Test 9: Verify that the proof for "Bob" matches the root hash of the tree.
        Debug.Assert(MerkleTree.VerifyProof(targetData, proof, tree.Root.Hash));  // Expected to be true.

        // Test 10: Verify that the proof fails for an element not present ("Alice").
        Debug.Assert(MerkleTree.VerifyProof("Alice", proof, tree.Root.Hash) == false);  // Expected to be false.

        // Proves that the Merkle Tree can generate valid proofs for elements and reject incorrect ones.
    }
}
{
    // **Section 6: Proof of Work Mining Test**
    // Perform mining to find a valid hash with 5 leading zeros.
    var (proofOfWork, hash) = ProofOfWork.Mine("Hello World2", 5);

    // Output the proof of work (nonce) and corresponding hash.
    Console.WriteLine($"Proof of work: {proofOfWork} Hash: {hash}");

    // Proves that the mining function can find a valid nonce that produces a hash with the specified difficulty.
}
{
    // **Section 7: Digital Signatures - Signing and Verification Test**
    // Generate a key pair (private and public keys) for signing and verification.
    var (privateKey, publicKey) = Signatures.Gen();

    // Convert the data to bytes for signing.
    var data = Encoding.UTF8.GetBytes("Hello World");

    // Generate a digital signature for the data using the private key.
    var signature = Signatures.Sign(privateKey, data);

    // Test 11: Verify the signature using the public key.
    var verified = Signatures.Verify(publicKey, data, signature);
    Debug.Assert(verified == true);  // Expected to be true, since the correct private key was used to sign the data.

    // Proves that data signed with a private key can be verified with the corresponding public key, ensuring authenticity.
}