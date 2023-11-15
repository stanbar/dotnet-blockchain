using Blockchain;

namespace Simple;
public record Transaction(string From, string To, decimal Amount) : ITransaction
{
    public long Size()
    {
        return From.Length + To.Length + Amount.ToString().Length;
    }
    public static Transaction Coinbase(string recipent) => new("Coinbase", recipent, 10.0m);

}