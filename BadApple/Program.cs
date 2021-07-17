using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace BadApple
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (!File.Exists("frames/1.jpg"))
            {
                Console.Write("Video path: ");
                string cmd = $"/C ffmpeg -i \"{Console.ReadLine()}\" -r 15/1 -vf scale=1920:-1 frames/%01d.jpg";
                Process p = Process.Start("CMD.exe", cmd);
                p.WaitForExit();
            }
            else
            {
                Console.WriteLine("Using already found frames.");
            }
            CreateFrames();
        }

        private static void CreateFrames()
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

            //generate frames
            foreach (string path in Directory.GetFiles("frames", "*.jpg"))
            {
                Bitmap frame = new Bitmap(path);
                Bitmap newFrame = new Bitmap(frame.Width, frame.Height);

                using (Graphics g = Graphics.FromImage(frame))
                {

                }
            }
        }
    }
}