using System;
using System.Diagnostics;
using System.IO;

namespace BadApple
{
    class Program
    {
        static void Main(string[] args)
        {
            if(!File.Exists("frames/1.jpg"))
            {
                Console.Write("Video path: ");
                string cmd = $"/C ffmpeg -i \"{Console.ReadLine()}\" -r 15/1 frames/%01d.jpg";
                Process p = Process.Start("CMD.exe", cmd);
                p.WaitForExit();
            }
            else
            {
                Console.WriteLine("Using already found frames.");
            }

        }

        static void CreateFrames()
        {

        }
    }
}
