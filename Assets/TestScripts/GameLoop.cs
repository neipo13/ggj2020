using System;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public int Rounds = 4;

    public SplashView splashView;
    public LobbyView lobbyView;
    //public CountdownView countdownView;
    //public DrawTimerView drawTimerView;
    //public NextPlayerView nextPlayerView;
    //public RoundView roundView;

    public List<int> Devices = new List<int>();

    public float DrawingRoundDuration = 90;
    public int numPlayers = 2;
    public int numRounds = 4;
    public float CountdownDuration = 4f;
    public float nextPlayerDuration = 2f;
    public float nextRoundDuration = 2f;

    // State vars
    private int Round = 0;
    private int PlayerIdx = 0;
    private Component CurrentView;

    //public static GameLoop instance;

    private void Awake()
    {
        AirConsole.instance.onConnect += OnPlayerConnect;
        AirConsole.instance.onDisconnect += OnPlayerDisconnect;

        SetView(splashView);
    }

    private void OnPlayerDisconnect(int device_id)
    {
        Devices.Remove(device_id);
    }

    private void OnPlayerConnect(int device_id)
    {
        Devices.Add(device_id);

        if (Devices.Count >= numPlayers)
            StartCoroutine(StartGame());
    }

    internal IEnumerator StartGame()
    {
        yield return StartCoroutine(SplashCo());
        yield return StartCoroutine(LobbyCo());
        yield return StartCoroutine(GameLoopCo());
    }

    private IEnumerator LobbyCo()
    {
        Debug.Log("Waiting for players to connect");
        SetView(lobbyView);
        yield return new WaitUntil(() => Devices.Count >= numPlayers);
        Debug.Log("All players connected");
    }

    private IEnumerator SplashCo()
    {
        yield return new WaitForSeconds(5f);
        yield break;
    }

    private IEnumerator GameLoopCo()
    {
        Debug.Log("GameLoopCo");

        for (int round = 0; round < Rounds; round++)
        {
            yield return StartCoroutine(RoundStart());

            for (PlayerIdx = 0; PlayerIdx < numPlayers; PlayerIdx++)
            {
                yield return StartCoroutine(CountdownCo());
                yield return StartCoroutine(DrawingTimerCo());
                yield return StartCoroutine(PostDrawingRest());
            }
            yield return StartCoroutine(RoundEnd());
        }

        yield return StartCoroutine(PlayAgain());

        yield break;
    }

    private IEnumerator RoundStart()
    {
        BroadcastToPhone(0, "RoundStart");
        //roundView.SetText("Round " + (Round + 1));
        //roundView.Show();
        yield return new WaitForSeconds(1f);
        //roundView.SetText("Get Ready!");
        yield return new WaitForSeconds(1f);
        //roundView.Hide();

        yield break;
    }

    private IEnumerator RoundEnd()
    {
        yield return new WaitForSeconds(nextRoundDuration);
    }

    private IEnumerator PlayAgain()
    {
        yield break;
    }

    private IEnumerator CountdownCo()
    {
        //countdownView.SetText("");
        //countdownView.gameObject.SetActive(true);


        float timeRemaining = CountdownDuration;
        while (timeRemaining > 0)
        {
            float time = timeRemaining - 1f;
            //countdownView.SetText(time > 0 ? time.ToString() : "Go!");
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            Debug.Log("tr: " + timeRemaining);
        }

        //countdownView.SetText("");
        //countdownView.gameObject.SetActive(false);

        yield break;
    }

    private IEnumerator DrawingTimerCo()
    {
        BroadcastToPhone(PlayerIdx, "DrawStart");

        float timeRemaining = DrawingRoundDuration;

        //drawTimerView.gameObject.SetActive(true);

        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            //drawTimerView.SetText(timeRemaining);
        }

        BroadcastToPhone(PlayerIdx, "DrawEnd");

        yield break;
    }

    private IEnumerator PostDrawingRest()
    {
        //nextPlayerView.Show();
        //yield return new WaitForSeconds(nextPlayerDuration);
        //nextPlayerView.Hide();

        yield break;
    }

    private void BroadcastToPhone(int playerIdx, string evt)
    {
        var msg = new Dictionary<String, String>();

        int deviceId = Devices[playerIdx];

        msg["event"] = evt;

        string data = JsonConvert.SerializeObject(msg);   
        AirConsole.instance.Message(deviceId, data);
    }

    private void SetView(Component component)
    {
        if (CurrentView != null)
            CurrentView.gameObject.SetActive(false);

        CurrentView = component;

        if (CurrentView != null)
            CurrentView.gameObject.SetActive(true);
    }
}
