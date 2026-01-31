using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SappUnityUtils.Numbers
{
    public class Vector3Calculator : ICalculator<Vector3>
    {
        public int Compare(Vector3 a, Vector3 b)
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

        public Vector3 Difference(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        public Vector3 Divide(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public Vector3 Divide(Vector3 a, int scalar)
        {
            return a / scalar;
        }

        public Vector3 Inverse(Vector3 a)
        {
            return -a;
        }

        public Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return Vector3.Cross(a, b);
        }

        public Vector3 Multiply(Vector3 a, int scalar)
        {
            return a * scalar;
        }

        public Vector3 Sum(Vector3 a, Vector3 b)
        {
            return a + b;
        }
    }
}
