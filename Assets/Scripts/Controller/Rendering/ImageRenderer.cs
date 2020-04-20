using UnityEngine;
using UnityEngine.UI;

public class ImageRenderer : BaseUnityRenderer
{
    public Image target;

    private Texture2D drawTexture;

    /// ==============================================
    /// <summary>
    ///
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

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

        this.Clear();
    }

    /// ==============================================
    public override void Clear()
    {
        base.Clear();

        Color[] colors = new Color[this.bufferSize.x * this.bufferSize.y];
        for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }

        this.drawTexture.SetPixels(colors);
        this.drawTexture.Apply();
    }

    /// ==============================================
    public override int DrawSpriteByte(byte data, int x, int y)
    {
        var collision = base.DrawSpriteByte(data, x, y);

        this.drawTexture.Apply();

        return collision;
    }

    /// ==============================================
    public override void DrawPixel(int x, int y)
    {
        this.drawTexture.SetPixel(x, y, Color.white);
    }

    /// ==============================================
    public override void ErasePixel(int x, int y)
    {
        this.drawTexture.SetPixel(x, y, Color.black);
    }
}
