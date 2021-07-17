using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BadApple
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Write("Video path: ");
            string video = Console.ReadLine();
            Directory.CreateDirectory("frames");
            if (!File.Exists("frames/001.jpg"))
            {
                string cmd = $"/C ffmpeg -i \"{video}\" -r 15/1 -vf scale=1920:-1 frames/%03d.jpg";
                Process p = Process.Start("CMD.exe", cmd);
                p.WaitForExit();
            }
            else
            {
                Console.WriteLine("Using already found frames.");
            }

            CreateFrames(video);
        }

        private static void CreateFrames(string video)
        {
            Console.Write("Image path: ");
            string source = Console.ReadLine();
            if (!File.Exists(source))
            {
                Console.WriteLine("Image does not exist.");
                return;
            }
            //resize img
            Bitmap sourceBitmap = new Bitmap(48, 48);
            using (Graphics g = Graphics.FromImage(sourceBitmap)) g.DrawImage(new Bitmap(source), 0, 0, 48, 48);

            Bitmap[] sources = new Bitmap[101];

            //generate frames
            foreach (string path in Directory.GetFiles("frames", "*.jpg"))
            {
                string newPath = path.Replace("frames", "output");
                if (File.Exists(newPath)) continue;
                Bitmap frame = new Bitmap(path);

                //create black bitmap
                Bitmap newFrame = new Bitmap(frame.Width, frame.Height);
                Graphics gr = Graphics.FromImage(newFrame);
                gr.Clear(Color.Black);

                using (Graphics g = Graphics.FromImage(newFrame))
                {
                    for (int y = 0; y < newFrame.Height; y += 48)
                    {
                        for (int x = 0; x < newFrame.Width; x += 48)
                        {
                            //crop area and get avg color
                            Rectangle area = new Rectangle(new Point(x, y), new Size(48, 48));
                            Bitmap crop = ImageProcessing.CropImage(frame, area);
                            double trans = 1 - Math.Round(ImageProcessing.CalculateAverageLightness(crop), 2);

                            //get source image with correct transparency
                            Bitmap sourceTrans = sources[Convert.ToInt32(trans * 100)];

                            //create source image if not exists
                            if (sourceTrans == null)
                            {
                                sources[Convert.ToInt32(trans * 100)] = sourceTrans = (Bitmap)ImageProcessing.ChangeImageOpacity(sourceBitmap, trans);
                                Console.WriteLine($"Created image with transparency level {trans}");
                            }

                            //place source image
                            g.DrawImage(sourceTrans, new Point(x, y));
                        }
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    newFrame.Save(newPath, ImageFormat.Jpeg);
                    Console.WriteLine($"{path} created.");
                }
            }
            Console.WriteLine("Processed all frames. Generating audio.");
            Process processAudio = Process.Start("CMD.exe", $"/C ffmpeg -i {video} -y output/audio.mp3");
            processAudio.WaitForExit();

            Console.WriteLine("Generating video.");
            Process processVideo = Process.Start("CMD.exe", $"/C ffmpeg -r 15 -i output/%03d.jpg -i output/audio.mp3 -c:v libx264 -c:a aac -pix_fmt yuv420p -crf 23 -r 15 -shortest -y output.mp4");
            processVideo.WaitForExit();

            Process.Start("CMD.exe", $"/C \"{Directory.GetCurrentDirectory()}\\output.mp4\"");
            Console.WriteLine("Done uwu");
        }
    }
}