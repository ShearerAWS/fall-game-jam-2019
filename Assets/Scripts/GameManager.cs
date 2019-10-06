using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance;
    public GameObject blueGoalMap, redGoalMap;
    public bool blueGoal, redGoal;
    public Color active, inactive;

    public List<string> levels;
    public int levelIndex = 0;
    public bool realStart = false;
    public bool lastLevel = false;
    public bool won = false;
    public bool died = false;

    public AudioClip goalEnterSFX, goalExitSFX, win1SFX, win2SFX;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoad;
        if (!realStart) {
            int i = 0;
            foreach(string l in levels) {
                if (SceneManager.GetActiveScene().name == l) {
                    levelIndex = i;
                }
                i++;
            }
        }
    }

    void Update() {
        if (realStart || lastLevel) {
            if(Input.anyKeyDown) {
                if (lastLevel) {
                    Invoke("TriggerNext",1f);
                } else {
                    TriggerNext();
                }
                
                realStart = false;
            }
        }   
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        blueGoalMap = GameObject.Find("Blue Goal");
        redGoalMap = GameObject.Find("Red Goal");
        blueGoal = false;
        redGoal = false;
        won = false;
        died = false;
    }

    public void RestartLevel() {
        CanvasController.instance.TriggerRestart();
    }

    public void Goal(int num, bool set) {
        if (won || died) return;
        if (num == 1) {
            bool prev = blueGoal;
            blueGoal = set;
            if(!prev && blueGoal && !redGoal) {
                AudioSource.PlayClipAtPoint(goalEnterSFX, Vector3.zero);
            } else if (prev && !blueGoal) {
                AudioSource.PlayClipAtPoint(goalExitSFX, Vector3.zero);
            }
            blueGoalMap.GetComponent<Tilemap>().color = blueGoal ? active : inactive;
        } else {
            bool prev = redGoal;
            redGoal = set;
            if(!prev && redGoal && !blueGoal) {
                AudioSource.PlayClipAtPoint(goalEnterSFX, Vector3.zero);
            } else if (prev && !redGoal) {
                AudioSource.PlayClipAtPoint(goalExitSFX, Vector3.zero);
            }
            redGoalMap.GetComponent<Tilemap>().color = redGoal ? active : inactive;
        }

        if (blueGoal && redGoal) {
            won = true;
            Sequence s = DOTween.Sequence();
            StartCoroutine("WinSequence");
            
            GameObject.Find("P1").GetComponent<PlayerController>().Win();
            GameObject.Find("P2").GetComponent<PlayerController>().Win();
            GameObject.Find("Rope").GetComponent<RopeController>().Win();
            Invoke("TriggerNext", 1.3f);
        }

    }
    IEnumerator WinSequence() {
        AudioSource.PlayClipAtPoint(win1SFX, Vector3.zero);
        yield return new WaitForSeconds(0.8f);
        AudioSource.PlayClipAtPoint(win2SFX, Vector3.zero);
    }

    public void TriggerNext() {
        CanvasController.instance.TriggerNext();
    }

    public void NextLevel() {
        levelIndex++;
        if (levelIndex == levels.Count + 1) lastLevel = true;
        if (levelIndex == levels.Count) Application.Quit();
        SceneManager.LoadScene(levels[levelIndex]);
    }

    
}
