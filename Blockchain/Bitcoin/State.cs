using System.Collections.Concurrent;
using Blockchain;

namespace Bitcoin;

public record UTXO(string TransactionId, int OutputIndex, decimal Amount, string ScriptSig)
{

    public override string ToString() => $"UTXO: {TransactionId}-{OutputIndex} {Amount} {ScriptSig}";

}
public class State : IState<Transaction>
{
    private readonly ConcurrentDictionary<string, UTXO> utxos = new();

    public void StateTransitionFunction(Block<Transaction> block)
    {
        foreach (var tx in block.Transactions)
        {
            // Process the inputs and remove spent UTXOs
            foreach (var input in tx.Inputs)
            {
                string utxoKey = $"{input.TxId}-{input.Index}";
                utxos.TryRemove(utxoKey, out _);
            }

            // Add new UTXOs for the outputs
            for (int i = 0; i < tx.Outputs.Count; i++)
            {
                var output = tx.Outputs[i];
                var utxo = new UTXO(tx.TxID(), i, output.Value, string.Join(",", output.ScriptPk)); // Simplified, assumes the first scriptPubKey item is the address
                AddUtxo(utxo);
            }
        }
    }

    public void AddUtxo(UTXO utxo)
    {
        // Add a UTXO to the state
        string utxoKey = $"{utxo.TransactionId}-{utxo.OutputIndex}";
        utxos[utxoKey] = utxo;
    }

    public void PrintState()
    {
        Console.WriteLine("[Bitcoin] State {");
        utxos.ToList().ForEach(x => Console.WriteLine($"    {x.Value}"));
        Console.WriteLine("}");
    }

    public bool Validate(Transaction tx)
    {
        if (tx.Inputs[0].ScriptSig[0] == "Coinbase")
        {
            return true;
        }
        if (tx.Inputs.Select(input => utxos[$"{input.TxId}-{input.Index}"].Amount).Sum() < tx.Outputs.Select(output => output.Value).Sum())
        {
            Console.WriteLine($"Spender has insufficient funds.");
            return false;
        }
        if (tx.Inputs.Any(input => !utxos.ContainsKey($"{input.TxId}-{input.Index}")))
        {
            Console.WriteLine($"Spender has invalid inputs. There is no UTXO for one of the inputs.");
            return false;
        }

        return true;
    }

    public List<UTXO> GetUtxos(string address)
    {
        return utxos.Values.Where(x => x.ScriptSig.Contains(address)).ToList();
    }

    public List<string> GetTxIDs()
    {
        return utxos.Select(x => x.Key).ToList();
    }
}