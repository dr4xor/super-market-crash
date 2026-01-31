using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SappUnityUtils.Numbers
{
    public class TimeMeasureBehaviour : MonoBehaviour
    {
        private List<ITimeDeltaRec> listeners = new List<ITimeDeltaRec>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].UpdateTimeDelta(Time.deltaTime);
            }
        }


        public void Register(ITimeDeltaRec listener)
        {
            listeners.Add(listener);
        }

        public void DeRegister(ITimeDeltaRec listener)
        {
            listeners.Remove(listener);
        }





        private static TimeMeasureBehaviour inst = null;
        public static TimeMeasureBehaviour Instance
        {
            get
            {
                if (inst == null)
                {
                    GameObject instObj = new GameObject("Time Measure");
                    inst = instObj.AddComponent<TimeMeasureBehaviour>();

                    DontDestroyOnLoad(inst);
                }
                return inst;
            }
        }
    }

}