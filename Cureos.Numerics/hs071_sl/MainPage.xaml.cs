// Copyright (c) 2011 Anders Gustafsson, Cureos AB.
// All rights reserved. This software and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html

using System;
using System.Text;
using System.Windows;
using Cureos.Numerics;

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
            /* create the IpoptProblems */
            var p = new HS071();
            var r = new RosenbrockPostOffice();

            /* allocate space for the initial point and set the values */
            double[] x = { 1.0, 5.0, 5.0, 1.0 };

            try
            {
                var output = new StringBuilder();

                double obj;
                var problem = Ipopt.CreateIpoptProblem(p._n, p._x_L, p._x_U, p._m, p._g_L, p._g_U, p._nele_jac,
                                                         p._nele_hess, IpoptIndexStyle.C,
                                                         p.eval_f, p.eval_g, p.eval_grad_f, p.eval_jac_g, p.eval_h);

                // Set some options.  The following ones are only examples,
                // they might not be suitable for your problem.
                Ipopt.AddIpoptNumOption(problem, "tol", 1e-7);
                Ipopt.AddIpoptStrOption(problem, "mu_strategy", "adaptive");

                // Solve the problem.
                var status = Ipopt.IpoptSolve(problem, x, null, out obj, null, null, null, IntPtr.Zero);

                // Free problem resources.
                Ipopt.FreeIpoptProblem(problem);

                output.AppendLine("HS071");
                output.AppendLine("=====");
                output.AppendFormat("Optimization return status: {0}{1}{1}", status, Environment.NewLine);
                output.AppendFormat("Objective function value f = {0}{1}", obj, Environment.NewLine);
                for (int i = 0; i < 4; ++i) output.AppendFormat("x[{0}]={1}{2}", i, x[i], Environment.NewLine);
                output.AppendLine();

                // Also optimize Rosenbrock post office problem
                problem = Ipopt.CreateIpoptProblem(r._n, r._x_L, r._x_U, r._m, r._g_L, r._g_U, r._nele_jac,
                                                         r._nele_hess, IpoptIndexStyle.C,
                                                         r.eval_f, r.eval_g, r.eval_grad_f, r.eval_jac_g, r.eval_h);
                Ipopt.AddIpoptStrOption(problem, "hessian_approximation", "limited-memory");
                Ipopt.AddIpoptIntOption(problem, "limited_memory_max_history", 5);
#if INTERMEDIATE
                Ipopt.SetIntermediateCallback(problem, r.intermediate);
#endif

                x = new[] { 10.0, 10.0, 10.0 };
                status = Ipopt.IpoptSolve(problem, x, null, out obj, null, null, null, IntPtr.Zero);

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
