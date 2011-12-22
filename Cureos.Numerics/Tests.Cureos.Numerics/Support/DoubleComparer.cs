// Copyright (c) 2010-2011 Anders Gustafsson, Cureos AB.
// All rights reserved. Any unauthorised reproduction of this 
// material will constitute an infringement of copyright.

using System;
using System.Collections;

namespace Cureos.Numerics.Support
{
    public class DoubleComparer : IComparer
    {
        #region FIELDS

        private readonly double _absTol;

        #endregion
        
        #region CONSTRUCTORS

        public DoubleComparer(double absTol)
        {
            _absTol = absTol;
        }

        #endregion
        
        #region Implementation of IComparer

        public int Compare(object x, object y)
        {
            var xd = (double)x;
            var yd = (double)y;
            return Math.Abs(xd - yd) < _absTol ? 0 : xd.CompareTo(yd);
        }
        
        #endregion
    }
}