using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    private AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip newMusic) {
        if(audioSource.clip != newMusic) {
            audioSource.clip = newMusic;
            audioSource.Play();
        }
    }

    public void StopMusic() {
        audioSource.Stop();
    }
}
