using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class VotingData : object
{
    public string action;

    [JsonProperty("vote-data")]
    public List<int> voteData;
}
