using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using Microsoft.Win32;
using PuzzleDev.Models;

namespace PuzzleDev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class Exten
    {
        static Exten()
        {
            imageDatas1 = new List<ImageData[]>();
        }

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public static void Save(this BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
        public static List<bool> GetHash(Bitmap bmpSource,string side)
        {
            List<bool> lResult = new List<bool>();
            Bitmap bmpMin = new Bitmap(bmpSource, new System.Drawing.Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    switch (side)
                    {
                        case "right":
                            lResult.Add(bmpMin.GetPixel(bmpMin.Width - 1, j).GetBrightness() < 0.5f);
                            break;
                        case "left":
                            lResult.Add(bmpMin.GetPixel(0, j).GetBrightness() < 0.5f);
                            break;
                        case "top":
                            lResult.Add(bmpMin.GetPixel(i, bmpMin.Height - 1).GetBrightness() < 0.5f);
                            break;
                        case "down":
                            lResult.Add(bmpMin.GetPixel(i, 0).GetBrightness() < 0.5f);
                            break;
                    }
                }
            }
            return lResult;
        }
        static void printArr(ImageData[] a, int n)
        {
            ImageData[] arr = new ImageData[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = a[i];
            }
            imageDatas1.Add(arr);
        }
        public static List<ImageData[]> imageDatas1 { get; set; }
        public static void heapPermutation(ImageData[] a, int size,int n)
        {
            if (size == 1)
                printArr(a, n);

            for (int i = 0; i < size; i++)
            {
                heapPermutation(a, size - 1, n);

                if (size % 2 == 1)
                {
                    ImageData temp = a[0];
                    a[0] = a[size - 1];
                    a[size - 1] = temp;
                }

                else
                {
                    ImageData temp = a[i];
                    a[i] = a[size - 1];
                    a[size - 1] = temp;
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        BitmapImage bitmap_t = new BitmapImage();
        public string filename { get; set; } = " ";

        List<BitmapImage> mage = new List<BitmapImage>();
        public double Heights { get; set; }
        public double Widths { get; set; }
        public int h { get; set; }
        public int w { get; set; }

        static string startupPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        static String searchFolder = startupPath + "\\Pictures\\";

        static int max = Int32.MaxValue / 20;


        ImageData[] imagesTotal = new ImageData[max];

        //Bitmap[] imagesbitTotal = new Bitmap[max];

        public MainWindow()
        {
            InitializeComponent();

               Check.IsEnabled = false;
               Auto.IsEnabled = false;
               Play.IsEnabled = false;
               
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(HeightText.Text) && String.IsNullOrEmpty(WeightText.Text))
            {
                FileNameLabel.Content = "Enter Height and Width!";
            }
            else {
                BrowseButton.IsEnabled = false;
                Check.IsEnabled = false;
                Auto.IsEnabled = false;
                Play.IsEnabled = true;
                System.IO.DirectoryInfo di = new DirectoryInfo(searchFolder);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.InitialDirectory = "c:\\";
                dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() ?? false)
                {
                    string selectedFileName = dlg.FileName;
                    FileNameLabel.Content = "Ready!";
                    filename = selectedFileName;
                    bitmap_t.BeginInit();
                    bitmap_t.UriSource = new Uri(selectedFileName);
                    bitmap_t.EndInit();
                    Exten.Save(bitmap_t, startupPath + "\\full.jpg");
                }
                Heights = double.Parse(HeightText.Text);
                Widths = double.Parse(WeightText.Text);
                h = int.Parse(HeightText.Text);
                w = int.Parse(WeightText.Text);
                List<BitmapImage> l = new List<BitmapImage>();
                int total = h * w;
                for (int i = 0; i < total; i++)
                {
                    l.Add(Exten.ToBitmapImage(Pictures()[i]));
                    mage.Add(Exten.ToBitmapImage(Pictures()[i]));
                }


                ImageData[] images = new ImageData[total];
                for (int i = 0; i < total; i++)
                {
                    images[i] = new ImageData { Images = l[i] };
                }

                for (int i = 0; i < total; i++)
                {
                    string img;
                    if (i < 9)
                    {
                        img = "img0" + (i + 1) + ".jpg";
                    }
                    else
                    {
                        img = "img" + (i + 1) + ".jpg";
                    }
                    Exten.Save(mage[i], startupPath + "\\Pictures\\" + img);
                }
            }

        }
        public List<Bitmap> Pictures()
        {

            System.Drawing.Image img = System.Drawing.Image.FromFile(filename);
            int widthThird = (int)((double)img.Width / Widths + 0.5);
            int heightThird = (int)((double)img.Height / Heights + 0.5);
            Bitmap[,] bmps = new Bitmap[h, w];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                {
                    bmps[i, j] = new Bitmap(widthThird, heightThird);
                    Graphics g = Graphics.FromImage(bmps[i, j]);
                    g.DrawImage(img, new System.Drawing.Rectangle(0, 0, widthThird, heightThird), new System.Drawing.Rectangle(j * widthThird, i * heightThird, widthThird, heightThird), GraphicsUnit.Pixel);
                    g.Dispose();
                }
            List<Bitmap> pic = new List<Bitmap>();
            foreach (var x in bmps)
            {
                pic.Add(x);
            }
            return pic;


        }
        public static System.Windows.Controls.Image global_sender;
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {

            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            global_sender = image;
            DragDrop.DoDragDrop(image, image.Source, DragDropEffects.Copy);

        }

        private void Image_Drop(object sender, DragEventArgs e)
        {

            ImageSource temp = global_sender.Source;
            global_sender.Source = ((System.Windows.Controls.Image)sender).Source;
            ((System.Windows.Controls.Image)sender).Source = temp;

            int index = -1;
            for (int i = 0; i < h*w; i++)
            {
                var lbi = this.piccc3.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (lbi == null) continue;
                if (IsMouseOverTarget(lbi, e.GetPosition((IInputElement)lbi)))
                {
                    index = i;
                    break;
                }
            }
            //this.listik.Content = index;
            imagesTotal[index] = new ImageData { Images = (BitmapImage)temp };
        }
        private static bool IsMouseOverTarget(Visual target, System.Windows.Point point)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            return bounds.Contains(point);
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        //public int Check_C(ImageData[] imageDatas)
        //{
        //    int imageWidth = 500;
        //    int imageHeight = 500;
        //    DrawingVisual drawingVisual = new DrawingVisual();
        //    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        //    {

        //        int num = 0;

        //        for (int i = 0; i < h; i++)
        //        {
        //            for (int j = 0; j < w; j++)
        //            {
        //                drawingContext.DrawImage(imageDatas[num].Images, new Rect(j * imageHeight, i * imageWidth, imageWidth, imageHeight));
        //                num++;
        //            }
        //        }
        //    }
        //    RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * w, imageHeight * h, 96, 96, PixelFormats.Pbgra32);
        //    bmp.Render(drawingVisual);

        //    MemoryStream stream = new MemoryStream();
        //    BitmapEncoder encoder = new BmpBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(bmp));
        //    encoder.Save(stream);
        //    Bitmap bitmap = new Bitmap(stream);
        //    //System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
        //    //img.Save(@"C:\Users\victo\source\repos\PuzzleDev\PuzzleDev\full_recon.jpg");
        //    Bitmap bmp2 = new Bitmap(startupPath + "\\full.jpg");
        //    Bitmap bmp3 = new Bitmap(bmp2, new System.Drawing.Size(imageWidth * w, imageHeight * h));
        //    bmp3.SetResolution(96, 96);

        //    //bmp3.Save(@"C:\Users\victo\source\repos\PuzzleDevCom\PuzzleDevCom\Pictures\full2.jpg");


        //    List<bool> iHash1 = Exten.GetHash(bitmap);
        //    List<bool> iHash2 = Exten.GetHash(bmp3);

        //    int count = 0;
        //    for (int i = 0; i < iHash1.Count; i++)
        //    {
        //        if (iHash1[i] != iHash2[i])
        //        {
        //            count += 1;
        //        }
        //    }
        //    return count;
        //}

        public int Check_C(ImageData[] arr1)
        {

            Bitmap[] imageDatas = new Bitmap[h * w];
            int f1 = 0;
            for (int i = 0; i < h * w; i++)
            {
                imageDatas[f1] = Exten.BitmapImage2Bitmap(arr1[f1].Images);
                f1++;
            }
            Bitmap[,] arr = new Bitmap[h, w];
            int num = 0;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    arr[i, j] = imageDatas[num];
                    num++;
                }
            }
            string Hash1 = "";
            string Hash2 = "";

            int count = 0;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    Hash1 += "\n";
                    Hash2 += "\n";
                    if (i - 1 >= 0)
                    {
                        List<bool> iHash1 = Exten.GetHash(arr[i, j], "down");
                        List<bool> iHash2 = Exten.GetHash(arr[i - 1, j], "top");
                        for (int f = 0; f < iHash1.Count; f++)
                        {
                            if (iHash1[f] != iHash2[f])
                            {
                                count += 1;
                            }
                        }
                    }
                    if (j - 1 >= 0)
                    {
                        List<bool> iHash1 = Exten.GetHash(arr[i, j], "left");
                        List<bool> iHash2 = Exten.GetHash(arr[i, j - 1], "right");
                        for (int f = 0; f < iHash1.Count; f++)
                        {
                            Hash1 += iHash1[f].ToString();
                            Hash2 += iHash2[f].ToString();
                            if (iHash1[f] != iHash2[f])
                            {
                                count += 1;
                            }
                        }
                    }
                }
            }
            
            return count;
        }
        private void Check_Click(object sender, RoutedEventArgs e)
        {
            int c = Check_C(imagesTotal);
            if (c < 3)
            {
                this.listik.Content = "You win!";
            }
            else
            {
                this.listik.Content = c;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Play.IsEnabled = false;
            Check.IsEnabled = true;
            Auto.IsEnabled = true;
            int fCount = Directory.GetFiles(searchFolder, "*.jpg", SearchOption.TopDirectoryOnly).Length;
            List<BitmapImage> bitmaps = new List<BitmapImage>();
            for (int i = 0; i < fCount; i++)
            {
                string img;
                if (i < 9)
                {
                    img = "img0" + (i + 1) + ".jpg";
                }
                else
                {
                    img = "img" + (i + 1) + ".jpg";
                }
                BitmapImage bitmap = new BitmapImage(new Uri(searchFolder + img));
                bitmaps.Add(bitmap);
            }

            Random rand = new Random();
            var nums = Enumerable.Range(0, fCount).ToArray();
            for (int i = 0; i < nums.Length; ++i)
            {
                int randomIndex = rand.Next(nums.Length);
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }
            ImageData[] images_r = new ImageData[fCount];
            for (int i = 0; i < fCount; i++)
            {
                images_r[nums[i]] = new ImageData { Images = bitmaps[i] };
            }
            //this.SizeToContent = SizeToContent.Height;
            this.SizeToContent = SizeToContent.Width;
            this.piccc2.Width = 100 * w;
            this.piccc2.Height = 100 * h;
            this.piccc2.ItemsSource = images_r;
            ImageData[] images2 = new ImageData[fCount];
            Bitmap bmp = new Bitmap(50, 50);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                System.Drawing.Rectangle ImageSize = new System.Drawing.Rectangle(0, 0, 50, 50);
                graph.FillRectangle(System.Drawing.Brushes.White, ImageSize);
            }
            //for (int i = 0; i < fCount; i++)
            //{
            //    imagesbitTotal[i] = bmp;
            //}
            for (int i = 0; i < fCount; i++)
            {
                images2[i] = new ImageData { Images = Exten.ToBitmapImage(bmp) };
            }
            this.piccc3.Width = 100 * w;
            this.piccc3.Height = 100 * h;
            this.piccc3.ItemsSource = images2;
            for (int i = 0; i < fCount; i++)
            {
                imagesTotal[i] = (ImageData)piccc3.Items[i];
            }

        }

        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            //this.listik.Content = "It can take a few moment...";
            int size = w * h;
            ImageData[] images_r = new ImageData[size];
            for (int i = 0; i < size; i++)
            {
                images_r[i] = (ImageData)piccc2.Items[i];
            }
            Exten.heapPermutation(images_r, size, size);
            Dictionary<int, (int, ImageData[])> res = new Dictionary<int, (int, ImageData[])>(Exten.imageDatas1.Count);
            for (int i = 0; i < Exten.imageDatas1.Count; i++)
            {
                res.Add(i, (Check_C(Exten.imageDatas1[i]), Exten.imageDatas1[i]));
            }
            var keyAndValue = res.OrderBy(kvp => kvp.Value.Item1).First();
            this.listik.Content = "Done"; //+ keyAndValue.Value.Item1;
            this.piccc3.Width = 100 * w;
            this.piccc3.Height = 100 * h;
            this.piccc3.ItemsSource = keyAndValue.Value.Item2;

        }
    }
}
