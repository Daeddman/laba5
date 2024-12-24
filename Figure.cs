using System.Drawing;
using System.Threading;

public abstract class Figure
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Figure(int x, int y)
    {
        X = x;
        Y = y;
    }

    public abstract void DrawBlack(Graphics g);
    public abstract void HideDrawingBackGround(Graphics g, Color backColor);

    public void MoveRight(int delta)
    {
        X += delta;
    }
}
