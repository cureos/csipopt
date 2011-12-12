// Copyright (C) 2010 Anders Gustafsson and others. All Rights Reserved.
// This code is published under the Eclipse Public License.
//
// Author:  Anders Gustafsson, Cureos AB 2011-12-12

using System;
using System.Runtime.InteropServices;

namespace Cureos.Numerics
{
    #region ENUMERATIONS

    /// <summary>
    /// Return codes for the Solve call for an IPOPT problem 
    /// </summary>
    public enum IpoptReturnCode
    {
        Solve_Succeeded = 0,
        Solved_To_Acceptable_Level = 1,
        Infeasible_Problem_Detected = 2,
        Search_Direction_Becomes_Too_Small = 3,
        Diverging_Iterates = 4,
        User_Requested_Stop = 5,
        Feasible_Point_Found = 6,

        Maximum_Iterations_Exceeded = -1,
        Restoration_Failed = -2,
        Error_In_Step_Computation = -3,
        Maximum_CpuTime_Exceeded = -4,
        Not_Enough_Degrees_Of_Freedom = -10,
        Invalid_Problem_Definition = -11,
        Invalid_Option = -12,
        Invalid_Number_Detected = -13,

        Unrecoverable_Exception = -100,
        NonIpopt_Exception_Thrown = -101,
        Insufficient_Memory = -102,
        Internal_Error = -199
    }

    /// <summary>
    /// Enumeration to indicate the mode in which the algorithm is
    /// </summary>
    public enum IpoptAlgorithmMode
    {
        RegularMode = 0,
        RestorationPhaseMode = 1
    }

    #endregion

