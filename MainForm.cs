using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private Figure currentFigure;
    private CancellationTokenSource cancellationTokenSource;

    public MainForm()
    {
        InitializeComponent();
        this.Paint += MainForm_Paint;
    }

    private void CircleButton_Click(object sender, EventArgs e)
    {
        currentFigure = new Circle(200, 300, 50);
        Invalidate();
    }

    private void SquareButton_Click(object sender, EventArgs e)
    {
        currentFigure = new Square(200, 300, 100);
        Invalidate();
    }

    private void RhombButton_Click(object sender, EventArgs e)
    {
        currentFigure = new Rhomb(200, 300, 120, 80);
        Invalidate();
    }

    private async void MoveButton_Click(object sender, EventArgs e)
    {
        if (currentFigure == null) return;

        cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        try
        {
            for (int i = 0; i < 50; i++)
            {
                if (token.IsCancellationRequested)
                    break;

                currentFigure.MoveRight(5); // Використання методу для зміщення
                Invalidate();               // Виклик Paint для перемалювання
                await Task.Delay(100);      // Очікування між кроками
            }
        }
        catch (OperationCanceledException)
        {
            // Якщо операцію скасовано
        }
    }


    private void StopButton_Click(object sender, EventArgs e)
    {
        cancellationTokenSource?.Cancel();
    }

    private void MainForm_Paint(object sender, PaintEventArgs e)
    {
        if (currentFigure != null)
        {
            currentFigure.DrawBlack(e.Graphics); // Малювання фігури
        }
    }
}
