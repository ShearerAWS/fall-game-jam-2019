using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RopeMenu : MonoBehaviour {
    
    public GameObject Player1, Player2;
    public GameObject RopePiece, Text;
    public string title;
    public int pieces = 5;
    public float ropeMultiplier = 1f;
    private List<Transform> linePos = new List<Transform>();
    public Material ropeMat;
    public List<RopePiece> ropePieces;
    public Vector3 offset;
    
    void Start() {
        Vector3 p1Pos = Player1.transform.position;
        Vector3 p2Pos = Player2.transform.position;
        GameObject prev = Player1;

        linePos.Add(Player1.transform);
        for (int i = 0; i < pieces; i++) {
            Vector3 pos = Vector3.Lerp(p1Pos, p2Pos, (float) (i+1) / (pieces + 1));
            GameObject piece = Instantiate(RopePiece, pos, Quaternion.identity);
            GameObject letter = Instantiate(Text, pos + offset, Quaternion.identity);
            letter.transform.parent = piece.transform;
            letter.GetComponent<TextMeshPro>().text = title.ToCharArray()[i].ToString();
            linePos.Add(piece.transform);
            //piece.GetComponent<RopePiece>().prev = prev.transform;
            piece.GetComponent<DistanceJoint2D>().connectedBody = prev.GetComponent<Rigidbody2D>();
            piece.GetComponent<DistanceJoint2D>().distance = (piece.transform.position - prev.transform.position).magnitude * ropeMultiplier;
            prev = piece;
            piece.transform.parent = transform;
            ropePieces.Add(piece.GetComponent<RopePiece>());
        }
        DistanceJoint2D joint = prev.AddComponent<DistanceJoint2D>();
        joint.autoConfigureDistance = false;
        joint.distance = (joint.transform.position - Player2.transform.position).magnitude * ropeMultiplier;
        
        joint.connectedBody = Player2.GetComponent<Rigidbody2D>();
        
        
        linePos.Add(Player2.transform);
        GetComponent<LineRenderer>().positionCount = linePos.Count;
        foreach (RopePiece p in ropePieces) {
            p.Win();
        }
    }

    void Update() {
        
        int i = 0;
        Vector3 prev = Vector3.zero;
        foreach(Transform t in linePos) {
            GetComponent<LineRenderer>().SetPosition(i, t.position);
            prev = t.position;
            i++;
        }
        GetComponent<LineRenderer>().material = ropeMat;

        // if (pasthit && !hit) {
        //     foreach (RopePiece p in ropePieces) {
        //         p.SetParticles(false);
        //     }
        // }
    }

}
