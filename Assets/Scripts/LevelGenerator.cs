﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public int LevelLength = 500;
    public int LevelHeight = 300;
    public int Seed = 0;
    public UnityEngine.Tilemaps.Tilemap tilemap;
    public UnityEngine.Tilemaps.TileBase ground;
    public GameObject player;

    private System.Random prng;

    // Use this for initialization
    void Awake () {
        if (Seed == 0) prng = new System.Random();
        else prng = new System.Random(Seed);
        int[,] map = generate();

        int top = 0, bottom = 0;
        for(int yy = 0; yy < LevelHeight; yy++) {
            bool any = false;
            for(int xx = 0; xx < LevelLength; xx++) {
                if(map[xx, yy] != 0) {
                    any = true;
                    break;
                }
            }
            if (any) {
                bottom = Math.Max(0, yy - 2);
                break;
            }
        }

        for (int yy = LevelHeight-1; yy >= 0; yy--) {
            bool any = false;
            for (int xx = 0; xx < LevelLength; xx++) {
                if (map[xx, yy] != 0) {
                    any = true;
                    break;
                }
            }
            if (any) {
                top = Math.Min(yy + 2, LevelHeight - 1);
                break;
            }
        }

        Debug.LogFormat("Map between {0} and {1}", bottom, top);
        for (int xx = 0; xx < LevelLength; xx++) {
            for (int yy = bottom; yy < top; yy++) {
                if (map[xx, yy] == 0) {
                    Vector3Int index = new Vector3Int(xx, yy-bottom, 0);
                    tilemap.SetTile(index, ground);
                }
            }
        }

        for (int xx = 0; xx < 3; xx++) {
            for (int yy = bottom; yy < top; yy++) {
                Vector3Int index = new Vector3Int(LevelLength + xx, yy - bottom, 0);
                tilemap.SetTile(index, ground);
            }
        }

        for (int yy = bottom, xx = 0; yy < top; yy++, xx++) {
            if(map[xx, yy] != 0) {
                player.transform.SetPositionAndRotation(new Vector3(xx+1, yy - bottom + 5), new Quaternion());
                break;
            }
        }
    }

    struct Point
    {
        public Point(int a, int b) { x = a; y = b; }
        public int x;
        public int y;
    }

    int Height(List<Point> lines, int xx)
    {
        int before = 0;
        while (xx > lines[before + 1].x) before++;
        int gap = lines[before + 1].x - lines[before].x;
        float distance = (float)(xx - lines[before].x) / gap;
        return (int)(lines[before + 1].y * distance + lines[before].y * (1 - distance));
    }

    void Square(int[,] grid, int cx, int cy, int square_width, int square_height)
    {
        int hw = (int)Math.Ceiling(square_width / 2.0);
        int hh = (int)Math.Ceiling(square_height / 2.0);
        for (int xx = 0; xx < square_width; xx++) {
            int tx = Math.Min(LevelLength - 1, Math.Max(0, cx + xx - hw));
            for (int yy = 0; yy < square_height; yy++) {
                int ty = Math.Min(LevelHeight - 1, Math.Max(0, cy + yy - hh));
                grid[tx, ty] = 1;
            }
        }
    }

    int Clamp(int a, int b, int c)
    {
        return Math.Min(c, Math.Max(a, b));
    }

    int[,] smooth_grid(int[,] grid, int smooth_range) {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        int[,] smoothed = new int[width, height];
    
        for(int xx = 0; xx < width; xx++){
            for(int yy = 0; yy < height; yy++){
                int total = 0;
                int hit = 0;
                for(int in_x = Clamp(xx - smooth_range, 0, width-1); in_x <= Clamp(xx + smooth_range, 0, width-1); in_x++){
                    for(int in_y = Clamp(yy - smooth_range, 0, height-1); in_y <= Clamp(yy + smooth_range, 0, height-1); in_y++){
                        total += grid[in_x, in_y];
                        hit += 1;
                    }
                }
                smoothed[xx, yy] = Mathf.RoundToInt((float)total/(float)hit);
            }
        }

        return smoothed;
    }

    int[,] generate() { 
        int[,] grid = new int[LevelLength, LevelHeight];
        int width = LevelLength;
        int height = LevelHeight;

        int prev = height / 2;
        List<Point> lines = new List<Point>();
        for(int start = 0; start < LevelLength-40; start += prng.Next(5, 50)){
            lines.Add(new Point(start, prev));
            prev = Math.Min(LevelHeight - 1, Math.Max(0, prev + prng.Next(-100, 50)));
        }
        lines.Add(new Point(width, prev));

        int nextWidth = prng.Next(5, 20);
        int prev_height = Height(lines, nextWidth);
        for(int start = nextWidth; start < width;) {

            int bwidth = nextWidth;
            nextWidth = prng.Next(2, 8);

            int step = ((bwidth + nextWidth) >> 1);
            int nextStart = start + prng.Next(step / 2, step);


            int bheight = Math.Max(prng.Next(3, 12), 1 + prev_height - Height(lines, start));
            int up = prng.Next(bheight/4, bheight / 2);
            prev_height = Height(lines, start) + up;

            Square(grid, start, Height(lines, start) + up, bwidth, bheight);

            start = nextStart;
        }

        return smooth_grid(grid, 1);
    }
}
