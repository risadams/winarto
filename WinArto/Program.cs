using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        public static Task LoadPageAsync(ChromiumWebBrowser browser, string address, string basePath, string path, BreakPoint bp, string suffix)
        {
            var tcs = new TaskCompletionSource<bool>();

            void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
            {
                //Wait for while page to finish loading not just the first frame
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= OnLoadingStateChanged;
                    tcs.TrySetResult(true);

                    browser.ScreenshotAsync()
                        .ContinueWith(task =>
                        {
                            var screenshotPath = Path.Combine(basePath, $"{path}_{bp.Name}_{suffix}.jpg");

                            ConsoleUtils.WriteLineColor($"Screenshot ready. Saving to {screenshotPath}", ConsoleColor.Yellow);

                            var bitmap = task.Result;

                            // Save the Bitmap to the path.
                            bitmap.Save(screenshotPath);

                            // We no longer need the Bitmap.
                            // Dispose it to avoid keeping the memory alive.  Especially important in 32-bit applications.
                            bitmap.Dispose();
                        });
                }
            }

            browser.LoadingStateChanged += OnLoadingStateChanged;

            if (!string.IsNullOrEmpty(address))
            {
                browser.Size = new Size(bp.Width, 2500);
                browser.Load(address);
            }

            return tcs.Task;
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task Main()
        {
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            var settings = new CefSettings
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName, "BrowserCache"),
                UserAgent = $"{Application.ProductName}/{Application.ProductVersion},(LIKE: Chrome/71.0.2)",
                WindowlessRenderingEnabled = true,
                LogSeverity = LogSeverity.Info
            };

            Cef.Initialize(settings, true, browserProcessHandler: null);

            ConsoleUtils.WriteLineColor("Loading Browser Settings", ConsoleColor.White);
            ConsoleUtils.WriteLineColor($"\t{settings.UserAgent}", ConsoleColor.Gray);
            ConsoleUtils.WriteLineColor("Loading Configuration", ConsoleColor.White);

            _browser = new ChromiumWebBrowser(string.Empty);

            _browser.BrowserInitialized += async (s, e) =>
            {
                await MainAsync();
            };

            await Task.Run(() =>
            {
                //Hack to get VS to stop complaining, does nothing
            });

            Console.ReadKey();
            Cef.Shutdown();
        }

        private static async Task GatherSnapshots(ChromiumWebBrowser browser, TestSet testSet)
        {
            var local = new Uri(testSet.LocalDomain);
            var remote = new Uri(testSet.RemoteDomain);
            var basePath = $"{Settings.Default.OutputFolder}\\{GetHostFileName(remote)}";

            if (!Directory.Exists(Settings.Default.OutputFolder))
            {
                Directory.CreateDirectory(Settings.Default.OutputFolder);
            }

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            foreach (var path in testSet.Paths)
            {
                foreach (var bp in testSet.BreakPoints)
                {
                    ConsoleUtils.WriteLineColor($"\tFetching {path} at breakpoint {bp.Name}", ConsoleColor.Gray);
                    await LoadPageAsync(browser, new Uri(local, path).ToString(), basePath, path, bp, "local");
                    await LoadPageAsync(browser, new Uri(remote, path).ToString(), basePath, path, bp, "remote");
                }
            }
            
        }

        private static string GetHostFileName(Uri url) => url.Host.TrimStart('/', '\\');

        private static async Task MainAsync()
        {
            if (File.Exists(Settings.Default.Configuration))
            {
                var json = File.ReadAllText(Settings.Default.Configuration);
                var sets = JsonConvert.DeserializeObject<IEnumerable<TestSet>>(json);

                await LoadPageAsync(_browser, "127.0.0.1", "", "", new BreakPoint {Width = 0}, "");
                foreach (var set in sets)
                {
                    await GatherSnapshots(_browser, set);
                }
                ConsoleUtils.WriteLineCautionTape("COMPLETE - Press Any Key");
            }
            else
            {
                ConsoleUtils.WriteLineColor($"Missing config file: {Settings.Default.Configuration}", ConsoleColor.Red);
            }
        }
    }
}