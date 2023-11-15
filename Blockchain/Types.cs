namespace Blockchain;
public interface ITransaction
{
    long Size();
    string ToString();
}

public interface IState<Tx> 
where Tx : ITransaction
{
    void StateTransitionFunction(Block<Tx> block);
    void PrintState();
    bool Validate(Tx tx);
}