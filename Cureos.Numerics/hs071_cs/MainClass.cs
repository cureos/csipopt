// Copyright (C) 2010 Anders Gustafsson and others. All Rights Reserved.
// This code is published under the Eclipse Public License.
//
// Author:  Anders Gustafsson, Cureos AB 2010-12-07

using System;
using Cureos.Numerics;

namespace hs071_cs
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
            /* create the IpoptProblem */
            HS071 p = new HS071();

            /* allocate space for the initial point and set the values */
            double[] x = { 1.0, 5.0, 5.0, 1.0 };

            double obj;
            var problem = Ipopt.CreateIpoptProblem(p._n, p._x_L, p._x_U, p._m, p._g_L, p._g_U, p._nele_jac,
                                                     p._nele_hess, 0,
                                                     p.eval_f, p.eval_g, p.eval_grad_f, p.eval_jac_g, p.eval_h);

            // Set some options.  The following ones are only examples,
            // they might not be suitable for your problem.
            Ipopt.AddIpoptOption(problem, "tol", 1e-7);
            Ipopt.AddIpoptOption(problem, "mu_strategy", "adaptive");
            Ipopt.AddIpoptOption(problem, "output_file", "hs071.txt");
#if INTERMEDIATE
            Ipopt.SetIntermediateCallback(problem, p.intermediate);
#endif
            // Solve the problem.
            IpoptReturnCode status =
                (IpoptReturnCode)Ipopt.IpoptSolve(problem, x, null, out obj, null, null, null, IntPtr.Zero);

            // Free problem resources.
            Ipopt.FreeIpoptProblem(problem);

            Console.WriteLine("{0}{0}Optimization value: {1}, return status: {2}{0}{0}", Environment.NewLine, obj,
                              status);

            for (int i = 0; i < 4; ++i) Console.WriteLine("x[{0}]={1}", i, x[i]);

            Console.WriteLine("{0}Press <RETURN> to exit...", Environment.NewLine);
            Console.ReadLine();
        }
    }
}