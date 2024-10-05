using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proga_5_1b_sch_3sem
{
    public partial class Form1 : Form
    {
        // Изображение, загруженное из файла
        private Image image;
        // Флаг для выделения области на изображении
        private bool rubberBand;
        // Начальная точка выделения
        private Point startPoint;
        // Конечная точка выделения
        private Point endPoint;
        // Вырезанное изображение
        private Image croppedImage;
        // Угол поворота изображения
        private int rotationAngle;
        // Точка копирования изображения
        private Point copyPoint;
        // Флаг для копирования изображения
        private bool isCopying;

        public Form1()
        {
            InitializeComponent();

            // Загрузка изображения из файла
            image = Image.FromFile("C:\\Users\\Vladislav\\Documents\\qt proect\\proga_5_1_spl_3sem\\panda.jpeg");

            // Инициализация переменных
            rubberBand = false; // Флаг для выделения области
            startPoint = Point.Empty; // Начальная точка выделения
            endPoint = Point.Empty; // Конечная точка выделения
            croppedImage = null; // Вырезанное изображение
            rotationAngle = 0; // Угол поворота
            copyPoint = new Point(1, 1); // Точка копирования
            isCopying = false; // Флаг для копирования

            // Установка свойства для предотвращения мерцания во время перерисовки
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        // Обработчик события нажатия кнопки мыши
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                if (croppedImage != null)
                {
                    isCopying = true;
                    copyPoint = e.Location;
                }
                else
                {
                    rubberBand = true;
                    startPoint = e.Location;
                }
            }
        }

        // Обработчик события движения мыши
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (rubberBand)
            {
                endPoint = e.Location;
                Invalidate();
            }
            else if (isCopying)
            {
                copyPoint = e.Location;
                Invalidate();
            }
        }

        // Обработчик события отпускания кнопки мыши
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (rubberBand)
            {
                rubberBand = false;
                Rectangle rect = GetNormalizedRectangle(startPoint, endPoint);
                croppedImage = CropImage(image, rect);
                Invalidate(); // Вызов перерисовки формы
            }
            else if (isCopying)
            {
                isCopying = false;
            }
        }

        // Обработчик события перерисовки формы
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(image, 0, 0); // Рисуем исходное изображение

            base.OnPaint(e);

            if (rubberBand)
            {
                using (Pen pen = new Pen(Color.Red, 2)) // Создаем красный карандаш толщиной 2 пикселя
                {
                    e.Graphics.DrawRectangle(pen, GetNormalizedRectangle(startPoint, endPoint)); // Рисуем прямоугольник выделения
                }
            }

            if (croppedImage != null)
            {
                // Поворачиваем вырезанное изображение на заданный угол
                e.Graphics.TranslateTransform(copyPoint.X, copyPoint.Y);
                e.Graphics.RotateTransform(rotationAngle);
                e.Graphics.TranslateTransform(-copyPoint.X, -copyPoint.Y);

                e.Graphics.DrawImage(croppedImage, copyPoint); // Рисуем вырезанное изображение
            }
        }

        // Установка угла поворота изображения
        public void SetRotationAngle(int angle)
        {
            rotationAngle = angle;
        }

        // Обработчик события нажатия клавиши
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Left:
                    copyPoint.X -= 10;
                    Invalidate();
                    break;
                case Keys.Right:
                    copyPoint.X += 10;
                    Invalidate();
                    break;
                case Keys.Up:
                    copyPoint.Y -= 10;
                    Invalidate();
                    break;
                case Keys.Down:
                    copyPoint.Y += 10;
                    Invalidate();
                    break;
                case Keys.R: // Если нажата клавиша "R", поворачиваем изображение на 90 градусов
                    SetRotationAngle((rotationAngle + 90) % 360);
                    Invalidate(); // Вызов перерисовки формы
                    break;
            }
        }

        // Получение нормализованного прямоугольника выделения
        private Rectangle GetNormalizedRectangle(Point start, Point end)
        {
            int x1 = Math.Max(Math.Min(start.X, end.X), 0);
            int y1 = Math.Max(Math.Min(start.Y, end.Y), 0);
            int x2 = Math.Min(Math.Max(start.X, end.X), image.Width);
            int y2 = Math.Min(Math.Max(start.Y, end.Y), image.Height);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        // Вырезание изображения по прямоугольной области
        private Image CropImage(Image img, Rectangle rect)
{
    Bitmap bmpImage = new Bitmap(img);
    Bitmap croppedBitmap = new Bitmap(rect.Width, rect.Height);

    using (Graphics g = Graphics.FromImage(croppedBitmap))
    {
        g.DrawImage(bmpImage, 0, 0, rect, GraphicsUnit.Pixel);
    }

    return croppedBitmap;
}
    }
}