using System;
using UnityEngine;

namespace Singleplayer.Turrets
{
    public class TimeTickSystem : MonoBehaviour
    {
        public class OnTickEventArgs : EventArgs
        {
            public int tick;
        }
        public static event EventHandler<OnTickEventArgs> OnTick;
        public static event EventHandler<OnTickEventArgs> OnTick_5;
    
    
        private const float TICK_TIMER_MAX = 0.2f;


        private int tick;

        private float tickTimer;

        private void Awake()
        {
            tick = 0;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_TIMER_MAX)
            {
                tickTimer -= TICK_TIMER_MAX;
                tick++;
                if(OnTick != null) OnTick(this,new OnTickEventArgs{tick = tick});
                if (tick % 5 == 0)
                {
                    if(OnTick_5 != null) OnTick_5(this,new OnTickEventArgs{tick = tick});
                }
            }
        }
    }
}
