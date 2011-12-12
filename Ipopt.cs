// Copyright (C) 2010 Anders Gustafsson and others. All Rights Reserved.
// This code is published under the Eclipse Public License.
//
// Author:  Anders Gustafsson, Cureos AB 2010-12-08

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

    #region PUBLIC DELEGATES

    /// <summary>
    /// Delegate defining the callback function for evaluating the value of
    /// the objective function.  Return value should be set to false if
    /// there was a problem doing the evaluation.
    /// </summary>
    /// <param name="n">Number of problem variables</param>
    /// <param name="x">Problem variables</param>
    /// <param name="new_x">true if problem variables are new for this call, false otherwise</param>
    /// <param name="obj_value">Evaluated objective function value</param>
    /// <returns>true if evaluation succeeded, false otherwise</returns>
    public delegate bool EvaluateObjectiveDelegate(int n, double[] x, bool new_x, out double obj_value);

    /// <summary>
    /// Delegate defining the callback function for evaluating the gradient of
    /// the objective function.  Return value should be set to false if
    /// there was a problem doing the evaluation.
    /// </summary>
    /// <param name="n">Number of problem variables</param>
    /// <param name="x">Problem variables</param>
    /// <param name="new_x">true if problem variables are new for this call, false otherwise</param>
    /// <param name="grad_f">Objective function gradient</param>
    /// <returns>true if evaluation succeeded, false otherwise</returns>
    public delegate bool EvaluateObjectiveGradientDelegate(int n, double[] x, bool new_x, double[] grad_f);

    /// <summary>
    /// Delegate defining the callback function for evaluating the value of
    /// the constraint functions.  Return value should be set to false if
    /// there was a problem doing the evaluation.
    /// </summary>
    /// <param name="n">Number of problem variables</param>
    /// <param name="x">Problem variables</param>
    /// <param name="new_x">true if problem variables are new for this call, false otherwise</param>
    /// <param name="m">Number of constraint functions</param>
    /// <param name="g">Calculated values of the constraint functions</param>
    /// <returns>true if evaluation succeeded, false otherwise</returns>
    public delegate bool EvaluateConstraintsDelegate(int n, double[] x, bool new_x, int m, double[] g);

    /// <summary>
    /// Delegate defining the callback function for evaluating the Jacobian of
    /// the constraint functions.  Return value should be set to false if
    /// there was a problem doing the evaluation.
    /// </summary>
    /// <param name="n">Number of problem variables</param>
    /// <param name="x">Problem variables</param>
    /// <param name="new_x">true if problem variables are new for this call, false otherwise</param>
    /// <param name="m">Number of constraint functions</param>
    /// <param name="nele_jac">Number of non-zero elements in the Jacobian</param>
    /// <param name="iRow">Row indices of the non-zero Jacobian elements, defined here if values is null</param>
    /// <param name="jCol">Column indices of the non-zero Jacobian elements, defined here if values is null</param>
    /// <param name="values">Values of the non-zero Jacobian elements</param>
    /// <returns>true if evaluation succeeded, false otherwise</returns>
    public delegate bool EvaluateJacobianDelegate(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values);

    /// <summary>
    /// Delegate defining the callback function for evaluating the Hessian of
    /// the Lagrangian function.  Return value should be set to false if
    /// there was a problem doing the evaluation.
    /// </summary>
    /// <param name="n">Number of problem variables</param>
    /// <param name="x">Problem variables</param>
    /// <param name="new_x">true if problem variables are new for this call, false otherwise</param>
    /// <param name="obj_factor">Multiplier for the objective function in the Lagrangian</param>
    /// <param name="m">Number of constraint functions</param>
    /// <param name="lambda">Multipliers for the constraint functions in the Lagrangian</param>
    /// <param name="new_lambda">true if lambda values are new for this call, false otherwise</param>
    /// <param name="nele_hess">Number of non-zero elements in the Hessian</param>
    /// <param name="iRow">Row indices of the non-zero Hessian elements, defined here if values is null</param>
    /// <param name="jCol">Column indices of the non-zero Hessian elements, defined here if values is null</param>
    /// <param name="values">Values of the non-zero Hessian elements</param>
    /// <returns>true if evaluation succeeded, false otherwise</returns>
    public delegate bool EvaluateHessianDelegate(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, 
    int nele_hess, int[] iRow, int[] jCol, double[] values);

    /// <summary>
    /// Delegate defining the callback function for giving intermediate
    /// execution control to the user.  If set, it is called once per
    /// iteration, providing the user with some information on the state
    /// of the optimization.  This can be used to print some
    /// user-defined output.  It also gives the user a way to terminate
    /// the optimization prematurely.  If this method returns false,
    /// Ipopt will terminate the optimization.
    /// </summary>
    /// <param name="alg_mod">Current Ipopt algorithm mode</param>
    /// <param name="iter_count">Current iteration count</param>
    /// <param name="obj_value">The unscaled objective value at the current point</param>
    /// <param name="inf_pr">The scaled primal infeasibility at the current point</param>
    /// <param name="inf_du">The scaled dual infeasibility at the current point</param>
    /// <param name="mu">The barrier parameter value at the current point</param>
    /// <param name="d_norm">The infinity norm (max) of the primal step 
    /// (for the original variables x and the internal slack variables s)</param>
    /// <param name="regularization_size">Value of the regularization term for the Hessian of the Lagrangian 
    /// in the augmented system</param>
    /// <param name="alpha_du">The stepsize for the dual variables</param>
    /// <param name="alpha_pr">The stepsize for the primal variables</param>
    /// <param name="ls_trials">The number of backtracking line search steps</param>
    /// <returns>true if the optimization should proceeded after callback return, false if
    /// the optimization should be terminated prematurely</returns>
    public delegate bool IntermediateDelegate(IpoptAlgorithmMode alg_mod, int iter_count, double obj_value, double inf_pr, double inf_du,
    double mu, double d_norm, double regularization_size, double alpha_du, double alpha_pr, int ls_trials);

    #endregion

    public sealed class Ipopt : IDisposable
    {
        #region INTERNAL DELEGATES - BRIDGES BETWEEN DLL INTERFACE AND C# CALLBACK FUNCTIONS

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe private delegate int Eval_F_CB(int n, double* x, int new_x, double* obj_value, void* user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe private delegate int Eval_Grad_F_CB(int n, double* x, int new_x, double* grad_f, void* user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe private delegate int Eval_G_CB(int n, double* x, int new_x, int m, double* g, void* user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe private delegate int Eval_Jac_G_CB(int n, double* x, int new_x, int m, int nele_jac, int* iRow, int* jCol, double* values, void* user_data);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe private delegate int Eval_H_CB(int n, double* x, int new_x, double obj_factor, int m, double* lambda, int new_lambda, int nele_hess,
            int* iRow, int* jCol, double* values, void* user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe private delegate int Intermediate_CB(int alg_mod, int iter_count, double obj_value, double inf_pr, double inf_du,
            double mu, double d_norm, double regularization_size, double alpha_du, double alpha_pr, int ls_trials, void* user_data);

        #endregion

        #region INTERNAL CALLBACK FUNCTION CLASSES

        private class ObjectiveEvaluator
        {
            private readonly EvaluateObjectiveDelegate m_eval_f;

            internal ObjectiveEvaluator(EvaluateObjectiveDelegate eval_f)
            {
                m_eval_f = eval_f;
            }

            unsafe internal int Evaluate(int n, double* p_x, int new_x, double* p_obj_value, void* user_data)
            {
                double[] x = CreateArray(n, p_x);
                return m_eval_f(n, x, new_x == TRUE, out *p_obj_value) ? TRUE : FALSE;
            }
        }

        private class ConstraintsEvaluator
        {
            private readonly EvaluateConstraintsDelegate m_eval_g;

            internal ConstraintsEvaluator(EvaluateConstraintsDelegate eval_g)
            {
                m_eval_g = eval_g;
            }

            unsafe internal int Evaluate(int n, double* p_x, int new_x, int m, double* p_g, void* user_data)
            {
                double[] x = CreateArray(n, p_x);
                double[] g = new double[m];

                int ret = m_eval_g(n, x, new_x == TRUE, m, g) ? TRUE : FALSE;
                CopyToPointer(m, g, p_g);

                return ret;
            }
        }

        private class ObjectiveGradientEvaluator
        {
            private readonly EvaluateObjectiveGradientDelegate m_eval_grad_f;

            internal ObjectiveGradientEvaluator(EvaluateObjectiveGradientDelegate eval_grad_f)
            {
                m_eval_grad_f = eval_grad_f;
            }

            unsafe internal int Evaluate(int n, double* p_x, int new_x, double* p_grad_f, void* user_data)
            {
                double[] x = CreateArray(n, p_x);
                double[] grad_f = new double[n];

                int ret = m_eval_grad_f(n, x, new_x == TRUE, grad_f) ? TRUE : FALSE;
                CopyToPointer(n, grad_f, p_grad_f);

                return ret;
            }
        }

        private class JacobianEvaluator
        {
            private readonly EvaluateJacobianDelegate m_eval_jac_g;

            internal JacobianEvaluator(EvaluateJacobianDelegate eval_jac_g)
            {
                m_eval_jac_g = eval_jac_g;
            }

            unsafe internal int Evaluate(int n, double* p_x, int new_x, int m, int nele_jac, int* p_iRow, int* p_jCol, double* p_values, void* user_data)
            {
                double[] x = CreateArray(n, p_x);
                int[] iRow = CreateArray(nele_jac, p_iRow);
                int[] jCol = CreateArray(nele_jac, p_jCol);
                double[] values = CreateArray(nele_jac, p_values);

                int ret = m_eval_jac_g(n, x, new_x == TRUE, m, nele_jac, iRow, jCol, values) ? TRUE : FALSE;
                CopyToPointer(nele_jac, iRow, p_iRow);
                CopyToPointer(nele_jac, jCol, p_jCol);
                CopyToPointer(nele_jac, values, p_values);

                return ret;
            }
        }

        private class HessianEvaluator
        {
            private readonly EvaluateHessianDelegate m_eval_h;

            internal HessianEvaluator(EvaluateHessianDelegate eval_h)
            {
                m_eval_h = eval_h;
            }

            unsafe internal int Evaluate(int n, double* p_x, int new_x, double obj_factor, int m, double* p_lambda, int new_lambda, int nele_hess,
                int* p_iRow, int* p_jCol, double* p_values, void* user_data)
            {
                double[] x = CreateArray(n, p_x);
                double[] lambda = CreateArray(m, p_lambda);
                int[] iRow = CreateArray(nele_hess, p_iRow);
                int[] jCol = CreateArray(nele_hess, p_jCol);
                double[] values = CreateArray(nele_hess, p_values);

                int ret = m_eval_h(n, x, new_x == TRUE, obj_factor, m, lambda, new_lambda == TRUE, nele_hess, iRow, jCol, values) ? TRUE : FALSE;
                CopyToPointer(nele_hess, iRow, p_iRow);
                CopyToPointer(nele_hess, jCol, p_jCol);
                CopyToPointer(nele_hess, values, p_values);

                return ret;
            }
        }

        private class IntermediateReporter
        {
            private readonly IntermediateDelegate m_intermediate;

            internal IntermediateReporter(IntermediateDelegate intermediate)
            {
                m_intermediate = intermediate;
            }

            unsafe internal int Report(int alg_mod, int iter_count, double obj_value, double inf_pr, double inf_du,
                double mu, double d_norm, double regularization_size, double alpha_du, double alpha_pr, int ls_trials, void* user_data)
            {
                return m_intermediate((IpoptAlgorithmMode)alg_mod, iter_count, obj_value, inf_pr, inf_du, mu, d_norm, 
                    regularization_size, alpha_du, alpha_pr, ls_trials) ? TRUE : FALSE;
            }
        }

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

        private const int TRUE = 1;
        private const int FALSE = 0;

        private IntPtr m_problem;
        private bool m_disposed;

        private readonly Eval_F_CB m_eval_f;
        private readonly Eval_G_CB m_eval_g; 
        private readonly Eval_Grad_F_CB m_eval_grad_f;
        private readonly Eval_Jac_G_CB m_eval_jac_g;
        private readonly Eval_H_CB m_eval_h;
        private Intermediate_CB m_intermediate;

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
        public Ipopt(int n, double[] x_L, double[] x_U, int m, double[] g_L, double[] g_U, int nele_jac, int nele_hess,
            EvaluateObjectiveDelegate eval_f, EvaluateConstraintsDelegate eval_g, EvaluateObjectiveGradientDelegate eval_grad_f, 
            EvaluateJacobianDelegate eval_jac_g, EvaluateHessianDelegate eval_h)
        {
            unsafe
            {
                fixed (double* p_x_L = x_L, p_x_U = x_U, p_g_L = g_L, p_g_U = g_U)
                {
                    m_eval_f = new ObjectiveEvaluator(eval_f).Evaluate;
                    m_eval_g = new ConstraintsEvaluator(eval_g).Evaluate; 
                    m_eval_grad_f = new ObjectiveGradientEvaluator(eval_grad_f).Evaluate;
                    m_eval_jac_g = new JacobianEvaluator(eval_jac_g).Evaluate;
                    m_eval_h = new HessianEvaluator(eval_h).Evaluate;
                    m_intermediate = null;

                    m_problem = CreateIpoptProblem(n, p_x_L, p_x_U, m, p_g_L, p_g_U, nele_jac, nele_hess, 0,
                        m_eval_f, m_eval_g, m_eval_grad_f, m_eval_jac_g, m_eval_h);

                    if (m_problem == IntPtr.Zero)
                    {
                        throw new ArgumentException("Failed to initialize IPOPT problem");
                    }
                }
            }

            m_disposed = false;
        }

        /// <summary>
        /// Destructor for IPOPT problem
        /// </summary>
        ~Ipopt()
        {
            DisposeImpl();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Implement the IDisposable interface to release the Ipopt DLL resources held by this class
        /// </summary>
        public void Dispose()
        {
            DisposeImpl();
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
            return AddIpoptStrOption(m_problem, keyword, val) == TRUE;
        }

        /// <summary>
        /// Function for adding a floating point option.
        /// </summary>
        /// <param name="keyword">Name of option</param>
        /// <param name="val">Floating point value of option</param>
        /// <returns>true if setting option succeeded, false if the option could not be set (e.g., if keyword is unknown)</returns>
        public bool AddOption(string keyword, double val)
        {
            return AddIpoptNumOption(m_problem, keyword, val) == TRUE;
        }

        /// <summary>
        /// Function for adding an integer option.
        /// </summary>
        /// <param name="keyword">Name of option</param>
        /// <param name="val">Integer value of option</param>
        /// <returns>true if setting option succeeded, false if the option could not be set (e.g., if keyword is unknown)</returns>
        public bool AddOption(string keyword, int val)
        {
            return AddIpoptIntOption(m_problem, keyword, val) == TRUE;
        }

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
            unsafe
            {
                fixed (double* p_x_scaling = x_scaling, p_g_scaling = g_scaling)
                {
                    return SetIpoptProblemScaling(m_problem, obj_scaling, p_x_scaling, p_g_scaling) == TRUE;
                }
            }

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
        public bool SetIntermediateCallback(IntermediateDelegate intermediate)
        {
            unsafe
            {
                m_intermediate = new IntermediateReporter(intermediate).Report;
                return SetIntermediateCallback(m_problem, m_intermediate) == TRUE;
            }
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
            unsafe
            {
                fixed (double* p_obj_val = &obj_val, p_x = x, p_g = g, p_mult_g = mult_g, p_mult_x_L = mult_x_L, p_mult_x_U = mult_x_U)
                {
                    return (IpoptReturnCode)IpoptSolve(m_problem, p_x, p_g, p_obj_val, p_mult_g, p_mult_x_L, p_mult_x_U, null);
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void DisposeImpl()
        {
            if (!m_disposed)
            {
                FreeIpoptProblem(m_problem);
                m_problem = IntPtr.Zero;
                m_disposed = true;
            }
        }

        unsafe private static void CopyToPointer(int n, double[] x, double* p_x)
        {
            if (x != null && p_x != null) Marshal.Copy(x, 0, (IntPtr)p_x, n);
        }

        unsafe private static void CopyToPointer(int n, int[] x, int* p_x)
        {
            if (x != null && p_x != null) Marshal.Copy(x, 0, (IntPtr)p_x, n);
        }

        unsafe private static double[] CreateArray(int n, double* p_x)
        {
            if (p_x == null) return null;

            double[] x = new double[n];
            Marshal.Copy((IntPtr)p_x, x, 0, n);
            return x;
        }

        unsafe private static int[] CreateArray(int n, int* p_x)
        {
            if (p_x == null) return null;

            int[] x = new int[n];
            Marshal.Copy((IntPtr)p_x, x, 0, n);
            return x;
        }

        #endregion

        #region DLL METHOD DECLARATIONS

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        unsafe private static extern IntPtr CreateIpoptProblem(int n, double* x_L, double* x_U, int m, double* g_L, double* g_U,  int nele_jac, 
            int nele_hess, int index_style, Eval_F_CB eval_f, Eval_G_CB eval_g, Eval_Grad_F_CB eval_grad_f, Eval_Jac_G_CB eval_jac_g, Eval_H_CB eval_h);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreeIpoptProblem(IntPtr ipopt_problem);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int AddIpoptStrOption(IntPtr ipopt_problem, string keyword, string val);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int AddIpoptNumOption(IntPtr ipopt_problem, string keyword, double val);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int AddIpoptIntOption(IntPtr ipopt_problem, string keyword, int val);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int OpenIpoptOutputFile(IntPtr ipopt_problem, string file_name, int print_level);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        unsafe private static extern int SetIpoptProblemScaling(IntPtr ipopt_problem, double obj_scaling, double* x_scaling, double* g_scaling);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetIntermediateCallback(IntPtr ipopt_problem, Intermediate_CB intermediate_cb);

        [DllImport(IpoptDllName, CallingConvention = CallingConvention.Cdecl)]
        unsafe private static extern int IpoptSolve(
            IntPtr ipopt_problem, double* x, double* g, double* obj_val, double* mult_g, double* mult_x_L, double* mult_x_U, void* user_data);

        #endregion
    }
}
