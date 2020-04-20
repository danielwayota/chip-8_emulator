using UnityEngine;

using System.Collections.Generic;

public class GameObjectRenderer : BaseUnityRenderer
{
    public Transform pseudoPixelAnchor;
    public GameObject pseudoPixelPrefab;
    public Vector2 pseudoPixelSize;

    protected List<GameObject> pseudoPixelPool;

    protected GameObject[,] screen;

    /// ======================================================
    protected override void Awake()
    {
        base.Awake();

        this.pseudoPixelPool = new List<GameObject>();

        this.screen = new GameObject[this.bufferSize.x, this.bufferSize.y];
    }

    /// ======================================================
    protected GameObject GetNextPseudoPixel()
    {
        foreach (var objxel in this.pseudoPixelPool)
        {
            if (objxel.activeSelf == false)
            {
                return objxel;
            }
        }

        var go = Instantiate(this.pseudoPixelPrefab);
        this.pseudoPixelPool.Add(go);

        go.transform.parent = this.pseudoPixelAnchor;

        return go;
    }

    /// ======================================================
    protected Vector3 MapPixelPosition(int x, int y)
    {
        var xx = x * this.pseudoPixelSize.x;
        var yy = y * this.pseudoPixelSize.y;

        xx -= (this.bufferSize.x * this.pseudoPixelSize.x) / 2f;
        yy -= (this.bufferSize.y * this.pseudoPixelSize.y) / 2f;

        return new Vector3(xx, yy, 0);
    }

    /// ======================================================
    public override void Clear()
    {
        base.Clear();

        foreach (var item in this.pseudoPixelPool)
        {
            item.SetActive(false);
        }

        for (int x = 0; x < this.bufferSize.x; x++)
        {
            for (int y = 0; y < this.bufferSize.y; y++)
            {
                this.screen[x, y] = null;
            }
        }
    }

    /// ======================================================
    public override void DrawPixel(int x, int y)
    {
        var pseudoPixel = this.GetNextPseudoPixel();

        pseudoPixel.transform.localPosition = this.MapPixelPosition(x, y);
        pseudoPixel.SetActive(true);

        this.screen[x, y] = pseudoPixel;
    }

    /// ======================================================
    public override void ErasePixel(int x, int y)
    {
        if (this.screen[x, y] != null)
        {
            var go = this.screen[x, y];
            go.SetActive(false);

            this.screen[x, y] = null;
        }
    }
}
