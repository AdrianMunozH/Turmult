using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTEffect : Effect
{
    public int ticks;
    public int tick;
    public float damagePerTick;

    private bool _isTicking;
    // Start is called before the first frame update
    private void Start()
    {
        _type = EffectType.DOT;
    }

    public override void Hit()
    {
        tick = 0;
        _isTicking = true;
        TimeTickSystem.OnTick += TimeTickSystem_OnTick;
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        if (_isTicking)
        {
            tick += 1;
            if (tick >= ticks)
            {
                
                _isTicking = false;
            }
            else
            {
                
                if(targetEnemy != null)
                    targetEnemy.TakeDamage(damagePerTick);
            }
        }
    }
}
