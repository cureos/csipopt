// Copyright (c) 2012 Anders Gustafsson, Cureos AB.
// All rights reserved. This software and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html

using System;
using System.IO;
using Cureos.Numerics.Nlp;
using Cureos.Numerics.Support;
using NUnit.Framework;

namespace Cureos.Numerics
{
    [TestFixture]
    public class S225ManagedTests
    {
        #region Fields

        private S225 _instance;

        #endregion

        #region SetUp and TearDown

        [SetUp]
        public void Setup()
        {
            _instance = new S225();
        }

        [TearDown]
        public void Teardown()
        {
            _instance.Dispose();
            _instance = null;
        }

        #endregion

        #region Unit tests

        [Test]
        public void SolveProblem_SpecifiedStartGuess_ReturnSucceededStatus()
        {
            var x = new[] { 3.0, 1.0 };
            double obj;

            const IpoptReturnCode expected = IpoptReturnCode.Solve_Succeeded;
            var actual = _instance.SolveProblem(x, out obj, null, null, null, null);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SolveProblem_SpecifiedStartGuess_YieldsOptimalVariables()
        {
            var expected = new[] { 1.0, 1.0 };

            var actual = new[] { 3.0, 1.0 };
            double obj;
            _instance.SolveProblem(actual, out obj, null, null, null, null);

            CollectionAssert.AreEqual(expected, actual, new DoubleComparer(1.0e-5));
        }

        [Test]
        public void SolveProblem_SpecifiedStartGuess_YieldsOptimalObjectiveFunction()
        {
            var x = new[] { 3.0, 1.0 };

            const double expected = 2.0;
            double actual;
            _instance.SolveProblem(x, out actual, null, null, null, null);

            Assert.AreEqual(expected, actual, 2.0e-7);
        }

        [Test]
        public void AddOption_SpecifyingOutputFile_OutputFileGeneratedUponSolve()
        {
            const string logFileName = "s225.txt";
            if (File.Exists(logFileName)) File.Delete(logFileName);

            var x = new[] { 3.0, 1.0 };
            double obj;
            _instance.AddOption("output_file", logFileName);
            _instance.SolveProblem(x, out obj, null, null, null, null);

            const bool expected = true;
            var actual = File.Exists(logFileName);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
