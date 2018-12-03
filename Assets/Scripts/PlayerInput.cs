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
    private Vector3Int start_point;
    public Tilemap terrain;
    public TileBase tombTile;

    public GameObject playerSpriteObject = null;

    [Header("Viscera")]
    [Range(0, 20)]
    public int GenerateVisceraOnDeathAmount = 5;
    [Range(0.0f, 120.0f)]
    public float VisceraLifespan = 15.0f;
    public GameObject[] Viscera;

    [Header("Sounds")]
    public AudioClip Jump;
    public AudioClip TombSound;
    public AudioClip GenericDeath;
    public AudioClip AltarDeath;
    private AudioSource audioSource;
    private AudioClip deathSound;

    private bool jumping = false;
    private bool isRunning = false;
    private bool isColliding = false;
    private bool isDead = false;
    private int jump_cooldown = 0;

    private Animator animator = null;
    private SpriteRenderer spriteRenderer = null;
    private Animations currentAnimation = Animations.STAND;

	// Use this for initialization
	void Start () {
        body = this.GetComponent<Rigidbody2D>();
        start_point = terrain.WorldToCell(this.transform.position);

        animator = this.GetComponentInChildren<Animator>();
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();

        audioSource = this.GetComponent<AudioSource>();
        deathSound = null;
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
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

        if (Vector2.Dot(best.normal, Vector2.down) > 0.7) return new Vector2(0, 0);

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

        if (Input.GetButtonDown("Break")) {
            Debug.Log("Trying to break a grave");
            int distance = 999999;
            Vector3Int match = new Vector3Int(0, 0, 0);
            Vector3Int player = terrain.WorldToCell(this.transform.position);
            Debug.Log(this.transform.position);

            for(int dx = -5; dx <= 5; dx++) {
                for(int dy = -5; dy <= 5; dy++) {
                    TileBase tile = terrain.GetTile(new Vector3Int(player.x + dx, player.y + dy, 0));
                    if(tile == tombTile) {
                        if (System.Math.Abs(dx) + System.Math.Abs(dy) < distance) {
                            match.x = player.x + dx;
                            match.y = player.y + dy;
                            distance = System.Math.Abs(dx) + System.Math.Abs(dy);
                        }
                    }
                }
            }

            if(distance != 999999) {
                Debug.LogFormat("Broke grave at {0} {0}", match.x, match.y);
                terrain.SetTile(match, null);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (jump_cooldown > 0) jump_cooldown--;
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

        if (Input.GetButton("Jump") && jump_cooldown == 0) {
            Vector2 direction = JumpDirection();
            if (direction.magnitude > 0.01) {
                PlaySound(Jump);
                jump_cooldown = 3;
                direction.y += 0.6f;
                direction.Normalize();
                direction.x *= 10f;
                direction.y *= 10f;
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

    public void Die()
    {
        if (isDead) return;
        body.velocity = Vector2.zero;

        // Can't die on the starting line
        Vector3Int point = terrain.WorldToCell(this.transform.position);
        if ((point - start_point).magnitude < 3) return;

        // Subtract one life
        Scoreboard.LostLife();

        // Start death animations
        if ((Viscera != null) && (Viscera.Length > 0) && (!isDead))
        {
            int randomInt = 0;
            for (int i = 0; i < GenerateVisceraOnDeathAmount; i++)
            {
                randomInt = Random.Range(0, Viscera.Length);
                GameObject v = Instantiate(Viscera[randomInt]);
                v.transform.position = this.transform.position;
                v.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-20.0f, 20.0f), Random.Range(-20.0f, 20.0f));
                Destroy(v, VisceraLifespan);
            }
        }

        body.simulated = false;
        playerSpriteObject.SetActive(false);
        
        foreach (GameObject fin in GameObject.FindGameObjectsWithTag("Finish"))
        {
            BoxCollider2D box = fin.GetComponent<BoxCollider2D>();
            if (box != null && box.bounds.Contains(this.transform.position))
            {
                deathSound = AltarDeath;
                break;
            }
        }

        if (deathSound == null)
            deathSound = GenericDeath;

        PlaySound(deathSound);
        deathSound = null;

        Invoke("Dead", 2.5f);
        isDead = true;
    }

    private void Dead() {
        PlaySound(TombSound);
        // Check for game over
        if (Scoreboard.GetLives() == 0)
        {
            foreach (GameObject fin in GameObject.FindGameObjectsWithTag("Finish"))
            {
                BoxCollider2D box = fin.GetComponent<BoxCollider2D>();
                if (box != null && box.bounds.Contains(this.transform.position))
                {
                    LevelExit exit = fin.GetComponent<LevelExit>();
                    if (exit.Reset)
                    {
                        // Don't send us to purgatory
                        exit.Exit();
                        return;
                    }

                    // If we die on an Altar at the end
                    Scoreboard.FinishLevel();
                    break;
                }
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene("Purgatory");
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
        Vector3Int point = terrain.WorldToCell(this.transform.position);
        terrain.SetTile(point, tombTile);
        Invoke("ReturnPlayerToStart", 1.5f);
    }

    void ReturnPlayerToStart() {
        playerSpriteObject.SetActive(true);
        body.simulated = true;

        this.transform.position = terrain.CellToWorld(start_point);

        isDead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isColliding = true;

        TrapCollider trap_script = collision.gameObject.GetComponent<TrapCollider>();
        if (trap_script != null)
        {
            if (trap_script.DeathSound != null)
                deathSound = trap_script.DeathSound;
            else
                deathSound = GenericDeath;

            Debug.Log("Death by Trap");
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TrapCollider trap_script = collision.gameObject.GetComponent<TrapCollider>();
        if (trap_script != null) {
            if (trap_script.DeathSound != null)
                deathSound = trap_script.DeathSound;
            else
                deathSound = GenericDeath;

            Debug.Log("Death by Trap");
            Die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }
}
