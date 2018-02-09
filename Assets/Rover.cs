using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine.Internal;
using UnityEngine.Events;
using UnityEngine.UI;
/*OPIS FORMATU  
 * nazwa tej aplikacji = RangeVisualizer 
 * Obiekt JSONA
 * {"target":aplikacja,
 *  "data":{
 *          "msgId":dowolne, ale inne niż poprzednia wiadomość. Zabeczpieczenie przed kilkakrotnym odebraniem UDP,
 *          "type": [rotate, move, detect],
 *          "id": id detektora int,
 *          "range": odległość od detektora float
 *          
 *          "dst": długośc przesunięcia przy move w m float
 *          
 *          "angle": kąt w kątach 0-360 float
 *          }
 * }
 *          
 * 
 * 
 */

public class Rover : MonoBehaviour {
	public static Detector[] detectors;
    public static List<GameObject> lines = new List<GameObject>();
    public static List<GameObject> points = new List<GameObject>();
	public GameObject point;
	public GameObject line;
	public Text log;
    public Text pointDataIndicator;
	public static String lastMsg = "";
    public static List<int> ids = new List<int>();
	// Use this for initialization
	public Detector getDetectorById(int id){
		return detectors.Where (x => x.id == id).ToList()[0];
	}
	void Start () {
		detectors = GetComponentsInChildren<Detector> ();
		foreach (var d in detectors) {
			d.lineObj = line;
			d.point = point;
			d.log = log;
		}

		IPEndPoint localpt = new IPEndPoint (IPAddress.Any, 5555);

		UdpClient socket = new UdpClient(); // `new UdpClient()` to auto-pick port
		socket.Client.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		// schedule the first receive operation:
		socket.Client.Bind(localpt);
		socket.BeginReceive(new AsyncCallback(OnUdpData), socket);

		// sending data (for the sake of simplicity, back to ourselves):
		IPEndPoint target = new IPEndPoint(IPAddress.Parse("127.0.0.1"),5555);
		// send a couple of sample messages:
        if (true){ //może jednak nie wysyłaj? xd
            for (int num = 1; num <= 3; num++) {

                byte[] message3= Encoding.ASCII.GetBytes("{\"target\":\"RangeVisualizer\",\"data\":{\"type\":\"detect\",\"id\":0, \"range\":1, \"dst\":1, \"angle\":15, \"msgId\":0}}");

                socket.Send(message3, message3.Length, target);
                byte[] message = Encoding.ASCII.GetBytes("{\"target\":\"RangeVisualizer\",\"data\":{\"type\":\"rotate\",\"id\":0, \"range\":1, \"dst\":1, \"angle\":15, \"msgId\":1}}");

                socket.Send(message, message.Length, target);

                byte[] message2 = Encoding.ASCII.GetBytes("{\"target\":\"RangeVisualizer\",\"data\":{\"type\":\"detect\",\"id\":0, \"range\":1, \"dst\":1, \"angle\":15, \"msgId\":2}}");

                socket.Send(message2, message2.Length, target);
            }
        }

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Q)) {
			Camera.main.GetComponent<GhostFreeRoamCamera> ().enabled = !Camera.main.GetComponent<GhostFreeRoamCamera> ().enabled;
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			Camera.main.GetComponent<GhostFreeRoamCamera> ().enabled = false;
			Camera.main.orthographic = !Camera.main.orthographic;
		}
	}
    public void RemoveAll()
    {
        foreach (var l in lines)
        {
            Destroy(l.gameObject);
        }
        foreach (var d in detectors)
        {
            d.points.Clear();
        }
        foreach (var p in points)
        {
            Destroy(p.gameObject);
        }
    }
	static void OnUdpData(IAsyncResult result) {
		// this is what had been passed into BeginReceive as the second parameter:
		UdpClient socket = result.AsyncState as UdpClient;
		// points towards whoever had sent the message:
		IPEndPoint source = new IPEndPoint(0, 0);
		// get the actual message and fill out the source:
		byte[] message = socket.EndReceive(result, ref source);
		// do what you'd like with `message` here:
		Debug.Log("Got " + message.Length + " bytes from " + source);
		var msg = Encoding.ASCII.GetString(message);
		Debug.Log (msg);
		if (msg != lastMsg) {
			lastMsg = msg;
			var parsed = TinyJson.JsonParser.FromJson<object> (msg);
			Debug.Log (((Dictionary<string,object>)parsed) ["target"] as String);
			if (((Dictionary<string,object>)parsed) ["target"] as String == "RangeVisualizer") {
				Debug.Log ("true");
                
				var data = ((Dictionary<string,object>)parsed) ["data"];
                int id = (int)((Dictionary<string, object>)data)["msgId"];
                if (!ids.Contains(id))

                {
                    ids.Add(id);
                    if (((Dictionary<string, object>)data)["type"] as String == "detect")
                {
                    Debug.Log("a");
                    int index = (int)((Dictionary<string, object>)data)["id"];
                    
                   
                        float range;
                        Debug.Log("a");
                        try
                        {
                            range = (float)((Dictionary<string, object>)data)["range"];
                        }
                        catch (Exception e)
                        {
                            range = Convert.ToSingle(((int)((Dictionary<string, object>)data)["range"]));
                        }
                        Debug.Log("a");
                        Debug.Log(index);
                        Debug.Log(range);


                        UnityMainThreadDispatcher.Instance().Enqueue(() => detectors[index].detect(range));
                    }
                    else if (((Dictionary<string, object>)data)["type"] as String == "move")
                    {
                        float dst;
                        Debug.Log("a");
                        try
                        {
                            dst = (float)((Dictionary<string, object>)data)["x"];
                        }
                        catch (Exception e)
                        {
                            dst = Convert.ToSingle(((int)((Dictionary<string, object>)data)["x"]));
                        }

                        UnityMainThreadDispatcher.Instance().Enqueue(() => GameObject.FindObjectOfType<Rover>().transform.position = GameObject.FindObjectOfType<Rover>().transform.position + GameObject.FindObjectOfType<Rover>().transform.forward * dst);
                    }
                    else if (((Dictionary<string, object>)data)["type"] as String == "rotate")
                    {
                        float angle;
                        Debug.Log("a");
                        try
                        {
                            angle = (float)((Dictionary<string, object>)data)["angle"];
                        }
                        catch (Exception e)
                        {
                            angle = Convert.ToSingle(((int)((Dictionary<string, object>)data)["angle"]));
                        }

                        UnityMainThreadDispatcher.Instance().Enqueue(() => GameObject.FindObjectOfType<Rover>().transform.Rotate(0, angle, 0));
                    }
                }

            }
			
		}

		// schedule the next receive operation once reading is done:
		socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
	}
}
