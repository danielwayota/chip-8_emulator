using UnityEngine;
using UnityEngine.UI;

public class ImageRenderer : BaseUnityRenderer
{
    public Image target;

    public Vector2Int bufferSize;

    private Texture2D drawTexture;

    private byte[,] collisionBuffer;

    /// ==============================================
    /// <summary>
    ///
    /// </summary>
    void Awake()
    {
        this.drawTexture = new Texture2D(
            this.bufferSize.x,
            this.bufferSize.y,
            TextureFormat.RGBA32,
            1,
            true
        );
        this.drawTexture.filterMode = FilterMode.Point;

        this.target.sprite = Sprite.Create(
            this.drawTexture,
            new Rect(0, 0, this.bufferSize.x, this.bufferSize.y),
            new Vector3(.5f, .5f)
        );

        this.collisionBuffer = new byte[this.bufferSize.x, this.bufferSize.y];

        this.Clear();
    }

    /// ==============================================
    void Update()
    {

    }

    /// ==============================================
    public override void Clear()
    {
        Color[] colors = new Color[this.bufferSize.x * this.bufferSize.y];
        for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }

        for (int x = 0; x < this.bufferSize.x; x++)
        {
            for (int y = 0; y < this.bufferSize.y; y++)
            {
                this.collisionBuffer[x, y] = 0;
            }
        }

        this.drawTexture.SetPixels(colors);
        this.drawTexture.Apply();
    }

    /// ==============================================
    public override int DrawSpriteByte(byte data, int x, int y)
    {
        // Wrap coordinates
        y %= this.drawTexture.height;

        y = (this.drawTexture.height -1) - y;

        int hasBeenSomeCollsion = 0;

        for (int xoffset = 0; xoffset < 8; xoffset++)
        {
            if ((data & (0x80 >> xoffset)) != 0)
            {
                int xx = (x + xoffset) % this.drawTexture.width;
                int collisionData = this.collisionBuffer[xx, y];

                if (collisionData != 0)
                {
                    hasBeenSomeCollsion = 1;

                    this.collisionBuffer[xx, y] = 0;
                    this.drawTexture.SetPixel(xx, y, Color.black);
                }
                else
                {
                    this.collisionBuffer[xx, y] = 1;
                    this.drawTexture.SetPixel(xx, y, Color.white);
                }
            }
        }

        this.drawTexture.Apply();

        return hasBeenSomeCollsion;
    }
}
