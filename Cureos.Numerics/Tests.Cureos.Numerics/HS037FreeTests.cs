// Copyright (c) 2012 Anders Gustafsson, Cureos AB.
// All rights reserved. This software and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html

using System;
using Cureos.Numerics.Nlp;
using Cureos.Numerics.Support;
using NUnit.Framework;

namespace Cureos.Numerics
{
    [TestFixture]
    public class HS037FreeTests
    {
        #region Fields

        private HS037 _hs037;
        private IntPtr _instance;

        #endregion

        #region SetUp and TearDown

        [SetUp]
        public void Setup()
        {
            _hs037 = new HS037();
            _instance = IpoptAdapter.CreateIpoptProblem(_hs037._n, _hs037._x_L, _hs037._x_U, _hs037._m, _hs037._g_L,
                                                        _hs037._g_U, _hs037._nele_jac, _hs037._nele_hess, IpoptIndexStyle.C,
                                                        _hs037.eval_f, _hs037.eval_g, _hs037.eval_grad_f, _hs037.eval_jac_g,
                                                        _hs037.eval_h);
            IpoptAdapter.AddIpoptStrOption(_instance, "hessian_approximation", "limited-memory");
            IpoptAdapter.AddIpoptIntOption(_instance, "limited_memory_max_history", 5);
        }

        [TearDown]
        public void Teardown()
        {
            IpoptAdapter.FreeIpoptProblem(_instance);
            _hs037 = null;
        }

        #endregion

        #region Unit tests

        [Test]
        public void IpoptSolve_SpecifiedStartGuess_ReturnsSucceededStatus()
        {
            var x = new[] { 10.0, 10.0, 10.0 };
            double obj;

            const IpoptReturnCode expected = IpoptReturnCode.Solve_Succeeded;
            var actual = IpoptAdapter.IpoptSolve(_instance, x, null, out obj, null, null, null, IntPtr.Zero);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IpoptSolve_SpecifiedStartGuess_YieldsExpectedOptimalVariables()
        {
            var actual = new[] { 10.0, 10.0, 10.0 };
            double obj;

            var expected = new[] { 24.0, 12.0, 12.0 };
            IpoptAdapter.IpoptSolve(_instance, actual, null, out obj, null, null, null, IntPtr.Zero);
            CollectionAssert.AreEqual(expected, actual, new DoubleComparer(1.0e-5));
        }

        [Test]
        public void IpoptSolve_SpecifiedStartGuess_YieldsExpectedOptimalObjectiveValue()
        {
            const double expected = -3456.0;

            var x = new[] { 10.0, 10.0, 10.0 };
            double actual;
            IpoptAdapter.IpoptSolve(_instance, x, null, out actual, null, null, null, IntPtr.Zero);

            Assert.AreEqual(expected, actual, 1.0e-3);
        }

        [Test]
        public void SetIntermediateCallback_CallbackFunctionDefined_CallbackFunctionCalled()
        {
            const bool expected = true;
            IpoptAdapter.SetIntermediateCallback(_instance, _hs037.intermediate);
            _hs037.hasIntermediateBeenCalled = false;

            var x = new[] { 10.0, 10.0, 10.0 };
            double obj;
            IpoptAdapter.IpoptSolve(_instance, x, null, out obj, null, null, null, IntPtr.Zero);
            var actual = _hs037.hasIntermediateBeenCalled;

            Assert.AreEqual(expected, actual);
        }
        // TODO Add test to verify that intermediate function is called

        #endregion
    }
}
