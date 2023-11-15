using System.Security.Cryptography;
using System.Text;
using Blockchain;

namespace Bitcoin;

// TxID is the hash of the transaction excluding witnesses
public record Input(string TxId, long Index, string[] ScriptSig);
public record Output(long Value, string[] ScriptPk);
public record Transaction(List<Input> Inputs, List<Output> Outputs, byte[] Witness, byte[] Locktime) : ITransaction
{
    public string TxID()
    {
        // compute TxID as a hash of the transaction excluding the witness
        // Create a hash of the transaction excluding the witness
        using SHA256 sha256 = SHA256.Create();
        byte[] data = Inputs
            .SelectMany(input => Encoding.UTF8.GetBytes(input.TxId))
            .Concat(Inputs.SelectMany(input => BitConverter.GetBytes(input.Index)))
            .Concat(Inputs.SelectMany(input => input.ScriptSig.SelectMany(script => Encoding.UTF8.GetBytes(script))))
            .Concat(Outputs.SelectMany(output => BitConverter.GetBytes(output.Value)))
            .Concat(Outputs.SelectMany(output => output.ScriptPk.SelectMany(script => Encoding.UTF8.GetBytes(script))))
            .Concat(Locktime)
            .ToArray();

        byte[] hashBytes = sha256.ComputeHash(data);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
    public long Size() => 0;
    private static readonly Random rand = new();
    public static Transaction Coinbase(string recipent)
    {
        // he scriptSig contains arbitrary data. This often includes the block height to ensure the uniqueness of the coinbase transaction's TxID and can include extra nonce values and other miner-specific data.
        var buff = new byte[32];
        rand.NextBytes(buff);
        // string representation of buff
        var hex = BitConverter.ToString(buff).Replace("-", "").ToLowerInvariant();

        var scriptSig = new string[] { hex };

        return new Transaction(
            new List<Input> { new("Coinbase", 0xFFFFFFFF, scriptSig) },
            new List<Output> { new(10, new string[] { recipent+"Sig" }) },
            new byte[] { },
            new byte[] { }
        );
    }
}