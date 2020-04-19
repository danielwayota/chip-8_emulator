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
    /// <summary>
    /// Executes one machine cicle
    /// </summary>
    public void Tick()
    {
        // Fetch instruction
        // Decode instruction
        // Execute instruction
        // Advance program counter
    }
}
