using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float speed = 0.05f;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position = this.transform.position + transform.forward * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position = this.transform.position + transform.right*-1 * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position = this.transform.position + transform.right * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position = this.transform.position + transform.forward*-1 * speed;
        }
    }
}
