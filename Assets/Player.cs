using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public AudioClip pickSound;
    public AudioClip dropSound;
    public AudioClip tendSound;
    public AudioClip eatenSound;

    public bool disableInput;
    public bool facingRight;
    public enum Facing {Top, Right, Bottom, Left};
    public Facing facing;
    public enum Mode { Carrying, None, Tending };
    public Mode mode;
    public float moveSpeed;
    public float curSpeed;
    public float tendTimer; // Time player is stuck tending
    public float movementModifier = 1;  // Modified by colliding with Splash obj
    public float pushForce;
    private AudioSource audioSource;
    private Rigidbody2D rb2d;
    private Animator animator;
    public GameObject gameManager;
    public GameObject heart;
    public GameObject pickedSlime;
    public GameObject objInFront;     // Slime referenced by FrontalCollider
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mode = Mode.None;
    }
    private void Update()
    {
        if (!disableInput)
        { 
            bool spaceKeyFree = true; // Prevents Space doing multiple actions at single frame
            // HANDLE USE KEY
            if (Input.GetKeyDown("space") || Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return") || Input.GetKey(KeyCode.KeypadEnter))
            {
                if (mode == Mode.None)
                {
                    if  (objInFront != null)
                    { 
                        if ((objInFront.GetComponent<Slime>().mood < 2) && (objInFront.GetComponent<Slime>().mode != Slime.Mode.Exploding))
                        {
                            TendSlime(objInFront);
                        }
                        else if ((objInFront.GetComponent<Slime>().mood == 2) && (objInFront.GetComponent<Slime>().mode != Slime.Mode.Multiplying))
                        {
                            PickSlime(objInFront);
                        }

                    } else {Debug.Log("Player wants to interact but objInFront is NULL!");}
                    spaceKeyFree = false;   // Reserve space key for the duration of this frame
                }

                if (mode == Mode.Carrying && (pickedSlime != null) && spaceKeyFree)
                {
                    if (objInFront != null)
                    {
                        if (objInFront.name == "Buyer")
                        {
                            SellSlime(pickedSlime);
                            objInFront.GetComponent<Buyer>().BuySlime();    // Sell one
                        }

                    }
   
                    else // By default just drop Slime
                    {
                        DropSlime(pickedSlime);
                    }
                }

            }
            // HANDLE SLIME TENDING TIMING
            if (tendTimer > 0) // If timer is set and mood is not worst
            {
                tendTimer -= Time.deltaTime;
            }
            else if ((tendTimer <= 0) && (mode == Mode.Tending))
            {
                mode = Mode.None; // Free player
            }
        }
        else
            animator.SetBool("isMoving", false);

    } // End update
    void SellSlime(GameObject slime)
    {
        audioSource.PlayOneShot(pickSound, 1f);
        Debug.Log("Sold slime");
        DestroyObject(slime);
        mode = Mode.None;
        pickedSlime = null; // Drop reference as well
        gameManager.GetComponent<GameManager>().slimesSold++; // For score calculations
    }
    public void DropSlime(GameObject slime)
    {
        audioSource.PlayOneShot(dropSound, 1f);
        Debug.Log("Dropped slime");
        slime.GetComponent<Slime>().disableInput = false;
        slime.transform.position = transform.position; // Drop at feet
        slime.transform.SetParent(null);   // Unparent
        slime.GetComponent<Rigidbody2D>().simulated = true;
        slime.GetComponent<CircleCollider2D>().isTrigger = false;
        slime.GetComponent<SpriteRenderer>().sortingLayerName = "Actors";
        mode = Mode.None;
        pickedSlime = null; // Drop reference as well
    }
    void PickSlime(GameObject slime)
    {
        //audioSource.PlayOneShot(pickSound, 1f);

        Debug.Log("Picked slime");
        slime.GetComponent<CircleCollider2D>().isTrigger = true;
        pickedSlime = slime; // Store reference for dropping
        pickedSlime.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, 0f);
        pickedSlime.transform.SetParent(transform);   // Parent to Player
        pickedSlime.GetComponent<Slime>().disableInput = true;
        pickedSlime.GetComponent<Rigidbody2D>().simulated = false;
        slime.GetComponent<SpriteRenderer>().sortingLayerName = "Carried";
        mode = Mode.Carrying;
    }
    void TendSlime(GameObject slime)
    {
        audioSource.PlayOneShot(tendSound, 1f);
        Debug.Log("Tended slime");
        Instantiate(heart, slime.transform.position, Quaternion.identity);
        slime.GetComponent<Slime>().ChangeMood(1);  // Who's a happy Slime? Yes you are, yes you are!
        //mode = Mode.Tending;
    }
    void Animate()
    {
        float absHS = Mathf.Abs(rb2d.velocity.x);
        float absVS = Mathf.Abs(rb2d.velocity.y);
        if (absHS > 0 || absVS > 0)                 // Check against absolute value
            animator.SetBool("isMoving", true);
        else
            animator.SetBool("isMoving", false);
        if (mode == Mode.Carrying)
            animator.SetBool("isCarrying", true);
        else
            animator.SetBool("isCarrying", false);
        if (Input.GetAxisRaw("Horizontal") < 0 && facingRight)    // Check against player input instead of rb.velocity, because slimes push player
                Flip();
        else if (Input.GetAxisRaw("Horizontal") > 0 && !facingRight)
                Flip();
     
        }
    void FixedUpdate()
    {
        if (!disableInput)
        {
            // ANIMATION
            Animate();
            // MOVEMENT
            curSpeed = moveSpeed;
            //maxSpeed = curSpeed;
            // Move senteces
            // Debug.Log("H: "+Input.GetAxis("Horizontal")); // -1 to +1
            //Debug.Log("V: " + Input.GetAxis("Vertical"));

            float horizontalSpeed = Input.GetAxisRaw("Horizontal");// GetAxis vs GetAxisRaw?
            float verticalSpeed = Input.GetAxisRaw("Vertical");
            rb2d.velocity = new Vector2(Mathf.Lerp(0, horizontalSpeed * curSpeed * movementModifier, 1f), Mathf.Lerp(0, verticalSpeed * curSpeed * movementModifier, 1f));
            //rb2d.velocity = new Vector2(Mathf.Clamp(horizontalSpeed*10, -2, 2), Mathf.Clamp(verticalSpeed*10, -2, 2));
        }

    }
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("Player collided with "+ col.gameObject);
        if (col.gameObject.name == "Splash")
        {
            movementModifier = 0.5f;
        }
        else if (col.gameObject.name == "BossSlime")
        {
            Debug.Log("Player collided with BossSlime");
            audioSource.PlayOneShot(eatenSound, 1f);

            gameManager.GetComponent<GameManager>().playerEaten = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        //Debug.Log("Player collided with "+ col.gameObject);
        if (col.gameObject.name == "Splash")
        {
            movementModifier = 1f;

        }
    }
    public void Hold()  
    {
            rb2d.velocity = new Vector2(0f, 0f); 
    }

    
}

