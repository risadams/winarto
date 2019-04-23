using System.ComponentModel;

namespace WinArto
{
    /// <summary>
    ///     Class ProgressReportingEventArgs.
    ///     Implements the <see cref="System.ComponentModel.ProgressChangedEventArgs" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.ProgressChangedEventArgs" />
    public class ProgressReportingEventArgs : ProgressChangedEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.ComponentModel.ProgressChangedEventArgs" /> class.
        /// </summary>
        /// <param name="progressPercentage">The percentage of an asynchronous task that has been completed.</param>
        /// <param name="userState">A unique user state.</param>
        public ProgressReportingEventArgs(int progressPercentage, object userState) : base(progressPercentage, userState)
        {
        }

        /// <summary>
        ///     Gets or sets the current item progress.
        /// </summary>
        /// <value>The current item progress.</value>
        public int CurrentItemProgress { get; set; }

        /// <summary>
        ///     Converts to talitems.
        /// </summary>
        /// <value>The total items.</value>
        public int TotalItems { get; set; }
    }
}