using System.Drawing;

public class Rhomb : Figure
{
    private int HorDiagLen { get; }
    private int VertDiagLen { get; }

    public Rhomb(int x, int y, int horDiagLen, int vertDiagLen) : base(x, y)
    {
        HorDiagLen = horDiagLen;
        VertDiagLen = vertDiagLen;
    }

    public override void DrawBlack(Graphics g)
    {
        Point[] points =
        {
            new Point(X, Y - VertDiagLen / 2),
            new Point(X + HorDiagLen / 2, Y),
            new Point(X, Y + VertDiagLen / 2),
            new Point(X - HorDiagLen / 2, Y)
        };
        g.DrawPolygon(Pens.Black, points);
    }

    public override void HideDrawingBackGround(Graphics g, Color backColor)
    {
        using (Brush brush = new SolidBrush(backColor))
        {
            Point[] points =
            {
                new Point(X, Y - VertDiagLen / 2),
                new Point(X + HorDiagLen / 2, Y),
                new Point(X, Y + VertDiagLen / 2),
                new Point(X - HorDiagLen / 2, Y)
            };
            g.FillPolygon(brush, points);
        }
    }
}
