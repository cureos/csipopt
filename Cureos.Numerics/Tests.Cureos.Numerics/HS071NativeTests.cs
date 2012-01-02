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
    public class HS071NativeTests
    {
        #region Fields

        private HS071 _instance;

        #endregion

        #region SetUp and TearDown

        [SetUp]
        public void Setup()
        {
            _instance = new HS071();
            _instance.AddOption("tol", 1e-7);
            _instance.AddOption("mu_strategy", "adaptive");
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
        public void SolveProblem_StandardOptions_SucceededReturnStatus()
        {
            double[] x = { 1.0, 5.0, 5.0, 1.0 };
            double obj;

            const IpoptReturnCode expected = IpoptReturnCode.Solve_Succeeded;
            var actual = _instance.SolveProblem(x, out obj, null, null, null, null);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SolveProblem_StandardOptions_OptimalObjectiveValueReturned()
        {
            double[] x = { 1.0, 5.0, 5.0, 1.0 };

            const double expected = 17.0140173;
            double actual;
            _instance.SolveProblem(x, out actual, null, null, null, null);
            Assert.AreEqual(expected, actual, 2.0e-7);
        }

        [Test]
        public void SolveProblem_StandardOptions_OptimalVectorReturned()
        {
            double[] actual = { 1.0, 5.0, 5.0, 1.0 };
            double obj;
            var expected = new[] { 1.0, 4.742994, 3.8211503, 1.3794082 };
            _instance.SolveProblem(actual, out obj, null, null, null, null);
            CollectionAssert.AreEqual(expected, actual, new DoubleComparer(1.0e-5));
        }

        [Test]
        public void AddOption_NameOfOutputFile_FileGenerated()
        {
            const string fileName = "hs071.txt";
            if (File.Exists(fileName)) File.Delete(fileName);
            _instance.AddOption("output_file", fileName);

            double[] x = { 1.0, 5.0, 5.0, 1.0 };
            double obj;
            _instance.SolveProblem(x, out obj, null, null, null, null);

            const bool expected = true;
            var actual = File.Exists(fileName);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddOption_PrintUserOptions_ListedInOutputFile()
        {
            const string fileName = "hs071.txt";
            if (File.Exists(fileName)) File.Delete(fileName);
            _instance.AddOption("output_file", fileName);
            _instance.AddOption("print_user_options", "yes");

            double[] x = { 1.0, 5.0, 5.0, 1.0 };
            double obj;
            _instance.SolveProblem(x, out obj, null, null, null, null);
            _instance.Dispose();

            const bool expected = true;
            var actual = File.ReadAllText(fileName).Contains("List of user-set options");
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
