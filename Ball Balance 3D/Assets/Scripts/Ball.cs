using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
    public static Ball Instance;
	Camera cam;
	Vector3 camDir;
	public float accel = 10000;
    void Awake()
    {
        Instance = this;
    }
	void Start () {
		transform.position = new Vector3 (0, 2, 0);
		cam = GameObject.Find ("Main Camera").camera;
		camDir = new Vector3 ();
	}
	
	void Update () {
		camDir.Set (transform.position.x-cam.transform.position.x, 0, transform.position.z-cam.transform.position.z);
		camDir.Normalize ();
		if (transform.position.y < -5)
			ResetBall ();
	}

	void FixedUpdate()
	{
		if (Input.GetKey (KeyCode.W)) {
			rigidbody.AddForce(camDir.x*Time.deltaTime*accel, 0, camDir.z*Time.deltaTime*accel);
		}
		if (Input.GetKey (KeyCode.A)) {
			rigidbody.AddForce(-camDir.z*Time.deltaTime*accel, 0, camDir.x*Time.deltaTime*accel);
		}
		if (Input.GetKey (KeyCode.D)) {
			rigidbody.AddForce(camDir.z*Time.deltaTime*accel, 0, -camDir.x*Time.deltaTime*accel);
		}
		if (Input.GetKey (KeyCode.S)) {
			rigidbody.AddForce(-camDir.x*Time.deltaTime*accel, 0, -camDir.z*Time.deltaTime*accel);
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			ResetBall();
		}
	}

	void ResetBall()
	{
		//transform.position.Set (0, 2, 0);
		transform.position = new Vector3 (0, 2, 0);
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;

	}


}
