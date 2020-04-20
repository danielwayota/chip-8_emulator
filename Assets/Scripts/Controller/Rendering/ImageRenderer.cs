using UnityEngine;
using UnityEngine.UI;

public class ImageRenderer : BaseUnityRenderer
{
    public Image target;

    public Vector2Int bufferSize;

    private Texture2D drawTexture;

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

        this.Clear();
    }

    /// ==============================================
    void Update()
    {

    }

    public override void Clear()
    {
        Color[] colors = new Color[this.bufferSize.x * this.bufferSize.y];
        for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0, 0, 0, 0.8f);
        }

        this.drawTexture.SetPixels(colors);
        this.drawTexture.Apply();
    }
}
