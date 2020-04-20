using UnityEngine;

public abstract class BaseUnityRenderer : MonoBehaviour, IRenderer
{
    public abstract void Clear();
    public abstract int DrawSpriteByte(byte data, int x, int y);
}
