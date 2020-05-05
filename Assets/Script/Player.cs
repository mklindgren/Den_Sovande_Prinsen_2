using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public static int sleep;//ändrad från private
    public float restartLevelDelay = 1f;
    public Text sleepText;
    public SleepBar sleepBar;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;

    private Vector2 touchOrigin = -Vector2.one;

    private int keyLayer, doorLayer;
    private bool hasKey;


    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        sleep = GameMngr.instance.playerSleepPoints;

        sleepText.text = "Food: " + sleep;

        keyLayer = LayerMask.NameToLayer("Key");
        doorLayer = LayerMask.NameToLayer("Door");

        base.Start();
    }

    private void OnDisable()
    {
        GameMngr.instance.playerSleepPoints = sleep;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMngr.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }

#else
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }
            }
        }

#endif

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        sleep--;
        sleepText.text = "Food: " + sleep;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();

        GameMngr.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit" || (other.gameObject.layer == doorLayer && hasKey))
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            sleep += pointsPerFood;
            sleepText.text = "+" + pointsPerFood + " Food: " + sleep;
            //SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            sleep += pointsPerFood;
            sleepText.text = "+" + pointsPerSoda + " Food: " + sleep;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.layer == keyLayer)
        {
            hasKey = true;
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseSleep(int loss)
    {
        animator.SetTrigger("playerHit");
        sleep -= loss;
        sleepText.text = "-" + loss + " Food: " + sleep;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (sleep <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameMngr.instance.GameOver();
        }
    }
}
