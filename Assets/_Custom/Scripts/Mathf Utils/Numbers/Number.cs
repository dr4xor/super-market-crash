using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SappUnityUtils.Numbers
{
    public class Number<T>
    {
        private T value;

        public Number(T value)
        {
            this.value = value;
        }

        public static Type GetCalculatorType()
        {
            Type tType = typeof(T);
            Type calculatorType = null;
            if (tType == typeof(int))
            {
                calculatorType = typeof(Int32Calculator);
            }
            else if (tType == typeof(float))
            {
                calculatorType = typeof(FloatCalculator);
            }
            else if (tType == typeof(Vector3))
            {
                calculatorType = typeof(Vector3Calculator);
            }
            else if (tType == typeof(Vector2Double))
            {
                calculatorType = typeof(Vector2DoubleCalculator);
            }
            else
            {
                throw new InvalidCastException(String.Format("Unsupported Type- Type {0} does not have a partner implementation of interface ICalculator<T>", tType.Name));
            }
            return calculatorType;
        }



        private static ICalculator<T> fCalculator = null;



        public static ICalculator<T> Calculator
        {
            get
            {
                if (fCalculator == null)
                {
                    MakeCalculator();
                }
                return fCalculator;
            }
        }

        public static void MakeCalculator()
        {
            Type calculatorType = GetCalculatorType();
            fCalculator = Activator.CreateInstance(calculatorType) as ICalculator<T>;
        }


        public static int Compare(T a, T b)
        {
            return Calculator.Compare(a, b);
        }

        public static T Difference(T a, T b)
        {
            return Calculator.Difference(a, b);
        }

        public static T Divide(T a, T b)
        {
            return Calculator.Divide(a, b);
        }

        public static T Divide(T a, int scalar)
        {
            return Calculator.Divide(a, scalar);
        }

        public static T Inverse(T a)
        {
            return Calculator.Inverse(a);
        }

        public static T Multiply(T a, T b)
        {
            return Calculator.Multiply(a, b);
        }

        public static T Multiply(T a, int scalar)
        {
            return Calculator.Multiply(a, scalar);
        }

        public static T Sum(T a, T b)
        {
            return Calculator.Sum(a, b);
        }






        public static implicit operator Number<T>(T a)
        {
            return new Number<T>(a);
        }

        public static implicit operator T(Number<T> a)
        {
            return a.value;
        }

        public static Number<T> operator +(Number<T> a, Number<T> b)
        {
            return Sum(a, b);
        }
        public static Number<T> operator -(Number<T> a, Number<T> b)
        {
            return Difference(a, b);
        }
        public static bool operator >(Number<T> a, Number<T> b)
        {
            return Compare(a, b) > 0;
        }
        public static bool operator <(Number<T> a, Number<T> b)
        {
            return Compare(a, b) < 0;
        }
        public static bool operator ==(Number<T> a, Number<T> b)
        {
            return Compare(a, b) == 0;
        }
        public static bool operator !=(Number<T> a, Number<T> b)
        {
            return Compare(a, b) == 0;
        }
        public static Number<T> operator *(Number<T> a, Number<T> b)
        {
            return Multiply(a, b);
        }
        public static Number<T> operator *(Number<T> a, int scalar)
        {
            return Multiply(a, scalar);
        }
        public static Number<T> operator /(Number<T> a, Number<T> b)
        {
            return Divide(a, b);
        }
        public static Number<T> operator /(Number<T> a, int scalar)
        {
            return Divide(a, scalar);
        }


    }

}