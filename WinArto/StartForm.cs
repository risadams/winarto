using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WinArto
{
    /// <summary>
    ///     Class StartForm.
    ///     Implements the <see cref="System.Windows.Forms.Form" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class StartForm : Form
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StartForm" /> class.
        /// </summary>
        public StartForm()
        {
            AutoScaleDimensions = new SizeF(5f, 12f);
            AutoScaleMode = AutoScaleMode.Font;

            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the test set path.
        /// </summary>
        /// <value>The test set path.</value>
        private string TestSetPath { get; set; }

        /// <summary>
        ///     Gets or sets the test sets.
        /// </summary>
        /// <value>The test sets.</value>
        private IEnumerable<TestSet> TestSets { get; set; }

        /// <summary>
        ///     Handles the Click event of the btnLoadTestSet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void btnLoadTestSet_Click(object sender, EventArgs e)
        {
            if (dlgTestSetDialog.ShowDialog(this) == DialogResult.OK)
            {
                TestSetPath = dlgTestSetDialog.FileName;
                var rawData = File.ReadAllText(TestSetPath);
                TestSets = JsonConvert.DeserializeObject<IEnumerable<TestSet>>(rawData);

                var testSets = TestSets.ToList();
                if (testSets.Any())
                {
                    foreach (var set in testSets)
                    {
                        var remote = new Uri(set.RemotePath);
                        var local = new Uri(set.LocalPath);

                        var browser1 = new AwaitableWebBrowser();
                        var browser2 = new AwaitableWebBrowser();

                        InitBrowserControl(browser1);
                        InitBrowserControl(browser2);

                        browser1.DocumentCompleted += (bs, be) => TakeScreenshot((WebBrowser) bs, $@"C:\temp\WinArto\remote\{GetHostFileName(be.Url)}-{GetPathFileName(be.Url)}.jpg");
                        browser2.DocumentCompleted += (bs, be) => TakeScreenshot((WebBrowser) bs, $@"C:\temp\WinArto\locals\{GetHostFileName(be.Url)}-{GetPathFileName(be.Url)}.jpg");

                        await QueueNavigate(browser1, remote);
                        await QueueNavigate(browser2, local);
                    }
                }

                Close();
                Application.Exit();
            }
        }

        /// <summary>
        ///     Handles the Load event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(@"C:\temp\WinArto\remote\"))
            {
                Directory.CreateDirectory(@"C:\temp\WinArto\locals\");
            }

            if (!Directory.Exists(@"C:\temp\WinArto\remote\"))
            {
                Directory.CreateDirectory(@"C:\temp\WinArto\remote\");
            }

            btnLoadTestSet.Enabled = true;
        }

        /// <summary>
        ///     Gets the name of the path file.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>System.String.</returns>
        private static string GetPathFileName(Uri url) => url.LocalPath.TrimStart('/', '\\');
        private static string GetHostFileName(Uri url) => url.Host.TrimStart('/', '\\');

        /// <summary>
        ///     Initializes the browser control.
        /// </summary>
        /// <param name="webBrowser">The web browser.</param>
        private static void InitBrowserControl(WebBrowser webBrowser)
        {
            webBrowser.AllowNavigation = true;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.Width = 1300;
        }

        /// <summary>
        ///     Queues the navigate.
        /// </summary>
        /// <param name="wb">The wb.</param>
        /// <param name="url">The URL.</param>
        /// <returns>Task.</returns>
        private async Task QueueNavigate(AwaitableWebBrowser wb, Uri url)
        {
            await Task.Run(() =>
            {
                Invoke(new Action(() =>
                {
                    wb.NavigateAndWait(url.ToString());
                }));
            });
        }

        /// <summary>
        ///     Takes the screenshot.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="imagePath">The image path.</param>
        private static void TakeScreenshot(WebBrowser browser, string imagePath)
        {
            browser.Width = browser.Document.Body.ScrollRectangle.Width;
            browser.Height = browser.Document.Body.ScrollRectangle.Height;

            using (var pic = new Bitmap(browser.Width, browser.Height))
            {
                browser.DrawToBitmap(pic, new Rectangle(0, 0, pic.Width, pic.Height));
                pic.Save(imagePath, ImageFormat.Jpeg);
            }
        }
    }
}