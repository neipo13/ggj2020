using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NDream.AirConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AirPlayTestController : MonoBehaviour {

    public Transform LineRendererParent;
    public GameLoop gameLoop;
    public Painting painting;

    public List<StrokeData> CurrentStrokeData = new List<StrokeData>();
    public LineRenderer CurrentLine;
    public int LineVertexCount;

    public static AirPlayTestController I;

    private bool newStroke;
    private StrokeData point;

    List<GameObject> lineObjs = new List<GameObject>();

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;
        I = this;
    }

    public void StartRecording()
    {
        StartCoroutine(RecordingCo());
    }

    IEnumerator RecordingCo()
    {
        // Show the painting for a single frame, just so it gets drawn to the
        // render texture, then we need to hide it, or it'll clobber the line
        // renderers
        painting.Show();
        yield return new WaitForEndOfFrame();
        painting.Hide();

        while(true)
        {
            yield return new WaitForEndOfFrame();

            // Trim the line back down to the smallest it can be
            if (CurrentLine != null && CurrentLine.positionCount >= 2)
            {
                var last = CurrentLine.GetPosition(CurrentLine.positionCount - 1);
                CurrentLine.positionCount = 1;
                CurrentLine.SetPosition(0, last);
                LineVertexCount = 1;
            }
        }
    }

    void OnMessage (int from, JToken message) {

        string action = message["action"].ToString();

        if(action == "send-stroke-data")
        {
            point = JsonConvert.DeserializeObject<StrokeData>(message["stroke-data"].ToString());
            //CurrentStrokeData.Add(strokeData);
            //RenderLine(CurrentStrokeData);

            if(newStroke)
            {
                ColorUtility.TryParseHtmlString(point.color, out Color color);
                CurrentLine.material.color = color;
                newStroke = false;
            }

            if (CurrentLine != null)
            {
                CurrentLine.positionCount = LineVertexCount + 1;
                CurrentLine.SetPosition(LineVertexCount, new Vector3(point.x / 100f, -point.y / 100f, 10f));
                LineVertexCount++;
            }
        }

        if(action == "new-stroke")
        {
            if (CurrentLine == null)
            {
                CurrentLine = SpawnNewLine();
                CurrentLine.positionCount = 0;
            }

            newStroke = true;
        }
    }

    public void Clear()
    {
        //clear old objects
        foreach (var obj in lineObjs)
        {
            Destroy(obj);
        }
        lineObjs.Clear();
    }

    private LineRenderer SpawnNewLine()
    {
        //CurrentStrokeData = new List<StrokeData>();
        LineVertexCount = 0;

        //create a gameobject
        var obj = new GameObject("Line");
        obj.transform.parent = LineRendererParent;
        obj.layer = 8;

        obj.transform.localPosition = new Vector3(0, 0, 0.1f);
        obj.transform.localScale = Vector3.one;

        //obj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        lineObjs.Add(obj);

        //create a line renderer component
        var line = obj.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        //lineRenderer.SetWidth(0.05f, 0.05f);

        // I scaled up the image
        line.SetWidth(0.075f, 0.075f);

        //CurrentLine.material.color = color;

        //plot all the points on the line & set color/thickness
        line.useWorldSpace = false;

        return line;
    }
}