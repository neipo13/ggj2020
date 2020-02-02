using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NDream.AirConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AirPlayTestController : MonoBehaviour {

    public Transform LineRendererParent;

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("sending draw");
            AirConsole.instance.Message(1, new SimpleMessage{action="draw"});
            AirConsole.instance.Message(2, new SimpleMessage{action="wait"});
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            AirConsole.instance.Message(1, new SimpleMessage{action="win"});
            AirConsole.instance.Message(2, new SimpleMessage{action="lose"});
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            AirConsole.instance.Message(1, new SimpleMessage{action="vote"});
            AirConsole.instance.Message(2, new SimpleMessage{action="vote"});
        }
    }

    List<GameObject> lineObjs = new List<GameObject>();

    void OnMessage (int from, JToken message) {
        //We check if the message I receive has an "action" parameter and if it's a swipe
        if (message["action"] != null && message["action"].ToString () == "send-image-data") {
            //We log the whole vector to see its values 
            var imgData = JsonConvert.DeserializeObject<ImageDataModels> (message.ToString ());

            List<List<ImageData>> lines = new List<List<ImageData>> ();
            List<ImageData> currentLine = new List<ImageData> ();
            for (int i = 0; i < imgData.data.Count; i++) {
                var current = imgData.data[i];
                if ((current.dragging && i < (imgData.data.Count - 1)) || currentLine.Count == 0) {
                    currentLine.Add (current);
                } else {
                    lines.Add (currentLine);
                    currentLine = new List<ImageData> ();
                    currentLine.Add (current);
                }
            }
            
            //clear old objects
            foreach(var obj in lineObjs)
            {
                Destroy(obj);
            }
            lineObjs.Clear();

            for (int j = 0; j < lines.Count; j++) {
                var line = lines[j];

                //create a gameobject
                var obj = new GameObject ("Line_" + j);
                obj.transform.parent = LineRendererParent;

                // z offset is because new lines need to render on top of old
                obj.transform.localPosition = new Vector3(0, 0, -j * 0.1f);
                obj.transform.localScale = Vector3.one;

                //obj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
                lineObjs.Add(obj);

                //create a line renderer component
                var lineRenderer = obj.AddComponent<LineRenderer> ();
                lineRenderer.material = new Material (Shader.Find ("Sprites/Default"));
                lineRenderer.SetWidth(0.05f, 0.05f);
                Color color;
                ColorUtility.TryParseHtmlString (line.First ().color, out color);
                lineRenderer.material.color = color;

                //plot all the points on the line & set color/thickness
                lineRenderer.positionCount = line.Count;
                lineRenderer.useWorldSpace = false;
                for (int k = 0; k < line.Count; k++) {
                    var point = line[k];
                    lineRenderer.SetPosition (k, new Vector3 (point.x / 100f, -point.y / 100f, 10f));
                }
            }

        }
    }
}