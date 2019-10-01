using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour {
    public static CanvasController instance;
    public string transitionMode;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            TriggerRestart();
        }
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        instance.GetComponent<Animator>().ResetTrigger("restart");
        instance.GetComponent<Animator>().ResetTrigger("next");
    }

    public void TriggerRestart() {
        transitionMode = "restart";
        instance.GetComponent<Animator>().SetTrigger("restart");
    }

    public void TriggerNext() {
        transitionMode = "next";
        instance.GetComponent<Animator>().SetTrigger("next");
    }

    public void TranisitionEnd() {
        if (transitionMode == "restart") {
            RestartLevel();
        } else {
            NextLevel();
        }
    }
    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel() {
        GameManager.instance.NextLevel();
    }
}
