using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Detector : MonoBehaviour {
	public int id;
	public Text log;
	public GameObject point;
	public GameObject lineObj;
    public Rover rover;
   
	public List<GameObject> points = new List<GameObject> ();
    private void Start()
    {
        rover = GameObject.FindObjectOfType<Rover>().GetComponent<Rover>();
    }
    public void detect(float distance){
		log.text = log.text + "\n\r reading from: " + id.ToString () + ":" + distance.ToString ()+"m";
		Vector3 detectorPos = transform.position;
		Vector3 detectorDirection = transform.forward;
		Quaternion rotation = transform.rotation;

		Vector3 spawnPos = detectorPos + detectorDirection*distance;
        var newPoint= Instantiate(point, spawnPos, rotation);
        Rover.points.Add(newPoint);
        point.GetComponent<Point>().detector = this.gameObject;
        point.GetComponent<Point>().distance = distance;
        points.Add(newPoint);
		var line = Instantiate (lineObj, spawnPos, rotation);
        Rover.lines.Add(line);

		line.GetComponent<LineRenderer> ().SetPosition (0, transform.position);
		line.GetComponent<LineRenderer> ().SetPosition (1, spawnPos);
	}

}
