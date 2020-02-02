using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils
{
    public static List<E> Shuffle<E>(List<E> inputList)
    {
        List<E> randomList = new List<E>();
        while (inputList.Count > 0)
        {
            int randomIndex = Random.Range(0, inputList.Count);
            randomList.Add(inputList[randomIndex]); //add it to the new, random list
            inputList.RemoveAt(randomIndex); //remove to avoid duplicates
        }

        return randomList; //return the new random list
    }
}
