using UnityEngine;
using System.Collections;

public class Beh3 : AIBehaviour
{
	public float distance = 32*5;
	PathApproach path = new PathApproach();
	public override void UpdateBehaviour(AIBehaviour mb, Entity e)
	{
		if (e.stunnedFor > 0) {
			e.stunnedFor -= Time.deltaTime;
			return;
		}
		float d = Vector2.Distance(Player.Instance.gameObject.transform.position, mb.gameObject.transform.position);
		if (distance < d)
		{
			mb.Current = path;
		}
		else
			mb.Current = Shoot.INSTANCE;
		
	}
	
}