using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace HandwritingGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<string> fontNames = new List<string>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fontNames.Add("云烟体");
            fontNames.Add("李国夫手写体");
            fontNames.Add("青叶手写体");
            fontNames.Add("义启手写体");
            //fontNames.Add("Aa日式标题字");
        }

        OpenFileDialog openFileDialog = new OpenFileDialog()
        {
            Title = "打开纯文本文件",
            Filter = "文本文件|*.txt|所有文件|*.*"
        };
        private void Button_Import(object sender, RoutedEventArgs e)
        {
#if DEBUG

            Build(@"1 smart   2 large   3 clean   4 high   5 active   6 nice   7 fast   8 modern   9 polite   10 ever
11 sometimes   12 city   13 building   14 like   15 look   16 drink   17 idea   18 amazing   19 little
20 coffee   21 rest   22 tea   23 sugar   24 please   25 tired   26 break   27 ready   28 shall
29 milk   30 black   31 house   32 need   33 breakfast   34 other    35 toast  36 else   37 hungry
38 important   39 hurry   40 sunny    41 usually   42 side   43 mouth   44 fry   45 water

1 boat trip   2 very much   3 tell me   4 lots of   5 market tour   6 in the morning   7 a lot of
8 clean and fast   9 how about   10 many people   11 high building   12 sound nice   13 new friend
14 take a rest   15 good idea   16 drink coffee   17 black coffee   18 red sun   19 other side
20 no hurry

1  早餐在加拿大很重要
2 我们通常在早上吃很多食物
3 当你煎蛋时, 你只煎一个面
4 所以另一面像红太阳一样
5 有时我们在早上吃吐司面包
6 我已经去过那里几次了
7 那是个现代大都市
8 它们又干净又快捷, 但有时候太过拥挤
9 这里有许多非常高的建筑
10 它们看起来很不错
11 我们休息一下吧
12 那是个好主意
13 我听说你也非常喜欢喝咖啡
14 我也不喜欢咖啡里的牛奶或糖
15 我只喜欢黑咖啡


");
            return;
#endif

            if (openFileDialog.ShowDialog() ?? false)
            {
                Build(File.ReadAllText(openFileDialog.FileName));
            }


        }

        void Build(string text)
        {
            sp.Document.Blocks.Clear();

            Paragraph p = new Paragraph() { Margin = new Thickness(0) };

            foreach (char c in text)
            {
                switch (c)
                {
                    case '\r':
                        break;
                    case '\n':
                        //p.Inlines.Add(new Run("\n"));
                        sp.Document.Blocks.Add(p);

                        p = new Paragraph() { Margin = new Thickness(0) };
                        p.Inlines.Add(new Run(" "));
                        break;
                    case ' ':
                        p.Inlines.Add(new Run(" "));
                        break;
                    case >= '0' and <= '9':
                        p.Inlines.Add(RandomRun(c.ToString(), "义启手写体"));
                        break;
                    default:
                        p.Inlines.Add(RandomRun(c.ToString()));
                        break;
                }
            }
        }



        StackPanel CreateLinePanel()
        {
            return new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new(RandomTow, RandomTow, RandomTow, RandomTow),
            };
        }

        Run RandomRun(string text, string? fontname = null)
        {
            return new Run()
            {
                Text = text,
                FontFamily =
                    fontname != null ?
                    new FontFamily(fontname) :
                    new FontFamily(fontNames[Random.Shared.Next(0, fontNames.Count)]),
                FontSize = 20 + RandomTow,
                TextEffects =
                {
                    new TextEffect()
                    {
                        Transform = new TranslateTransform()
                        {
                            Y = RandomDouble(-20, 20),
                        }
                    }
                }
            };
        }

        UserControl CreateString(string text, string fontName = null)
        {
            var content = new UserControl()
            {
                Content = text,
                Padding = new Thickness(RandomHalf, RandomHalf, RandomHalf, RandomTow),
                Margin = new Thickness(RandomHalf, RandomHalf, RandomHalf, RandomTow),
                FontSize = 20 + RandomTow,
            };
            if (string.IsNullOrEmpty(fontName))
            {
                content.FontFamily = new FontFamily(fontNames[Random.Shared.Next(0, fontNames.Count)]);
            }
            else
            {
                content.FontFamily = new FontFamily(fontName);
            }
            return content;
        }

        double RandomDouble(double min, double max)
        {
            return Random.Shared.NextDouble() * (max - min) + min;
        }


        double RandomHalf => RandomDouble(0, 0.5);
        double RandomSignedHalf => RandomDouble(-0.5, 0.5);
        double RandomOne => RandomDouble(0, 1);
        double RandomTow => RandomDouble(0, 2);
        SaveFileDialog sfd = new SaveFileDialog()
        {
            Title = "保存文件",
            Filter = "png|*.png"
        };
        private void Button_Export(object sender, RoutedEventArgs e)
        {
            sp.Background = (useBackground.IsChecked ?? false) ? Brushes.White : Brushes.Transparent;

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)root.ActualWidth, (int)root.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(sp);

            BitmapEncoder encoder = new PngBitmapEncoder();
            if (sfd.ShowDialog() ?? false)
            {
                using FileStream fs = File.Create(sfd.FileName);
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
            }
        }
    }
}