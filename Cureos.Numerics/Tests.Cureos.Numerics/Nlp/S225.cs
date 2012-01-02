// Copyright (C) 2010-2011 Anders Gustafsson and others. All Rights Reserved.
// This code is published under the Eclipse Public License.
//
// Author:  Anders Gustafsson, Cureos AB 2010-12-20

namespace Cureos.Numerics.Nlp
{
    /// <summary>
    /// C# implementation of Schittkowski problem no. 225
    /// Adapted from http://www.orfe.princeton.edu/~rvdb/ampl/nlmodels/s/s225.mod
    /// </summary>
    public class S225 : IpoptProblem
    {
        public S225()
            : base(
            2,  // Number of variables
            new[] { NegativeInfinity, NegativeInfinity },   // Lower variable bounds
            new[] { PositiveInfinity, PositiveInfinity },   // Upper varable bounds
            5,  // Number of constraints
            new[] { 1.0, 1.0, 9.0, 0.0, 0.0 },  // Lower constraint bounds
            new[] { PositiveInfinity, PositiveInfinity, PositiveInfinity, PositiveInfinity, PositiveInfinity }, // Upper constraint bounds
            10, // Number of elements in (sparse) Jacobian
            0,  // Number of elements in (sparse) Hessian; 0 since Hessian approximation is applied
            false,  // Use native callback functions?
            true,   // Use limited-memory Hessian approximation?
            false)  // Use intermediate callback function?
        {
        }

        public override bool eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            obj_value = x[0] * x[0] + x[1] * x[1];

            return true;
        }

        public override bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            grad_f[0] = 2.0 * x[0];
            grad_f[1] = 2.0 * x[1];

            return true;
        }

        public override bool eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            g[0] = x[0] + x[1];
            g[1] = x[0] * x[0] + x[1] * x[1];
            g[2] = 9.0 * x[0] * x[0] + x[1] * x[1];
            g[3] = x[0] * x[0] - x[1];
            g[4] = x[1] * x[1] - x[0];

            return true;
        }

        public override bool eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                /* set the structure of the jacobian */
                /* this particular jacobian is dense */

                iRow[0] = 0;
                jCol[0] = 0;
                iRow[1] = 0;
                jCol[1] = 1;
                iRow[2] = 1;
                jCol[2] = 0;
                iRow[3] = 1;
                jCol[3] = 1;
                iRow[4] = 2;
                jCol[4] = 0;
                iRow[5] = 2;
                jCol[5] = 1;
                iRow[6] = 3;
                jCol[6] = 0;
                iRow[7] = 3;
                jCol[7] = 1;
                iRow[8] = 4;
                jCol[8] = 0;
                iRow[9] = 4;
                jCol[9] = 1;
            }
            else
            {
                /* return the values of the jacobian of the constraints */

                values[0] = 1.0;
                values[1] = 1.0;
                values[2] = 2.0 * x[0];
                values[3] = 2.0 * x[1];
                values[4] = 18.0 * x[0];
                values[5] = 2.0 * x[1];
                values[6] = 2.0 * x[0];
                values[7] = -1.0;
                values[8] = -1.0;
                values[9] = 2.0 * x[0];
            }

            return true;
        }
    }
}
