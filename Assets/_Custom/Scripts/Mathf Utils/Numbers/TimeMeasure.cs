using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SappUnityUtils.Numbers
{
    public class TimeMeasure : ITimeDeltaRec
    {
        private bool isMeasuring = false;

        public TimeMeasure()
        {

        }

        ~TimeMeasure()
        {
            StopMeasure();
        }

        public void StartMeasure()
        {
            if (!isMeasuring)
            {
                isMeasuring = true;
                
                if (TimeMeasureBehaviour.Instance != null)
                {
                    TimeMeasureBehaviour.Instance.Register(this);
                }
                PassedTime = 0f;
            }
        }

        public float StopMeasure()
        {
            if (isMeasuring)
            {
                if (TimeMeasureBehaviour.Instance != null)
                {
                    TimeMeasureBehaviour.Instance?.DeRegister(this);
                }
                isMeasuring = false;
            }
            return PassedTime;
        }

        public void UpdateTimeDelta(float timeDelta)
        {
            PassedTime += timeDelta;
        }

        public bool IsMeasuring
        {
            get
            {
                return isMeasuring;
            }
        }

        public float PassedTime
        {
            get; protected set;
        } = 0f;
    }

}
