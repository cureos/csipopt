// Copyright (c) 2010-2011 Anders Gustafsson, Cureos AB.
// All rights reserved. Any unauthorised reproduction of this 
// material will constitute an infringement of copyright.

using Cureos.Numerics.Nlp;
using Cureos.Numerics.Support;
using NUnit.Framework;

namespace Cureos.Numerics
{
    [TestFixture]
    public class NativeSubclassingTests
    {
        #region Unit tests

        [Test]
        public void SolveProblem_HessianApproximation_ReturnsSucceededStatus()
        {
            var instance = new HS040(true, true, false);
            var x = new[] { 0.8, 0.8, 0.8, 0.8 };

            const IpoptReturnCode expected = IpoptReturnCode.Solve_Succeeded;
            double obj;
            var actual = instance.SolveProblem(x, out obj, null, null, null, null);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SolveProblem_HessianApproximation_ReturnsTrueOptimalValue()
        {
            var instance = new HS040(true, true, false);
            var x = new[] { 0.8, 0.8, 0.8, 0.8 };

            const double expected = -0.25;
            double actual;
            instance.SolveProblem(x, out actual, null, null, null, null);

            Assert.AreEqual(expected, actual, 1e-7);
        }

        [Test]
        public void SolveProblem_HessianApproximation_ConstraintsSatisfied()
        {
            var instance = new HS040(true, true, false);
            var x = new[] { 0.8, 0.8, 0.8, 0.8 };

            var expected = new[] { 1.0, 0.0, 0.0 };
            double obj;
            var actual = new double[3];
            instance.SolveProblem(x, out obj, actual, null, null, null);

            CollectionAssert.AreEqual(expected, actual, new DoubleComparer(1.0e-5));
        }

        [Test]
        public void SolveProblem_HessianApproximation_VariablesEqualOptimalValues()
        {
            var instance = new HS040(true, true, false);
            var actual = new[] { 0.8, 0.8, 0.8, 0.8 };

            var expected = new[] { 0.793701, 0.707107, 0.529732, 0.840896 };
            double obj;
            instance.SolveProblem(actual, out obj, null, null, null, null);

            CollectionAssert.AreEqual(expected, actual, new DoubleComparer(1.0e-5));
        }

        [Test]
        public void SolveProblem_HessianApproximation_NumberOfIterationsSufficientlyLow()
        {
            var instance = new HS040(true, true, true);
            var x = new[] { 0.8, 0.8, 0.8, 0.8 };

            const int expected = 20;
            double obj;
            instance.SolveProblem(x, out obj, null, null, null, null);
            var actual = instance.NumberIterations;

            Assert.Less(actual, expected);
        }

        #endregion
    }
}
