using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SlotMachine
{
    public partial class Form1 : Form
    {
        private PictureBox pbSlot1;
        private PictureBox pbSlot2;
        private PictureBox pbSlot3;

        private TextBox txtBet;
        private Label lblBalance;
        private Label lblResult;
        private Label lblError;
        private Button btnAddBalance;
        private Timer spinTimer;

        private Panel machinePanel;
        private Panel reelsPanel;

        private int addBalanceClicks = 0;
        private int balance = 1000;

        // Можливі символи (з вагами)
        private (Symbol symbol, int weight)[] weightedSymbols;

        private int spinCount = 0;
        private int spinDuration = 20;
        private Random random = new Random();

        // Індекси фінальних символів
        private int[] finalIndexes = new int[3];

        // Поточні індекси (для анімації)
        private int[] currentIndexes = new int[3];

        // Єдиний елемент керування, що містить і стержень, і кружок
        private LeverControl leverControl;

        public Form1()
        {
            InitializeComponent();

            // Ініціалізація масиву з вагами
            weightedSymbols = new (Symbol symbol, int weight)[]
            {
                (new Cherry(),     50),
                (new Lemon(),      30),
                (new Watermelon(), 10),
                (new Grape(),      5),
                (new Seven(),      1)
            };
        }

        private void InitializeComponent()
        {
            this.txtBet = new TextBox();
            this.lblBalance = new Label();
            this.lblResult = new Label();
            this.lblError = new Label();
            this.btnAddBalance = new Button();
            this.spinTimer = new Timer();

            this.SuspendLayout();

            // Параметри форми
            this.ClientSize = new Size(1000, 700);
            this.Text = "Slot Machine";
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.BackColor = Color.Black;

            // Панель машини
            machinePanel = new Panel();
            machinePanel.Size = new Size(800, 600);
            machinePanel.Location = new Point(100, 50);
            machinePanel.BackColor = Color.Transparent;
            machinePanel.Paint += MachinePanel_Paint;
            this.Controls.Add(machinePanel);

            // Панель з барабанами
            reelsPanel = new Panel();
            reelsPanel.Size = new Size(400, 180);
            reelsPanel.Location = new Point((machinePanel.Width - 400) / 2, 200);
            reelsPanel.BackColor = Color.Transparent;
            reelsPanel.Paint += ReelsPanel_Paint;
            machinePanel.Controls.Add(reelsPanel);

            // PictureBox для першого барабана
            pbSlot1 = new PictureBox();
            pbSlot1.Size = new Size(100, 100);
            pbSlot1.Location = new Point(25, 40);
            pbSlot1.SizeMode = PictureBoxSizeMode.StretchImage;
            pbSlot1.BackColor = Color.Black;
            reelsPanel.Controls.Add(pbSlot1);

            // PictureBox для другого барабана
            pbSlot2 = new PictureBox();
            pbSlot2.Size = new Size(100, 100);
            pbSlot2.Location = new Point(150, 40);
            pbSlot2.SizeMode = PictureBoxSizeMode.StretchImage;
            pbSlot2.BackColor = Color.Black;
            reelsPanel.Controls.Add(pbSlot2);

            // PictureBox для третього барабана
            pbSlot3 = new PictureBox();
            pbSlot3.Size = new Size(100, 100);
            pbSlot3.Location = new Point(275, 40);
            pbSlot3.SizeMode = PictureBoxSizeMode.StretchImage;
            pbSlot3.BackColor = Color.Black;
            reelsPanel.Controls.Add(pbSlot3);

            // Текстове поле для введення ставки
            this.txtBet.Font = new Font("Segoe UI", 14F, FontStyle.Italic, GraphicsUnit.Point);
            this.txtBet.ForeColor = Color.Gray;
            this.txtBet.Location = new Point((machinePanel.Width - 150) / 2, 400);
            this.txtBet.Size = new Size(150, 32);
            this.txtBet.Text = "Enter Bet";
            this.txtBet.GotFocus += new EventHandler(this.TxtBet_GotFocus);
            this.txtBet.LostFocus += new EventHandler(this.TxtBet_LostFocus);
            this.txtBet.TextChanged += new EventHandler(this.TxtBet_TextChanged);
            machinePanel.Controls.Add(this.txtBet);

            // Лейбл Балансу
            this.lblBalance.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblBalance.ForeColor = Color.Yellow;
            this.lblBalance.BackColor = Color.Transparent;
            this.lblBalance.Location = new Point(20, 20);
            this.lblBalance.Size = new Size(200, 30);
            this.lblBalance.Text = $"Balance: {balance}";
            machinePanel.Controls.Add(this.lblBalance);

            // Лейбл для помилок
            this.lblError.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblError.ForeColor = Color.White;
            this.lblError.BackColor = Color.Transparent;
            this.lblError.Location = new Point((machinePanel.Width - 400) / 2, 440);
            this.lblError.Size = new Size(400, 30);
            machinePanel.Controls.Add(this.lblError);

            // Лейбл для результату
            this.lblResult.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblResult.ForeColor = Color.White;
            this.lblResult.BackColor = Color.Transparent;
            this.lblResult.Location = new Point(20, machinePanel.Height - 50);
            this.lblResult.Size = new Size(300, 30);
            this.lblResult.Text = "Result: -";
            machinePanel.Controls.Add(this.lblResult);

            // Кнопка "Add Balance"
            this.btnAddBalance.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.btnAddBalance.ForeColor = Color.White;
            this.btnAddBalance.BackColor = Color.DarkGreen;
            this.btnAddBalance.FlatStyle = FlatStyle.Flat;
            this.btnAddBalance.Location = new Point(machinePanel.Width - 200, machinePanel.Height - 60);
            this.btnAddBalance.Size = new Size(150, 40);
            this.btnAddBalance.Text = "Add Balance";
            this.btnAddBalance.Click += new EventHandler(this.BtnAddBalance_Click);
            machinePanel.Controls.Add(this.btnAddBalance);

            // Створюємо кастомний LeverControl
            leverControl = new LeverControl();
            // Розміщуємо його праворуч від панелі машини
            leverControl.Location = new Point(machinePanel.Right + 30, 200);
            leverControl.LeverPulled += (s, e) =>
            {
                // Коли ручку дотягли назад вгору — виконуємо спін
                BtnSpin_Click(null, null);
            };
            this.Controls.Add(leverControl);

            // Таймер для анімації обертання барабанів
            this.spinTimer.Interval = 100;
            this.spinTimer.Tick += new EventHandler(this.SpinTimer_Tick);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void MachinePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle machineRect = new Rectangle(0, 0, machinePanel.Width - 1, machinePanel.Height - 1);
            using (LinearGradientBrush bodyBrush = new LinearGradientBrush(
                machineRect, Color.DarkRed, Color.Black, LinearGradientMode.Vertical))
            {
                g.FillRectangle(bodyBrush, machineRect);
            }

            using (Pen pen = new Pen(Color.Gold, 5))
            {
                g.DrawRectangle(pen, machineRect);
            }

            int marqueeHeight = 80;
            Rectangle marqueeRect = new Rectangle(
                (machinePanel.Width - 400) / 2, 20, 400, marqueeHeight);

            using (GraphicsPath path = new GraphicsPath())
            {
                // Напівкруглий верхній банер
                path.AddArc(marqueeRect.X, marqueeRect.Y,
                            marqueeRect.Width, marqueeRect.Height * 2, 180, 180);
                path.CloseFigure();

                using (LinearGradientBrush marqueeBrush = new LinearGradientBrush(
                    marqueeRect, Color.Yellow, Color.Red, LinearGradientMode.Vertical))
                {
                    g.FillPath(marqueeBrush, path);
                }

                using (Pen marqueePen = new Pen(Color.Black, 3))
                {
                    g.DrawPath(marqueePen, path);
                }

                // Напис "SLOT MACHINE"
                using (Font f = new Font("Segoe UI", 20F, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    string title = "SLOT MACHINE";
                    SizeF textSize = g.MeasureString(title, f);
                    float tx = marqueeRect.X + (marqueeRect.Width - textSize.Width) / 2;
                    float ty = marqueeRect.Y + (marqueeHeight - textSize.Height) / 2;
                    g.DrawString(title, f, textBrush, tx, ty);
                }
            }
        }

        private void ReelsPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Зовнішній золотий градієнт
            using (LinearGradientBrush brush = new LinearGradientBrush(
                reelsPanel.ClientRectangle, Color.Gold, Color.DarkGoldenrod, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, reelsPanel.ClientRectangle);
            }

            Rectangle innerRect = new Rectangle(5, 5, reelsPanel.Width - 10, reelsPanel.Height - 10);
            using (SolidBrush innerBrush = new SolidBrush(Color.Black))
            {
                g.FillRectangle(innerBrush, innerRect);
            }

            // Золота рамка
            using (Pen pen = new Pen(Color.Gold, 3))
            {
                g.DrawRectangle(pen, 1, 1, reelsPanel.Width - 2, reelsPanel.Height - 2);
            }
        }

        // Отримати випадковий індекс з урахуванням ваги
        private int GetRandomIndexFromWeights()
        {
            int totalWeight = 0;
            foreach (var ws in weightedSymbols)
                totalWeight += ws.weight;

            int rnd = random.Next(1, totalWeight + 1);
            int cumulative = 0;
            for (int i = 0; i < weightedSymbols.Length; i++)
            {
                cumulative += weightedSymbols[i].weight;
                if (rnd <= cumulative)
                    return i;
            }
            return 0;
        }

        private void CheckResult()
        {
            int i1 = finalIndexes[0];
            int i2 = finalIndexes[1];
            int i3 = finalIndexes[2];

            if (i1 == i2 && i2 == i3)
            {
                int bet = int.Parse(txtBet.Text);
                Symbol sym = weightedSymbols[i1].symbol;

                int winnings = sym.GetWinnings(bet);

                balance += winnings;
                lblBalance.Text = $"Balance: {balance}";
                lblResult.Text = $"You Win: {winnings}!";
            }
            else
            {
                lblResult.Text = "Try Again!";
            }

            // Якщо баланс > 0, тоді можна знову грати
            leverControl.Enabled = (balance > 0);
        }

        // Викликається з LeverControl, коли ручка "дійшла" назад угору
        private void BtnSpin_Click(object sender, EventArgs e)
        {
            int bet;
            lblError.Text = "";

            if (!int.TryParse(txtBet.Text, out bet) || bet <= 0)
            {
                lblError.Text = "Invalid bet amount!";
                return;
            }

            if (balance < bet)
            {
                lblError.Text = "Not enough balance!";
                return;
            }

            // Генеруємо фінальний результат
            for (int i = 0; i < 3; i++)
            {
                finalIndexes[i] = GetRandomIndexFromWeights();
            }

            // Віднімаємо ставку з балансу
            balance -= bet;
            lblBalance.Text = $"Balance: {balance}";

            // Початкові (поточні) позиції
            for (int i = 0; i < 3; i++)
            {
                currentIndexes[i] = GetRandomIndexFromWeights();
            }

            // Показати "випадкові" символи для анімації
            pbSlot1.Image = weightedSymbols[currentIndexes[0]].symbol.GetImage();
            pbSlot2.Image = weightedSymbols[currentIndexes[1]].symbol.GetImage();
            pbSlot3.Image = weightedSymbols[currentIndexes[2]].symbol.GetImage();
            lblResult.Text = "Spinning...";

            spinTimer.Start();
            spinCount = 0;

            // На час обертання відключимо ручку
            leverControl.Enabled = false;
        }

        // Крок анімації обертання барабанів
        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                currentIndexes[i] = (currentIndexes[i] + 1) % weightedSymbols.Length;
            }

            pbSlot1.Image = weightedSymbols[currentIndexes[0]].symbol.GetImage();
            pbSlot2.Image = weightedSymbols[currentIndexes[1]].symbol.GetImage();
            pbSlot3.Image = weightedSymbols[currentIndexes[2]].symbol.GetImage();

            spinCount++;

            if (spinCount >= spinDuration)
            {
                spinTimer.Stop();
                spinCount = 0;

                // Фінальні зображення
                pbSlot1.Image = weightedSymbols[finalIndexes[0]].symbol.GetImage();
                pbSlot2.Image = weightedSymbols[finalIndexes[1]].symbol.GetImage();
                pbSlot3.Image = weightedSymbols[finalIndexes[2]].symbol.GetImage();

                // Перевірка результату
                CheckResult();
            }
        }

        // Додаємо баланс (обмеження 3 рази)
        private void BtnAddBalance_Click(object sender, EventArgs e)
        {
            if (addBalanceClicks >= 3)
            {
                lblError.Text = "You can only add balance 3 times!";
                return;
            }

            balance += 1000;
            lblBalance.Text = $"Balance: {balance}";
            addBalanceClicks++;
        }

        // При фокусі в полі для ставки
        private void TxtBet_GotFocus(object sender, EventArgs e)
        {
            if (txtBet.Text == "Enter Bet")
            {
                txtBet.Text = "";
                txtBet.ForeColor = Color.Black;
            }
        }

        // При втраті фокуса в полі для ставки
        private void TxtBet_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBet.Text))
            {
                txtBet.Text = "Enter Bet";
                txtBet.ForeColor = Color.Gray;
            }
        }

        // Зміна тексту в полі для ставки (щоб вмикати/вимикати ручку)
        private void TxtBet_TextChanged(object sender, EventArgs e)
        {
            int bet;
            leverControl.Enabled = (int.TryParse(txtBet.Text, out bet) && bet > 0 && balance >= bet);
        }

        // =======================
        // Класи-символи (зображення + підрахунок виграшу)
        // =======================
        abstract class Symbol
        {
            public abstract Image GetImage();
            public abstract int GetWinnings(int bet);
        }

        class Cherry : Symbol
        {
            private static readonly Image cherryImage = Image.FromFile("C:\\Users\\oleks\\Desktop\\Casino\\Casino\\cherry.png");

            public override Image GetImage() => cherryImage;
            public override int GetWinnings(int bet) => bet * 2;
        }

        class Lemon : Symbol
        {
            private static readonly Image lemonImage = Image.FromFile("C:\\Users\\oleks\\Desktop\\Casino\\Casino\\Lemon.png");

            public override Image GetImage() => lemonImage;
            public override int GetWinnings(int bet) => bet * 3;
        }

        class Watermelon : Symbol
        {
            private static readonly Image watermelonImage = Image.FromFile("C:\\Users\\oleks\\Desktop\\Casino\\Casino\\Watermelon.png");

            public override Image GetImage() => watermelonImage;
            public override int GetWinnings(int bet) => bet * 5;
        }

        class Grape : Symbol
        {
            private static readonly Image grapeImage = Image.FromFile("C:\\Users\\oleks\\Desktop\\Casino\\Casino\\Grape.png");

            public override Image GetImage() => grapeImage;
            public override int GetWinnings(int bet) => bet * 7;
        }

        class Seven : Symbol
        {
            private static readonly Image sevenImage = Image.FromFile("C:\\Users\\oleks\\Desktop\\Casino\\Casino\\Seven.png");

            public override Image GetImage() => sevenImage;
            public override int GetWinnings(int bet) => 10000;
        }
    }

    // ============
    // Користувацький елемент керування, що містить і стержень, і кружок
    // і анімує їхнє «опускання/піднімання» при кліку
    // ============
    public class LeverControl : Control
    {
        private float angle = 0f;
        private float maxAngle = 30f; // Максимальний “нахил”

        // Лічильник кроків у анімації
        private int animationStep = 0;
        // Таймер для анімації
        private Timer animationTimer;

        // Ця подія спрацьовує, коли ручку “потягнули і відпустили”
        public event EventHandler LeverPulled;

        public LeverControl()
        {
            this.DoubleBuffered = true;
            this.Size = new Size(60, 200);

            // Ініціалізуємо таймер і підписуємося на його Tick
            animationTimer = new Timer();
            animationTimer.Interval = 30; // швидкість анімації (30 мс на крок)
            animationTimer.Tick += AnimationTimer_Tick;
        }

        /// <summary>
        /// Кут, який відображає, наскільки "сильно" важіль нахилений
        /// 0 = стоїть прямо
        /// maxAngle = максимально "в глибину"
        /// </summary>
        public float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                if (angle < 0) angle = 0;
                if (angle > maxAngle) angle = maxAngle;
                Invalidate(); // Перемалювати контроль
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 1) Вибираємо pivot внизу контролю (де шарнір ручки):
            float pivotX = this.Width / 2f;
            float pivotY = this.Height - 10f;

            // Зміщаємо (0;0) у pivot:
            e.Graphics.TranslateTransform(pivotX, pivotY);

            // ------------------------
            // 1. Розрахунок "фейкової перспективи"
            // ------------------------
            float perspectiveFactor = (float)Math.Sin(Math.PI * angle / (2.0 * maxAngle));
            // Можна іншу формулу, наприклад (angle / maxAngle), залежно від бажаного ефекту

            // ------------------------
            // 2. Малюємо "стержень" (трапецією)
            // ------------------------
            int rodHeight = 100;     // Довжина ручки
            int bottomWidth = 20;    // Ширина знизу
            int topWidth = 20;       // Ширина зверху без нахилу

            // Зменшуємо верхню ширину пропорційно perspectiveFactor
            float currentTopWidth = topWidth * (1f - 0.5f * perspectiveFactor);

            // Координати 4 кутів трапеції:
            // Низ: ( -bottomWidth/2, 0 ) .. ( +bottomWidth/2, 0 )
            // Верх: ( -currentTopWidth/2, -rodHeight ) .. ( +currentTopWidth/2, -rodHeight )
            PointF p1 = new PointF(-bottomWidth / 2f, 0);
            PointF p2 = new PointF(+bottomWidth / 2f, 0);
            PointF p3 = new PointF(+currentTopWidth / 2f, -rodHeight);
            PointF p4 = new PointF(-currentTopWidth / 2f, -rodHeight);

            using (GraphicsPath rodPath = new GraphicsPath())
            {
                rodPath.AddPolygon(new PointF[] { p1, p2, p3, p4 });

                using (PathGradientBrush pgb = new PathGradientBrush(rodPath))
                {
                    pgb.CenterColor = Color.Silver;
                    pgb.SurroundColors = new Color[] { Color.Gray };
                    e.Graphics.FillPath(pgb, rodPath);
                }

                e.Graphics.DrawPath(Pens.Black, rodPath);
            }

            // ------------------------
            // 3. Малюємо "кулю" (knob) зверху
            // ------------------------
            int knobRadius = 20; // Початковий радіус
            float currentKnobRadius = knobRadius * (1f - 0.5f * perspectiveFactor);

            float knobCenterY = -rodHeight;
            float knobCenterX = 0f;

            // Описуємо еліпс
            float knobRectX = knobCenterX - currentKnobRadius;
            float knobRectY = knobCenterY - currentKnobRadius;
            float knobDiameter = currentKnobRadius * 2;

            RectangleF knobRect = new RectangleF(knobRectX, knobRectY, knobDiameter, knobDiameter);

            using (GraphicsPath knobPath = new GraphicsPath())
            {
                knobPath.AddEllipse(knobRect);

                using (PathGradientBrush pgb = new PathGradientBrush(knobPath))
                {
                    pgb.CenterColor = Color.Red;
                    pgb.SurroundColors = new Color[] { Color.DarkRed };
                    e.Graphics.FillEllipse(pgb, knobRect);
                }
            }

            e.Graphics.DrawEllipse(Pens.Black, knobRect);

            // Скидаємо трансформацію
            e.Graphics.ResetTransform();
        }

        // Коли клацаємо по контролу — запускаємо анімацію
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            // Починаємо анімацію: обнуляємо кроки й стартуємо timer
            animationStep = 0;
            animationTimer.Start();
        }

        // Анімація в 20 кроків: 10 кроків "вниз" + 10 кроків "вгору"
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            int totalSteps = 20; // Загальна кількість кроків (10 + 10)
            float localMaxAngle = maxAngle; // Можна взяти змінну maxAngle з поля

            if (animationStep <= 10)
            {
                // Йдемо "вниз" від 0° до localMaxAngle
                Angle = (localMaxAngle / 10f) * animationStep;
            }
            else
            {
                // Йдемо "вгору" від localMaxAngle до 0°
                float stepsDown = animationStep - 10;
                Angle = localMaxAngle - (localMaxAngle / 10f) * stepsDown;
            }

            animationStep++;

            if (animationStep > totalSteps)
            {
                // Завершили анімацію — стоп
                animationTimer.Stop();
                Angle = 0f; // Повертаємо вихідне положення (можна й залишити будь-який)

                // Сигналізуємо, що ручку “потягнули й відпустили”
                LeverPulled?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
