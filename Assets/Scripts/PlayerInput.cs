using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInput : MonoBehaviour {

    private enum Animations
    {
        STAND = 0,
        IDLE = 1,
        RUN = 2,
        JUMP = 3
    };

    private Rigidbody2D body;
    private Vector2Int start_point;
    public Tilemap terrain;
    public TileBase tombTile;

    public GameObject playerSpriteObject = null;

    [Range(0, 20)]
    public int GenerateViceraOnDeathAmount = 5;

    [Range(0.0f, 60.0f)]
    public float ViceraLifespan = 15.0f;
    public GameObject[] Viscera;

    private bool jumping = false;
    private bool isRunning = false;
    private bool isColliding = false;
    private bool isDead = false;

    private Animator animator = null;
    private SpriteRenderer spriteRenderer = null;
    private Animations currentAnimation = Animations.STAND;
    
    Vector2Int roundToGrid(Vector2 point)
    {
        return new Vector2Int(
            Mathf.RoundToInt(point.x - 0.5f),
            Mathf.RoundToInt(point.y - 0.5f)
        );
    }

	// Use this for initialization
	void Start () {
        body = this.GetComponent<Rigidbody2D>();
        start_point = roundToGrid(this.transform.position);

        animator = this.GetComponentInChildren<Animator>();
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
    }
	
    Vector2 JumpDirection()
    {
        ContactPoint2D[] contacts = new ContactPoint2D[100];
        int count = body.GetContacts(contacts);
        if (count == 0) return new Vector2(0, 0);

        ContactPoint2D best = contacts[0];
        for(int ii = 1; ii < count; ii++) {
            if(best.point.y > contacts[ii].point.y) {
                best = contacts[ii];
            }
        }
        return best.normal;
    }

    private void Update()
    {
        if (playerSpriteObject.activeSelf)
        {
            if (jumping)
            {
                if (currentAnimation != Animations.STAND)
                {
                    currentAnimation = Animations.STAND;
                    animator.Play("CharacterStand");
                }
            }
            else if (isRunning)
            {
                if (!jumping)
                {
                    if (currentAnimation != Animations.RUN)
                    {
                        currentAnimation = Animations.RUN;
                        animator.Play("CharacterRun");
                    }
                }
            }
            else
            {
                if (currentAnimation != Animations.IDLE)
                {
                    currentAnimation = Animations.IDLE;
                    animator.Play("CharacterIdle");
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        float move = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Horizontal") > 0)
        {
            spriteRenderer.flipX = false;
            isRunning = true;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            spriteRenderer.flipX = true;
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        float target_speed = 10 * move;
        float max_change = 50 * Time.fixedDeltaTime;

        body.velocity = new Vector2(
            Mathf.MoveTowards(body.velocity.x, target_speed, max_change),
            body.velocity.y
        );

        if (Input.GetButton("Jump")) {
            Vector2 direction = JumpDirection();
            if (direction.magnitude > 0.01) {
                direction.y += 0.6f;
                direction.Normalize();
                direction.x *= 6f;
                direction.y *= 6f;
                body.velocity = body.velocity + direction;
            }

            if (!jumping)
            {
                jumping = true;
            }
        }

        if (isColliding)
        {
            jumping = false;
        }

        if (Input.GetButton("Die")) {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        body.velocity = Vector2.zero;

        // Can't die on the starting line
        Vector2Int point = roundToGrid(this.transform.position);
        if ((point - start_point).magnitude < 3) return;

        // Subtract one life
        Scoreboard.LostLife();

        // Start death animations
        if ((Viscera != null) && (Viscera.Length > 0) && (!isDead))
        {
            int randomInt = 0;
            for (int i = 0; i < GenerateViceraOnDeathAmount; i++)
            {
                randomInt = Random.Range(0, Viscera.Length);
                GameObject v = Instantiate(Viscera[randomInt]);
                v.transform.position = this.transform.position;
                v.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-20.0f, 20.0f), Random.Range(-20.0f, 20.0f));
                Destroy(v, ViceraLifespan);
            }
        }

        body.simulated = false;
        playerSpriteObject.SetActive(false);
        Invoke("Dead", 2.5f);
        isDead = true;
    }

    private void Dead() {
        // Check for game over
        if (Scoreboard.GetLives() == 0) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("End");
            return;
        }

        // Check if we have finished the map
        foreach (GameObject fin in GameObject.FindGameObjectsWithTag("Finish")) {
            BoxCollider2D box = fin.GetComponent<BoxCollider2D>();
            if (box != null && box.bounds.Contains(this.transform.position)) {
                fin.GetComponent<LevelExit>().Exit();
                return;
            }
        }

        // Just die
        Vector2Int point = roundToGrid(this.transform.position);
        terrain.SetTile(new Vector3Int(point.x, point.y, 0), tombTile);
        Invoke("ReturnPlayerToStart", 1.5f);
    }

    void ReturnPlayerToStart() { 
        playerSpriteObject.SetActive(true);
        body.simulated = true;
        Vector2Int point = roundToGrid(this.transform.position);
        
        this.transform.position = new Vector2(start_point.x + 0.5f, start_point.y + 0.5f);

        isDead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isColliding = true;

        TrapCollider trap_script = collision.gameObject.GetComponent<TrapCollider>();
        if (trap_script != null) Die();
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }
}
