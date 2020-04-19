using UnityEngine;

using System.IO;

public class MachineController : MonoBehaviour
{
    [Header("File")]
    [Tooltip("Relative file path from StreamingAssets")]
    public string filePath;

    [Header("Hook debugger")]
    public bool hookDebugger = false;

    private Chip8 machine;

    private MachineDebugger debugger;

    /// ============================================
    /// <summary>
    ///
    /// </summary>
    void Start()
    {
        this.machine = new Chip8();

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
        this.machine.Tick();
    }
}
