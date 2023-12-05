// https://ethervm.io/
// https://github.com/ethereum/go-ethereum/blob/master/core/vm/instructions.go#L495
// https://solidity-by-example.org/

namespace Ethereum;

public enum OpCode
{
    STOP = 0x00, // Halts execution
    SLOAD = 0x01,  // Load a value onto the stack
    SSTORE = 0x02, // Store a value from the stack
    JUMPIF = 0x03, // Jump to an address if the top of the stack is true
    JUMP = 0x04,  // Jump to an address
    RETURN = 0x05, // Return a value
    MLOAD = 0x06,  // Load a value onto the stack
    SETGREATER = 0x07, // Set top of the stack to true if second value is greater than top
}
