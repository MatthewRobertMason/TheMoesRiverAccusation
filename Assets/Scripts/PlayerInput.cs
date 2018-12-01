using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    private Rigidbody2D body;
    private Vector2Int start_point;
    public UnityEngine.Tilemaps.Tilemap tilemap;
    public UnityEngine.Tilemaps.TileBase tombTile;

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
        }

        if (Input.GetButton("Die")) {
            Die();
        }
    }

    void Die()
    {
        Vector2Int point = roundToGrid(this.transform.position);
        if (point == start_point) return;

        tilemap.SetTile(new Vector3Int(point.x, point.y, 0), tombTile);
        this.transform.position = new Vector2(start_point.x + 0.5f, start_point.y + 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
                    
    }
}
