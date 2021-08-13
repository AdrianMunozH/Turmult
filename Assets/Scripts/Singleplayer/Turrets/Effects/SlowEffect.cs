namespace Singleplayer.Turrets.Effects
{
    public class SlowEffect : Effect
    {
        public float slow;

        public int ticks;
        public int tick;

        private bool isTicking;
        // Start is called before the first frame update
        void Start()
        {
            _type = EffectType.SLOW;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void Hit()
        {
            tick = 0;
            isTicking = true;
            TimeTickSystem.OnTick += TimeTickSystem_OnTick;
        }
    
        private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (isTicking)
            {
                tick += 1;
                if (tick >= ticks)
                {

                    if (targetEnemy != null)
                        targetEnemy.isSlowed = false;
                    isTicking = false;
                }
                else
                {
                
                    if(targetEnemy != null)
                        targetEnemy.Slow(slow);
                }
            }
        }
    }
}
