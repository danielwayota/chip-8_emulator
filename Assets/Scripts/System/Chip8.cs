using UnityEngine;

/// <summary>
/// Documentation:
/// http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
/// </summary>
public class Chip8
{
    // Registers
    public byte[] v { protected set; get; }

    public short i { protected set; get; }

    // Timers
    public byte sound { protected set; get; }
    public byte delay { protected set; get; }

    public short programCounter { protected set; get; }

    // Stack
    public short[] stack { protected set; get; }
    public byte stackPointer { protected set; get; }

    // RAM
    public byte[] ram  { protected set; get; }

    public IRenderer screen;

    public const short PROGRAM_START = 0x200;

    /// =============================================
    /// <summary>
    /// Creates a new instance of the machine
    /// </summary>
    public Chip8()
    {
        // 16 V registers. From V0 to VF
        this.v = new byte[0x10];

        // Stack.
        this.stack = new short[0x10];

        // The RAM is 4KB long
        this.ram = new byte[4096];

        this.LoadCharacterSprites();
        this.Restart();
    }

    /// =============================================
    /// <summary>
    /// Clears all registers
    /// </summary>
    public void Restart()
    {
        for (int i = 0; i < 0x10; i++)
        {
            this.v[i] = 0;
            this.stack[i] = 0;
        }

        this.i = 0;

        this.sound = 0;
        this.delay = 0;

        this.programCounter = PROGRAM_START;
        this.stackPointer = 0;
    }

    /// =============================================
    public void LoadProgram(byte[] program)
    {
        int index = PROGRAM_START;
        foreach(var data in program)
        {
            this.ram[index] = data;

            index++;
        }
    }

    /// =============================================
    public void LoadCharacterSprites()
    {
        byte[] characterSprites = {
            // 0 - 0
            0xF0, 0x90, 0x90, 0x90, 0xF0,
            // 1 - 5
            0x20, 0x60, 0x20, 0x20, 0x70,
            // 2 - 10
            0xF0, 0x10, 0xF0, 0x80, 0xF0,
            // 3
            0xF0, 0x10, 0xF0, 0x10, 0xF0,
            // 4
            0x90, 0x90, 0xF0, 0x10, 0x10,
            // 5
            0xF0, 0x80, 0xF0, 0x10, 0xF0,
            // 6
            0xF0, 0x80, 0xF0, 0x90, 0xF0,
            // 7
            0xF0, 0x10, 0x20, 0x40, 0x40,
            // 8
            0xF0, 0x90, 0xF0, 0x90, 0xF0,
            // 9
            0xF0, 0x90, 0xF0, 0x10, 0xF0,
            // A
            0xF0, 0x90, 0xF0, 0x90, 0x90,
            // B
            0xE0, 0x90, 0xE0, 0x90, 0xE0,
            // C
            0xF0, 0x80, 0x80, 0x80, 0xF0,
            // D
            0xE0, 0x90, 0x90, 0x90, 0xE0,
            // E
            0xF0, 0x80, 0xF0, 0x80, 0xF0,
            // F
            0xF0, 0x80, 0xF0, 0x80, 0x80
        };

        var index = 0;

        foreach (var data in characterSprites)
        {
            this.ram[index] = data;
            index++;
        }
    }

    /// =============================================
    /// <summary>
    /// Executes one machine cicle
    /// </summary>
    public void Tick()
    {
        // Fetch instruction
        byte up = this.ram[this.programCounter];
        byte down = this.ram[this.programCounter + 1];

        short opCode = (short)(up << 8 | down);

        this.ExecuteOpCode(opCode);

        // Advance program counter
        this.programCounter += 2;

        if (this.programCounter > this.ram.Length)
        {
            this.programCounter = PROGRAM_START;
        }
    }

