namespace SappUnityUtils.Numbers
{
    public class MovingAverage<T>
    {
        private TimeMeasure timeMeasure;

        private bool _isDestructed;
        private T[] values;
        private float[] valueTimes;
        private int index = 0;

        public MovingAverage()
        {
            initTimeMeasure();
            values = new T[256];
            initValueTimes();
        }

        public MovingAverage(int arrayLen)
        {
            initTimeMeasure();
            values = new T[arrayLen];
            initValueTimes();
        }

        public void DestructObject()
        {
            if (timeMeasure != null)
            {
                timeMeasure.StopMeasure();
            }

            timeMeasure   = null;
            _isDestructed = true;
        }

        private void initTimeMeasure()
        {
            timeMeasure = new TimeMeasure();
        }
        private void initValueTimes()
        {
            valueTimes = new float[values.Length];
            for (int i = 0; i < valueTimes.Length; i++)
            {
                valueTimes[i] = float.PositiveInfinity;
            }
        }

        public void AddValue(T val)
        {
            if (_isDestructed)
            {
                return;
            }

            values[index] = val;
            index++;
            index %= values.Length;

            if (timeMeasure.IsMeasuring)
            {
                valueTimes[index] = timeMeasure.StopMeasure();
            }
            timeMeasure.StartMeasure();
        }

        public T AverageValue(float pastTime)
        {
            Number<T> summed = sumValuesOverPastTime(pastTime, out int summedCount);

            return summed / summedCount;
        }

        public T SummedValue(float pastTime)
        {
            Number<T> summed = sumValuesOverPastTime(pastTime, out int summedCount);

            return summed;
        }

        private Number<T> sumValuesOverPastTime(float pastTime, out int valuesCount)
        {
            Number<T> summed = values[index];
            float summedTime = valueTimes[index];
            int summedCount = 1;
            for (int i = 1; i < values.Length; i++)
            {
                if (summedTime >= pastTime)
                {
                    break;
                }

                int adjIndex = index - i;
                if (adjIndex < 0)
                    adjIndex += values.Length;

                summedTime += valueTimes[adjIndex];
                summed = summed + values[adjIndex];
                summedCount++;
            }

            valuesCount = summedCount;

            return summed;
        }

        public T GetValueInPast(float timeInPast)
        {
            float summedTime = 0f;

            for (int i = 0; i < values.Length; i++)
            {
                int adjIndex = index - i;
                if (adjIndex < 0)
                    adjIndex += values.Length;

                summedTime += valueTimes[adjIndex];

                if (summedTime >= timeInPast)
                {
                    return values[adjIndex];
                }
            }

            // Return the oldest value
            return values[(index + 1) % values.Length];
        }
    }
}