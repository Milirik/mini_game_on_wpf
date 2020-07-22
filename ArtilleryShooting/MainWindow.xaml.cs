using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArtilleryShooting
{
    public partial class MainWindow : Window
    {

        public Random rnd = new Random();
        DispatcherTimer timer = new DispatcherTimer();
        public int k = 0, v = 10;

        //характеристики пушки
        private double angle = 0.0;//угол между продольной осью пушки и oX в радианах (заданное здесь значение будет начальным)
        //булевые переменные, используемые для управления в ручном режиеме:
        private bool p_up = false;//зажата клавиша вверх
        private bool p_down = false;//зажата клавиша вниз
        private bool shoot = false;//зажата клавиша пробела
        private bool popal = false;

        Double[] coords = new Double[5] { 0, 0, 0, 0, 0 };
        List<Yadro> yadras = new List<Yadro>{};

        //браши для спрайтов
        ImageBrush PyshkaSkin = new ImageBrush();
        ImageBrush MishenSkin = new ImageBrush();
      

        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(Engine);
            timer.Interval = TimeSpan.FromMilliseconds(20);

            PyshkaSkin.ImageSource = new BitmapImage(new Uri("Images/pyshka.jpg", UriKind.Relative));
            Pyshka.Fill = PyshkaSkin;

            MishenSkin.ImageSource = new BitmapImage(new Uri("Images/mishen.jpg", UriKind.Relative));
            Mishen.Fill = MishenSkin;
        }

        private void GameArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
            {
                button.Content = "БАХ";
                CreateYadro();
            }

            if (e.Key == Key.W) {
                p_up = true;
                button.Background = Brushes.LightBlue;
                button.Content = "Поднимаем вверх";
            }
            if (e.Key == Key.S)
            {
                p_down = true;
                button.Background = Brushes.LightGreen;
                button.Content = "Поднимаем вниз";
            }   
        }

        private void GameArea_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F) shoot = false;
            if (e.Key == Key.W) p_up = false;
            if (e.Key == Key.S) p_down = false;
        }

        class Yadro {
            public Rectangle yadr_rect = new Rectangle();
            public double x = 0;
            public double y;
            public double angle;
            public double get_y {
                get {
                    angle -= 0.5;
                    return this.angle / 10;
                }
                set {  }
            }

            public Yadro(double angle) {
                this.angle = angle;

                yadr_rect.Width = 10;
                yadr_rect.Height = 10;
                yadr_rect.Fill = Brushes.Green;
                ImageBrush YadroSkin = new ImageBrush();
                YadroSkin.ImageSource = new BitmapImage(new Uri("Images/yadro.jpg", UriKind.Relative));
                yadr_rect.Fill = YadroSkin;
            }
        }

        private void CreateYadro() {
            Yadro tmp = new Yadro(angle);
            yadras.Add(tmp);
            GameArea.Children.Add(tmp.yadr_rect);
        }

        bool Hit(double mishenx, double misheny, double yadrox, double yadroy, Yadro y)
        {
            bool ret = false;
            if (Math.Sqrt(Math.Pow(mishenx - yadrox, 2.0) + Math.Pow(misheny - yadroy, 2.0)) < Mishen.Width / 2.0 + y.yadr_rect.Width / 2.0)
            {
                ret = true;
            }
            return ret;
        }

        private void Engine(object sender, EventArgs e)
        {
            if (p_up)
            { angle += 1; }
            if (p_down)
            { angle -= 1; }

            Pyshka.RenderTransform = new RotateTransform(-angle);

            if (yadras.Count>0) {
                foreach(Yadro y in yadras)
                {
                    Canvas.SetLeft(y.yadr_rect, y.x);
                    Canvas.SetBottom(y.yadr_rect, y.y);
                    y.x += 5;
                    y.y += y.get_y;
                    if (Hit(Canvas.GetLeft(Mishen), Canvas.GetBottom(Mishen), Canvas.GetLeft(y.yadr_rect), Canvas.GetBottom(y.yadr_rect), y))
                    {
                        Mishen.Stroke = Brushes.Black;
                        Mishen.StrokeThickness = 4;
                        popal = true;
                    }
                }
                List<Yadro> s = yadras.ToList(); 
                foreach (Yadro y in s)
                {
                    if (y.x > 1010 || y.y > 610)
                    {
                        yadras.Remove(y);
                    }
                }
            }
            if (popal) {
                Mishen.Stroke = Brushes.Black;
                Mishen.StrokeThickness = 0;
                Canvas.SetLeft(Mishen, this.rnd.Next(300, 800));
                Canvas.SetBottom(Mishen, this.rnd.Next(100, 500));
                popal = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            button.Content = "В АТАКУ!";
        }
    }
}
