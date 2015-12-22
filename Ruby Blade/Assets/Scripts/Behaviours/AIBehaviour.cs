using UnityEngine;
using System.Collections;

public class AIBehaviour : MonoBehaviour
{
    Entity entity;

    public State Current { get;	set;}
		
    protected virtual void Start()
    {
        entity = gameObject.GetComponent<Entity>();
    }

    void Update()
    {
        UpdateBehaviour(this, entity);
        if (Current != null)
            Current.Update(entity);
    }
    public virtual void UpdateBehaviour(AIBehaviour b, Entity entity)
	{
    }
}
