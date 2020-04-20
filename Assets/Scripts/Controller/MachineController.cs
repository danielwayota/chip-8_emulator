using UnityEngine;

using System.IO;

public class MachineController : MonoBehaviour
{
    [Header("File")]
    [Tooltip("Relative file path from StreamingAssets")]
    public string filePath;

    public BaseUnityRenderer machineScreen;

    [Header("Debug")]
    public bool hookDebugger = false;
    public bool singleStep = false;

    private Chip8 machine;

    private MachineDebugger debugger;

    // Original keyboard
    // 1  2  3  C
    // 4  5  6  D
    // 7  8  9  E
    // A  0  B  F

    private KeyCode[] keyCodes = new KeyCode[] {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha3,
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V,
    };

    /// ============================================
    /// <summary>
    ///
    /// </summary>
    void Start()
    {
        this.machine = new Chip8();

        this.machine.screen = this.machineScreen;

        var path = Path.Combine(Application.streamingAssetsPath, this.filePath);
        byte[] program = File.ReadAllBytes(path);

        this.machine.LoadProgram(program);

        if (this.hookDebugger)
        {
            this.debugger = FindObjectOfType<MachineDebugger>();
            this.debugger.machine = this.machine;
        }
    }

    /// ============================================
    void Update()
    {
        byte i = 0;
        foreach (var key in this.keyCodes)
        {
            if (Input.GetKeyUp(key))
            {
                this.machine.InputUp(i);
            }
            else if (Input.GetKeyDown(key))
            {
                this.machine.InputDown(i);
            }
            i++;
        }

        if (singleStep)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.machine.Tick();
            }
        }
        else
        {
            this.machine.Tick();
        }
    }
}
