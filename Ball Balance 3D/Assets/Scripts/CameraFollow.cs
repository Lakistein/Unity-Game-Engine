﻿using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public Transform target;            // The position that that camera will be following.
	public float smoothing = 5f;        // The speed with which the camera will be following.
	
	Vector3 offset;                     // The initial offset from the target.
	void Start ()
	{
		// Calculate the initial offset.
		offset = transform.position - target.position;
		offset = offset * 0.1f;
	}
	
	void FixedUpdate ()
	{

		if (Input.GetKey (KeyCode.RightArrow)) {
			offset = Quaternion.Euler(0,Time.deltaTime*100,0)*offset;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			offset = Quaternion.Euler(0,-Time.deltaTime*100,0)*offset;

		}

		// Create a postion the camera is aiming for based on the offset from the target.
		Vector3 targetCamPos = target.position + offset;
		
		// Smoothly interpolate between the camera's current position and it's target position.
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
		transform.LookAt (target);
	}
}