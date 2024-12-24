// MainForm.Designer.cs
using System;
using System.Drawing;
using System.Windows.Forms;

partial class MainForm
{
    private Button circleButton;
    private Button squareButton;
    private Button rhombButton;
    private Button moveButton;
    private Button stopButton;

    private void InitializeComponent()
    {
        this.circleButton = new Button();
        this.squareButton = new Button();
        this.rhombButton = new Button();
        this.moveButton = new Button();
        this.stopButton = new Button();

        // circleButton
        this.circleButton.Location = new Point(10, 10);
        this.circleButton.Name = "circleButton";
        this.circleButton.Size = new Size(75, 23);
        this.circleButton.Text = "Circle";
        this.circleButton.Click += new EventHandler(this.CircleButton_Click);

        // squareButton
        this.squareButton.Location = new Point(100, 10);
        this.squareButton.Name = "squareButton";
        this.squareButton.Size = new Size(75, 23);
        this.squareButton.Text = "Square";
        this.squareButton.Click += new EventHandler(this.SquareButton_Click);

        // rhombButton
        this.rhombButton.Location = new Point(200, 10);
        this.rhombButton.Name = "rhombButton";
        this.rhombButton.Size = new Size(75, 23);
        this.rhombButton.Text = "Rhomb";
        this.rhombButton.Click += new EventHandler(this.RhombButton_Click);

        // moveButton
        this.moveButton.Location = new Point(300, 10);
        this.moveButton.Name = "moveButton";
        this.moveButton.Size = new Size(100, 23);
        this.moveButton.Text = "Move Right";
        this.moveButton.Click += new EventHandler(this.MoveButton_Click);

        // stopButton
        this.stopButton.Location = new Point(410, 10);
        this.stopButton.Name = "stopButton";
        this.stopButton.Size = new Size(75, 23);
        this.stopButton.Text = "Stop";
        this.stopButton.Click += new EventHandler(this.StopButton_Click);

        // MainForm
        this.ClientSize = new Size(800, 600);
        this.Controls.Add(this.circleButton);
        this.Controls.Add(this.squareButton);
        this.Controls.Add(this.rhombButton);
        this.Controls.Add(this.moveButton);
        this.Controls.Add(this.stopButton);
        this.Text = "Figures Example";
    }
}