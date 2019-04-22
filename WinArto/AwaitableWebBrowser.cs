using System;
using System.Threading;
using System.Windows.Forms;

namespace WinArto
{
    /// <summary>
    ///     Class AwaitableWebBrowser.
    ///     Implements the <see cref="System.Windows.Forms.WebBrowser" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.WebBrowser" />
    public class AwaitableWebBrowser : WebBrowser
    {
        /// <summary>
        ///     Delegate AbsolutelyCompleteEventHandler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public delegate void AbsolutelyCompleteEventHandler(object sender, EventArgs e);

        /// <summary>
        ///     The on document complete count
        /// </summary>
        private int _onDocumentCompleteCount;

        /// <summary>
        ///     The on navigated count
        /// </summary>
        private int _onNavigatedCount;

        /// <summary>
        ///     The on navigating count
        /// </summary>
        private int _onNavigatingCount;


        /// <summary>
        ///     Initializes a new instance of the <see cref="AwaitableWebBrowser" /> class.
        /// </summary>
        public AwaitableWebBrowser()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     This property returns true when all the counters have become equal
        ///     signifying that the navigation has completely completed
        /// </summary>
        /// <value><c>true</c> if busy; otherwise, <c>false</c>.</value>
        public bool Busy
        {
            get
            {
                //sometimes the first navigating event isn't fired so we just have to make sure the navigating count is 
                //more than the navigated, navigated should never be more than navigating
                var bBusy = !(_onNavigatingCount <= _onNavigatedCount);
                //if our navigating counts check out, we should always have a documentcompleted count
                //for each navigated event that is fired
                if (!bBusy)
                {
                    bBusy = _onNavigatedCount > _onDocumentCompleteCount;
                }
                else
                {
                    bBusy = _onNavigatedCount != _onDocumentCompleteCount;
                    if (!bBusy)
                    {
                        bBusy = !(_onNavigatedCount > 0);
                    }
                }

                return bBusy;
            }
        }

        /// <summary>
        ///     This event is raised when the current document is entirely complete,
        ///     and no more navigation is expected to take place for it to load.
        /// </summary>
        public event AbsolutelyCompleteEventHandler AbsolutelyComplete;

        /// <summary>
        ///     This method is used to clear the counters.  Should not be used
        ///     externally, but I left it open for testing, and just in case
        ///     scenarios
        /// </summary>
        public void ClearCounters()
        {
            _onNavigatingCount = 0;
            _onNavigatedCount = 0;
            _onDocumentCompleteCount = 0;
        }

        /// <summary>
        ///     This method should be used in place of the Navigate method to navigate to
        ///     a specific URL.  The navigate method was not overridden because it might
        ///     be required in future modifications to have access to both methods.  When
        ///     calling this NavigateAndWait method, control will not be returned to the
        ///     calling class until the document has completely loaded
        /// </summary>
        /// <param name="url">The URL.</param>
        public void NavigateAndWait(string url)
        {
            ClearCounters();
            Navigate(url);
            WaitUntilComplete();
        }

        /// <summary>
        ///     This method is used to wait until the page has completely loaded.  Use
        ///     after calling a submit, or click, or similar method to not execute further
        ///     code in the calling class until it has completed.  Also helps to reduce
        ///     processor load
        /// </summary>
        public void WaitUntilComplete()
        {
            //first we wait to make sure it starts
            while (!Busy)
            {
                Application.DoEvents();
                //we should sleep for a moment to let the processor have a timeslice
                //for something else - in other words, don't hog the resources
                Thread.Sleep(1);
            }

            //now we wait until it is done
            while (Busy)
            {
                Application.DoEvents();
                //we should sleep for a moment to let the processor have a timeslice
                //for something else - in other words, don't hog the resources
                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Called when [absolutely complete].
        /// </summary>
        protected void OnAbsolutelyComplete()
        {
            ClearCounters();
            AbsolutelyComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.WebBrowser.DocumentCompleted" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.WebBrowserDocumentCompletedEventArgs" /> that contains the event data.</param>
        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            _onDocumentCompleteCount += 1;
            base.OnDocumentCompleted(e);
            if (!Busy)
            {
                OnAbsolutelyComplete();
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.WebBrowser.Navigated" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.WebBrowserNavigatedEventArgs" /> that contains the event data.</param>
        protected override void OnNavigated(WebBrowserNavigatedEventArgs e)
        {
            _onNavigatedCount += 1;
            base.OnNavigated(e);
            if (!Busy)
            {
                OnAbsolutelyComplete();
            }
        }


        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.WebBrowser.Navigating" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.WebBrowserNavigatingEventArgs" /> that contains the event data.</param>
        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            //we have to catch the following three event callers to keep a count
            //of them so that we will be able to determine when the navigation
            //process actually completes
            _onNavigatingCount += 1;
            base.OnNavigating(e);
            if (!Busy)
            {
                OnAbsolutelyComplete();
            }
        }

        /// <summary>
        ///     This method is required for Windows Forms designer support.
        ///     Do not change the method contents inside the source code editor. The Forms designer might
        ///     not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            //
            //browserwrapper
            //
            //Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Name = "AwaitableWebBrowser";
        }
    }
}