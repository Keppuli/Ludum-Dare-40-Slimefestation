using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyer : MonoBehaviour {
    public GameObject speechBubble;


    private float fadeTimer = 1f;
    public float speechTimer;

    public float moveSpeed = 1f;
    public float nextVisitTimer;
    public float nextVisitTimerMin;
    public float nextVisitTimerMax;

    public float waitingTimer = 10f;
    public float waitingTimerAmount = 10f;

    public int buyAmount;
    public int buyAmountMin;
    public int buyAmountMax;

    public bool isWaiting = false;
    public bool isAway = true;

    private new SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        buyAmount = Random.Range(1, 4);

    }
    private void Update()
    {
        if (nextVisitTimer > 0)
        {
            nextVisitTimer -= Time.deltaTime;
        }
        else if (nextVisitTimer <= 0 && buyAmount > 0)
        {
            Arrive();
        }
        if (buyAmount == 0)
        {
            //Debug.Log("Buyer needs have been satisfied. ");
            // DestroyObject(this.gameObject);
            Leave();
        }
    }
    public void BuySlime()
    {
        buyAmount--;
        Debug.Log("Buyer bought one slime, "+ buyAmount + " more to go.");
    }
    public void Arrive()
    {
        //Debug.Log("Buyer arrived.");
        isAway = false;
        FadeIn();                       // Fade sprite
        renderer.flipX = false;         // Sprite point right
        if (transform.position.x < -8.5f)
        { 
            transform.position += transform.right * moveSpeed;
        }
        else
        {
            if (!isWaiting)
            {
                speechTimer = 8f;
                isWaiting = true;
                Debug.Log("Buyer is waiting");
                waitingTimer = waitingTimerAmount;
            }
        }
        if (speechTimer < 4f)
            speechBubble.SetActive(true);
        if (speechTimer > 0)
            speechTimer -= Time.deltaTime;
        else
            speechBubble.SetActive(false);
      
     

    }
    public void Leave()
    {
        speechBubble.SetActive(false);
        //Debug.Log("Buyer left.");
        isWaiting = false;
        FadeOut();                      // Fade sprite
        renderer.flipX = true;          // Sprite point left
        //Vector3 target = new Vector3(-10f, -0.5f, 0f);
        if (transform.position.x > -11f)
        { 
            transform.position -= transform.right * moveSpeed;
        }
        else 
        {
            if (!isAway)
            {
                Debug.Log("Buyer is away.");
                isAway = true;
                nextVisitTimer = Random.Range(nextVisitTimerMin, nextVisitTimerMax);   // Set duration for next visit
                buyAmount = Random.Range(buyAmountMin, buyAmountMax); // Set random buy amount

            }
        }
    }
    void FadeIn()
    {
        if (fadeTimer < 1)
        {
            fadeTimer += Time.deltaTime;
        }
        renderer.color = new Color(1, 1, 1, fadeTimer);
    }
    void FadeOut()
    {
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
        }
        renderer.color = new Color(1, 1, 1, fadeTimer);
    }
}
