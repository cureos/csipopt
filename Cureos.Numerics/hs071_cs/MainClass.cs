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
            using (var hs071 = new HS071())
            {
                // Set some options.  The following ones are only examples,
                // they might not be suitable for your problem.
                hs071.AddOption("tol", 1e-7);
                hs071.AddOption("mu_strategy", "adaptive");
                hs071.AddOption("output_file", "hs071.txt");
                hs071.AddOption("print_user_options", "yes");

                // Solve the problem.
                status = hs071.SolveProblem(x, out obj, null, null, null, null);
            }

            Console.WriteLine("{0}{0}Optimization value: {1}, return status: {2}{0}{0}", Environment.NewLine, obj,
                              status);

            for (int i = 0; i < 4; ++i) Console.WriteLine("x[{0}]={1}", i, x[i]);

            /* Next, solve the Schittkowski no. 225 problem */
            x = new[] { 3.0, 1.0 };

            using (var s225 = new S225())
            {
                // Set some options.  The following ones are only examples,
                // they might not be suitable for your problem.
                s225.AddOption("tol", 1e-7);
                s225.AddOption("mu_strategy", "adaptive");
                s225.AddOption("output_file", "s225.txt");

                // Solve the problem.
                status = s225.SolveProblem(x, out obj, null, null, null, null);
            }

            Console.WriteLine("{0}{0}Optimization value: {1}, return status: {2}{0}{0}", Environment.NewLine, obj,
                              status);

            for (int i = 0; i < 2; ++i) Console.WriteLine("x[{0}]={1}", i, x[i]);

            Console.WriteLine("{0}Press <RETURN> to exit...", Environment.NewLine);
            Console.ReadLine();
        }
    }
}