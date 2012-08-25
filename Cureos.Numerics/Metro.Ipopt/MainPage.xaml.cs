using System;
using System.Text;
using Cureos.Numerics;
using Cureos.Numerics.Nlp;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Metro.Ipopt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void OptimizeButtonClick(object sender, RoutedEventArgs e)
        {
            /* allocate space for the initial point and set the values */
            double[] x = { 1.0, 5.0, 5.0, 1.0 };

            try
            {
                var output = new StringBuilder();

                IpoptReturnCode status;
                double obj;
                using (var hs071 = new HS071())
                {
                    // Set some options.  The following ones are only examples,
                    // they might not be suitable for your problem.
                    hs071.AddOption("tol", 1e-7);
                    hs071.AddOption("mu_strategy", "adaptive");

                    // Solve the problem.
                    status = hs071.SolveProblem(x, out obj, null, null, null, null);
                }

                output.AppendLine("HS071");
                output.AppendLine("=====");
                output.AppendFormat("Optimization return status: {0}{1}{1}", status, Environment.NewLine);
                output.AppendFormat("Objective function value f = {0}{1}", obj, Environment.NewLine);
                for (int i = 0; i < 4; ++i) output.AppendFormat("x[{0}]={1}{2}", i, x[i], Environment.NewLine);
                output.AppendLine();

                IpoptResult.Text = output.ToString();
            }
            catch (Exception exc)
            {
                IpoptResult.Text = exc.GetType().FullName + ": " + exc.Message + Environment.NewLine + exc.StackTrace;
            }
        }
    }
}
