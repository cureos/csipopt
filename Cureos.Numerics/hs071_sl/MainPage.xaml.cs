// Copyright (c) 2011 Anders Gustafsson, Cureos AB.
// All rights reserved. This software and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html

using System;
using System.Text;
using System.Windows;
using Cureos.Numerics;
using Cureos.Numerics.Nlp;

namespace hs071_sl
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void StartOptButton_Click(object sender, RoutedEventArgs e)
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

                // Also optimize Rosenbrock post office problem
                var r = new HS037();
                var rosenbrock = IpoptAdapter.CreateIpoptProblem(r._n, r._x_L, r._x_U, r._m, r._g_L, r._g_U, r._nele_jac,
                                                         r._nele_hess, IpoptIndexStyle.C,
                                                         r.eval_f, r.eval_g, r.eval_grad_f, r.eval_jac_g, r.eval_h);
                IpoptAdapter.AddIpoptStrOption(rosenbrock, "hessian_approximation", "limited-memory");
                IpoptAdapter.AddIpoptIntOption(rosenbrock, "limited_memory_max_history", 5);

                x = new[] { 10.0, 10.0, 10.0 };
                status = IpoptAdapter.IpoptSolve(rosenbrock, x, null, out obj, null, null, null, IntPtr.Zero);

                output.AppendLine("Rosenbrock Post Office");
                output.AppendLine("======================");
                output.AppendFormat("Optimization return status: {0}{1}{1}", status, Environment.NewLine);
                output.AppendFormat("Objective function value f = {0}{1}", obj, Environment.NewLine);
                for (int i = 0; i < 3; ++i) output.AppendFormat("x[{0}]={1}{2}", i, x[i], Environment.NewLine);

                OptOutputTextBox.Text = output.ToString();
            }
            catch (Exception exc)
            {
                OptOutputTextBox.Text = exc.GetType().FullName + ": " + exc.Message + Environment.NewLine + exc.StackTrace;
            }
        }
    }
}
