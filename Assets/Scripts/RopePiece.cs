using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePiece : MonoBehaviour {
    
    public Transform prev;
    public ParticleSystem electric, win;

    public void SetParticles(bool set) {
        if (set) {
            electric.Play();
        } else {
            electric.Stop();
        }

    }

    public void Win() {
        win.Play();
    }
}
