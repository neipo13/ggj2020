using System;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Linq;

public class GameLoop : MonoBehaviour
{
    public int Rounds = 4;

    public SplashView splashView;
    public LobbyView lobbyView;
    public CountdownView countdownView;
    public VoteResultsView voteResultsView;
    public DrawTimerView drawTimerView;

    public List<Sprite> sprites;

    public Painting Painting;
    //public NextPlayerView nextPlayerView;
    //public RoundView roundView;

    public List<int> Devices = new List<int>();

    public float DrawingRoundDuration = 100000f;
    public int numPlayers = 2;
    public int numRounds = 4;
    public float CountdownDuration = 4f;
    public float nextPlayerDuration = 2f;
    public float nextRoundDuration = 2f;
    public float voteResultsDuration = 3f;

    // State vars
    private int Round = 0;
    private int PlayerIdx = 0;
    private int numPeopleVoted = 0;
    private Component CurrentView;
    private int paintingIdx = 0;

    public int[] PlayerScores = {0, 0, 0, 0};

    //public static GameLoop instance;

    private void Awake()
    {
        AirConsole.instance.onConnect += OnPlayerConnect;
        AirConsole.instance.onDisconnect += OnPlayerDisconnect;
        AirConsole.instance.onMessage += OnMsg;

        SetView(splashView);
    }

    private void OnMsg(int from, JToken data)
    {
        if (data["action"] != null && data["action"].ToString () == "send-vote-data") {
            var vData = JsonConvert.DeserializeObject<VotingData> (data.ToString ());
            PlayerScores[vData.voteData[0]-1] += 5;
            PlayerScores[vData.voteData[1]-1] += 3;
            PlayerScores[vData.voteData[2]-1] += 1;
            numPeopleVoted+=1;
        }
    }

    private void OnPlayerDisconnect(int device_id)
    {
        Devices.Remove(device_id);
    }

    private void OnPlayerConnect(int device_id)
    {
        Devices.Add(device_id);
        Debug.Log(Devices.Count + " players connected");

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
        SetView(null);
    }

    private IEnumerator SplashCo()
    {
        yield return new WaitForSeconds(5f);
        yield break;
    }

    private IEnumerator GameLoopCo()
    {
        Debug.Log("GameLoopCo");

        // Shuffle paintings at the start of each game
        sprites = Utils.Shuffle(sprites);

        for (int round = 0; round < Rounds; round++)
        {
            yield return StartCoroutine(RoundStart());

            for (PlayerIdx = 0; PlayerIdx < numPlayers; PlayerIdx++)
            {
                yield return StartCoroutine(CountdownCo());
                yield return StartCoroutine(DrawingTimerCo());
                yield return StartCoroutine(PostDrawingRest());
            }

            yield return StartCoroutine(VoteCo());
            yield return StartCoroutine(RoundEnd());
        }

        yield return StartCoroutine(PlayAgain());

        yield break;
    }

    private IEnumerator RoundStart()
    {
        Debug.Log("Round Start");

        // Go to the next painting
        paintingIdx++;
        var idx = paintingIdx % sprites.Count;
        var sprite = sprites[idx];
        Painting.SetSprite(sprite);
        BroadcastToAll(new RoundStartData(sprite.name + ".png"));

        // Show painting object
        Painting.gameObject.SetActive(true);

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

    private IEnumerator VoteCo()
    {
        numPeopleVoted = 0;
        // Put phone client in the vote state
        BroadcastToAll("vote");
        yield return new WaitUntil(() => { return numPeopleVoted >= numPlayers; });

        int winningPlayerIdx = SelectWinner();
        voteResultsView.SetText(string.Format("Player {0}  Wins!", winningPlayerIdx));
        BroadcastToPlayer(winningPlayerIdx, "win");
        BroadcastToOthers(winningPlayerIdx, "lose");

        yield return new WaitForSeconds(voteResultsDuration);
    }

    private int SelectWinner()
    {
        int maxScore = 0;
        int winningPlayerId = 0;
        for(int i = 0; i < PlayerScores.Length; i++){
            if(PlayerScores[i] > maxScore  ||
                (PlayerScores[i] == maxScore && UnityEngine.Random.Range(0, 2) > 0))
            {
                maxScore = PlayerScores[i];
                winningPlayerId = i;
            }
        }
        return winningPlayerId;
    }

    private IEnumerator PlayAgain()
    {
        yield break;
    }

    private IEnumerator CountdownCo()
    {
        // Erase previous players drawing
        Painting.ClearLines();

        countdownView.SetText("");
        SetView(countdownView);

        float timeRemaining = CountdownDuration;
        while (timeRemaining > 0)
        {
            float time = timeRemaining - 1f;
            countdownView.SetText(time > 0 ? time.ToString() : "Go!");
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            Debug.Log("tr: " + timeRemaining);
        }

        countdownView.SetText("");
        SetView(null);

        yield break;
    }

    private IEnumerator DrawingTimerCo()
    {
        //tell phone 
        BroadcastToPlayer(PlayerIdx, "DrawStart");
        //tell everyone else to wait
        BroadcastToOthers(PlayerIdx, "wait");

        float timeRemaining = DrawingRoundDuration;

        drawTimerView.Show();
        
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            Debug.Log(timeRemaining);
            drawTimerView.SetTimeRemaining(timeRemaining);
        }

        drawTimerView.Hide();

        BroadcastToPlayer(PlayerIdx, "wait");

        yield break;
    }

    private IEnumerator PostDrawingRest()
    {
        //nextPlayerView.Show();
        // yield return new WaitForSeconds(nextPlayerDuration);
        //nextPlayerView.Hide();

        yield break;
    }

    private void BroadcastToOthers(int self, string evt)
    {
        for (var i = 0; i < numPlayers; i++)
        {
            if (i == self) continue;
            BroadcastToPlayer(i, evt);
        }
    }

    private void BroadcastToPlayer(int playerIdx, string evt)
    {
        int deviceId = Devices[playerIdx]; 
        AirConsole.instance.Message(deviceId, evt);
    }

    private void BroadcastToAll(string evt)
    {
        for(var i = 0; i < numPlayers; i++)
        {
            BroadcastToPlayer(i, evt);
        }
    }

    private void BroadcastToPlayer(int playerIdx, object obj)
    {
        int deviceId = Devices[playerIdx];
        AirConsole.instance.Message(deviceId, obj);
    }

    private void BroadcastToAll(object obj)
    {
        for(var i = 0; i < numPlayers; i++)
        {
            BroadcastToPlayer(i, obj);
        }
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
