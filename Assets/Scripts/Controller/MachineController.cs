using UnityEngine;

using System.IO;

public class MachineController : MonoBehaviour
{
    [Header("File")]
    [Tooltip("Relative file path from StreamingAssets")]
    public string filePath;

    private Chip8 machine;

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
    }
}
