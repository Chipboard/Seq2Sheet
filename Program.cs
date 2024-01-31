using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Seq2Sheet
{
    internal static partial class Program
    {
        static SeqForm form = new SeqForm();
        static string[] imagePaths = new string[0];
        static Image[] images = new Image[0];

        static int imageWidth, imageHeight;
        static int averageCropX, averageCropY;

        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            form.DragDrop += Form_DragDrop;
            form.loadButton.Click += LoadButton_Click;
            form.fileList.SelectedIndexChanged += FileList_SelectedIndexChanged;
            form.generateButton.Click += GenerateButton_Click;
            form.cropButton.Click += CropButton_Click;
            Application.Run(form);
        }

        private static void CropButton_Click(object? sender, EventArgs e)
        {
            if(images.Length == 0)
                return;

            averageCropX = 0;
            averageCropY = 0;

            Rectangle cropRect = new Rectangle(int.MaxValue,int.MaxValue,int.MinValue,int.MinValue);

            imageWidth = 0;
            imageHeight = 0;
            for(int i = 0; i < images.Length; i++)
            {
                cropRect = GetNonAlphaBoundingBox(images[i] as Bitmap, i, cropRect);
            }

            averageCropX /= images.Length;
            averageCropY /= images.Length;

            form.averageCrop.Text = $"Average Crop: {averageCropX}x{averageCropY}";
            form.finalCrop.Text = $"Final Crop: {cropRect.Width}x{cropRect.Height}";

            if (cropRect.Width == 0 || cropRect.Height == 0 || cropRect.Width == int.MinValue + 1 || cropRect.Height == int.MinValue + 1)
            {
                MessageBox.Show("Invalid crop rect! Did you set the alpha threshold too high?");
                return;
            }

            //MessageBox.Show($"{cropRect.Top} : {cropRect.Bottom} : {cropRect.Left} : {cropRect.Right}");
            //MessageBox.Show($"{cropRect.Width}x{cropRect.Height}");
            imageWidth = cropRect.Width;
            imageHeight = cropRect.Height;

            List<Action> actionList = new();
            for(int i = 0; i < images.Length; i += 8)
            {
                int endIndex = i + 8;

                if(endIndex > images.Length)
                    endIndex = images.Length;

                actionList.Add(() => ThreadCrop(i, endIndex, cropRect));
                Parallel.Invoke(actionList.ToArray());
                actionList.Clear();
            }

            form.imagePreview.Image = images[0];
        }

        static void ThreadCrop(int startIndex, int endIndex, Rectangle cropRect)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                Bitmap croppedImage = CropImage(images[i] as Bitmap, cropRect);

                images[i].Dispose();
                images[i] = croppedImage;
            }
        }

        static Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmp = new(cropArea.Width, cropArea.Height);
            using (Graphics gph = Graphics.FromImage(bmp))
            {
                gph.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        private static void GenerateButton_Click(object? sender, EventArgs e)
        {
            if (images.Length == 0)
            {
                MessageBox.Show("You need to specify the sequence images first!");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Images|*.png;";
            saveDialog.Title = "Save Spritesheet";
            saveDialog.ShowDialog();

            if (saveDialog.FileName != "")
            {
                FileStream fs = (FileStream)saveDialog.OpenFile();

                Bitmap spriteSheet = GenerateSpritesheet();
                spriteSheet.Save(fs, ImageFormat.Png);
                spriteSheet.Dispose();

                fs.Close();
                fs.Dispose();
            }
        }

        private static Bitmap GenerateSpritesheet()
        {
            int imageCount = images.Length;

            // Calculate the column count and row count for a square-ish distribution
            int columnCount = (int)Math.Floor(Math.Sqrt(imageCount));
            int rowCount = (int)Math.Ceiling((double)imageCount / columnCount);

            int spritesheetWidth = columnCount * imageWidth;
            int spritesheetHeight = rowCount * imageHeight;

            Bitmap bmp = new Bitmap(spritesheetWidth, spritesheetHeight);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < imageCount; i++)
                {
                    int column = i % columnCount;
                    int row = i / columnCount;

                    int x = column * imageWidth;
                    int y = row * imageHeight;

                    // Assuming images is an array of Image objects
                    g.DrawImage(images[i], new Rectangle(x, y, imageWidth, imageHeight));
                }
            }

            return bmp;
        }


        static Rectangle GetNonAlphaBoundingBox(Bitmap image, int index, Rectangle compare)
        {
            int minX = compare.Left;
            int minY = compare.Top;
            int maxX = compare.Right;
            int maxY = compare.Bottom;
            int alphaThreshold = (int)form.alphaThreshold.Value;

            int currentMinX = int.MaxValue;
            int currentMinY = int.MaxValue;
            int currentMaxX = int.MinValue;
            int currentMaxY = int.MinValue;

            BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;

                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        byte alpha = ptr[3]; // Alpha component (assuming 32bppArgb format)

                        if (alpha > alphaThreshold)
                        {
                            minX = Math.Min(minX, x);
                            minY = Math.Min(minY, y);
                            maxX = Math.Max(maxX, x);
                            maxY = Math.Max(maxY, y);

                            currentMinX = Math.Min(currentMinX, x);
                            currentMinY = Math.Min(currentMinY, y);
                            currentMaxX = Math.Max(currentMaxX, x);
                            currentMaxY = Math.Max(currentMaxY, y);
                        }

                        ptr += 4; // Move to the next pixel
                    }

                    ptr += bmpData.Stride - (bmpData.Width * 4); // Move to the next row
                }
            }
            image.UnlockBits(bmpData);

            // Calculate width and height of the bounding box
            int width = maxX - minX;
            int height = maxY - minY;
            int currentWidth = currentMaxX - currentMinX;
            int currentHeight = currentMaxY - currentMinY;

            Rectangle final = new Rectangle(minX, minY, width, height);

            averageCropX += final.Width;
            averageCropY += final.Height;

            // form.fileList.Items[index] = $"{currentWidth}x{currentHeight}";

            // Create and return the Rectangle
            return final;
        }

        private static void FileList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (form.fileList.SelectedIndex == -1)
                return;

            form.imagePreview.Image = images[form.fileList.SelectedIndex];
        }

        private static void Form_DragDrop(object? sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        // https://stackoverflow.com/questions/6723487/how-to-sort-a-string-array-by-numeric-style
        public static void NumericalSort(string[] ar)
        {
            Regex rgx = SortRegex();
            Array.Sort(ar, (a, b) =>
            {
                var ma = rgx.Matches(a);
                var mb = rgx.Matches(b);
                for (int i = 0; i < ma.Count; ++i)
                {
                    int ret = ma[i].Groups[1].Value.CompareTo(mb[i].Groups[1].Value);
                    if (ret != 0)
                        return ret;

                    ret = int.Parse(ma[i].Groups[2].Value) - int.Parse(mb[i].Groups[2].Value);
                    if (ret != 0)
                        return ret;
                }

                return 0;
            });
        }

        private static void LoadButton_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Application.StartupPath;
                var codecs = ImageCodecInfo.GetImageEncoders();
                var codecFilter = "Image Files|";
                foreach (var codec in codecs)
                {
                    codecFilter += codec.FilenameExtension + ";";
                }
                openFileDialog.Filter = codecFilter;
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] fileNames = openFileDialog.FileNames;
                    NumericalSort(fileNames);

                    form.averageCrop.Text = "Average Crop: None";
                    form.finalCrop.Text = "Final Crop: None";

                    images = new Image[fileNames.Length];
                    imagePaths = new string[fileNames.Length];
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        if (images[i] != null)
                            images[i].Dispose();

                        imagePaths[i] = Path.GetFileName(fileNames[i]);
                        images[i] = Image.FromFile(fileNames[i]);

                        if (images[i].Width > imageWidth)
                            imageWidth = images[i].Width;

                        if (images[i].Height > imageHeight)
                            imageHeight = images[i].Height;
                    }

                    form.imagePreview.Image = images[0];

                    form.fileList.Items.Clear();
                    form.fileList.Items.AddRange(imagePaths);
                }
            }
        }

        [GeneratedRegex("([^0-9]*)([0-9]+)")]
        private static partial Regex SortRegex();
    }
}