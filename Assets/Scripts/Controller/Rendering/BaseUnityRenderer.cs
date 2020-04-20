using UnityEngine;

public abstract class BaseUnityRenderer : MonoBehaviour, IRenderer
{
    public Vector2Int bufferSize;

    protected byte[,] collisionBuffer;

    /// ==============================================
    protected virtual void Awake()
    {
        this.collisionBuffer = new byte[this.bufferSize.x, this.bufferSize.y];
    }

    /// ==============================================
    public virtual int DrawSpriteByte(byte data, int x, int y)
    {
        // Wrap coordinates
        y %= this.bufferSize.y;

        y = (this.bufferSize.y -1) - y;

        int hasBeenSomeCollsion = 0;

        for (int xoffset = 0; xoffset < 8; xoffset++)
        {
            if ((data & (0x80 >> xoffset)) != 0)
            {
                int xx = (x + xoffset) % this.bufferSize.x;
                int collisionData = this.collisionBuffer[xx, y];

                if (collisionData != 0)
                {
                    hasBeenSomeCollsion = 1;

                    this.collisionBuffer[xx, y] = 0;
                    this.ErasePixel(xx, y);
                }
                else
                {
                    this.collisionBuffer[xx, y] = 1;
                    this.DrawPixel(xx, y);
                }
            }
        }

        return hasBeenSomeCollsion;
    }

    public abstract void DrawPixel(int x, int y);
    public abstract void ErasePixel(int x, int y);

    /// ==============================================
    public abstract void Clear();
}
