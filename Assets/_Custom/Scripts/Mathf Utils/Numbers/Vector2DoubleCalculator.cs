using SappUnityUtils.Numbers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SappUnityUtils.Numbers
{
    public class Vector2DoubleCalculator : ICalculator<Vector2Double>
    {
        public int Compare(Vector2Double a, Vector2Double b)
        {
            if (a == b)
            {
                return 0;
            }
            if (a.sqrMagnitude > b.sqrMagnitude)
            {
                return 1;
            }
            return -1;
        }

        public Vector2Double Difference(Vector2Double a, Vector2Double b)
        {
            return a - b;
        }

        public Vector2Double Divide(Vector2Double a, Vector2Double b)
        {
            return new Vector2Double(a.x / b.x, a.y / b.y);
        }

        public Vector2Double Divide(Vector2Double a, int scalar)
        {
            return a / scalar;
        }

        public Vector2Double Inverse(Vector2Double a)
        {
            return -a;
        }

        public Vector2Double Multiply(Vector2Double a, Vector2Double b)
        {
            return Vector2Double.Cross(a, b);
        }

        public Vector2Double Multiply(Vector2Double a, int scalar)
        {
            return a * scalar;
        }

        public Vector2Double Sum(Vector2Double a, Vector2Double b)
        {
            return a + b;
        }
    }
}