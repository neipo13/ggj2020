using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NDream.AirConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Vectrosity;

public class AirPlayTestController : MonoBehaviour {

    public Transform LineRendererParent;
    public GameLoop gameLoop;

    private VectorLine line;

    public List<StrokeData> CurrentStrokeData = new List<StrokeData>();
    public VectorLine CurrentLine;
    public int NumLines;
    public int LineVertexCount;

    void Awake () {
        AirConsole.instance.onMessage += OnMessage;
        
    }

    List<VectorLine> lineObjs = new List<VectorLine>();

    void OnMessage (int from, JToken message) {

        string action = message["action"].ToString();

        if(action == "send-stroke-data")
        {
            var point = JsonConvert.DeserializeObject<StrokeData>(message["stroke-data"].ToString());

            if(LineVertexCount == 0)
            {
                ColorUtility.TryParseHtmlString(point.color, out Color color);
                CurrentLine.color = color;
            }

            var p = new Vector3(point.x / 100f, -point.y / 100f, 0f);
            p = LineRendererParent.TransformPoint(p);
            CurrentLine.points3.Add(p);
            CurrentLine.Draw();
        }

        if(action == "new-stroke")
        {
            CurrentLine = SpawnNewLine();
        }
    }

    public void Clear()
    {
        //clear old objects
        for(int i = 0; i < lineObjs.Count; i++)
        {
            var obj = lineObjs[i];
            VectorLine.Destroy(ref obj);
        }

        lineObjs.Clear();

        NumLines = 0;
    }

    private VectorLine SpawnNewLine()
    {
        //CurrentStrokeData = new List<StrokeData>();
        LineVertexCount = 0;
        NumLines++;

        line = new VectorLine("Line_" + NumLines, new List<Vector3>(), 7f, LineType.Continuous);
        line.endPointsUpdate = 1;
        lineObjs.Add(line);

        return line;
    }
}