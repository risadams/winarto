using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.OffScreen;
using Newtonsoft.Json;
using WinArto.Properties;

namespace WinArto
{
    internal static class Program
    {
        private static ChromiumWebBrowser _browser;

        public static Task LoadPageAsync(ChromiumWebBrowser browser, string address, int width)
        {
            var tcs = new TaskCompletionSource<bool>();

            void OnHandler(object sender, LoadingStateChangedEventArgs args)
            {
                //Wait for while page to finish loading not just the first frame
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= OnHandler;
                    tcs.TrySetResult(true);
                }
            }

            browser.LoadingStateChanged += OnHandler;

            browser.Size = new Size(width, 2500);
            browser.Load(address);
            return tcs.Task;
        }

        private static async Task GatherSnapshots(ChromiumWebBrowser browser, TestSet testSet)
        {
            var local = new Uri(testSet.LocalDomain);
            var remote = new Uri(testSet.RemoteDomain);
            var localBasePath = $"{Settings.Default.OutputFolder}/local/{GetHostFileName(local)}";
            var remoteBasePath = $"{Settings.Default.OutputFolder}/remote/{GetHostFileName(remote)}";

            if (!Directory.Exists(localBasePath))
            {
                Directory.CreateDirectory(localBasePath);
            }

            if (!Directory.Exists(remoteBasePath))
            {
                Directory.CreateDirectory(remoteBasePath);
            }

            foreach (var path in testSet.Paths)
            {
                ConsoleUtils.WriteLineColor($"Loading {local}\\{path}", ConsoleColor.White);
                foreach (var bp in testSet.BreakPoints)
                {
                    var ssPath = $"{localBasePath}\\{bp.Name}";
                    if (!Directory.Exists(ssPath))
                    {
                        Directory.CreateDirectory(ssPath);
                    }

                    ConsoleUtils.WriteLineColor($"\tFetching at breakpoint {bp.Name}", ConsoleColor.Gray);
                    await LoadPageAsync(browser, new Uri(local, path).ToString(), bp.Width);
                }

                ConsoleUtils.WriteLineColor($"Loading {remote}\\{path}", ConsoleColor.White);
                foreach (var bp in testSet.BreakPoints)
                {
                    var ssPath = $"{remoteBasePath}\\{bp.Name}";
                    if (!Directory.Exists(ssPath))
                    {
                        Directory.CreateDirectory(ssPath);
                    }

                    ConsoleUtils.WriteLineColor($"\tFetching at breakpoint {bp.Name}", ConsoleColor.Gray);
                    await LoadPageAsync(browser, new Uri(remote, path).ToString(), bp.Width);
                    await browser.ScreenshotAsync()
                        .ContinueWith(task =>
                        {
                            var screenshotPath = Path.Combine(ssPath, $"{path}.jpg");

                            ConsoleUtils.WriteLineColor($"Screenshot ready. Saving to {screenshotPath}", ConsoleColor.White);

                            var bitmap = task.Result;

                            // Save the Bitmap to the path.
                            bitmap.Save(screenshotPath);

                            // We no longer need the Bitmap.
                            // Dispose it to avoid keeping the memory alive.  Especially important in 32-bit applications.
                            bitmap.Dispose();
                        });
                }
            }
        }

        private static string GetHostFileName(Uri url) => url.Host.TrimStart('/', '\\');

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static async Task Main()
        {
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            var settings = new CefSettings
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName, "BrowserCache"),
                UserAgent = $"{Application.ProductName}/{Application.ProductVersion},(LIKE: Chrome/71.0.2)",
                WindowlessRenderingEnabled = true,
                LogSeverity = LogSeverity.Error
            };

            Cef.Initialize(settings, true, browserProcessHandler: null);

            ConsoleUtils.WriteLineColor("Loading Browser Settings", ConsoleColor.White);
            ConsoleUtils.WriteLineColor($"\t{settings.UserAgent}", ConsoleColor.Gray);
            ConsoleUtils.WriteLineColor("Loading Configuration", ConsoleColor.White);

            Thread.Sleep(1000); //wait for init to complete;

            _browser = new ChromiumWebBrowser();

            _browser.BrowserInitialized += async (s, e) =>
            {
                await MainAsync();
            };

            await Task.Run(() =>
            {
            });

            Console.ReadKey(); //wait for something;

            Cef.Shutdown();
        }

        private static async Task MainAsync()
        {
            if (File.Exists(Settings.Default.Configuration))
            {
                var json = File.ReadAllText(Settings.Default.Configuration);
                var sets = JsonConvert.DeserializeObject<IEnumerable<TestSet>>(json);
                foreach (var set in sets)
                {
                    await GatherSnapshots(_browser, set);
                }
            }
            else
            {
                ConsoleUtils.WriteLineColor($"Missing config file: {Settings.Default.Configuration}", ConsoleColor.Red);
            }
        }
    }
}