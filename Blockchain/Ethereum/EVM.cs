namespace Ethereum;

class EVM {

    public static void ExecuteCode(AccountState contractState, byte[] args)
    {
        var bytecode = contractState.Code!;
        var storage = contractState.Storage;
        var stack = new Stack<int>();
        var memory = new List<int>();

        for (int i = 0; i < args.Length; i++)
        {
            memory.Add(args[i]);
        }

        try
        {
            int pc = 0; // Program counter
            while (pc < bytecode.Length)
            {
                var code = bytecode[pc];
                if (stack.Count > 0) {
                    Console.WriteLine($"Stack: {string.Join(", ", stack)}");
                } else {
                    Console.WriteLine("Stack: []");
                }
                Console.WriteLine($"{Enum.GetName(typeof(OpCode), code)}");

                switch (code)
                {
                    case (byte)OpCode.MLOAD:
                        // Load a value from memory onto the stack
                        // For simplicity, assuming next byte in bytecode is the memory address to load from
                        pc++;
                        var memAddress = bytecode[pc];
                        var value = memory[memAddress];
                        stack.Push(value);
                        break;

                    case (byte)OpCode.SLOAD:
                        // Load a value onto the stack
                        // For simplicity, assuming next byte in bytecode is the address to load from
                        pc++;
                        var address = bytecode[pc]!;
                        stack.Push(storage.GetValueOrDefault(address,0));
                        break;

                    case (byte)OpCode.SSTORE:
                        // Store a value from the stack into storage
                        // Assuming next byte in bytecode is the address to store to
                        pc++;
                        address = bytecode[pc];
                        int valueToStore = stack.Pop();
                        storage[address] = valueToStore;
                        break;

                    case (byte)OpCode.SETGREATER:
                        // Compare top two values on stack, set result on stack
                        int value1 = stack.Pop();
                        int value2 = stack.Pop();
                        stack.Push(value1 > value2 ? 1 : 0);
                        break;

                    case (byte)OpCode.JUMPIF:
                        // Jump if top of stack is true (non-zero)
                        int condition = stack.Pop();
                        pc++;
                        int jumpAddress = bytecode[pc];
                        if (condition != 0)
                        {
                            pc = jumpAddress;
                            continue;
                        }
                        break;

                    case (byte)OpCode.JUMP:
                        // Unconditional jump
                        pc++;
                        jumpAddress = bytecode[pc];
                        pc = jumpAddress;
                        continue;

                    case (byte)OpCode.RETURN:
                        // Handle the return value as needed
                        break;

                    case (byte)OpCode.STOP:
                        return;
                }

                pc++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Contract execution failed: {ex.Message}");
        }
    }

    public static string GenerateContractAddress(string creatorAddress, int creatorNonce)
    {
        var hash = Crypto.Hash.Sha256String(creatorAddress + creatorNonce);
        return hash;
    }

}