using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour {
    
    public GameObject Player1, Player2;
    public GameObject RopePiece;
    public int pieces = 5;
    public float distanceMultiplier = 1f;
    public float ropeMultiplier = .6f;
    private List<Transform> linePos = new List<Transform>();
    public LayerMask dangerLayer;
    public Material bad, good, win;
    public List<RopePiece> ropePieces;
    public bool hit = false;
    public bool won = false;
    public AudioClip shockSFX;
    
    void Start() {
        Vector3 p1Pos = Player1.transform.position;
        Vector3 p2Pos = Player2.transform.position;
        GameObject prev = Player1;

        linePos.Add(Player1.transform);
        for (int i = 0; i < pieces; i++) {
            Vector3 pos = Vector3.Lerp(p1Pos, p2Pos, (float) (i+1) / (pieces + 1));
            GameObject piece = Instantiate(RopePiece, pos, Quaternion.identity);
            linePos.Add(piece.transform);
            //piece.GetComponent<RopePiece>().prev = prev.transform;
            piece.GetComponent<DistanceJoint2D>().connectedBody = prev.GetComponent<Rigidbody2D>();
            piece.GetComponent<DistanceJoint2D>().distance = (piece.transform.position - prev.transform.position).magnitude * distanceMultiplier * ropeMultiplier;
            prev = piece;
            piece.transform.parent = transform;
            ropePieces.Add(piece.GetComponent<RopePiece>());
        }
        DistanceJoint2D joint = prev.AddComponent<DistanceJoint2D>();
        joint.autoConfigureDistance = false;
        joint.distance = (joint.transform.position - Player2.transform.position).magnitude * distanceMultiplier * ropeMultiplier;
        
        joint.connectedBody = Player2.GetComponent<Rigidbody2D>();
        
        
        linePos.Add(Player2.transform);
        GetComponent<LineRenderer>().positionCount = linePos.Count;
        Player1.GetComponent<DistanceJoint2D>().distance *= distanceMultiplier;
    }

    void Update() {
        
        int i = 0;
        Vector3 prev = Vector3.zero;
        bool pasthit = hit;
        foreach(Transform t in linePos) {
            if (i != 0) {
                hit = hit || Physics2D.Linecast(t.position, prev, dangerLayer);
                // t.GetComponent<DistanceJoint2D>().enabled = false;
            }
            GetComponent<LineRenderer>().SetPosition(i, t.position);
            prev = t.position;
            i++;
        }
        if (won || pasthit) return;
        GetComponent<LineRenderer>().material = hit ? bad : good;
        if (!pasthit && hit) {
            foreach (RopePiece p in ropePieces) {
                p.SetParticles(true);
            }
            Player1.GetComponent<PlayerController>().Hurt();
            Player2.GetComponent<PlayerController>().Hurt();
            AudioSource.PlayClipAtPoint(shockSFX, Vector3.zero);
            GameManager.instance.died = true;
            Invoke("Restart", 0.75f);
        }

        // if (pasthit && !hit) {
        //     foreach (RopePiece p in ropePieces) {
        //         p.SetParticles(false);
        //     }
        // }
    }

    public void Win() {
        won = true;
        GetComponent<LineRenderer>().material = win;
        foreach (RopePiece p in ropePieces) {
            p.Win();
        }
    }

    public void Restart() {
        GameManager.instance.RestartLevel();
    }
}
