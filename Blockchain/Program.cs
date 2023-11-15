
{ // Simple
    var archive = new Blockchain.Archive<Simple.Transaction>("simpleArchive");
    var blockchain = new Blockchain.BC<Simple.Transaction>(archive);
    var state = new Simple.State();
    var blocksPerSeconds = 5;
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
{ // Bitcoin
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