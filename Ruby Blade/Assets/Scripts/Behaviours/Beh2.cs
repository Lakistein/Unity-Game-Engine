using UnityEngine;
using System.Collections;

public class Beh2 : AIBehaviour
{
    public float distance = 16;
    public float fleeDistance = 96;
    bool heal = false;
    PathApproach path = new PathApproach();

    protected override void Start()
    {
        base.Start();
    }

    public override void UpdateBehaviour(AIBehaviour mb, Entity e)
    {
        if(e.stunnedFor > 0)
        {
            e.stunnedFor -= Time.deltaTime;
            return;
        }
        float d = Vector2.Distance(Player.Instance.gameObject.transform.position, mb.gameObject.transform.position);
        if(e.HP > 20 && !heal)
        {
            if(distance < d)
            {
                mb.Current = path;
            }
            else
                mb.Current = Attack.INSTANCE;
        }
        else
        {
            heal = true;
            if(!particleSystem.isPlaying)
                particleSystem.Play();
            if(d < fleeDistance)
            {
                mb.Current = Flee.INSTANCE;
                particleSystem.Stop();
                particleSystem.Clear();
            }
            else
            {
                mb.Current = Heal.INSTANCE;
                if(e.HP >= e.maxHP)
                {
                    particleSystem.Stop();
                    particleSystem.Clear();
                    heal = false;
                }
            }
        }
    }
}
