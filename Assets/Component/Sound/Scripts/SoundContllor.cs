using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundContllor : MonoBehaviour {
  
    AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        this.audio.Play();
        this.audio.Pause();
    }
    public void Music()
    {
        this.audio.UnPause();
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            Debug.Log("MusicStart");
            Music();
        }
    }
}
