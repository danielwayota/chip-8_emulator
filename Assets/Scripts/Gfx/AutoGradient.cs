using UnityEngine;

public class AutoGradient : MonoBehaviour
{
    public Gradient gradient;

    public string shaderName = "_Color";

    private void OnEnable()
    {
        float percent = (this.transform.localPosition.y + 16f) / 32f;

        var color = this.gradient.Evaluate(percent);

        var renderer = this.GetComponentInChildren<MeshRenderer>();
        renderer.material.SetColor(this.shaderName, color);
    }
}
