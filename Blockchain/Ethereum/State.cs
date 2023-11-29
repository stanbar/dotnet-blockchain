using Blockchain;

namespace Ethereum;

public record AccountState(decimal Balance, int Nonce, byte[] Code, Dictionary<int, int> Storage);

public class State : IState<Transaction>
{
    private readonly Dictionary<string, AccountState> state = new();

    public void StateTransitionFunction(Block<Transaction> block)
    {
        foreach (var tx in block.Transactions)
        {
            try
            {
                ProcessTransaction(tx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction processing failed: {ex.Message}");
            }
        }
    }

    private void ProcessTransaction(Transaction tx)
    {
        if (tx.Data.Length == 0) 
        { // Handle Ether transfer
            TransferEther(tx);
        }
        else if (string.IsNullOrEmpty(tx.To)) 
        { // Contract creation
            DeployContract(tx);
        }
        else 
        { // Contract call
            ExecuteContract(tx);
        }
    }

    private void TransferEther(Transaction tx)
    {
        var from = state.GetValueOrDefault(tx.From, new AccountState(0, 0, Array.Empty<byte>(), new Dictionary<int, int>()));
        var to = state.GetValueOrDefault(tx.To, new AccountState(0, 0, Array.Empty<byte>(), new Dictionary<int, int>()));

        state[tx.From] = new AccountState(
            from.Balance - tx.Value,
            from.Nonce + 1,
            from.Code,
            from.Storage);

        state[tx.To] = new AccountState(
            to.Balance + tx.Value,
            to.Nonce,
            to.Code,
            to.Storage);
    }

    private void DeployContract(Transaction tx)
    {
        var newContractAddress = EVM.GenerateContractAddress(tx.From, state[tx.From].Nonce);

        state[tx.From] = new AccountState(
            state[tx.From].Balance - tx.Value,
            state[tx.From].Nonce + 1,
            state[tx.From].Code,
            state[tx.From].Storage);

        state[newContractAddress] = new AccountState(
            tx.Value,
            0,
            tx.Data,
            new Dictionary<int, int>()
        );
    }

    private void ExecuteContract(Transaction tx)
    {
        var contractAddress = tx.To;
        var args = tx.Data;

        if (!state.ContainsKey(contractAddress))
        {
            Console.WriteLine("Calling contract does not exist.");
            return;
        }
        EVM.ExecuteCode(state[contractAddress], args);

        state[tx.From] = new AccountState(
            state[tx.From].Balance - tx.Value,
            state[tx.From].Nonce + 1,
            state[tx.From].Code,
            state[tx.From].Storage);
    }
    public bool Validate(Transaction tx)
    {
        return true;
    }

    public int GetAccountNonce(string creatorAddress)
    {
        var accountState = state.GetValueOrDefault(creatorAddress);
        return accountState == null ? 0 : accountState.Nonce;
    }
    public void PrintState()
    {
        Console.WriteLine("[Ethereum] State {");
        state.ToList().ForEach(x =>
        {
            var account = x.Key;
            var accountState = x.Value;
            var code = x.Value.Code.Length > 0 ? Convert.ToHexString(Crypto.Hash.Sha256Bytes(x.Value.Code)) : "EOA";
            Console.WriteLine($"    {x.Key}:\n\t\tBalance: {x.Value.Balance},\n\t\tNonce: {x.Value.Nonce},\n\t\tCode: {code},\n\t\tStorage:");
            accountState.Storage.ToList().ForEach(x =>
            {
                var key = x.Key;
                var value = x.Value;
                Console.WriteLine($"\t\t\t{key} => {value}");
            });
        });
        Console.WriteLine("}");
    }
}