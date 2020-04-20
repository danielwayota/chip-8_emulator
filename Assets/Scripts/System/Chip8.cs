using UnityEngine;

/// <summary>
/// Documentation:
/// http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
/// </summary>
public class Chip8
{
    // Registers
    public byte[] v { protected set; get; }

    public byte i { protected set; get; }

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
            // 0
            0xF0, 0x90, 0x90, 0x90, 0xF0,
            // 1
            0x20, 0x60, 0x20, 0x20, 0x70,
            // 2
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

        // Execute instruction

        byte codeIndex = (byte)((opCode & 0xF000) >> 12);

        switch (codeIndex)
        {
            case 0:
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
                break;

            // 6xkk - LD Vx, byte
            // Set Vx = kk.
            // The interpreter puts the value kk into register Vx.
            case 6:
                int x = (opCode & 0xF00) >> 8;
                byte kk = (byte)(opCode & 0xFF);

                this.v[x] = kk;

                break;

        }
    }
}
