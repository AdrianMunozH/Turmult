using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public abstract class  Projectile : MonoBehaviour
{
    public float speed = 70f;
    public Transform target;
    //public float damage;
    // Start is called before the first frame update
    public List<Effect> effects;
    // vllt mit scriptableobject für die base stats von den effects 
    // sollte vermieden werden das hier jetzt tausende stats nochmal stehen
  

    // Start is called before the first frame update
    void Start()
    {
        foreach (var effect in effects)
        {
            if(target != null)
                effect.targetEnemy = target.gameObject.GetComponent<EnemyMovement>();
            
            if (effect._type == EffectType.DMG)
            {
                DmgEffect eff = (DmgEffect)effect;
                //eff.damage = damage; // brauche ich vllt gar nicht
            }

            if (effect._type == EffectType.AOE)
            {
                AoeEffect eff = (AoeEffect)effect;
                //eff.damage = damage;
                if(eff.AOE == null)
                    eff.AOE = GetComponent<SphereCollider>();
            }

            if (effect._type == EffectType.SLOW)
            {
                
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        // Falls Gegner schon tot oder Ende erreicht hat, Bullet zerstören
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float frameDistance = speed * Time.deltaTime;
        //Treffer!
        if (dir.magnitude <= frameDistance)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * frameDistance, Space.World);
    }
    public void Seek(Transform _target)
    {
        //TODO: SET EFFECTS OR SPEED
        target = _target;
    }

    public abstract void HitTarget();
}
