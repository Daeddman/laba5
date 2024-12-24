using System.Drawing;

public class Circle : Figure
{
    private int Radius { get; }

    public Circle(int x, int y, int radius) : base(x, y)
    {
        Radius = radius;
    }

    public override void DrawBlack(Graphics g)
    {
        g.DrawEllipse(Pens.Black, X - Radius, Y - Radius, Radius * 2, Radius * 2);
    }

    public override void HideDrawingBackGround(Graphics g, Color backColor)
    {
        using (Brush brush = new SolidBrush(backColor))
        {
            g.FillEllipse(brush, X - Radius, Y - Radius, Radius * 2, Radius * 2);
        }
    }
}