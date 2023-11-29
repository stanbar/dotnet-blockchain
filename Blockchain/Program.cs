// Simple();
// BitcoinDemo();
using System.Diagnostics;

await EthereumDemo();

async Task EthereumDemo()
{
    var archive = new Blockchain.Archive<Ethereum.Transaction>("ethereumArchive");
    var blockchain = new Blockchain.BC<Ethereum.Transaction>(archive);
    var state = new Ethereum.State();
    var miner = new Miner();
    var blocksPerSeconds = 1;
    var node = new Blockchain.Node<Ethereum.Transaction, Ethereum.State>(blockchain, state, miner, blocksPerSeconds: blocksPerSeconds);
    var coinbaseFactory = () => Ethereum.Transaction.Coinbase("Alice");
    _ = node.Start(coinbaseFactory);
    await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 2));


    /*
    contract FindMax {
    uint public value;
    
    function max(uint a) public returns(uint) {
        if (a >= value)  {
            value = a;
        }
        return value;
    }
    */

    var findMaxContractBytecode = new byte[] { 
        // max function
        (byte)Ethereum.OpCode.MLOAD, 0x00, // Load onto stack argument 'a' from memory[0]
        (byte)Ethereum.OpCode.SLOAD, 0x00, // Load 'value' from storage[0]
        (byte)Ethereum.OpCode.SETGREATER, // Check if 'value' > 'a'
        (byte)Ethereum.OpCode.JUMPIF, // Jump (skip) if true
        0x0B, // Address to jump. 11 to hex is 0x0B
        // if
        (byte)Ethereum.OpCode.MLOAD, 0x00, // Load 'a'
        (byte)Ethereum.OpCode.SSTORE, 0x00, // Store 'a' in storage[0]
        // end if
        (byte)Ethereum.OpCode.SLOAD, 0x00, // Load 'value' from storage[0]
        (byte)Ethereum.OpCode.RETURN, // Return 'value'
        (byte)Ethereum.OpCode.STOP // Stop execution
    };

    {
        // Transaction 1: Deploy contract
        var tx = new Ethereum.Transaction("Alice", "", 0, findMaxContractBytecode);
        node.SubmitTransaction(tx);
        await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 2));
    }

    var accountNonce = state.GetAccountNonce("Alice");
    Debug.Assert(accountNonce > 0); // Contract creation should increment nonce
    var accountNonceForContractCreation = accountNonce - 1;
    var contractAddress = Ethereum.EVM.GenerateContractAddress("Alice", accountNonceForContractCreation);

    {
        var tx = new Ethereum.Transaction("Alice", contractAddress, 0, new byte[] { 1 });
        node.SubmitTransaction(tx);
        await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 2));
    }
    {
        var tx = new Ethereum.Transaction("Alice", contractAddress, 0, new byte[] { 3 });
        node.SubmitTransaction(tx);
        await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 2));
    }
    {
        var tx = new Ethereum.Transaction("Alice", contractAddress, 0, new byte[] { 2 });
        node.SubmitTransaction(tx);
        await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 2));
    }
    {
        var tx = new Ethereum.Transaction("Alice", contractAddress, 0, new byte[] { 5 });
        node.SubmitTransaction(tx);
        await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 2));
    }
    {
        var tx = new Ethereum.Transaction("Alice", contractAddress, 0, new byte[] { 10 });
        node.SubmitTransaction(tx);
        await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 2));
    }

{

        var erc20TokensContractBytecode = new byte[] { 
        // max function
        (byte)Ethereum.OpCode.MLOAD, 0x00, // Load onto stack argument 'a' from memory[0]
        (byte)Ethereum.OpCode.SLOAD, 0x00, // Load 'value' from storage[0]
        (byte)Ethereum.OpCode.SETGREATER, // Check if 'value' > 'a'
        (byte)Ethereum.OpCode.JUMPIF, // Jump (skip) if true
        0x0B, // Address to jump. 11 to hex is 0x0B
        // if
        (byte)Ethereum.OpCode.MLOAD, 0x00, // Load 'a'
        (byte)Ethereum.OpCode.SSTORE, 0x00, // Store 'a' in storage[0]
        // end if
        (byte)Ethereum.OpCode.SLOAD, 0x00, // Load 'value' from storage[0]
        (byte)Ethereum.OpCode.RETURN, // Return 'value'
        (byte)Ethereum.OpCode.STOP // Stop execution
    };
    }
}



