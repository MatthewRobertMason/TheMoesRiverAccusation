using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Linq;

public class ShadowLight : MonoBehaviour
{
    public Texture2D tex;
    public float lightDistance = 10.0f;
    public float raycastTolerance = 0.001f;
    public float numberOfExtraRayCastPoints = 360;
    public float penetrationDistance = 0.5f;

    public bool lightFlicker = false;
    public float flickerDistance = 0.5f;
    public float flickerAmount = 0.0f;

    public enum lightingType
    {
        Static,
        Dynamic
    }
    public lightingType lightType = lightingType.Static;

    // For Diagnostic purposes
    [Header("Diagnostic")]
    public float minPointx = 0.0f;
    public float minPointy = 0.0f;
    public float maxPointx = 0.0f;
    public float maxPointy = 0.0f;
    
    void Start ()
    {
        flickerAmount = Random.Range(-flickerDistance, flickerDistance);
        CreateSpriteMap();
    }
	
    private void CreateSpriteMap()
    {
        Physics2D.queriesStartInColliders = true;

        Tilemap[] tileMaps = FindObjectsOfType<Tilemap>();
        CompositeCollider2D collider = null;
        
        foreach (Tilemap tm in tileMaps)
        {
            if (tm.GetComponent<CompositeCollider2D>() != null)
            {
                collider = tm.GetComponent<CompositeCollider2D>();
                break;
            }
        }

        Vector2[][] points = new Vector2[collider.pathCount][];

        for (int path = 0; path < collider.pathCount; path++)
        {
            points[path] = new Vector2[collider.GetPathPointCount(path)];
            collider.GetPath(path, points[path]);
        }

        List<Vector2> searchPoints = new List<Vector2>();

        Vector2 pos = new Vector2(this.transform.position.x, this.transform.position.y);

        foreach (Vector2[] path in points)
        {
            foreach (Vector2 vector in path)
            {
                if (Vector2.Distance(vector, pos) < lightDistance)
                {
                    searchPoints.Add(vector);
                }
            }
        }

        //searchPoints = searchPoints.OrderBy(v => Mathf.Atan2((v - pos).x, (v - pos).y) * Mathf.Rad2Deg).ToList();
        List<Vector2> rayTracedPoints = new List<Vector2>();
        
        Vector2 direction;
        float dist = lightDistance;

        if (lightFlicker)
        {
            flickerAmount += Random.Range(-flickerDistance / 10.0f, flickerDistance / 10.0f);
            flickerAmount = Mathf.Clamp(flickerAmount, -flickerDistance, flickerDistance);

            dist = lightDistance + flickerAmount;
        }

        rayTracedPoints.Add(RaytracedPoint(Physics2D.Raycast(pos, Vector2Rotate(Vector2.down, -raycastTolerance), dist), pos, Vector2.down));
        rayTracedPoints.Add(RaytracedPoint(Physics2D.Raycast(pos, Vector2.down, dist), pos, Vector2.down));
        rayTracedPoints.Add(RaytracedPoint(Physics2D.Raycast(pos, Vector2Rotate(Vector2.down, raycastTolerance), dist), pos, Vector2.down));

        foreach (Vector2 v in searchPoints)
        {
            direction = v - pos;
            Vector2 _direction = direction;
            
            _direction = Vector2Rotate(direction, -raycastTolerance);
            rayTracedPoints.Add(RaytracedPoint(Physics2D.Raycast(pos, _direction, dist), pos, _direction));

            rayTracedPoints.Add(RaytracedPoint(Physics2D.Raycast(pos, direction, dist), pos, direction));

            _direction = Vector2Rotate(direction, raycastTolerance);
            rayTracedPoints.Add(RaytracedPoint(Physics2D.Raycast(pos, _direction, dist), pos, _direction));
        }

        for (int r = 0; r < numberOfExtraRayCastPoints; r++)
        {
            direction = Vector2Rotate(Vector2.down, (360.0f / numberOfExtraRayCastPoints) *r);
            rayTracedPoints.Add(RaytracedPoint(Physics2D.Raycast(pos, direction, dist), pos, direction));
        }

        rayTracedPoints = rayTracedPoints.OrderBy(v => Mathf.Atan2((v - pos).x, (v - pos).y) * Mathf.Rad2Deg).ToList();
        
        List<Vector2> spriteVerts = new List<Vector2>();
        
        spriteVerts.Add(new Vector2(pos.x, pos.y));
        
        foreach (Vector2 v in rayTracedPoints)
        {
            spriteVerts.Add(new Vector2(v.x, v.y));
        }

        //bool fixX = false;
        //bool fixY = false;
        minPointx = spriteVerts.Min(p => p.x);
        minPointy = spriteVerts.Min(p => p.y);
        maxPointx = spriteVerts.Max(p => p.x);
        maxPointy = spriteVerts.Max(p => p.y);
        
        if (minPointx < 0.0f)
        {
            //fixX = true;
            //minPointx -= minPointx;
            maxPointx -= minPointx;
        }

        if (minPointy < 0.0f)
        {
            //fixY = true;
            //minPointy -= minPointy;
            maxPointy -= minPointy;
        }

        List<Vector2> verts = new List<Vector2>();
        
        foreach (Vector2 v in spriteVerts)
        {
            verts.Add(new Vector2(
                ((v.x - minPointx) / maxPointx)*16.0f, 
                ((v.y - minPointy) / maxPointy)*16.0f
            ));
            
            
            if ((verts.Last().x > 16.0f) || (verts.Last().x < 0.0f) ||
                (verts.Last().y > 16.0f) || (verts.Last().y < 0.0f))
            {
                Debug.DrawLine(pos, v,Color.red);
            }
            else
                Debug.DrawLine(pos, v, Color.blue);
            
        }
        
        int vertCount = verts.Count();
        int triPoints = (vertCount+1) * 3;
        ushort[] tris = new ushort[triPoints+3];

        ushort j = 1;
        for (int i = 0; i < triPoints; i += 3)
        {
            tris[i] = 0;
            tris[i + 1] = (ushort)(j % vertCount);
            tris[i + 2] = (ushort)((j + 1) % vertCount);
            j++;
        }

        tris[triPoints]     = 0;
        tris[triPoints + 1] = (ushort)(vertCount-1);
        tris[triPoints + 2] = 1;

        Rect rect = new Rect(0, 0, 16, 16);

        Vector2[] sVerts = verts.ToArray();

        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0.0f, 0.0f), 16);
        SpriteMask mask = this.GetComponentInChildren<SpriteMask>();

        mask.sprite = sprite;
        mask.gameObject.transform.position = new Vector2(pos.x - (pos.x-minPointx), pos.y - (pos.y-minPointy));
        mask.gameObject.transform.localScale = new Vector3(maxPointx, maxPointy, 1.0f);
        
        sprite.OverrideGeometry(
            sVerts, 
            tris
        );
    }

	void Update ()
    {
        if (lightType == lightingType.Dynamic)
            CreateSpriteMap();
    }

    private Vector2 Vector2Rotate(Vector2 v, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * v;
    }

    private Vector2 RaytracedPoint(RaycastHit2D rayHit, Vector2 pos, Vector2 direction)
    {
        if (rayHit.collider != null)
        {
            return rayHit.point + (direction.normalized * penetrationDistance);
        }
        else
        {
            // We've gone into open air and need to make a point
            Vector2 newPoint;

            if (lightFlicker)
            {
                newPoint = pos + ((direction.normalized * lightDistance) + (direction.normalized * (penetrationDistance + flickerAmount)));
            }
            else
            {
                newPoint = pos + ((direction.normalized * lightDistance) + (direction.normalized * penetrationDistance));
            }

            return newPoint;
        }
    }
}
