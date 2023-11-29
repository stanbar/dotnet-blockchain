using Blockchain;

namespace Ethereum;

public record Transaction(string From, string To, decimal Value, byte[] Data) : ITransaction
{
    public long Size()
    {
        return From.Length + To.Length + Value.ToString().Length;
    }
    public static Transaction Coinbase(string recipent) => new("Coinbase", recipent, 10.0m, new byte[] { });

}