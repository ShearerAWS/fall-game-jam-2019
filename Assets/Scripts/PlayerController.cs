using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public int playerNum;
    public float runSpeed = 3f;
    public Vector2 airControl = new Vector2(3,0);
    //public float runTopSpeed = 3f;
    public float jumpHeight = 6f;

    public bool grounded = false;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask lockGroundLayer;
    public LayerMask goalGroundLayer;
    public LayerMask otherPlayerLayer;
    public int groundedFrameCheck = 0;
    private Rigidbody2D rb;

    private bool locked = false;

    public PlayerController otherPlayer;

    public bool win = false;

    public AudioClip jumpSFX, landSFX, lockSFX, lockFailSFX;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Animator>().SetBool("running", false);
        GetComponent<Animator>().SetBool("grounded", true);
    }

    void Update() {
        if (win) {
            rb.velocity = Vector2.zero;
            GetComponent<Animator>().SetBool("running", false);
            GetComponent<Animator>().SetBool("grounded", true);
            return;
        }
        float xAxisRaw =  Input.GetAxisRaw("Horizontal" + playerNum);
        float xAxis =  Input.GetAxis("Horizontal" + playerNum);
        float yAxisRaw =  Input.GetAxisRaw("Vertical" + playerNum);
        float yAxis =  Input.GetAxis("Vertical" + playerNum);

        float xSpeed = xAxis * runSpeed;
        float ySpeed = rb.velocity.y;

        bool wasGrounded = grounded;
        if (groundedFrameCheck == 0) {
            grounded = Physics2D.OverlapCircle(groundCheck.position, .01f, groundLayer);
        } else {
            groundedFrameCheck--;
        }
        bool lockGrounded = grounded && Physics2D.OverlapCircle(groundCheck.position, .01f, lockGroundLayer);
        bool otherPlayerGrounded = grounded && Physics2D.OverlapCircle(groundCheck.position, .01f, otherPlayerLayer);
        GameManager.instance.Goal(playerNum,Physics2D.OverlapCircle(groundCheck.position, .01f, goalGroundLayer));

        if (!wasGrounded && grounded) {
            AudioSource.PlayClipAtPoint(landSFX, transform.position);
        }
        if (xAxisRaw != 0) {
            GetComponent<SpriteRenderer>().flipX = xSpeed < 0;
        }

        if (grounded) {
            if (!RopePulled()) {
                rb.velocity = new Vector2(xSpeed, rb.velocity.y);
                if (otherPlayerGrounded) {
                    rb.velocity += new Vector2(otherPlayer.GetComponent<Rigidbody2D>().velocity.x, 0);
                } 
            } else {
                rb.AddForce(new Vector2(xSpeed, 0));
            }

            // jump
            if (yAxisRaw > 0) {
                groundedFrameCheck = 5;
                grounded = false;
                rb.AddForce(new Vector2(0,jumpHeight));
                AudioSource.PlayClipAtPoint(jumpSFX, transform.position);
            }
        } else {
            rb.AddForce(new Vector2(xAxis * airControl.x, yAxis * airControl.y));
        }

        bool wasLocked = locked;
        locked = lockGrounded && yAxisRaw < 0;
        if (!wasLocked && locked) {
            AudioSource.PlayClipAtPoint(lockSFX, transform.position);
        }
        if (grounded && !lockGrounded && Input.GetButtonDown("Vertical" + playerNum) && yAxisRaw < 0) {
            AudioSource.PlayClipAtPoint(lockFailSFX, transform.position);
        }
        GetComponent<FixedJoint2D>().enabled = locked;
        GetComponent<Animator>().SetBool("running", xAxisRaw != 0f);
        GetComponent<Animator>().SetBool("grounded", grounded);
        GetComponent<Animator>().SetFloat("yVel", rb.velocity.y);
        GetComponent<Animator>().SetBool("lock", locked);
    }

    bool RopePulled() {
        if (playerNum == 2) return otherPlayer.RopePulled();
        float dist = (otherPlayer.transform.position - transform.position).magnitude;
        float ropeDist = GetComponent<DistanceJoint2D>().distance;
        return (ropeDist - dist < .05f);
           
    }

    public void Hurt() {
        GetComponent<Animator>().SetBool("lock", false);
        GetComponent<Animator>().SetTrigger("hurt");
        GetComponent<FixedJoint2D>().enabled = false;
        locked = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(0,400));
        grounded = false;
        groundedFrameCheck = 10;
    }

    public void Win() {
        GetComponent<Animator>().SetBool("running", false);
        GetComponent<Animator>().SetBool("grounded", true);
        rb.velocity = Vector2.zero;
        win = true;
    }
}
