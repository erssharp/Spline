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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Polyline pl = new Polyline();
        static Polyline plsin = new Polyline();
        static Rectangle rect = new Rectangle();

        public MainWindow()
        {
            pl.Stroke = Brushes.Black;
            pl.StrokeThickness = 2;
            pl.Opacity = 0.8;
            plsin.Stroke = Brushes.Red;
            plsin.StrokeThickness = 2;
            rect.Width = Math.PI * 200 + 50;
            rect.Height = 250;
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.Fill = Brushes.LightGray;
            rect.Stroke = Brushes.Black;
            rect.Margin = new Thickness(25, 75, 0, 0);
            InitializeComponent();
            grid.Children.Add(rect);
            grid.Children.Add(plsin);
            grid.Children.Add(pl);
            hght = Height;
            wd = Width;
            DrawSin();
        }

        static double hght = 0;
        static double wd = 0;

        static double ao = 1;
        static double bo = 0;

        static double cn = 0;
        static double an = 1;

        static double d_o = 0;
        static double d_n = 0;

        static double lambdao = -(bo / ao);
        static double muo = d_o / ao;

        static private double GetH(int N)
        {
            return Math.PI / N;
        }

        static private void DrawSin()
        {
            for (double i = 0; i < Math.PI; i += 0.01)
                plsin.Points.Add(new Point(i * 200 + 50, 300 - Math.Sin(i) * 200));
        }

        List<Ellipse> ell = new List<Ellipse>();
        Stack<double> dm = new Stack<double>();
        Stack<double> ks = new Stack<double>();

        private double Spline(int N)
        {
            double h = GetH(N);
            double c = h / 6;
            double a = h * 2 / 3;
            double b = h / 6;

            List<double> ai = new List<double>();
            List<double> ci = new List<double>();
            List<double> bi = new List<double>();
            List<double> di = new List<double>();
            List<double> xi = new List<double>();
            List<double> yi = new List<double>();

            ai.Add(ao);
            ci.Add(0);
            bi.Add(bo);
            xi.Add(0);
            yi.Add(0);
            di.Add(d_o);

            for (int i = 1; i < N; i++)
            {
                double x = h * i;
                xi.Add(x);
                double y = Math.Sin(x);
                yi.Add(y);
                double y1 = Math.Sin(x + h);
                double d = ((y1 - y) - (y - yi[i - 1])) / h;
                di.Add(d);
                ai.Add(a);
                bi.Add(b);
                ci.Add(c);
            }

            ci.Add(cn);
            bi.Add(0);
            ai.Add(an);
            xi.Add(h * N);
            yi.Add(0);
            di.Add(d_n);

            List<double> lambdai = new List<double>();
            List<double> mui = new List<double>();
            lambdai.Add(lambdao);
            mui.Add(muo);

            for (int i = 1; i <= N; i++)
            {
                double lambda = -bi[i] / (ai[i] + ci[i] * lambdai[i - 1]);
                double mu = (di[i] - ci[i] * mui[i - 1]) / (ai[i] + ci[i] * lambdai[i - 1]);
                lambdai.Add(lambda);
                mui.Add(mu);
            }

            double mn = mui.Last();
            List<double> mi = new List<double>();

            for (int i = 0; i < N; i++)
                mi.Add(0);

            mi.Add(mn);

            for (int i = N - 1; i >= 0; i--)
            {
                double m = lambdai[i] * mi[i + 1] + mui[i];
                mi[i] = m;
            }

            List<double> Si = new List<double>();
            Si.Add(0);
            pl.Points.Add(new Point(xi[0] * 200 + 50, 300));
            List<double> xx = new List<double>();
            xx.Add(0);

            for (int i = 1; i <= N; i++)
            {
                double xh = h * i;
                double xp = xi[i - 1];
                double x = xh - h / 2;
                double A = ((Math.Pow((xi[i] - x), 3) - h * h * (xi[i] - x)) / (h * 6)) * mi[i - 1];
                double B = ((Math.Pow((x - xi[i - 1]), 3) - h * h * (x - xi[i - 1])) / (h * 6)) * mi[i];
                double C = ((xi[i] - x) / h) * yi[i - 1];
                double D = ((x - xi[i - 1]) / h) * yi[i];
                double S = A + B + C + D;
                xx.Add(xh - h / 2);

                if (N > 320)
                    pl.Points.Add(new Point(x * 200 + 50, 300 - S * 200));

                Si.Add(S);
            }

            //for (int i = 0; i < N; i++)
            //{
            //    double xh = h * i;
            //    double x = xh;

            //    double A = ((Math.Pow((xi[i + 1] - x), 2) * (x - xi[i])) / (h * h)) * mi[i];
            //    double B = ((Math.Pow((x - xi[i]), 2) * (x - xi[i + 1])) / (h * h)) * mi[i + 1];
            //    double C = ((Math.Pow((xi[i + 1] - x), 2) * (2 * (x - xi[i]) + h)) / Math.Pow(h, 3)) * yi[i];
            //    double D = ((Math.Pow((x - xi[i]), 2) * (2 * (xi[i + 1] - x) + h)) / Math.Pow(h, 3)) * yi[i + 1];
            //    double S = A + B + C + D;
            //    //pl.Points.Add(new Point(x * 200 + 50, 300 - S * 200));

            //    //Ellipse e = new Ellipse();
            //    //e.HorizontalAlignment = HorizontalAlignment.Left;
            //    //e.VerticalAlignment = VerticalAlignment.Top;
            //    //e.Stroke = Brushes.Black;
            //    //e.Fill = Brushes.Black;
            //    //e.Width = 4;
            //    //e.Height = 4;
            //    //e.Margin = new Thickness(x * 200 + 48, 298 - S * 200, 0, 0);
            //    //ell.Add(e);
            //    //grid.Children.Add(e);

            //    x = xh + h / 2;
            //    A = ((Math.Pow((xi[i + 1] - x), 2) * (x - xi[i])) / (h * h)) * mi[i];
            //    B = ((Math.Pow((x - xi[i]), 2) * (x - xi[i + 1])) / (h * h)) * mi[i + 1];
            //    C = ((Math.Pow((xi[i + 1] - x), 2) * (2 * (x - xi[i]) + h)) / Math.Pow(h, 3)) * yi[i];
            //    D = ((Math.Pow((x - xi[i]), 2) * (2 * (xi[i + 1] - x) + h)) / Math.Pow(h, 3)) * yi[i + 1];
            //    S = A + B + C + D;
            //    xx.Add(x);
            //    //pl.Points.Add(new Point(x * 200 + 50, 300 - S * 200));
            //    Si.Add(S);
            //}

            for (int i = 0; i < N; i++)
            {
                double xh = h * i;
                double x = xh;
                if (N <= 320)
                    for (double j = xh; j < xh + h; j += h / 30)
                    {
                        x += h / 30;
                        double A = ((Math.Pow((xi[i + 1] - x), 2) * (x - xi[i])) / (h * h)) * mi[i];
                        double B = ((Math.Pow((x - xi[i]), 2) * (x - xi[i + 1])) / (h * h)) * mi[i + 1];
                        double C = ((Math.Pow((xi[i + 1] - x), 2) * (2 * (x - xi[i]) + h)) / Math.Pow(h, 3)) * yi[i];
                        double D = ((Math.Pow((x - xi[i]), 2) * (2 * (xi[i + 1] - x) + h)) / Math.Pow(h, 3)) * yi[i + 1];
                        double S = A + B + C + D;
                        pl.Points.Add(new Point(x * 200 + 50, 300 - S * 200));
                    }
            }

            Si.Add(0);
            xx.Add(0);
            if (N > 320)
                pl.Points.Add(new Point(xi.Last() * 200 + 50, 300 - Si.Last() * 200));

            double Dmax = 0;

            for (int i = 0; i <= N + 1; i++)
            {
                double y = Math.Sin(xx[i]);
                double Dcur = Math.Abs(Si[i] - y);
                if (Dcur > Dmax)
                    Dmax = Dcur;
            }
            return Dmax;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double kd = 0;
            pl.Points.Clear();
            int N = int.Parse(n.Text);
            double Dmax = Spline(N);

            if (dm.Count != 0)
            {
                kd = dm.Peek() / Dmax;
                ks.Push(kd);
            }

            double doc = 0;

            if (dm.Count != 0)
                doc = dm.Peek() / Math.Pow(2, 4);

            dm.Push(Dmax);

            if (kd == 0 && doc == 0)
                table.Items.Add(string.Format("N = {1}, Dmax = {0}, Doc = -, Kd = -", Dmax, N));
            else
                table.Items.Add(string.Format("N = {3}, Dmax = {0}, Doc = {1}, Kd = {2}", Dmax, doc, kd, N));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            table.Height += Height - hght;
            table.Width += Width - wd;
            hght = Height;
            wd = Width;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            table.Items.Clear();
            dm.Clear();
        }

    }
}
