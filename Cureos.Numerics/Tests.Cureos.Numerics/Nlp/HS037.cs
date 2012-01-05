// Copyright (C) 2010-2011 Anders Gustafsson and others. All Rights Reserved.
// This code is published under the Eclipse Public License.
//
// Author:  Anders Gustafsson, Cureos AB 2011-12-14

using System;
using System.Runtime.InteropServices;

namespace Cureos.Numerics.Nlp
{
    /// <summary>
    /// Implementation of Hock-Schittkowski problem no. 37 (Rosenbrock Post Office problem) 
    /// from the CUTE collection.
    /// Adapted from http://www.orfe.princeton.edu/~rvdb/ampl/nlmodels/cute/hs037.mod
    /// Optimal solution x* = { 24; 12; 12 }, f* = -3456
    /// </summary>
    public class HS037
    {
        public bool hasIntermediateBeenCalled;

        public int _n;
        public int _m;
        public int _nele_jac;
        public int _nele_hess;
        public double[] _x_L;
        public double[] _x_U;
        public double[] _g_L;
        public double[] _g_U;

        public HS037()
        {
            /* set the number of variables and allocate space for the bounds */
            /* set the values for the variable bounds */
            _n = 3;
            _x_L = new[] { 0.0, 0.0, 0.0 };
            _x_U = new[] { 42.0, 42.0, 42.0 };

            /* set the number of constraints and allocate space for the bounds */
            _m = 1;

            /* set the values of the constraint bounds */
            _g_L = new[] { 0.0 };
            _g_U = new[] { 72.0 };

            /* Number of nonzeros in the Jacobian of the constraints */
            _nele_jac = 3;

            /* Number of nonzeros in the Hessian of the Lagrangian (lower or
               upper triangual part only) */
            _nele_hess = 0;
        }

        [AllowReversePInvokeCalls]
        public IpoptBoolType eval_f(int n, double[] x, IpoptBoolType new_x, out double obj_value, IntPtr user_data)
        {
            obj_value = -x[0] * x[1] * x[2];
            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public IpoptBoolType eval_grad_f(int n, double[] x, IpoptBoolType new_x, double[] grad_f, IntPtr user_data)
        {
            grad_f[0] = -x[1] * x[2];
            grad_f[1] = -x[0] * x[2];
            grad_f[2] = -x[0] * x[1];
            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public IpoptBoolType eval_g(int n, double[] x, IpoptBoolType new_x, int m, double[] g, IntPtr user_data)
        {
            g[0] = x[0] + 2.0 * x[1] + 2.0 * x[2];
            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public IpoptBoolType eval_jac_g(int n, double[] x, IpoptBoolType new_x, int m, int nele_jac, 
            int[] iRow, int[] jCol, double[] values, IntPtr user_data)
        {
            if (values == null)
            {
                /* set the structure of the jacobian */
                /* this particular jacobian is dense */

                iRow[0] = 0;
                jCol[0] = 0;
                iRow[1] = 0;
                jCol[1] = 1;
                iRow[2] = 0;
                jCol[2] = 2;
            }
            else
            {
                /* return the values of the jacobian of the constraints */

                values[0] = 1.0; /* 0,0 */
                values[1] = 2.0; /* 0,1 */
                values[2] = 2.0; /* 0,2 */
            }

            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public IpoptBoolType eval_h(int n, double[] x, IpoptBoolType new_x, double obj_factor, int m, double[] lambda, 
            IpoptBoolType new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values, IntPtr user_data)
        {
            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public IpoptBoolType intermediate(IpoptAlgorithmMode alg_mod, int iter_count, double obj_value, double inf_pr, double inf_du,
            double mu, double d_norm, double regularization_size, double alpha_du, double alpha_pr, int ls_trials, IntPtr user_data)
        {
            hasIntermediateBeenCalled = true;
            return IpoptBoolType.True;
        }
    }
}
