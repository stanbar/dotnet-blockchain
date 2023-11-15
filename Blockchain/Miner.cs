using Blockchain;

public class Miner
{

    public long Mine(CandidateBlockHeader header)
    {
        return Crypto.ProofOfWork.Mine(header.ToString(), header.Bits);
    }
}
