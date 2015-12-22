using UnityEngine;
using System.Collections;

public class BoxRespawn : MonoBehaviour {
	Vector3 pos;
	Quaternion q;
	// Use this for initialization
	void Start () {
		pos = transform.position;
		q = transform.rotation;
		//pos = transform;
	}
	
	// Update is called once per frame
	void Update () {
	if (transform.position.y < -5) {
						transform.position = pos;
			transform.rotation = q;
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.velocity = Vector3.zero;
				
				}

	}
}
