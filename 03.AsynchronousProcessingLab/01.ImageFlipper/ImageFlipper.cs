namespace _01.ImageFlipper
{ 
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;

    public class ImageFlipper
    {
        public static void Main()
        {
            string currDirectory = Directory.GetCurrentDirectory();
            var directoryInfo = new DirectoryInfo(currDirectory + "\\Images");

            const string resultDirectory = "Result";
            if (Directory.Exists(resultDirectory))
            {
                Directory.Delete(resultDirectory, true);
            }

            Directory.CreateDirectory(resultDirectory);

            List<Task> tasks = new List<Task>();
            var files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                var task = Task.Run(() =>
                {
                    var image = Image.FromFile(file.FullName);
                    image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    image.Save($"{resultDirectory}\\flip-{file.Name}");

                    Console.WriteLine($"{file.Name} processed...");
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("--Finished--");
        }
    }
}