async void BitcoinDemo()
{
    var archive = new Blockchain.Archive<Bitcoin.Transaction>("bitcoinArchive");
    var blockchain = new Blockchain.BC<Bitcoin.Transaction>(archive);
    var state = new Bitcoin.State();
    var miner = new Miner();
    var blocksPerSeconds = 1;
    var node = new Blockchain.Node<Bitcoin.Transaction, Bitcoin.State>(blockchain, state, miner, blocksPerSeconds: blocksPerSeconds);
    var coinbaseFactory = () => Bitcoin.Transaction.Coinbase("Alice");
    _ = node.Start(coinbaseFactory);
    await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 5));

    // Alice -> Bob
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 3));
        var aliceUTXOs = state.GetUtxos("Alice").ToList();
        Console.WriteLine($"Alice has {aliceUTXOs.Count} UTXOs of total vaue {aliceUTXOs.Sum(x => x.Amount)}");

        var bobUTXOs = state.GetUtxos("Bob").ToList();
        Console.WriteLine($"Bob has {bobUTXOs.Count} UTXOs of total vaue {bobUTXOs.Sum(x => x.Amount)}");

        var firstUTXO = aliceUTXOs.First();

        Bitcoin.Input spendingInput = new(
            firstUTXO.TransactionId,
            firstUTXO.OutputIndex,
            new string[] { "AliceSig" }
        );

        Bitcoin.Output toBobOutput = new(10, new string[] { "BobSk" });

        var tx = new Bitcoin.Transaction(
            new List<Bitcoin.Input> { spendingInput },
            new List<Bitcoin.Output> { toBobOutput },
            new byte[] { },
            new byte[] { }
        );

        node.SubmitTransaction(tx);
    }

    // Multi sig transactions Bob -> Alice and Carol
    for (int i = 0; i < 5; i++)
    {
        await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 3));
        var carolUTXOs = state.GetUtxos("Carol").ToList();
        Console.WriteLine($"Carol has {carolUTXOs.Count} UTXOs of total vaue {carolUTXOs.Sum(x => x.Amount)}");
        var bobUTXOs = state.GetUtxos("Bob").ToList();
        Console.WriteLine($"Bob has {bobUTXOs.Count} UTXOs of total vaue {bobUTXOs.Sum(x => x.Amount)}");
        var firstUTXO = bobUTXOs.First();

        Bitcoin.Input spendingInput = new(
            firstUTXO.TransactionId,
            firstUTXO.OutputIndex,
            new string[] { "BobSig" }
        ); // here should be alice identity proof

        // The output will be the amount to send to Bob, with Bob's script public key.
        Bitcoin.Output toBobOutput = new(10, new string[] { "CarolSk", "AliceSk" });

        var tx = new Bitcoin.Transaction(
            new List<Bitcoin.Input> { spendingInput },
            new List<Bitcoin.Output> { toBobOutput },
            new byte[] { },
            new byte[] { }
        );

        node.SubmitTransaction(tx);
    }
}

async void SimpleBC()
{
    var archive = new Blockchain.Archive<Simple.Transaction>("simpleArchive");
    var blockchain = new Blockchain.BC<Simple.Transaction>(archive);
    var state = new Simple.State();
    var blocksPerSeconds = 3;
    var miner = new Miner();
    var node = new Blockchain.Node<Simple.Transaction, Simple.State>(blockchain, state, miner, blocksPerSeconds: blocksPerSeconds);
    var coinbaseFactory = () => Simple.Transaction.Coinbase("Alice");
    _ = node.Start(coinbaseFactory);

    // Wait for first coinbase
    await Task.Delay(TimeSpan.FromSeconds(blocksPerSeconds * 2));
    // write generator for transactions
    for (int i = 0; i < 10; i++)
    {
        var tx = new Simple.Transaction("Alice", "Bob", 1.0m);
        await Task.Delay(TimeSpan.FromSeconds(1));
        node.SubmitTransaction(tx);
    }
}
