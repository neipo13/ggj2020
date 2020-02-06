using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeToVoteView : ViewBase
{
    public TextMeshProUGUI textMesh;
    public GameObject PaintingsParent;
    public GameObject WinnerFrame;
    public GameObject VotingScene;

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

        VotingScene.SetActive(true);
        textMesh.gameObject.SetActive(true);
        WinnerFrame.gameObject.SetActive(false);
        ForEachPainting((painting, idx) =>
        {
            var show = (idx + 1) <= GameLoop.I.numPlayers;
            painting.SetActive(show);
        });
    }

    public void ShowWinner(int winningPlayerIdx)
    {
        textMesh.gameObject.SetActive(false);
        ForEachPainting((Painting, idx) => {
            var show = (idx == winningPlayerIdx);
            if (show)
            {
                var frameParent = Painting.transform.GetChild(1);
                WinnerFrame.transform.parent = frameParent;
                WinnerFrame.transform.localPosition = Vector3.zero;
                WinnerFrame.gameObject.SetActive(true);
            }
            Painting.SetActive(show);
        });
    }

    private void ForEachPainting(Action<GameObject,int> act)
    {
        for (int i = 0; i < PaintingsParent.transform.childCount; i++)
        {
            var child = PaintingsParent.transform.GetChild(i).gameObject;
            act(child, i);
        }
    }

    public override void Hide()
    {
        base.Hide();
        VotingScene.SetActive(false);
        ForEachPainting((p, idx) => {
            p.SetActive(false);
        });
        WinnerFrame.gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
