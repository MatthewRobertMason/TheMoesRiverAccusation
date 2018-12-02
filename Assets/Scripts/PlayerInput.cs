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

    private bool jumping = false;

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

        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();

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

	// Update is called once per frame
	void FixedUpdate () {
        float move = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Horizontal") > 0)
        {
            spriteRenderer.flipX = false;
            
            if (currentAnimation != Animations.RUN)
            {
                animator.Play((int)Animations.RUN);
            }
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            spriteRenderer.flipX = true;
            if (currentAnimation != Animations.RUN)
            {
                animator.Play((int)Animations.RUN);
                currentAnimation = Animations.RUN;
            }
        }
        else
        {
            animator.Play((int)Animations.STAND);
            currentAnimation = Animations.STAND;
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

            if (currentAnimation != Animations.JUMP)
            {
                animator.Play((int)Animations.JUMP);
                currentAnimation = Animations.JUMP;
            }
        }

        if (Input.GetButton("Die")) {
            Die();
        }
    }

    void Die()
    {
        // Can't die on the starting line
        Vector2Int point = roundToGrid(this.transform.position);
        if ((point - start_point).magnitude < 3) return;

        // Subtract one life
        // TODO

        // Start death animations

        // Check if we have finished the map
        foreach(GameObject fin in GameObject.FindGameObjectsWithTag("Finish")) {
            BoxCollider2D box = fin.GetComponent<BoxCollider2D>();
            if(box != null && box.bounds.Contains(this.transform.position)) {
                fin.GetComponent<LevelExit>().Exit();
            }
        }

        // Just die
        terrain.SetTile(new Vector3Int(point.x, point.y, 0), tombTile);
        this.transform.position = new Vector2(start_point.x + 0.5f, start_point.y + 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TrapCollider trap_script = collision.gameObject.GetComponent<TrapCollider>();
        if (trap_script != null) Die();
    }

}
