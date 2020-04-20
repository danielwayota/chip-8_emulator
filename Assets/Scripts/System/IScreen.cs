/// <summary>
/// This interface helps the code to be portable
/// </summary>
public interface IRenderer
{
    void Clear();
    int DrawSpriteByte(byte data, int x, int y);
}
