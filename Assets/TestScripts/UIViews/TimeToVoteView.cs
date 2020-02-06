using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeToVoteView : ViewBase
{
    public TextMeshProUGUI textMesh;
    public GameObject PaintingsParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Show()
    {
        base.Show();
        PaintingsParent.SetActive(true);
        for(int i = 0; i < PaintingsParent.transform.childCount; i++)
        {
            var show = (i + 1) <= GameLoop.I.numPlayers;
            PaintingsParent.transform.GetChild(i).gameObject.SetActive(show);
        }
    }

    public override void Hide()
    {
        base.Hide();
        PaintingsParent.SetActive(false);
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
