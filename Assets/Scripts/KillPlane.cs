using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour {
    
    public AudioClip fallSFX;
    public bool fell = false;

    void OnTriggerEnter2D(Collider2D other) {
        if (fell) return;
        fell = true;
        GameManager.instance.RestartLevel();
        AudioSource.PlayClipAtPoint(fallSFX, Vector3.zero);
    }
}
