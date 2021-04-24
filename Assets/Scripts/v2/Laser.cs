using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform target;
    public float damage;
    public List<Effect> effects;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var effect in effects)
        {
            if(target != null)
                effect.targetEnemy = target.gameObject.GetComponent<EnemyMovement>();
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            HitTarget();

    }
    public void Seek(Transform _target)
    {
        //TODO: SET EFFECTS OR SPEED
        target = _target;
    }

    private void HitTarget()
    {
        foreach (var effect in effects)
        {
            effect.targetEnemy = target.GetComponent<EnemyMovement>();
            effect.Hit();
        }
    }
}
