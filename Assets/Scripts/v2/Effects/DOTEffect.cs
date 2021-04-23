using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTEffect : Effect
{
    public int ticks;
    public int tick;
    public float damagePerTick;

    private bool isTicking;
    // Start is called before the first frame update

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
                
                isTicking = false;
            }
            else
            {
                
                if(targetEnemy != null)
                    targetEnemy.TakeDamage(damagePerTick);
            }
        }
    }
}
