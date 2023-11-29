
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blockchain;

public record CandidateBlockHeader(string PreviousHash, long Timestamp, int Bits, string TxRoot)
{
    public override string ToString() => $"{PreviousHash}{Timestamp}{Bits}{TxRoot}";
}
public record BlockHeader
{
    public string PreviousHash { get; init; }
    public long Timestamp { get; init; }
    public int Bits { get; init; }
    public long Nonce { get; init; }
    public string TxRoot { get; init; }

    [JsonConstructor]
    public BlockHeader(string previousHash, long timestamp, int bits, long nonce, string txRoot)
    {
        PreviousHash = previousHash;
        Timestamp = timestamp;
        Bits = bits;
        Nonce = nonce;
        TxRoot = txRoot;
    }

    public BlockHeader(CandidateBlockHeader candidateBlockHeader, long nonce)
        : this(candidateBlockHeader.PreviousHash, candidateBlockHeader.Timestamp, candidateBlockHeader.Bits, nonce, candidateBlockHeader.TxRoot) { }

    public override string ToString() => $"{PreviousHash}{Timestamp}{Bits}{TxRoot}{Nonce}";
}


public record Block<Tx>(BlockHeader Header, List<Tx> Transactions)
where Tx : ITransaction
{
    public override string ToString() => Header.ToString();
    public string ToHash() => Crypto.Hash.Sha256String(Crypto.Hash.Sha256String(ToString()));
}

public class BC<Tx>
where Tx : ITransaction
{
    private readonly List<Block<Tx>> Chain = new();
    private readonly Archive<Tx> archive;

    public BC(Archive<Tx> archive)
    {
        this.archive = archive;
        if (archive.IsEmpty())
        {
            InitializeGenesisBlock();
        }
        else
        {
            LoadChainFromDisk();
        }
    }

    private void LoadChainFromDisk()
    {
        foreach (var file in Directory.GetFiles(archive.archivePath).OrderBy(x => x))
        {
            string json = File.ReadAllText(file);
            var block = JsonSerializer.Deserialize<Block<Tx>>(json) ?? throw new Exception("Failed to deserialize block.");
            Chain.Add(block);
        }
    }

    private void InitializeGenesisBlock() => AddBlock(BC<Tx>.CreateGenesisBlock());
    private static Block<Tx> CreateGenesisBlock() => new(new BlockHeader(
        Crypto.Hash.Sha256String("The Times 03/Jan/2009 Chancellor on brink of second bailout for banks."),
         0,
         0,
         0,
          ""),
           new List<Tx>());
    public Block<Tx> GetLatestBlock() => Chain.Last();
    public long GetHeight() => Chain.Count;
    public void AddBlock(Block<Tx> newBlock)
    {
        Chain.Add(newBlock);
        archive.SaveBlockToFile(newBlock, GetHeight());
    }
}