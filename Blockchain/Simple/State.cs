
using Blockchain;

namespace Simple;

public class State : IState<Transaction>
{
    private readonly Dictionary<string, decimal> state =  new(); // address -> balance

    public void StateTransitionFunction(Block<Transaction> block)
    {
        foreach (var tx in block.Transactions)
        {
            state[tx.From] = state.GetValueOrDefault(tx.From) - tx.Amount;
            state[tx.To] = state.GetValueOrDefault(tx.To) + tx.Amount;
        }
    }
    public void PrintState()
    {
        Console.WriteLine("[Simple] State {");
        state.ToList().ForEach(x => Console.WriteLine($"  {x.Key}: {x.Value}"));
        Console.WriteLine("}");
    }

    public bool Validate(Transaction tx)
    {
        if (tx.From == "Coinbase")
        {
            return true;
        }
        if (state.GetValueOrDefault(tx.From, 0) < tx.Amount)
        {
            Console.WriteLine($"Spender has insufficient funds. Has {state.GetValueOrDefault(tx.From, 0)} but needs {tx.Amount}");
            return false;
        } 
        return true;
    }
}