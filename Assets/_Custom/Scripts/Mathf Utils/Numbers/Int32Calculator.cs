namespace SappUnityUtils.Numbers
{
    public struct Int32Calculator : ICalculator<int>
    {
        public int Compare(int a, int b)
        {
            if (a == b)
                return 0;
            if (a > b)
                return 1;
            return -1;
        }

        public int Difference(int a, int b)
        {
            return a - b;
        }

        public int Divide(int a, int b)
        {
            return a / b;
        }

        public int Inverse(int a)
        {
            return -a;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }

        public int Sum(int a, int b)
        {
            return a + b;
        }
    }

}