    /// =============================================
    public void ExecuteOpCode(short opCode)
    {
        // Decode instruction
        byte codeIndex = (byte)((opCode & 0xF000) >> 12);

        // Execute instruction
        switch (codeIndex)
        {
            case 0:
                {
                    // 00E0 - CLS
                    // Clear the display.
                    if (opCode == 0x00E0)
                    {
                        this.screen.Clear();
                    }
                    // 00EE - RET
                    // Return from a subroutine.

                    // The interpreter sets the program counter to the address at the top of the stack,
                    // then subtracts 1 from the stack pointer.
                    else if (opCode == 0x00EE)
                    {
                        this.programCounter = this.stack[this.stackPointer];

                        this.stackPointer--;
                    }
                }
                break;

            case 1:
                // 1nnn - JP addr
                // Jump to location nnn.
                // The interpreter sets the program counter to nnn.
                {
                    var nnn = (short) (opCode & 0xFFF);
                    this.programCounter = nnn;

                    // NOTE: after every instruction, the program counter is increased, so
                    //  this is to avoid problems.
                    this.programCounter -= 2;
                }
                break;

            case 2:
                // 2nnn - CALL addr
                // Call subroutine at nnn.
                {
                    // The interpreter increments the stack pointer,
                    this.stackPointer++;
                    // then puts the current PC on the top of the stack. The PC is then set to nnn.
                    this.stack[this.stackPointer] = this.programCounter;

                    var nnn = (short) (opCode & 0xFFF);
                    this.programCounter = nnn;

                    // NOTE: after every instruction, the program counter is increased, so
                    //  this is to avoid problems.
                    this.programCounter -= 2;
                }
                break;

            case 3:
                // 3xkk - SE Vx, byte
                // Skip next instruction if Vx = kk.

                // The interpreter compares register Vx to kk, and if they are equal,
                // increments the program counter by 2.
                {
                    int x = (opCode & 0xF00) >> 8;
                    byte kk = (byte)(opCode & 0xFF);

                    if (this.v[x] == kk)
                    {
                        this.programCounter += 2;
                    }
                }
                break;

            case 4:
                // 4xkk - SNE Vx, byte
                // Skip next instruction if Vx != kk.

                // The interpreter compares register Vx to kk, and if they are not equal,
                // increments the program counter by 2.
                {
                    int x = (opCode & 0xF00) >> 8;
                    byte kk = (byte)(opCode & 0xFF);

                    if (this.v[x] != kk)
                    {
                        this.programCounter += 2;
                    }
                }
                break;
            case 5:
                // 5xy0 - SE Vx, Vy
                // Skip next instruction if Vx = Vy.

                // The interpreter compares register Vx to register Vy, and if they are equal,
                // increments the program counter by 2.
                {
                    int x = (opCode & 0xF00) >> 8;
                    int y = (opCode & 0xF0) >> 4;

                    if (this.v[x] == this.v[y])
                    {
                        this.programCounter += 2;
                    }
                }
                break;

            case 6:
                // 6xkk - LD Vx, byte
                // Set Vx = kk.
                // The interpreter puts the value kk into register Vx.
                {
                    int x = (opCode & 0xF00) >> 8;
                    byte kk = (byte)(opCode & 0xFF);

                    this.v[x] = kk;
                }
                break;

            case 7:
                // 7xkk - ADD Vx, byte
                // Set Vx = Vx + kk.
                // Adds the value kk to the value of register Vx, then stores the result in Vx.
                {
                    int x = (opCode & 0xF00) >> 8;
                    byte kk = (byte)(opCode & 0xFF);

                    this.v[x] += kk;
                }
                break;
            case 8:
                int opCode8Index = opCode & 0xF;

                switch (opCode8Index)
                {
                    case 0:
                        // 8xy0 - LD Vx, Vy
                        // Set Vx = Vy.
                        // Stores the value of register Vy in register Vx.
                        {
                            int x = (opCode & 0xF00) >> 8;
                            int y = (opCode & 0xF0) >> 4;

                            this.v[x] = this.v[y];
                        }
                        break;
                    case 1:
                        // 8xy1 - OR Vx, Vy
                        // Set Vx = Vx OR Vy.

                        // Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx.
                        // A bitwise OR compares the corrseponding bits from two values,
                        // and if either bit is 1, then the same bit in the result is also 1. Otherwise, it is 0.
                        {
                            var x = (opCode & 0xF00) >> 8;
                            var y = (opCode & 0xF0) >> 4;

                            this.v[x] = (byte)(this.v[x] | this.v[y]);
                        }

                        break;
                    case 2:
                        // 8xy2 - AND Vx, Vy
                        // Set Vx = Vx AND Vy.

                        // Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx.
                        // A bitwise AND compares the corrseponding bits from two values, and if both bits are 1,
                        // then the same bit in the result is also 1. Otherwise, it is 0.
                        {
                            var x = (opCode & 0xF00) >> 8;
                            var y = (opCode & 0xF0) >> 4;

                            this.v[x] = (byte)(this.v[x] & this.v[y]);
                        }
                        break;
                    case 3:
                        // 8xy3 - XOR Vx, Vy
                        // Set Vx = Vx XOR Vy.

                        // Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx.
                        // An exclusive OR compares the corrseponding bits from two values,
                        // and if the bits are not both the same, then the corresponding bit in the result is set to 1. Otherwise, it is 0.
                        {
                            var x = (opCode & 0xF00) >> 8;
                            var y = (opCode & 0xF0) >> 4;

                            this.v[x] = (byte)(this.v[x] ^ this.v[y]);
                        }
                        break;
                    case 4:
                        // 8xy4 - ADD Vx, Vy
                        // Set Vx = Vx + Vy, set VF = carry.

                        // The values of Vx and Vy are added together. If the result is greater than 8 bits (i.e., > 255,)
                        // VF is set to 1, otherwise 0. Only the lowest 8 bits of the result are kept, and stored in Vx.

                        {
                            var x = (opCode & 0xF00) >> 8;
                            var y = (opCode & 0xF0) >> 4;

                            int sum = ((int)this.v[x] + (int)this.v[y]);

                            this.v[0xF] = (byte)((sum > 0xFF) ? 1 : 0);

                            this.v[x] = (byte)(sum & 0xFF);
                        }
                        break;
                    case 5:
                        // 8xy5 - SUB Vx, Vy
                        // Set Vx = Vx - Vy, set VF = NOT borrow.

                        // If Vx > Vy, then VF is set to 1, otherwise 0.
                        // Then Vy is subtracted from Vx, and the results stored in Vx.
                        {
                            var x = (opCode & 0xF00) >> 8;
                            var y = (opCode & 0xF0) >> 4;

                            this.v[0xF] = (byte)((this.v[x] > this.v[y]) ? 1 : 0);

                            this.v[x] = (byte)((this.v[x] - this.v[y]) & 0xFF);
                        }
                        break;
                    case 6:
                        // 8xy6 - SHR Vx {, Vy}
                        // Set Vx = Vx SHR 1.

                        // If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0.
                        // Then Vx is divided by 2.
                        {
                            var x = (opCode & 0xF00) >> 8;

                            this.v[0xF] = (byte)(this.v[x] & 0x1);

                            this.v[x] = (byte)(this.v[x] >> 1);
                        }
                        break;
                    case 7:
                        // 8xy7 - SUBN Vx, Vy
                        // Set Vx = Vy - Vx, set VF = NOT borrow.

                        // If Vy > Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy,
                        // and the results stored in Vx.
                        {
                            var x = (opCode & 0xF00) >> 8;
                            var y = (opCode & 0xF0) >> 4;

                            this.v[0xF] = (byte)((this.v[y] > this.v[x]) ? 1 : 0);

                            this.v[x] = (byte)((this.v[y] - this.v[x]) & 0xFF);
                        }
                        break;
                    case 0xE:
                        // 8xyE - SHL Vx {, Vy}
                        // Set Vx = Vx SHL 1.

                        // If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0.
                        // Then Vx is multiplied by 2.
                        {
                            var x = (opCode & 0xF00) >> 8;

                            this.v[0xF] = (byte)((this.v[x] >> 7) & 0x01);

                            this.v[x] = (byte)(this.v[x] << 1);
                        }
                        break;
                }
                break;
            case 9:
                // 9xy0 - SNE Vx, Vy
                // Skip next instruction if Vx != Vy.

                // The values of Vx and Vy are compared, and if they are not equal,
                // the program counter is increased by 2.
                {
                    var x = (opCode & 0xF00) >> 8;
                    var y = (opCode & 0xF0) >> 4;

                    if (this.v[x] != this.v[y])
                    {
                        this.programCounter += 2;
                    }
                }
                break;
            case 0xA:
                // Annn - LD I, addr
                // Set I = nnn.
                // The value of register I is set to nnn.
                {
                    var nnn = (short) (opCode & 0xFFF);
                    this.i = nnn;
                }
                break;
            case 0xB:
                // Bnnn - JP V0, addr
                // Jump to location nnn + V0.

                // The program counter is set to nnn plus the value of V0.
                {
                    var nnn = opCode & 0xFFF;
                    var v0 = this.v[0];

                    this.programCounter = (short)(nnn + v0);

                    // NOTE: after every instruction, the program counter is increased, so
                    //  this is to avoid problems.
                    this.programCounter -= 2;
                }
                break;
            case 0xC:
                // Cxkk - RND Vx, byte
                // Set Vx = random byte AND kk.
                // The interpreter generates a random number from 0 to 255,
                // which is then ANDed with the value kk. The results are stored in Vx.
                // See instruction 8xy2 for more information on AND.
                {
                    var x = (opCode & 0xF00) >> 8;
                    var kk = (opCode & 0xFF);

                    var rnd = Random.Range(0, 256);

                    this.v[x] = (byte)(rnd & kk);
                }
                break;
            case 0xD:
                // Dxyn - DRW Vx, Vy, nibble
                // Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.

                // The interpreter reads n bytes from memory, starting at the address stored in I.
                // These bytes are then displayed as sprites on screen at coordinates (Vx, Vy).
                // Sprites are XORed onto the existing screen. If this causes any pixels to be erased,
                // VF is set to 1, otherwise it is set to 0. If the sprite is positioned so part of it is outside
                // the coordinates of the display, it wraps around to the opposite side of the screen.
                // See instruction 8xy3 for more information on XOR.
                {
                    var x = (opCode & 0xF00) >> 8;
                    var y = (opCode & 0xF0) >> 4;
                    var n = (opCode & 0xF);

                    byte[] spriteBytes = new byte[n];

                    for (int offset = 0; offset < n; offset++)
                    {
                        spriteBytes[offset] = this.ram[this.i  + offset];
                    }

                    int voffset = 0;
                    int collision = 0;
                    foreach (var data in spriteBytes)
                    {
                        collision |= this.screen.DrawSpriteByte(data, this.v[x], this.v[y] + voffset);

                        voffset ++;
                    }

                    this.v[0xF] = (byte)(collision & 0xFF);
                }

                break;

            case 0xE:
                int subECode = opCode & 0xFF;
                switch (subECode)
                {
                    case 0x9E:
                        // TODO: Input
                        // Ex9E - SKP Vx
                        // Skip next instruction if key with the value of Vx is pressed.

                        // Checks the keyboard, and if the key corresponding to the value of Vx is currently
                        // in the down position, PC is increased by 2.
                        break;
                    case 0xA1:
                        // TODO: input
                        // ExA1 - SKNP Vx
                        // Skip next instruction if key with the value of Vx is not pressed.

                        // Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position,
                        // PC is increased by 2.
                        break;
                }
                break;
            case 0xF:
                int subFCode = opCode & 0xFF;

                switch (subFCode)
                {
                    case 0x07:
                        // Fx07 - LD Vx, DT
                        // Set Vx = delay timer value.

                        // The value of DT is placed into Vx.
                        {
                            var x = (opCode & 0xF00) >> 8;

                            this.v[x] = this.delay;
                        }
                        break;
                    case 0x0A:
                        // TODO: input
                        // Fx0A - LD Vx, K
                        // Wait for a key press, store the value of the key in Vx.
                        // All execution stops until a key is pressed, then the value of that key is stored in Vx.
                        {
                            var x = (opCode & 0xF00) >> 8;
                        }
                        break;
                    case 0x15:
                        // Fx15 - LD DT, Vx
                        // Set delay timer = Vx.
                        // DT is set equal to the value of Vx.
                        {
                            var x = (opCode & 0xF00) >> 8;
                            this.delay = this.v[x];
                        }
                        break;
                    case 0x18:
                        // TODO: sound
                        // Fx18 - LD ST, Vx
                        // Set sound timer = Vx.

                        // ST is set equal to the value of Vx.
                        {
                            var x = (opCode & 0xF00) >> 8;
                            this.sound = this.v[x];
                        }
                        break;
                    case 0x1E:
                        // Fx1E - ADD I, Vx
                        // Set I = I + Vx.

                        // The values of I and Vx are added, and the results are stored in I.
                        {
                            var x = (opCode & 0xF00) >> 8;

                            this.i = (short)(this.i + this.v[x]);
                        }
                        break;

                    case 0x29:
                        // Fx29 - LD F, Vx
                        // Set I = location of sprite for digit Vx.

                        // The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx.
                        {
                            var x = (opCode & 0xF00) >> 8;

                            this.i = (short)(this.v[x] * 5);
                        }
                        break;
                    case 0x33:
                        // Fx33 - LD B, Vx
                        // Store BCD representation of Vx in memory locations I, I+1, and I+2.

                        // The interpreter takes the decimal value of Vx, and places the hundreds digit in memory
                        // at location in I, the tens digit at location I+1, and the ones digit at location I+2.
                        {
                            var x = (opCode & 0xF00) >> 8;

                            int num = this.v[x];

                            int hundreds = num / 100;
                            num -= hundreds;
                            int tens = num / 10;
                            num -= tens;
                            int ones = num;

                            this.ram[this.i + 0] = (byte)hundreds;
                            this.ram[this.i + 1] = (byte)tens;
                            this.ram[this.i + 2] = (byte)ones;
                        }
                        break;

                    case 0x55:
                        // Fx55 - LD [I], Vx
                        // Store registers V0 through Vx in memory starting at location I.
                        // The interpreter copies the values of registers V0 through Vx into memory, starting at the address in I.
                        {
                            var x = (opCode & 0xF00) >> 8;

                            for (int offset = 0; offset < x; offset++)
                            {
                                this.ram[this.i + offset] = this.v[offset];
                            }
                        }
                        break;

                    case 0x65:
                        // Fx65 - LD Vx, [I]
                        // Read registers V0 through Vx from memory starting at location I.
                        // The interpreter reads values from memory starting at location I into registers V0 through Vx.
                        {
                            var x = (opCode & 0xF00) >> 8;

                            for (int offset = 0; offset < x; offset++)
                            {
                                this.v[offset] = this.ram[this.i + offset];
                            }
                        }
                        break;

                }
                break;
        }
    }
}
