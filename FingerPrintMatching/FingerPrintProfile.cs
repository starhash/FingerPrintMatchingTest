using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FingerPrintMatching
{
    public class FingerPrintProfile
    {
        public List<FingerPrintNode> Points { get; set; }
        public Bitmap DirectionalField { get; set; }

        public enum Direction { N = 1, S = 2, E = 4, W = 8 }
        public struct FPoint
        { 
            public int X, Y;
            public Direction D;
            public FPoint(int x, int y, Direction dir)
            {
                X = x;
                Y = y;
                D = dir;
            }
            public FPoint N { get { return new FPoint(X, Y - 1, Direction.N); } }
            public FPoint S { get { return new FPoint(X, Y + 1, Direction.S); } }
            public FPoint W { get { return new FPoint(X - 1, Y, Direction.W); } }
            public FPoint E { get { return new FPoint(X + 1, Y, Direction.E); } }
            public FPoint NW { get { return new FPoint(X - 1, Y - 1, Direction.N); } }
            public FPoint NE { get { return new FPoint(X + 1, Y - 1, Direction.N); } }
            public FPoint SW { get { return new FPoint(X - 1, Y + 1, Direction.S); } }
            public FPoint SE { get { return new FPoint(X + 1, Y + 1, Direction.S); } }
        }
        public class DirectedPointList
        {
            public List<Point> List { get; set; }
            public Direction Direction { get; set; }

            public DirectedPointList(Direction d)
            {
                List = new List<Point>();
                Direction = d;
            }

            public DirectedPointList(List<Point> list, Direction d)
            {
                List = new List<Point>(list);
                Direction = d;
            }

            public void Add(Point p) { List.Add(p); }
        }
        public class DirectedList
        {
            public List<int> List { get; set; }
            public Direction Direction { get; set; }

            public DirectedList(Direction d)
            {
                List = new List<int>();
                Direction = d;
            }

            public DirectedList(List<int> list, Direction d)
            {
                List = new List<int>(list);
                Direction = d;
            }

            public void Add(int p) { List.Add(p); }
        }

        public FingerPrintProfile()
        {
            Points = new List<FingerPrintNode>();
        }

        public static FingerPrintProfile FromImage(Bitmap input)
        {
            FingerPrintProfile profile = new FingerPrintProfile();
            profile.DirectionalField = new Bitmap(input.Width, input.Height);
            Graphics g = Graphics.FromImage(profile.DirectionalField);
            int ci = 0, cj = 0, dw = 32, dh = 32;
            for (int i = 0; i < input.Width; i += 32)
            {
                for (int j = 0; j < input.Height; j += 32)
                {
                    if (ci + 32 > input.Width) dw = input.Width - i * 32;
                    if (cj + 32 > input.Height) dh = input.Height - j * 32;
                    Rectangle localRegion = new Rectangle(i, j, dw, dh);
                    Bitmap temp = new Crop(localRegion).Apply(input);
                    List<DirectedPointList> drs = GetDirectedLists(temp);
                    //List<DirectedList> dls = drs.Select((x) => FromDirectedPointLists(x)).ToList();
                    foreach(DirectedPointList d in drs)
                    {
                        Point f = d.List.First();
                        Point l = d.List.Last();
                        double m = (f.Y - l.Y) * 1.0 / (f.X - l.Y);
                        double a = Math.Atan(m);
                        Point middle = d.List.ElementAt(d.List.Count / 2);
                        float dd = 3.0f;
                        float sin = dd * (float)Math.Sin(a), cos = dd * (float)Math.Cos(a);
                        g.DrawLine(Pens.Red, new PointF(i + middle.X - cos, j + middle.Y + sin), new PointF(i + middle.X + cos, j + middle.Y - sin));
                    }
                }
            }
            ShowImage(profile.DirectionalField);
            return profile;
        }

        private static bool ColorEquals(Color a, Color b)
        {
            return a.A == b.A && a.B == b.B && a.G == b.G && a.R == b.R;
        }
        
        private static List<DirectedPointList> GetDirectedLists(Bitmap image)
        {
            image = new Bitmap(image);
            Color black = Color.Black;
            Color white = Color.White;
            List<DirectedPointList> directedlists = new List<DirectedPointList>();
            #region East
            for (int i = 1; i < image.Height; i++)
            {
                DirectedPointList t = new DirectedPointList(Direction.E);
                Color f = image.GetPixel(0, i);
                if (ColorEquals(f, black))
                {
                    FPoint tmp = new FPoint(0, i, Direction.E);
                    image.SetPixel(tmp.X, tmp.Y, Color.Green);
                    t.Add(new Point(0, i));
                    FPoint pixel = GetNext(image, tmp, black, white);
                    while (!(pixel.X == tmp.X && pixel.Y == tmp.Y))
                    {
                        if (image.GetPixel(pixel.X, pixel.Y) == Color.Green)
                            continue;
                        image.SetPixel(pixel.X, pixel.Y, Color.Green);
                        t.Add(new Point(pixel.X, pixel.Y));
                        tmp = pixel;
                        pixel = GetNext(image, tmp, black, white);
                        if (pixel.X == tmp.X && pixel.Y == tmp.Y)
                        {
                            Adjacency8 adj = GetAdjacency8(image, pixel);
                            pixel = adj.GetAny(black, pixel);
                        }
                    }
                }
                if (t.List.Count > 8)
                    directedlists.Add(t);
            }
            #endregion
            #region West
            for (int i = image.Height - 1; i > 0; i--)
            {
                DirectedPointList t = new DirectedPointList(Direction.W);
                Color f = image.GetPixel(image.Width - 1, i);
                if (ColorEquals(f, black))
                {
                    FPoint tmp = new FPoint(image.Width - 1, i, Direction.W);
                    image.SetPixel(tmp.X, tmp.Y, Color.Blue);
                    t.Add(new Point(image.Width - 1, i));
                    FPoint pixel = GetNext(image, tmp, black, white);
                    while (!(pixel.X == tmp.X && pixel.Y == tmp.Y))
                    {
                        if (image.GetPixel(pixel.X, pixel.Y) == Color.Blue)
                            continue;
                        image.SetPixel(pixel.X, pixel.Y, Color.Blue);
                        t.Add(new Point(pixel.X, pixel.Y));
                        tmp = pixel;
                        pixel = GetNext(image, tmp, black, white);
                        if (pixel.X == tmp.X && pixel.Y == tmp.Y)
                        {
                            Adjacency8 adj = GetAdjacency8(image, pixel);
                            pixel = adj.GetAny(black, pixel);
                        }
                    }
                }
                if (t.List.Count > 8)
                    directedlists.Add(t);
            }
            #endregion
            #region South
            for (int i = 1; i < image.Width - 1; i++)
            {
                DirectedPointList t = new DirectedPointList(Direction.S);
                Color f = image.GetPixel(i, 0);
                if (ColorEquals(f, black) && !ColorEquals(f, Color.Red))
                {
                    FPoint tmp = new FPoint(i, 0, Direction.S);
                    image.SetPixel(tmp.X, tmp.Y, Color.Red);
                    t.Add(new Point(i, 0));
                    FPoint pixel = GetNext(image, tmp, black, white);
                    while (!(pixel.X == tmp.X && pixel.Y == tmp.Y))
                    {
                        if (image.GetPixel(pixel.X, pixel.Y) == Color.Red)
                            continue;
                        image.SetPixel(pixel.X, pixel.Y, Color.Red);
                        t.Add(new Point(pixel.X, pixel.Y));
                        tmp = pixel;
                        pixel = GetNext(image, tmp, black, white);
                        if (pixel.X == tmp.X && pixel.Y == tmp.Y)
                        {
                            Adjacency8 adj = GetAdjacency8(image, pixel);
                            pixel = adj.GetAny(black, pixel);
                        }
                    }
                }
                if (t.List.Count > 8)
                    directedlists.Add(t);
            }
            #endregion
            #region North
            Stack<FPoint> stackn = new Stack<FPoint>();
            for (int i = 1; i < image.Width - 1; i++)
            {
                DirectedPointList t = new DirectedPointList(Direction.N);
                Color f = image.GetPixel(i, image.Height - 1);
                if (ColorEquals(f, black) && !ColorEquals(f, Color.Yellow))
                {
                    FPoint tmp = new FPoint(i, image.Height - 1, Direction.N);
                    image.SetPixel(tmp.X, tmp.Y, Color.Yellow);
                    t.Add(new Point(i, image.Height - 1));
                    FPoint pixel = GetNext(image, tmp, black, white);
                    while (!(pixel.X == tmp.X && pixel.Y == tmp.Y))
                    {
                        if (image.GetPixel(pixel.X, pixel.Y) == Color.Yellow)
                            continue;
                        image.SetPixel(pixel.X, pixel.Y, Color.Yellow);
                        tmp = pixel;
                        t.Add(new Point(pixel.X, pixel.Y));
                        pixel = GetNext(image, tmp, black, white);
                        if (pixel.X == tmp.X && pixel.Y == tmp.Y)
                        {
                            Adjacency8 adj = GetAdjacency8(image, pixel);
                            pixel = adj.GetAny(black, pixel);
                        }
                    }
                }
                if (t.List.Count > 8)
                    directedlists.Add(t);
            }
            #endregion
            //ShowImage(image);
            return directedlists;
        }

        private static DirectedList FromDirectedPointLists(DirectedPointList list)
        {
            DirectedList dl = new DirectedList(list.Direction);
            dl.List.AddRange(list.List.Select((x) => x.X * 32 + x.Y));
            return dl;
        }

        public static void ShowImage(Bitmap image)
        {
            Form f = new Form();
            PictureBox p = new PictureBox() { Dock = DockStyle.Fill, Image = image, SizeMode = PictureBoxSizeMode.Zoom };
            f.Controls.Add(p);
            f.ShowDialog();
        }

        public struct Adjacency8
        {
            public Color N, S, W, E, NW, NE, SW, SE;
            public Adjacency8(Color n, Color s, Color w, Color e, Color nw, Color ne, Color sw, Color se)
            {
                N = n; S = s; W = w; E = e;
                NW = nw; NE = ne; SW = sw; SE = se;
            }
            public FPoint GetAny(Color color, FPoint p)
            {
                if (ColorEquals(N, color)) { return new FPoint(p.X, p.Y - 1, p.D); }
                if (ColorEquals(S, color)) { return new FPoint(p.X, p.Y + 1, p.D); }
                if (ColorEquals(W, color)) { return new FPoint(p.X - 1, p.Y, p.D); }
                if (ColorEquals(E, color)) { return new FPoint(p.X + 1, p.Y, p.D); }
                if (ColorEquals(NW, color)) { return new FPoint(p.X - 1, p.Y - 1, p.D); }
                if (ColorEquals(NE, color)) { return new FPoint(p.X + 1, p.Y - 1, p.D); }
                if (ColorEquals(SW, color)) { return new FPoint(p.X - 1, p.Y + 1, p.D); }
                if (ColorEquals(SE, color)) { return new FPoint(p.X + 1, p.Y + 1, p.D); }
                return p;
            }
        }

        static Color GetPoint(Bitmap image, int x, int y)
        {
            try
            {
                return image.GetPixel(x, y);
            }
            catch(Exception) { }
            return Color.FromArgb(0, Color.Black);
        }

        static Adjacency8 GetAdjacency8(Bitmap image, FPoint p)
        {
            Adjacency8 adj = new Adjacency8();
            adj.N = GetPoint(image, p.X, p.Y - 1);
            adj.S = GetPoint(image, p.X, p.Y + 1);
            adj.W = GetPoint(image, p.X - 1, p.Y);
            adj.E = GetPoint(image, p.X + 1, p.Y);
            adj.NW = GetPoint(image, p.X - 1, p.Y - 1);
            adj.NE = GetPoint(image, p.X + 1, p.Y - 1);
            adj.SW = GetPoint(image, p.X - 1, p.Y + 1);
            adj.SE = GetPoint(image, p.X + 1, p.Y + 1);
            return adj;
        }

        static FPoint GetNext(Bitmap image, FPoint p, Color fill, Color outer)
        {
            List<FPoint> points = new List<FPoint>();
            if (p.X == image.Width - 1 && p.D == Direction.E) return p;
            else if (p.X == 0 && p.D == Direction.W) return p;
            else if (p.Y == image.Height - 1 && p.D == Direction.S) return p;
            else if (p.Y == 0 && p.D == Direction.N) return p;
            Adjacency8 adj = GetAdjacency8(image, p);
            if (p.D == Direction.E)
            {
                if (ColorEquals(adj.E, fill)) return p.E;
                else if (ColorEquals(adj.NE, fill)) { p = p.NE; p.D = Direction.N; }
                else if (ColorEquals(adj.SE, fill)) { p = p.SE; p.D = Direction.S; }
                else if (ColorEquals(adj.N, fill)) { p = p.N; p.D = Direction.E; }
                else if (ColorEquals(adj.S, fill)) { p = p.S; p.D = Direction.E; }
            }
            else if (p.D == Direction.W)
            {
                if (ColorEquals(adj.W, fill)) return p.W;
                else if (ColorEquals(adj.NW, fill)) { p = p.NW; p.D = Direction.N; }
                else if (ColorEquals(adj.SW, fill)) { p = p.SW; p.D = Direction.S; }
                else if (ColorEquals(adj.N, fill)) { p = p.N; p.D = Direction.W; }
                else if (ColorEquals(adj.S, fill)) { p = p.S; p.D = Direction.W; }
            }
            else if (p.D == Direction.S)
            {
                if (ColorEquals(adj.S, fill)) return p.S;
                else if (ColorEquals(adj.SE, fill)) { p = p.SE; p.D = Direction.E; }
                else if (ColorEquals(adj.SW, fill)) { p = p.SW; p.D = Direction.W; }
                else if (ColorEquals(adj.E, fill)) { p = p.E; p.D = Direction.E; }
                else if (ColorEquals(adj.W, fill)) { p = p.W; p.D = Direction.W; }
            }
            else if (p.D == Direction.N)
            {
                if (ColorEquals(adj.N, fill)) return p.N;
                else if (ColorEquals(adj.NE, fill)) { p = p.NE; p.D = Direction.E; }
                else if (ColorEquals(adj.NW, fill)) { p = p.NW; p.D = Direction.W; }
                else if (ColorEquals(adj.E, fill)) { p = p.E; p.D = Direction.N; }
                else if (ColorEquals(adj.W, fill)) { p = p.W; p.D = Direction.N; }
            }
            return p;
        }

        public class Spot
        {
            public int area;
            public float fullness;
            public Point point;
            public Rectangle rectangle;

            public Spot(int area, Point point, float fullness, Rectangle rectangle)
            {
                this.area = area;
                this.point = point;
                this.fullness = fullness;
                this.rectangle = rectangle;
            }
        }
    }
}
