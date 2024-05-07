using static WinFormsApp1.Program;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            double hue = 0;
            double sat = 0.5;
            double val = 1;

            var wheel = new ColorPicker(400);
            var img = wheel.DrawImage(hue, sat, val);
            using (var g = Graphics.FromImage(img))
            {
                var pen = val < 0.5 ? Pens.White : Pens.Black;

                var wheelPosition = wheel.GetWheelPosition(hue);
                g.DrawEllipse(pen, (float)wheelPosition.X - 5, (float)wheelPosition.Y - 5, 10, 10);

                var trianglePosition = wheel.GetTrianglePosition(sat, val);
                g.DrawEllipse(pen, (float)trianglePosition.X - 5, (float)trianglePosition.Y - 5, 10, 10);
            }

            PictureBox pb1 = new PictureBox();
            pb1.Image = img;
            pb1.SizeMode = PictureBoxSizeMode.AutoSize;

            Controls.Add(pb1);
        }
    }
}