using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour {

    public AudioClip multiplySound;
    public AudioClip explodeSound;

    public AudioClip attackSound;

    public bool disableInput;
    public bool facingRight = false;
    public bool touchingTarget = false;
    public int mood = 2;
    public float moodTimer; // Countdown to mood deterioration
    public float multiplyTimer;
    public float explodeTimer;
    public float attackTimer;
    public float splitTimer;        // Time it takes to split into 2 slimes when multiplying 
    public float splitBadTimer;        // Time it takes to split into 2 red slimes when exploding
    public float splitTimerAmount;      // For Reset  / Also used to reset splitBadTimer
    public float moodTimerAmount;        // For Reset 
    public float multiplyTimerAmount;   // Reset later / Also used to reset explodeTimer
    public float attackTimerAmount;   // Reset later 

    public enum Mode {Attacking,Exploding,Multiplying,None };
    public Mode mode;
    public Sprite[] sprite;
    public float moveSpeed = 1f;
    public float moveForceX; // added as velocity to RB2D
    public float moveForceY; // added as velocity to RB2D
    public float moveDuration = 0f; // For random movement
    public float randValue = 0f;    // For random movement
    private Rigidbody2D rb2d;
    public Transform target;
    public Transform player;

    public GameObject gameManager;
    public GameObject slimePrefab;
    public GameObject splashPrefab;
    public GameObject heart;

    private AudioSource audioSource;
    private Animator animator;
    private new SpriteRenderer renderer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        renderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        transform.parent = GameObject.Find("ACTORS").transform; // Organize Unity UI
        gameManager = GameObject.Find("GameManager");
        //player = GameObject.Find("Player").transform;
        target = GameObject.Find("Well").transform;
        moveSpeed = 1;
        moveForceX = 1;
        moveForceY = 1;
        mode = Mode.None;
        name = "Slime"; // Set name after instancing
        // ADD SLIME TO LIST WITH ALL THE REST
        List<GameObject> allSlimes = gameManager.GetComponent<GameManager>().allSlimes;
        allSlimes.Add(gameObject);
    }
    
    void Update()
    {
       
        if (!disableInput)
        {
            // HANDLE SPRITE ORIENTATION
            if (rb2d.velocity.x < 0f && facingRight)       // vel x -val and facing right
            {
                Flip();
            }
            else if (rb2d.velocity.x > 0f && !facingRight) // vel x +val and facing left
            {
                Flip();
            }
            MoodTimerResets();
        
            RedSlimeList();
            // ATTACK MODE
            if (mode == Mode.Attacking)
            {
                if (attackTimer > 0)
                {
                    attackTimer -= Time.deltaTime;
                }
                else 
                {
                    if (touchingTarget)
                    {
                        audioSource.PlayOneShot(attackSound, 0.5f);

                        gameManager.GetComponent<GameManager>().wellHealth--;   //-1 / second
                    attackTimer = attackTimerAmount;
                    }
                }
            }
            // CHECK FOR RETURNING FROM ATTACK MODE
            if (mood > 0 && mode == Mode.Attacking)
            {
                mode = Mode.None;
            }
            // CHECKS FOR NOT RUNNING MOOD AND SPRITE CHANGE
            if (mode != Mode.Multiplying)
            {
                if (mode != Mode.Exploding) 
                {
                    if (mode != Mode.Attacking) 
                    {
                        //Debug.Log("Got past multi and expl mode check ");
                        // VISUALIZE MOOD 
                        renderer.sprite = sprite[mood]; // sets the sprite as index on both correlate
                        // MOOD DETERIORATION
                        if (moodTimer > 0) // If timer is set
                        {
                            if (mood == 2)
                                moodTimer -= Time.deltaTime;
                            if (mood == 1)
                                moodTimer -= Time.deltaTime*0.5f; // Yellow deteriorate at half rate
                        }
                        else if (moodTimer <= 0 && mood > 0)
                        {
                            ChangeMood(-1);
                        }
                    }
                }
            }
            
            // MULTIPLYING SLIME
            if ((mood == 2) && (mode != Mode.Multiplying))  // Run multiply timer only when slime is happy and not already multiplying
            {
                if (multiplyTimer > 0)
                {
                    multiplyTimer -= Time.deltaTime;
                }
                else
                {
                    mode = Mode.Multiplying;        // Toggle mode
                    splitTimer = splitTimerAmount;  // Set splitting time 
                    renderer.sprite = sprite[3];  // Change appearance during multiplying 
                }
            }
            else if (mode == Mode.Multiplying)   // When multiply mode is active start split timer
            {
                if (splitTimer > 0)
                {
                    splitTimer -= Time.deltaTime;
                }
                else
                {
                    Multiply();
                }
            }
            // EXPLODING SLIME
            if ((mood == 0) && (mode != Mode.Exploding) && (mode != Mode.Attacking))  // Run multiply timer only when slime is unhappy and not already exploding
            {

                if (explodeTimer > 0)
                {
                    explodeTimer -= Time.deltaTime;
                }
                else
                {
                    mode = Mode.Exploding;              // Toggle mode
                    splitBadTimer = splitTimerAmount;  // Set splitting time 
                    renderer.sprite = sprite[4];       // Change appearance during explosion 
                }
            }
            else if (mode == Mode.Exploding)   // When multiply mode is active start split timer
            {
                
                if (splitBadTimer > 0)
                {
                    Debug.Log("timer running");

                    splitBadTimer -= Time.deltaTime;
                }
                else
                {
                    Explode();
                }
            }
        }
    }
    void Explode()
    {
        audioSource.PlayOneShot(explodeSound, 1f);

        //rb2d.isKinematic = false;
        explodeTimer = multiplyTimerAmount;    // Reset timer
        mode = Mode.None;   // Reset mode to allow other actions
        GameObject newSlime = Instantiate(slimePrefab, transform.position, Quaternion.identity);
        Instantiate(splashPrefab, transform.position, Quaternion.identity);
        newSlime.GetComponent<Slime>().mood = 0;    // New spawns are angry as well
    }
    void Multiply()
    {
        audioSource.PlayOneShot(multiplySound, 1f);

        //rb2d.isKinematic = false;
        Debug.Log("Slime multiplied!");
        multiplyTimer = multiplyTimerAmount;    // Reset timer
        mode = Mode.None;   // Reset mode to allow other actions
        Instantiate(slimePrefab, transform.position, Quaternion.identity);
    }
    // RESET MULTIPLY / EXPLODE TIMERS ACORDING TO MOOD
    void MoodTimerResets()
    {
        if (mood == 0)
        {
            multiplyTimer = multiplyTimerAmount; // If mood drops low reset multiplying
        }
        else if (mood == 1)
        {
            attackTimer = attackTimerAmount; // If mood rises reset attack timer
        }
        else if (mood == 2)
        {
            attackTimer = attackTimerAmount; // If mood rises reset attack timer
            explodeTimer = multiplyTimerAmount; // If mood rises high reset exploding
        }
    }
    // UPDATE RED SLIMES LIST
    void RedSlimeList()
    {
        if (mood == 0)
        {
            List<GameObject> redSlimes = gameManager.GetComponent<GameManager>().redSlimes;
            if (!redSlimes.Contains(gameObject))
            {
                redSlimes.Add(gameObject);
            }
        }
        else
        {
            List<GameObject> redSlimes = gameManager.GetComponent<GameManager>().redSlimes;
            if (redSlimes.Contains(gameObject))
            {
                redSlimes.Remove(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        // HANDLE MOVEMENT
        if (!disableInput)
        { 
            if (target != null)
            { 
            float distance = Vector3.Distance(target.position, transform.position);
            }
            //float playerDistance = Vector3.Distance(player.transform.position, transform.position);
            //Debug.Log("Distance to player: " + distance);
            if (mode == Mode.Multiplying)
            {
                //Debug.Log(rb2d.velocity);
                Hold(); // Reset velocity
                //rb2d.isKinematic = true;
            }
            else if (mode == Mode.Exploding)
            {
                Hold(); // Reset velocity
                //rb2d.isKinematic = true;
            }
            else if (mode == Mode.Attacking) 
            {
                if (touchingTarget)
                    Hold(); // Reset velocity
                else
                    MoveToTarget();
            }
          
            else
                MoveRandom();
        }
        else
            Hold(); // Reset velocity
    }
    void MoveToTarget()
    {
        //Debug.Log("Slime Moving To Target.");
        if (moveDuration > 0)
        {
            moveDuration -= Time.deltaTime;

        }
        else // Every set duration check distance and set velocity towards target
        {
            //Debug.Log("Move duration and velocity set.");
            moveDuration = 0.5f;
            float distanceX = Mathf.Clamp((target.position.x - transform.position.x), -1f, 1f);
            float distanceY = Mathf.Clamp((target.position.y - transform.position.y), -1f, 1f);
            rb2d.velocity = new Vector2(distanceX, distanceY);
        }

    }
    void MoveRandom()
    {
        //Debug.Log("Slime is moving randomly.");
        if (moveDuration > 0) 
        {
            moveDuration -= Time.deltaTime;
        }
        else 
        {
            //Debug.Log("Calculated movement randomness.");
            moveDuration = Random.Range(0.5f, 2.0f); // Set new duration for random movement
            randValue = Random.Range(0,2);         // Set new random values
            // FLIP COIN FOR MOVE OR IDLE
            if (randValue < 1f) // 50% of the time move around
            {
                moveForceX = Random.Range(-1, 1f);
                moveForceY = Random.Range(-1, 1f);
            }
            else    // 50% of the time Slime just idles 
            {
                moveForceX = 0f;
                moveForceY = 0f;
            }
        }
        rb2d.velocity = new Vector2(moveForceX, moveForceY);    // Gave up on lerping, stuff 100x faster now

    }
    public void Hold()
    {
        //Debug.Log("Slime Halted.");
        if ((rb2d.velocity.x > 0f) || (rb2d.velocity.y > 0f))
        { 
        rb2d.velocity = new Vector2(0f, 0f); // Reset Slime's velocity
        }
    }
    public void ChangeMood(int change)
    {
        Debug.Log("Slime mood changed by " + change);
        mood += change;                 // Update mood
        moodTimer = moodTimerAmount;    // Reset mood timer
    }

    void Flip()
    {
        facingRight = !facingRight;     // Invert bool
        // Multiply local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Well")
        {
            Debug.Log("Slime is touching Well");
            touchingTarget = true;
        }
        
    }
    private void OnDestroy()
    {
        //gameManager.GetComponent<GameManager>().slimeCount--;
        List<GameObject> allSlimes = gameManager.GetComponent<GameManager>().allSlimes;
        allSlimes.Remove(gameObject);
        List<GameObject> redSlimes = gameManager.GetComponent<GameManager>().redSlimes;
        redSlimes.Remove(gameObject);
    }
}
