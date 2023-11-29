namespace Blockchain;

public class Node<Tx, S>
where Tx : ITransaction
where S : IState<Tx>
{
    private readonly Queue<Tx> Mempool = new();
    private readonly BC<Tx> Blockchain;
    private readonly S State;
    private readonly Miner Miner;
    private readonly int MaxBlockSize;
    private readonly int Difficulty;
    private readonly int BlockEverySecond;
    public Node(BC<Tx> blockchain, S state, Miner miner, int maxBlockSize = 1_000_000, int difficulty = 4, int blocksPerSeconds = 5)
    {
        Blockchain = blockchain;
        State = state;
        Miner = miner;
        MaxBlockSize = maxBlockSize;
        Difficulty = difficulty;
        BlockEverySecond = blocksPerSeconds;
    }
    public async Task Start(Func<Tx> newCoinbase)
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(BlockEverySecond));
            var proposedBlock = WorkOnNextBlock(newCoinbase());
            Blockchain.AddBlock(proposedBlock);
            State.StateTransitionFunction(proposedBlock);
            State.PrintState();
        }
    }
    public Block<Tx> WorkOnNextBlock(Tx coinbase)
    {
        var size = 0L;
        var candidateTransactions = new List<Tx>
        {
            coinbase
        };

        while (size < MaxBlockSize && Mempool.Count > 0)
        {
            if (size + Mempool.Peek().Size() > MaxBlockSize)
            {
                break;
            }

            var tx = Mempool.Dequeue();
            candidateTransactions.Add(tx);
            size += tx.Size();
        }
        var lastBlock = Blockchain.GetLatestBlock();
        var txRoot = new Crypto.MerkleTree(
            candidateTransactions.Select(tx => tx.ToString()).ToList()
        ).Root.Hash;

        var candidateBlockHeader = new CandidateBlockHeader(lastBlock.ToHash(), DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Difficulty, txRoot);
        var nonce = Miner.Mine(candidateBlockHeader);

        var blockHeader = new BlockHeader(candidateBlockHeader, nonce);
        var proposedBlock = new Block<Tx>(blockHeader, candidateTransactions);

        return proposedBlock;
    }

    public void SubmitTransaction(Tx transaction)
    {
        if (!State.Validate(transaction))
        {
            throw new Exception("Invalid transaction");
        }
        Mempool.Enqueue(transaction);
    }
}