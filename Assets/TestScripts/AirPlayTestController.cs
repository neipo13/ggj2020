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

    public List<StrokeData> CurrentStrokeData = new List<StrokeData>();
    public LineRenderer CurrentLine;
    public int NumLines;
    public int LineVertexCount;

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;
    }

    List<GameObject> lineObjs = new List<GameObject>();

    void OnMessage (int from, JToken message) {

        string action = message["action"].ToString();

        if(action == "send-stroke-data")
        {
            var point = JsonConvert.DeserializeObject<StrokeData>(message["stroke-data"].ToString());
            //CurrentStrokeData.Add(strokeData);
            //RenderLine(CurrentStrokeData);

            if(LineVertexCount == 0)
            {
                Color color = Color.red;
                ColorUtility.TryParseHtmlString(point.color, out color);
                CurrentLine.material.color = color;
            }

            CurrentLine.positionCount = LineVertexCount + 1;
            CurrentLine.SetPosition(LineVertexCount, new Vector3(point.x / 100f, -point.y / 100f, 10f));
            LineVertexCount++;
        }

        if(action == "new-stroke")
        {
            CurrentLine = SpawnNewLine();
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

        NumLines = 0;
    }

    private LineRenderer SpawnNewLine()
    {
        //CurrentStrokeData = new List<StrokeData>();
        LineVertexCount = 0;
        NumLines++;

        //create a gameobject
        var obj = new GameObject("Line_");
        obj.transform.parent = LineRendererParent;

        // z offset is because new lines need to render on top of old
        obj.transform.localPosition = new Vector3(0, 0, -NumLines * 0.1f);
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
        line.positionCount = 0;
        line.useWorldSpace = false;

        return line;
    }

    public void RenderLine(List<StrokeData> line)
    {
        //create a gameobject
        var obj = new GameObject("Line_");
        obj.transform.parent = LineRendererParent;

        // z offset is because new lines need to render on top of old
        obj.transform.localPosition = new Vector3(0, 0, -1 * 0.1f);
        obj.transform.localScale = Vector3.one;

        //obj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        lineObjs.Add(obj);

        //create a line renderer component
        var lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //lineRenderer.SetWidth(0.05f, 0.05f);

        // I scaled up the image
        lineRenderer.SetWidth(0.075f, 0.075f);

        Color color;
        ColorUtility.TryParseHtmlString(line.First().color, out color);
        lineRenderer.material.color = color;

        //plot all the points on the line & set color/thickness
        lineRenderer.positionCount = line.Count;
        lineRenderer.useWorldSpace = false;
        for (int k = 0; k < line.Count; k++)
        {
            var point = line[k];
            lineRenderer.SetPosition(k, new Vector3(point.x / 100f, -point.y / 100f, 10f));
        }
    }

    public void RenderLineData(List<List<ImageData>> lines)
    {
        Clear();

        for (int j = 0; j < lines.Count; j++)
        {
            var line = lines[j];

            //create a gameobject
            var obj = new GameObject("Line_" + j);
            obj.transform.parent = LineRendererParent;

            // z offset is because new lines need to render on top of old
            obj.transform.localPosition = new Vector3(0, 0, -j * 0.1f);
            obj.transform.localScale = Vector3.one;

            //obj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
            lineObjs.Add(obj);

            //create a line renderer component
            var lineRenderer = obj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            //lineRenderer.SetWidth(0.05f, 0.05f);

            // I scaled up the image
            lineRenderer.SetWidth(0.075f, 0.075f);

            Color color;
            ColorUtility.TryParseHtmlString(line.First().color, out color);
            lineRenderer.material.color = color;

            //plot all the points on the line & set color/thickness
            lineRenderer.positionCount = line.Count;
            lineRenderer.useWorldSpace = false;
            for (int k = 0; k < line.Count; k++)
            {
                var point = line[k];
                lineRenderer.SetPosition(k, new Vector3(point.x / 100f, -point.y / 100f, 10f));
            }
        }
    }
}