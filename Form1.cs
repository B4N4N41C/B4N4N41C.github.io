using System.Collections.Specialized;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;
using ZedGraph;

namespace WinImageProcessing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int[,] _maxX;
        int[,] _maxY;
        byte[,] img;
        Bitmap bmp;
        OpenFileDialog dialog = new OpenFileDialog();

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(dialog.FileName);

                bmp = new Bitmap(dialog.FileName);

                img = new byte[bmp.Height, bmp.Width];
                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        Color c = bmp.GetPixel(j, i);
                        img[i, j] = (byte)((c.R + c.G + c.B) / 3);
                    }
                }
                trackBar1.Maximum = bmp.Height - 1;
                trackBar2.Maximum = bmp.Width - 1;

                _maxX = new int[bmp.Width, bmp.Height];
                _maxY = new int[bmp.Height, bmp.Width];
            }
            trackBar3.Minimum = 1;
            trackBar3.Maximum = 1000;
            trackBar4.Maximum = 255;
            trackBar5.Maximum = 200;
            int sum = 0;
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    sum += img[i, j];
                }
            }
            int sr = sum / (bmp.Width * bmp.Height);
            label5.Text = "Средний порог: " + sr.ToString();
        }

        private void построитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphPane pane = zedGraphControl1.GraphPane;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();

            // Создадим список точек
            PointPairList list = new PointPairList();

            double xmin = -50;
            double xmax = 50;

            // Заполняем список точек
            for (double x = xmin; x <= xmax; x += 0.01)
            {
                // добавим в список точку
                list.Add(x, Math.Sin(x));
            }

            // Создадим кривую с названием "Sinc",
            // которая будет рисоваться голубым цветом (Color.Blue),
            // Опорные точки выделяться не будут (SymbolType.None)
            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.None);

            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            // В противном случае на рисунке будет показана только часть графика,
            // которая умещается в интервалы по осям, установленные по умолчанию
            zedGraphControl1.AxisChange();

            // Обновляем график
            zedGraphControl1.Invalidate();
        }

        private void показать50СтрочкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphPane pane = zedGraphControl1.GraphPane;

            pane.CurveList.Clear();

            PointPairList list = new PointPairList();
            PointPairList maxY = new PointPairList();

            for (int x = 0; x < img.GetLength(1); x++)
                list.Add(x, img[trackBar1.Value, x]);

            for (int x = 0; x < img.GetLength(1); x++)
                if (_maxY[trackBar1.Value, x] > 0)
                    maxY.Add(x, _maxY[trackBar1.Value, x]);

            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.None);
            LineItem myCurveMaxY = pane.AddCurve("Max X", maxY, Color.Blue, SymbolType.Diamond);

            myCurveMaxY.Line.IsVisible = false;

            myCurveMaxY.Symbol.Fill.Color = Color.Blue;

            myCurveMaxY.Symbol.Fill.Type = FillType.Solid;

            myCurveMaxY.Symbol.Size = 7;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = String.Format("Текущее значение строки: {0}", trackBar1.Value);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = String.Format("Текущее значение столца: {0}", trackBar2.Value);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            GraphPane pane = zedGraphControl1.GraphPane;

            pane.CurveList.Clear();

            PointPairList list = new PointPairList();
            PointPairList maxX = new PointPairList();

            for (int x = 0; x < img.GetLength(0); x++)
                list.Add(x, img[x, trackBar2.Value]);

            for (int x = 0; x < img.GetLength(0); x++)
                if (_maxX[trackBar2.Value, x] > 0)
                    maxX.Add(x, _maxX[trackBar2.Value, x]);


            LineItem myCurve = pane.AddCurve("Sinc", list, Color.DarkRed, SymbolType.None);
            LineItem myCurveMaxY = pane.AddCurve("Max X", maxX, Color.DarkRed, SymbolType.Diamond);

            myCurveMaxY.Line.IsVisible = false;

            myCurveMaxY.Symbol.Fill.Color = Color.DarkRed;

            myCurveMaxY.Symbol.Fill.Type = FillType.Solid;

            myCurveMaxY.Symbol.Size = 7;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            GraphPane pane = zedGraphControl1.GraphPane;

            pane.CurveList.Clear();

            PointPairList listX = new PointPairList();
            PointPairList listY = new PointPairList();
            PointPairList maxX = new PointPairList();
            PointPairList maxY = new PointPairList();

            for (int x = 0; x < img.GetLength(0); x++)
                listX.Add(x, img[x, trackBar2.Value]);

            for (int x = 0; x < img.GetLength(1); x++)
                listY.Add(x, img[trackBar1.Value, x]);

            for (int x = 0; x < img.GetLength(0); x++)
                if (_maxX[trackBar2.Value, x] > 0)
                    maxX.Add(x, _maxX[trackBar2.Value, x]);


            for (int x = 0; x < img.GetLength(1); x++)
                if (_maxY[trackBar1.Value, x] > 0)
                    maxY.Add(x, _maxY[trackBar1.Value, x]);


            LineItem myCurveX = pane.AddCurve("X", listX, Color.DarkRed, SymbolType.None);
            LineItem myCurveY = pane.AddCurve("Y", listY, Color.Blue, SymbolType.None);
            LineItem myCurveMaxX = pane.AddCurve("Max X", maxX, Color.DarkRed, SymbolType.Diamond);
            LineItem myCurveMaxY = pane.AddCurve("Max Y", maxY, Color.Blue, SymbolType.Diamond);

            myCurveMaxX.Line.IsVisible = false;
            myCurveMaxY.Line.IsVisible = false;

            myCurveMaxX.Symbol.Fill.Color = Color.DarkRed;
            myCurveMaxY.Symbol.Fill.Color = Color.Blue;

            myCurveMaxX.Symbol.Fill.Type = FillType.Solid;
            myCurveMaxY.Symbol.Fill.Type = FillType.Solid;

            myCurveMaxX.Symbol.Size = 7;
            myCurveMaxY.Symbol.Size = 7;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label3.Text = String.Format("Шаг : {0}", trackBar3.Value);
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            label4.Text = String.Format("Порог: {0}", trackBar4.Value);
        }

        private void найтиМаксимумыToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int step = trackBar3.Value;
            int threshold = trackBar4.Value;

            //Максимальное значение по строке
            for (int i = 0; i < bmp.Width; i++)
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = bmp.Width;
                for (int j = step; j < bmp.Height; j = j + step)
                {
                    int indexMaxX = 0;
                    int indexMaxY = 0;
                    int max = 0;
                    for (int k = j - step; k <= j; k++)
                    {
                        if (max < img[k, i] && img[k, i] > threshold)
                        {
                            max = img[k, i];
                            indexMaxX = k;
                            indexMaxY = i;
                        }
                    }
                    for (int k = j - step; k <= j; k++)
                    {
                        _maxY[k, i] = 0;
                    }
                    _maxY[indexMaxX, indexMaxY] = max;
                }
                progressBar1.Value = i;
            }

            //Максимальное значение по столбцу
            for (int x = 0; x < bmp.Width - 1; x++)
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = bmp.Width;
                for (int j = step; j < bmp.Height; j = j + step)
                {
                    int indexMaxX = 0;
                    int indexMaxY = 0;
                    int max = 0;
                    for (int k = j - step; k < j; k++)
                    {
                        if (max < img[k, x] && img[k, x] > threshold)
                        {
                            max = img[k, x];
                            indexMaxX = x;
                            indexMaxY = k;
                        }
                    }
                    for (int k = j - step; k <= j; k++)
                    {
                        _maxX[x, k] = 0;
                    }
                    _maxX[indexMaxX, indexMaxY] = max;
                }
                progressBar1.Value = x;
            }
        }

        private void сохранитьРезультатToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                pictureBox1.Image.Save(saveFileDialog.FileName, ImageFormat.Bmp);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Bitmap blackAndWitheBmp = (Bitmap)pictureBox1.Image;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (_maxY[i, j] > 0)
                        blackAndWitheBmp.SetPixel(j, i, Color.White);
                    else
                        blackAndWitheBmp.SetPixel(j, i, Color.Black);
                }
            }

            pictureBox1.Image = blackAndWitheBmp;
            pictureBox1.Refresh();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Bitmap blackAndWitheBmp = (Bitmap)pictureBox1.Image;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (_maxX[j, i] > 0)
                        blackAndWitheBmp.SetPixel(j, i, Color.White);
                    else
                        blackAndWitheBmp.SetPixel(j, i, Color.Black);
                }
            }

            pictureBox1.Image = blackAndWitheBmp;
            pictureBox1.Refresh();
        }

        private void поСтрокеИСтолбцуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap blackAndWitheBmp = (Bitmap)pictureBox1.Image;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (_maxX[j, i] > 0)
                        blackAndWitheBmp.SetPixel(j, i, Color.White);
                    else
                        blackAndWitheBmp.SetPixel(j, i, Color.Black);
                }
            }

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (_maxY[i, j] > 0)
                        blackAndWitheBmp.SetPixel(j, i, Color.White);
                }
            }

            pictureBox1.Image = blackAndWitheBmp;
            pictureBox1.Refresh();
        }

        private void убратьЧёрныеТочкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap blackAndWitheBmp = (Bitmap)pictureBox1.Image;

            int size = trackBar5.Value;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixelColor = blackAndWitheBmp.GetPixel(x, y);
                    if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
                    {
                        int pointSize = GetBlackPointSize(blackAndWitheBmp, x, y);
                        if (pointSize < size)
                        {
                            RemoveBlackPoint(blackAndWitheBmp, x, y);
                        }
                    }
                }
            }
            for (int y = 1; y < bmp.Height - 1; y++)
            {
                for (int x = 1; x < bmp.Width - 1; x++)
                {
                    Color pixelColor = blackAndWitheBmp.GetPixel(x, y);
                    Color pixelColor1 = blackAndWitheBmp.GetPixel(x + 1, y);
                    Color pixelColor2 = blackAndWitheBmp.GetPixel(x - 1, y);
                    Color pixelColor3 = blackAndWitheBmp.GetPixel(x, y + 1);
                    Color pixelColor4 = blackAndWitheBmp.GetPixel(x, y - 1);
                    if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0 &&
                        pixelColor1.R != 0 && pixelColor1.G != 0 && pixelColor1.B != 0 &&
                        pixelColor2.R != 0 && pixelColor2.G != 0 && pixelColor2.B != 0 &&
                        pixelColor3.R != 0 && pixelColor3.G != 0 && pixelColor3.B != 0 &&
                        pixelColor4.R != 0 && pixelColor4.G != 0 && pixelColor4.B != 0 &&
                        x > 1 && y > 1 && x < bmp.Width - 1 && y < bmp.Height - 1)
                    {
                        blackAndWitheBmp.SetPixel(x, y, Color.White);
                    }
                }
            }
            pictureBox1.Image = blackAndWitheBmp;
            pictureBox1.Refresh();
        }

        public static int GetBlackPointSize(Bitmap bmp, int x, int y)
        {
            int size = 1;
            bmp.SetPixel(x, y, Color.White);
            if (x > 0 && bmp.GetPixel(x - 1, y).R == 0)
            {
                size += GetBlackPointSize(bmp, x - 1, y);
            }
            else if (x < bmp.Width - 1 && bmp.GetPixel(x + 1, y).R == 0)
            {
                size += GetBlackPointSize(bmp, x + 1, y);
            }
            else if (y > 0 && bmp.GetPixel(x, y - 1).R == 0)
            {
                size += GetBlackPointSize(bmp, x, y - 1);
            }
            else if (y < bmp.Height - 1 && bmp.GetPixel(x, y + 1).R == 0)
            {
                size += GetBlackPointSize(bmp, x, y + 1);
            }
            bmp.SetPixel(x, y, Color.Black);
            return size;
        }

        public static void RemoveBlackPoint(Bitmap bmp, int x, int y)
        {
            bmp.SetPixel(x, y, Color.White);
            if (x > 0 && bmp.GetPixel(x - 1, y).R == 0)
            {
                RemoveBlackPoint(bmp, x - 1, y);
            }
            else if (x < bmp.Width - 1 && bmp.GetPixel(x + 1, y).R == 0)
            {
                RemoveBlackPoint(bmp, x + 1, y);
            }
            else if (y > 0 && bmp.GetPixel(x, y - 1).R == 0)
            {
                RemoveBlackPoint(bmp, x, y - 1);
            }
            else if (y < bmp.Height - 1 && bmp.GetPixel(x, y + 1).R == 0)
            {
                RemoveBlackPoint(bmp, x, y + 1);
            }
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            label6.Text = String.Format("Размер шума: {0}", trackBar5.Value);
        }

        private void наложитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap blackAndWitheBmp = (Bitmap)pictureBox1.Image;

            bmp = new Bitmap(dialog.FileName);

            img = new byte[bmp.Height, bmp.Width];
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (blackAndWitheBmp.GetPixel(j, i).ToArgb() == Color.Black.ToArgb())
                    {
                        bmp.SetPixel(j, i, Color.Red);
                    }
                }
            }

            pictureBox1.Image = bmp;
            pictureBox1.Refresh();
        }

        private void вычислитьКоличествоПикселейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap blackAndWitheBmp = (Bitmap)pictureBox1.Image;

            int totalSize = 0;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (blackAndWitheBmp.GetPixel(j, i).ToArgb() == Color.Black.ToArgb())
                    {
                        totalSize++;
                    }
                }
            }
            SpotCounting();
            label7.Text = $"Количество пикселей: {totalSize}";
        }
        private void SpotCounting()
        {
            Bitmap blackAndWitheBmp = (Bitmap)pictureBox1.Image;
            int countSpot = 1;
            int[,] imgBlackAndWhite = new int[bmp.Height, bmp.Width];

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color c = blackAndWitheBmp.GetPixel(j, i);
                    imgBlackAndWhite[i, j] = (byte)(c.R + c.G + c.B);
                }
            }
            for (int i = 0; i < bmp.Height; i++)//i = y, j = x
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (imgBlackAndWhite[i, j] == 0)
                    {
                        if (GetBlackSpotSize(imgBlackAndWhite, countSpot, i, j, bmp.Height - 2, bmp.Width - 2) > 120)
                        { 
                            EditBlackSpot(imgBlackAndWhite, countSpot, i, j, bmp.Height - 2, bmp.Width - 2);
                            countSpot++;
                        }
                    }
                }
            }
            label8.Text = "Количество дефектов: " + countSpot; 
        }

        public static int GetBlackSpotSize(int[,] img, int count, int i, int j, int maxi, int maxj)
        {
            int size = 1;
            img[i, j] = count;
            if (i < maxi && img[i + 1, j] == 0)
            {
                size += GetBlackSpotSize(img, count, i + 1, j, maxi, maxj);
            }
            else if (j < maxj && img[i, j + 1] == 0)
            {
                size += GetBlackSpotSize(img, count, i, j + 1, maxi, maxj);
            }
            else if (i > 0 && img[i - 1, j] == 0)
            {
                size += GetBlackSpotSize(img, count, i - 1, j, maxi, maxj);
            }
            else if (j > 0 && img[i, j - 1] == 0)
            {
                size += GetBlackSpotSize(img, count, i, j - 1, maxi, maxj);
            }
            img[i, j] = 0;
            return size;
        }

        public static void EditBlackSpot(int[,] img, int count, int i, int j, int maxi, int maxj)
        {
            img[i, j] = count;

            if (i < maxi && img[i + 1, j] == 0)
            {
                EditBlackSpot(img, count, i + 1, j, maxi, maxj);
            }
            else if (j < maxj && img[i, j + 1] == 0)
            {
                EditBlackSpot(img, count, i, j + 1, maxi, maxj);
            }
            else if (i > 0 && img[i - 1, j] == 0)
            {
                EditBlackSpot(img, count, i - 1, j, maxi, maxj);
            }
            else if (j > 0 && img[i, j - 1] == 0)
            {
                EditBlackSpot(img, count, i, j - 1, maxi, maxj);
            }
        }
    }
}