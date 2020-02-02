using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource Source;
    public List<AudioClip> Clips;

    public AudioClip MummurClip;

    public static MusicManager I;

    // Start is called before the first frame update
    void Start()
    {
        I = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMurmur()
    {
        Source.clip = MummurClip;
        Source.Play();
    }

    public void PlayRandom()
    {
        var idx = Random.Range(0, Clips.Count);
        var clip = Clips[idx];
        Source.clip = clip;
        Source.Play();
    }
}
