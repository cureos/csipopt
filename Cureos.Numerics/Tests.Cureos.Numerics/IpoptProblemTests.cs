// Copyright (c) 2010-2012 Anders Gustafsson, Cureos AB.
// All rights reserved. Any unauthorised reproduction of this 
// material will constitute an infringement of copyright.

using System;
using System.IO;
using Cureos.Numerics.Nlp;
using NUnit.Framework;

namespace Cureos.Numerics
{
    [TestFixture]
    public class IpoptProblemTests
    {
        #region Unit tests

        [Test]
        public void Constructor_OptionsFileAvailable_UsingOptionsFileData()
        {
            const string logFileName = "hs071_opt.txt";
            if (File.Exists(logFileName)) File.Delete(logFileName);

            const string optFileName = "ipopt.opt";
            File.WriteAllLines(optFileName, new[] { String.Format("output_file {0}", logFileName) });

            var instance = new HS071();
            double obj;
            var x = new[] { 1.0, 5.0, 5.0, 1.0 };
            instance.SolveProblem(x, out obj);

            instance.Dispose();
            if (File.Exists(optFileName)) File.Delete(optFileName);

            const bool expected = true;
            var actual = File.Exists(logFileName);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OpenOutputFile_HighestPrintLevel_ProducesLargeLogFile()
        {
            const string logFileName = "hs040_dedicated.txt";
            if (File.Exists(logFileName)) File.Delete(logFileName);

            using (var instance = new HS040(false, false, false))
            {
                Assert.IsTrue(instance.OpenOutputFile(logFileName, 12));
                double obj;
                var x = new[] { 1.0, 1.0, 1.0, 1.0 };
                instance.SolveProblem(x, out obj);
            }
            const int expected = 10000;
            var actual = File.ReadAllText(logFileName).Length;
            Assert.Greater(actual, expected);
        }

        [Test]
        public void SetScaling_NonUnityVariableScaling_YieldsDifferentObjectiveValue()
        {
            // Get objective value for unity scaling
            double expected;
            using (var unity = new HS040(false, false, false))
            {
                var x = new[] { 0.8, 0.8, 0.8, 0.8 };
                unity.SolveProblem(x, out expected);
            }

            // Get objective value for non-unity scaling
            double actual;
            using (var scaled = new HS040(false, false, false))
            {
                var x = new[] { 0.8, 0.8, 0.8, 0.8 };
                Assert.IsTrue(scaled.SetScaling(1.0, new[] { 1.0e-6, 1.0e-3, 1.0, 1.0e3 }, new[] { 1.0, 1.0, 1.0 }));
                scaled.SolveProblem(x, out actual);
            }

            Assert.AreNotEqual(expected, actual);
        }

        [Test]
        public void SetScaling_NonUnityGradientScaling_YieldsDifferentObjectiveValue()
        {
            // Get objective value for unity scaling
            double expected;
            using (var unity = new HS040(false, false, false))
            {
                var x = new[] { 0.8, 0.8, 0.8, 0.8 };
                unity.SolveProblem(x, out expected);
            }

            // Get objective value for non-unity scaling
            double actual;
            using (var scaled = new HS040(false, false, false))
            {
                var x = new[] { 0.8, 0.8, 0.8, 0.8 };
                Assert.IsTrue(scaled.SetScaling(1.0, new[] { 1.0, 1.0, 1.0, 1.0 }, new[] { 1.0e-6, 1.0, 1.0e6 }));
                scaled.SolveProblem(x, out actual);
            }

            Assert.AreNotEqual(expected, actual);
        }

        #endregion
    }
}
