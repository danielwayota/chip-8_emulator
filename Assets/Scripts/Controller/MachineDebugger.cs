using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class MachineDebugger : MonoBehaviour
{
    [Header("Timer")]
    [Range(0.1f, 1f)]
    public float time = 1f;

    [Header("Displays")]
    public Text memoryDump;
    public Text vDump;
    public Text stackDump;
    public Text registersDump;


    [HideInInspector]
    public Chip8 machine;

    /// =============================================
    void Start()
    {
        StartCoroutine(this.Watch());
    }

    /// =============================================
    IEnumerator Watch()
    {

        while (true)
        {
            yield return new WaitForSeconds(this.time);

            if (this.machine == null)
            {
                continue;
            }

            this.ReadRam();
            this.ReadVRegisters();
            this.ReadStack();
            this.ReadRegisters();
        }
    }

    /// =============================================
    void ReadRam()
    {
        int length = this.machine.ram.Length;

        this.memoryDump.text = "";
        for (int i = this.machine.programCounter; i < length; i++)
        {
            this.memoryDump.text += this.machine.ram[i].ToString("X2");

            if (i % 2 == 1)
            {
                this.memoryDump.text += "\n";
            }
        }
    }

    /// =============================================
    void ReadVRegisters()
    {
        int length = this.machine.v.Length;

        this.vDump.text = "";
        for (int i = 0; i < length; i++)
        {
            var index = i.ToString("X2");
            var value = this.machine.v[i].ToString("X2");

            this.vDump.text += $"V{index} : {value}\n";
        }
    }

    /// =============================================
    void ReadStack()
    {
        int length = this.machine.stack.Length;

        this.stackDump.text = "";

        var ptr = this.machine.stackPointer;

        for (int i = 0; i < length; i++)
        {
            var value = this.machine.stack[i].ToString("X2");

            if (ptr == i)
            {
                this.stackDump.text += $"<color=#9b59b6>{value}</color> ";
            }
            else
            {
                this.stackDump.text += $"{value} ";
            }
        }
    }

    /// =============================================
    void ReadRegisters()
    {
        var i = this.machine.i.ToString("X2");
        var pc = this.machine.programCounter.ToString("X2");

        this.registersDump.text = $"I: {i} - PC: {pc}";
    }
}
