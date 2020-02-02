using System;
using TMPro;

public class DrawTimerView : ViewBase
{
    public TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTimeRemaining(float secs)
    {
        TimeSpan t = TimeSpan.FromSeconds(secs);

        string answer = string.Format("{0:D1}:{1:D2}",
                        t.Minutes,
                        t.Seconds);

        textMesh.text = answer;
    }
}
