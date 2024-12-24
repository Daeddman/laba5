using System.Drawing;

public class Square : Figure
{
    private int SideLength { get; }

    public Square(int x, int y, int sideLength) : base(x, y)
    {
        SideLength = sideLength;
    }

    public override void DrawBlack(Graphics g)
    {
        g.DrawRectangle(Pens.Black, X - SideLength / 2, Y - SideLength / 2, SideLength, SideLength);
    }

    public override void HideDrawingBackGround(Graphics g, Color backColor)
    {
        using (Brush brush = new SolidBrush(backColor))
        {
            g.FillRectangle(brush, X - SideLength / 2, Y - SideLength / 2, SideLength, SideLength);
        }
    }
}