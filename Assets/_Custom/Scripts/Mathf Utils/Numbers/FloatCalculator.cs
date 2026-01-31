namespace SappUnityUtils.Numbers
{
    public struct FloatCalculator : ICalculator<float>
    {
        public int Compare(float a, float b)
        {
            if (a == b)
                return 0;
            if (a > b)
                return 1;
            return -1;
        }

        public float Difference(float a, float b)
        {
            return a - b;
        }

        public float Divide(float a, float b)
        {
            return a / b;
        }

        public float Divide(float a, int scalar)
        {
            return a / scalar;
        }

        public float Inverse(float a)
        {
            return -a;
        }

        public float Multiply(float a, float b)
        {
            return a * b;
        }

        public float Multiply(float a, int scalar)
        {
            return a * scalar;
        }

        public float Sum(float a, float b)
        {
            return a + b;
        }
    }

}