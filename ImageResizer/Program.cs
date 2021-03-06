﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // sync 1.857s
            // async 1.224s
            // 34%
            int round = 10;

            //await Task.Run(() =>
            //{
            //    long total = 0;
            //    for (int i = 0; i < round; i++)
            //    {
            //        string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            //        string destinationPath = Path.Combine(Environment.CurrentDirectory, "output"); ;
            //        ImageProcess imageProcess = new ImageProcess();
            //        imageProcess.Clean(destinationPath);
            //        Stopwatch sw = new Stopwatch();
            //        sw.Start();
            //        imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
            //        sw.Stop();

            //        Console.WriteLine($"同步 花費時間: {sw.ElapsedMilliseconds} ms");
            //        total += sw.ElapsedMilliseconds;
            //    }

            //    Console.WriteLine($"同步 {total / round}");
            //});

            CancellationTokenSource cts = new CancellationTokenSource();

            #region 等候使用者輸入 取消 c 按鍵
            ThreadPool.QueueUserWorkItem(x =>
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.C)
                {
                    cts.Cancel();
                }
            });
            #endregion

            await Task.Run(async () =>
            {
                long total = 0;
                for (int i = 0; i < round; i++)
                {
                    string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
                    string destinationPath = Path.Combine(Environment.CurrentDirectory, "output"); ;
                    ImageProcess imageProcess = new ImageProcess();
                    imageProcess.Clean(destinationPath);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    try
                    {
                        await imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0, cts.Token);
                    }
                    catch
                    {
                        imageProcess.Clean(destinationPath);
                        Console.WriteLine("取消");
                        break;
                    }
                    await Task.Delay(1000);
                    sw.Stop();

                    Console.WriteLine($"非同步 花費時間: {sw.ElapsedMilliseconds} ms");
                    total += sw.ElapsedMilliseconds;
                }

                Console.WriteLine($"非同步 {total / round}");
            });

            Console.ReadLine();
        }
    }
}