    public sealed class CsIpopt : IDisposable
    {
        #region FUNCTION POINTER DELEGATES

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public delegate int Eval_F_CB(
        [MarshalAs(UnmanagedType.I4)] int n, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] x,
        [MarshalAs(UnmanagedType.I4)] int new_x, [MarshalAs(UnmanagedType.R8)] out double obj_value, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public delegate int Eval_Grad_F_CB(
        [MarshalAs(UnmanagedType.I4)] int n, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] x,
        [MarshalAs(UnmanagedType.I4)] int new_x, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] grad_f, 
        IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public delegate int Eval_G_CB(
        [MarshalAs(UnmanagedType.I4)] int n, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] x,
        [MarshalAs(UnmanagedType.I4)] int new_x, [MarshalAs(UnmanagedType.I4)] int m,
        [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] double[] g, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public delegate int Eval_Jac_G_CB(
        [MarshalAs(UnmanagedType.I4)] int n, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] x,
        [MarshalAs(UnmanagedType.I4)] int new_x, [MarshalAs(UnmanagedType.I4)] int m, [MarshalAs(UnmanagedType.I4)] int nele_jac,
        [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] int[] iRow,
        [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] int[] jCol,
        [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] double[] values, IntPtr user_data);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public delegate int Eval_H_CB(
        [MarshalAs(UnmanagedType.I4)] int n, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] x,
        [MarshalAs(UnmanagedType.I4)] int new_x, [MarshalAs(UnmanagedType.R8)] double obj_factor, [MarshalAs(UnmanagedType.I4)] int m,
        [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] double[] lambda, [MarshalAs(UnmanagedType.I4)] int new_lambda, 
        [MarshalAs(UnmanagedType.I4)] int nele_hess,
        [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] int[] iRow,
        [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] int[] jCol,
        [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] double[] values, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public delegate int Intermediate_CB([MarshalAs(UnmanagedType.I4)] int alg_mod, [MarshalAs(UnmanagedType.I4)] int iter_count,
        [MarshalAs(UnmanagedType.R8)] double obj_value, [MarshalAs(UnmanagedType.R8)] double inf_pr,
        [MarshalAs(UnmanagedType.R8)] double inf_du, [MarshalAs(UnmanagedType.R8)] double mu,
        [MarshalAs(UnmanagedType.R8)] double d_norm, [MarshalAs(UnmanagedType.R8)] double regularization_size,
        [MarshalAs(UnmanagedType.R8)] double alpha_du, [MarshalAs(UnmanagedType.R8)] double alpha_pr,
        [MarshalAs(UnmanagedType.I4)] int ls_trials, IntPtr user_data);

        #endregion

        #region FIELDS

        private const string IpoptDllName = "Ipopt39";

        /// <summary>
        /// Value to indicate that a variable or constraint function has no upper bound 
        /// (provided that IPOPT option "nlp_upper_bound_inf" is less than 2e19)
        /// </summary>
        public const double PositiveInfinity =  2.0e19;

        /// <summary>
        /// Value to indicate that a variable or constraint function has no lower bound 
        /// (provided that IPOPT option "nlp_lower_bound_inf" is greater than -2e19)
        /// </summary>
        public const double NegativeInfinity = -2.0e19;

        public const int TRUE = 1;
        public const int FALSE = 0;

        private IntPtr m_problem;
        private bool m_disposed;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Constructor for creating a new Ipopt Problem object.  This function
        /// returns an object that can be passed to the IpoptSolve call.  It
        /// contains the basic definition of the optimization problem, such
        /// as number of variables and constraints, bounds on variables and
        /// constraints, information about the derivatives, and the callback
        /// function for the computation of the optimization problem
        /// functions and derivatives.  During this call, the options file
        /// PARAMS.DAT is read as well.
        /// </summary>
        /// <param name="n">Number of optimization variables</param>
        /// <param name="x_L">Lower bounds on variables. This array of size n is copied internally, so that the
        /// caller can change the incoming data after return without that IpoptProblem is modified.  Any value 
        /// less or equal than the number specified by option 'nlp_lower_bound_inf' is interpreted to be minus infinity.</param>
        /// <param name="x_U">Upper bounds on variables. This array of size n is copied internally, so that the
        /// caller can change the incoming data after return without that IpoptProblem is modified.  Any value 
        /// greater or equal than the number specified by option 'nlp_upper_bound_inf' is interpreted to be plus infinity.</param>
        /// <param name="m">Number of constraints.</param>
        /// <param name="g_L">Lower bounds on constraints. This array of size m is copied internally, so that the
        /// caller can change the incoming data after return without that IpoptProblem is modified.  Any value 
        /// less or equal than the number specified by option 'nlp_lower_bound_inf' is interpreted to be minus infinity.</param>
        /// <param name="g_U">Upper bounds on constraints. This array of size m is copied internally, so that the
        /// caller can change the incoming data after return without that IpoptProblem is modified.  Any value 
        /// greater or equal than the number specified by option 'nlp_upper_bound_inf' is interpreted to be plus infinity.</param>
        /// <param name="nele_jac">Number of non-zero elements in constraint Jacobian.</param>
        /// <param name="nele_hess">Number of non-zero elements in Hessian of Lagrangian.</param>
        /// <param name="eval_f">Callback function for evaluating objective function</param>
        /// <param name="eval_g">Callback function for evaluating constraint functions</param>
        /// <param name="eval_grad_f">Callback function for evaluating gradient of objective function</param>
        /// <param name="eval_jac_g">Callback function for evaluating Jacobian of constraint functions</param>
        /// <param name="eval_h">Callback function for evaluating Hessian of Lagrangian function</param>
        public CsIpopt(int n, double[] x_L, double[] x_U, int m, double[] g_L, double[] g_U, int nele_jac, int nele_hess,
            Eval_F_CB eval_f, Eval_G_CB eval_g, Eval_Grad_F_CB eval_grad_f, Eval_Jac_G_CB eval_jac_g, Eval_H_CB eval_h)
        {
            m_problem = CreateIpoptProblem(n, x_L, x_U, m, g_L, g_U, nele_jac, nele_hess, 0,
                                           eval_f, eval_g, eval_grad_f, eval_jac_g, eval_h);

            if (m_problem == IntPtr.Zero)
            {
                throw new ArgumentException("Failed to initialize IPOPT problem");
            }

            m_disposed = false;
        }

        /// <summary>
        /// Destructor for IPOPT problem
        /// </summary>
        ~CsIpopt()
        {
            Dispose(false);
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Implement the IDisposable interface to release the Ipopt DLL resources held by this class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Function for adding a string option.
        /// </summary>
        /// <param name="keyword">Name of option</param>
        /// <param name="val">String value of option</param>
        /// <returns>true if setting option succeeded, false if the option could not be set (e.g., if keyword is unknown)</returns>
        public bool AddOption(string keyword, string val)
        {
            return AddIpoptOption(m_problem, keyword, val) == TRUE;
        }

        /// <summary>
        /// Function for adding a floating point option.
        /// </summary>
        /// <param name="keyword">Name of option</param>
        /// <param name="val">Floating point value of option</param>
        /// <returns>true if setting option succeeded, false if the option could not be set (e.g., if keyword is unknown)</returns>
        public bool AddOption(string keyword, double val)
        {
            return AddIpoptOption(m_problem, keyword, val) == TRUE;
        }

        /// <summary>
        /// Function for adding an integer option.
        /// </summary>
        /// <param name="keyword">Name of option</param>
        /// <param name="val">Integer value of option</param>
        /// <returns>true if setting option succeeded, false if the option could not be set (e.g., if keyword is unknown)</returns>
        public bool AddOption(string keyword, int val)
        {
            return AddIpoptOption(m_problem, keyword, val) == TRUE;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Method for opening an output file for a given name with given printlevel.
        /// </summary>
        /// <param name="file_name">Name of output file</param>
        /// <param name="print_level">Level of printed information</param>
        /// <returns>False, if there was a problem opening the file.</returns>
        public bool OpenOutputFile(string file_name, int print_level)
        {
            return OpenIpoptOutputFile(m_problem, file_name, print_level) == TRUE;
        }
#endif

        /// <summary>
        /// Optional function for setting scaling parameter for the NLP.
        /// This corresponds to the get_scaling_parameters method in TNLP.
        /// If the pointers x_scaling or g_scaling are null, then no scaling
        /// for x resp. g is done.
        /// </summary>
        /// <param name="obj_scaling">Scaling of the objective function</param>
        /// <param name="x_scaling">Scaling of the problem variables</param>
        /// <param name="g_scaling">Scaling of the constraint functions</param>
        /// <returns>true if scaling succeeded, false otherwise</returns>
        public bool SetScaling(double obj_scaling, double[] x_scaling, double[] g_scaling)
        {
            return SetIpoptProblemScaling(m_problem, obj_scaling, x_scaling, g_scaling) == TRUE;
        }

        /// <summary>
        /// Setting a callback function for the "intermediate callback"
        /// method in the optimizer.  This gives control back to the user once
        /// per iteration.  If set, it provides the user with some
        /// information on the state of the optimization.  This can be used
        /// to print some user-defined output.  It also gives the user a way
        /// to terminate the optimization prematurely.  If the callback
        /// method returns false, Ipopt will terminate the optimization.
        /// Calling this set method to set the CB pointer to null disables
        /// the intermediate callback functionality.
        /// </summary>
        /// <param name="intermediate">Intermediate callback function</param>
        /// <returns>true if the callback function could be set successfully, false otherwise</returns>
        public bool SetIntermediateCallback(Intermediate_CB intermediate)
        {
            return SetIntermediateCallback(m_problem, intermediate) == TRUE;
        }

        /// <summary>
        /// Function calling the IPOPT optimization algorithm for a problem previously defined with the constructor.
        /// IPOPT will use the options previously specified with AddOption for this problem.
        /// </summary>
        /// <param name="x">Input: Starting point; Output: Optimal solution</param>
        /// <param name="obj_val">Final value of objective function (output only - ignored if null on input)</param>
        /// <param name="g">Values of constraint at final point (output only - ignored if null on input)</param>
        /// <param name="mult_g">Final multipliers for constraints (output only - ignored if null on input)</param>
        /// <param name="mult_x_L">Final multipliers for lower variable bounds (output only - ignored if null on input)</param>
        /// <param name="mult_x_U">Final multipliers for upper variable bounds (output only - ignored if null on input)</param>
        /// <returns>Outcome of the optimization procedure (e.g., success, failure etc).</returns>
        public IpoptReturnCode SolveProblem(double[] x, out double obj_val, double[] g, double[] mult_g, double[] mult_x_L, double[] mult_x_U)
        {
            return (IpoptReturnCode)IpoptSolve(m_problem, x, g, out obj_val, mult_g, mult_x_L, mult_x_U, IntPtr.Zero);
        }

        #endregion

        #region PRIVATE METHODS

        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                // Dispose unmanaged resources
                FreeIpoptProblem(m_problem);

                // Dispose managed resources
                if (disposing)
                {
                    m_problem = IntPtr.Zero;
                }

                m_disposed = true;
            }
        }

        #endregion

        #region DLL METHOD DECLARATIONS

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateIpoptProblem(
            [MarshalAs(UnmanagedType.I4)] int n, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] x_L,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] double[] x_U, [MarshalAs(UnmanagedType.I4)] int m,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] double[] g_L,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] double[] g_U, [MarshalAs(UnmanagedType.I4)] int nele_jac,
            [MarshalAs(UnmanagedType.I4)] int nele_hess, [MarshalAs(UnmanagedType.I4)] int index_style, 
            Eval_F_CB eval_f, Eval_G_CB eval_g, Eval_Grad_F_CB eval_grad_f, Eval_Jac_G_CB eval_jac_g, Eval_H_CB eval_h);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeIpoptProblem(IntPtr ipopt_problem);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddIpoptStrOption")]
        public static extern int AddIpoptOption(IntPtr ipopt_problem, string keyword, string val);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddIpoptNumOption")]
        public static extern int AddIpoptOption(IntPtr ipopt_problem, string keyword, double val);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddIpoptIntOption")]
        public static extern int AddIpoptOption(IntPtr ipopt_problem, string keyword, int val);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenIpoptOutputFile(IntPtr ipopt_problem, string file_name, int print_level);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetIpoptProblemScaling(IntPtr ipopt_problem, [MarshalAs(UnmanagedType.R8)] double obj_scaling,
            [MarshalAs(UnmanagedType.LPArray)] double[] x_scaling, [MarshalAs(UnmanagedType.LPArray)] double[] g_scaling);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetIntermediateCallback(IntPtr ipopt_problem, Intermediate_CB intermediate_cb);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IpoptSolve(
            IntPtr ipopt_problem, [MarshalAs(UnmanagedType.LPArray)] double[] x, [MarshalAs(UnmanagedType.LPArray)] double[] g,
            [MarshalAs(UnmanagedType.R8)] out double obj_val, [MarshalAs(UnmanagedType.LPArray)] double[] mult_g,
            [MarshalAs(UnmanagedType.LPArray)] double[] mult_x_L, [MarshalAs(UnmanagedType.LPArray)] double[] mult_x_U, IntPtr user_data);

        #endregion
    }
}
