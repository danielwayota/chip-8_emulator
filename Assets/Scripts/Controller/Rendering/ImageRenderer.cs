using UnityEngine;
using UnityEngine.UI;

public class ImageRenderer : BaseUnityRenderer
{
    public Image target;

    public Vector2Int bufferSize;

    private Texture2D drawTexture;

    private byte[] collisionBuffer;

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

        this.collisionBuffer = new byte[this.bufferSize.x * this.bufferSize.y];

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
            this.collisionBuffer[i] = 0x00;
        }

        this.drawTexture.SetPixels(colors);
        this.drawTexture.Apply();
    }

    /// ==============================================
    public override int DrawSpriteByte(byte data, int x, int y)
    {
        // Wrap coordinates
        x %= this.drawTexture.width;
        y %= this.drawTexture.height;

        y = this.drawTexture.height - y;

        int mask = 128; // 0b10000000

        int hasBeenSomeCollsion = 0;

        for (int xoffset = 0; xoffset < 8; xoffset++)
        {
            int collisionIndex = (y * this.drawTexture.width + (x + xoffset)) % this.collisionBuffer.Length;

            int collisionData = this.collisionBuffer[collisionIndex];
            int spritePixel = (data & mask) != 0 ? 1 : 0;

            hasBeenSomeCollsion |= collisionData & spritePixel;

            int shouldDraw = collisionData ^ spritePixel;
            if (shouldDraw != 0)
            {
                this.drawTexture.SetPixel(x + xoffset, y, Color.white);
            }
            else
            {
                this.drawTexture.SetPixel(x + xoffset, y, Color.black);
            }
            mask >>= 1;
        }

        this.drawTexture.Apply();

        return hasBeenSomeCollsion;
    }
}
