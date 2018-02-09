using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Point : MonoBehaviour {
    public Text label;
    public GameObject detector;
    public float distance;
    private void Start()
    {
        label = GameObject.Find("pointDataIndicator").GetComponent<Text>();
    }
    private void OnMouseOver()
    {
        label.text = "At measurement: "+distance.ToString()+"; Now:"+Vector3.Distance(detector.transform.position, transform.position);
    }
    private void OnMouseExit()
    {
        label.text = "";
    }
}
