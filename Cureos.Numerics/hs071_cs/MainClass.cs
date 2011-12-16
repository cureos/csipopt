// Copyright (C) 2010-2011 Anders Gustafsson and others. All Rights Reserved.
// This code is published under the Eclipse Public License.
//
// Author:  Anders Gustafsson, Cureos AB 2011-12-13

using System;
using Cureos.Numerics;

namespace hs071_cs
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
            /* allocate space for the initial point and set the values */
            double[] x = { 1.0, 5.0, 5.0, 1.0 };

            IpoptReturnCode status;
            double obj;
            using (var problem = new HS071())
            {
                // Set some options.  The following ones are only examples,
                // they might not be suitable for your problem.
                problem.AddOption("tol", 1e-7);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("output_file", "hs071.txt");
                problem.AddOption("print_user_options", "yes");

                // Solve the problem.
                status = problem.SolveProblem(x, out obj, null, null, null, null);
            }

            Console.WriteLine("{0}{0}Optimization value: {1}, return status: {2}{0}{0}", Environment.NewLine, obj,
                              status);

            for (int i = 0; i < 4; ++i) Console.WriteLine("x[{0}]={1}", i, x[i]);

            Console.WriteLine("{0}Press <RETURN> to exit...", Environment.NewLine);
            Console.ReadLine();
        }
    }
}