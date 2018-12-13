using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dude {

    // Static constants and methods

    public enum Part
    {
        Face,
        Head,
        Eyes,
        Skin,
        Suit,
        Item,
        Hair
    }

    Dictionary<Part, int[]> colorThemeDefault = new Dictionary<Part, int[]>{
        {Part.Eyes, new[]{ 0xee7755 } },
        {Part.Skin, new[]{ 0xcccc77, 0xaaaa55, 0x888844 } },
        {Part.Suit, new[]{ 0x7722aa, 0x552277 } },
        {Part.Item, new[]{ 0xdd77bb, 0xaa5599, 0xeebbee } },
        {Part.Hair, new[]{ 0xeeeeee, 0xcccccc } },
    };

    static Dictionary<Part, int[][]> colorThemes = new Dictionary<Part, int[][]>{
        {Part.Eyes, new[]{
            new[]{0x222033},
            new[]{0x178178},
            new[]{0x7722ab},
            new[]{0x346524},
            new[]{0x5a8ca6},
            new[]{0xfafafa},
            new[]{0xababab},
            new[]{0x751f20}
        } },
        {Part.Skin, new[]{
            new[]{0xcccc77, 0xaaaa55, 0x888844},
            new[]{0xf0f0dd, 0xd1d1c2, 0xb1b1b1},
            new[]{0xccccbe, 0x877d78, 0x675d58},
            new[]{0xe6d1bc, 0xd9af83, 0xb98f73},
            new[]{0xcb9f76, 0xaf8055, 0x8f6035},
            new[]{0xa47d5b, 0x7c5e46, 0x5c3e56},
            new[]{0x7a3333, 0x56252f, 0x36051f},
            new[]{0x686e46, 0x505436, 0x303416},
            new[]{0xdcb641, 0xaa6622, 0x8a4602},
            new[]{0x72b8e4, 0x5d96ba, 0x3d76aa},
            new[]{0xaa4951, 0x8a344d, 0x6a142d},
            new[]{0x887777, 0x554444, 0x775555},
            new[]{0x434343, 0x353535, 0x3e3e3e},
            new[]{0x6cb832, 0x3c8802, 0x4c9812}
        } },
        {Part.Suit, new[]{
            new[]{0xccaa44, 0xaa6622},
            new[]{0xd04648, 0xaa3333},
            new[]{0xa9b757, 0x828a58},
            new[]{0xf0f0dd, 0xd1d1c2},
            new[]{0x944a9c, 0x5a3173},
            new[]{0x447ccf, 0x3d62b3},
            new[]{0x3e3e3e, 0x353535}
        } },
        {Part.Item, new[]{
            new[]{0xccaa44, 0xaa6622, 0xc89437},
            new[]{0xd04648, 0xaa3333, 0xcaacac},
            new[]{0xa9b757, 0x828a58, 0xc1cd74},
            new[]{0xf0f0dd, 0xd1d1c2, 0xfdfdfb},
            new[]{0x944a9c, 0x5a3173, 0xae68b6},
            new[]{0x447ccf, 0x3d62b3, 0x69b7d8},
            new[]{0x3e3e3e, 0x353535, 0x434343}
        } },
        {Part.Hair, new[]{
            new[]{0xebebeb, 0xc7c7c7},
            new[]{0xe4da99, 0xd9c868},
            new[]{0xb62f31, 0x751f20},
            new[]{0xcc7733, 0xbb5432},
            new[]{0x4d4e4c, 0x383839}
        } }
    };


    static readonly int[][] HEAD_ORIGINS = new [] {
        new [] {0},
        new [] {0, 1, 2, 1 },
        new [] {-1, -2, 0, 1 },
        new [] {-1, -1, -1, 1, 2, 1, 0 }
    };
    static readonly int[][] EYES_ORIGINS = HEAD_ORIGINS;

    public static int[] parseParamString(string seed) {
        var values = new int[7];
        for (int ii = 0; ii < 7; ii++) {
            values[ii] = int.Parse(seed.Substring(ii * 2 + 1, 2), System.Globalization.NumberStyles.HexNumber);
            values[ii] = System.Math.Min(values[ii], paramValueOptions((Part)ii) - 1);
        }
        return values;
    }

    public static int[] randomParamString()
    {
        var values = new int[7];
        for (int ii = 0; ii < 7; ii++) {
            values[ii] = Random.Range(0, paramValueOptions((Part)ii));
        }
        return values;
    }

    public static int paramValueOptions(Part part)
    {
        switch (part) {
            case Part.Face: return 13;
            case Part.Head: return 28;
            default:
                return colorThemes[part].Length;
        }
    }

    public Texture2D texture;
    public Texture2D base_image;
    public Texture2D parts_image;

    public int[] seed;

    public Dude(Texture2D template, Texture2D parts, int[] seed)
    {
        Debug.LogFormat("{0} {1} {2}", seed[0], seed[1], seed[2]);
        base_image = template;
        parts_image = parts;
        texture = new Texture2D(template.width, template.height);
        this.seed = seed;
        texture.SetPixels32(base_image.GetPixels32());

        buildPart(Part.Face, EYES_ORIGINS.Length, 3, EYES_ORIGINS);
        buildPart(Part.Head, HEAD_ORIGINS.Length, 0, HEAD_ORIGINS);
        buildPart(Part.Head, EYES_ORIGINS.Length, 1, HEAD_ORIGINS);
        buildPart(Part.Face, EYES_ORIGINS.Length, 2, EYES_ORIGINS);
        texture.Apply();

        recolorAll();
        texture.Apply();
    }

    // recalcVal
    void buildPart(Part part, int a, int t, int[][] r){
        for (int y = 0; y < r.Length; y++) {
            var n = r[y];
            for (int x = 0; x < n.Length; x++) {
                drawShape(x, y, seed[(int)part], t, n);
            }
        }
    }

    //function drawShape(e, a, t, r, n)
    void drawShape(int x, int y, int t, int r, int[] n){
        var c = false;
        if (t == 4 && r == 2) {
            if (y == 1) {
                if (y == 2 && x == 2) {
                    draw16(x * 16, y * 24 + 0, parts_image, 4, 4);
                    c = true;
                } else if (y == 1 && x == 1) {
                    draw16(x * 16, y * 24 + 1, parts_image, 4, 4);
                    c = true;
            } else if (y == 1 && x == 2) {
                    draw16(x * 16, y * 24 + 2, parts_image, 5, 4);
                    c = true;
            } else if (y == 1 && x == 3) {
                    draw16(x * 16, y * 24 + 1, parts_image, 4, 4);
                    c = true;
            }
            } else if (y == 3) {
                draw16(x * 16, y * 24 + EYES_ORIGINS[3][x], parts_image, 4, 4);
                c = true;
            }
        } else if (t == 6 && r == 0) {
            if (y == 1 && (x == 1 || x == 3)) {
                draw16(x * 16, y * 24 + HEAD_ORIGINS[1][x], parts_image, 6, 4);
                c = true;
            } else if (y == 1 && x == 2) {
                draw16(x * 16, y * 24 + HEAD_ORIGINS[1][x], parts_image, 7, 4);
                c = true;
            } else if (y == 2 && x == 3) {
                draw16(x * 16, y * 24 + HEAD_ORIGINS[1][x], parts_image, 6, 4);
                c = true;
            } else if (y == 3 && x == 2) {
                draw16(x * 16, y * 24 + 2, parts_image, 8, 4);
                c = true;
            } else if (y == 3 && (x == 5 || x == 3)) {
                draw16(x * 16, y * 24 + HEAD_ORIGINS[3][x], parts_image, 6, 4);
                c = true;
            } else if (y == 3 && x == 4) {
                draw16(x * 16, y * 24 + HEAD_ORIGINS[3][x], parts_image, 7, 4);
                c = true;
            } else if (y == 3 && x == 1) {
                draw16(x * 16, y * 24 + 2, parts_image, 9, 4);
                c = true;
            }
        } else if (t == 8 && r == 3) {
            if ((x == 1 || x == 3) && y == 3) {
                draw16(x * 16, y * 24 + EYES_ORIGINS[3][x], parts_image, 10, 4);
                c = true;
            }
            if (x == 2 && y == 3) {
                draw16(x * 16, y * 24 + EYES_ORIGINS[3][x], parts_image, 11, 4);
                c = true;
            }
        } else if (t == 13 && (r == 0 || r == 1)) {
            drawShape(x, y, 6, r, n);
            drawShape(x, y, 12, r, n);
            c = true;
        } else if (t == 14 && (r == 0 || r == 1)) {
            drawShape(x, y, 10, r, n);
            drawShape(x, y, 12, r, n);
            c = true;
        } else if (t == 15 && (r == 0 || r == 1)) {
            drawShape(x, y, 7, r, n);
            drawShape(x, y, 12, r, n);
            c = true;
        } else if (t == 23 && (r == 0 || r == 1)) {
            drawShape(x, y, 6, r, n);
            drawShape(x, y, 22, r, n);
            c = true;
        } else if (t == 24 && (r == 0 || r == 1)) {
            drawShape(x, y, 7, r, n);
            drawShape(x, y, 22, r, n);
            c = true;
        }
        if (!c) {
            draw16(x * 16, y * 24 + n[x], parts_image, t, r);
        }
    }

    void draw16(int a, int t, Texture2D r, int n = 0, int c = 0)
    {
        var i = 16;
        var o = 20;
        var d = 4;

        //        var data = r.GetPixels(n * i, c * o, i, o);
        //        texture.SetPixels(a, t + d, i, o, data);
        applyImage(a, t + d, n * i, c * o, i, o);
    }

    void applyImage(int dx, int dy, int sx, int sy, int width, int height)
    {
        var parts = parts_image.GetPixels32();
        var tex = texture.GetPixels32();

        for (int x = 0; x < width; x++) {
            if (dx + x >= texture.width) continue;
            for(int y = 0; y < height; y++) {
                int oy = texture.height - (dy + y);
                if (oy >= texture.height) continue;

                int part_index = sx + x + parts_image.width * (parts_image.height - (sy + y));
                if (part_index >= parts.Length) continue;
                var color = parts[part_index];
                if(color.a == 255) {
                    tex[dx + x + texture.width * oy] = color;
                    //set_pixel = true;
                }
            }
        }

        texture.SetPixels32(tex);
        texture.Apply();
    }

    void recolorAll() {
        var data = texture.GetPixels32();

        foreach (var part in colorThemes.Keys) {
            Debug.Log(part);
            var theme = colorThemes[part][seed[(int)part]];
            applyTheme(data, colorThemeDefault[part], theme);
        }

        texture.SetPixels32(data);
    }

    void applyTheme(Color32[] data, int[] current, int[] theme)
    {
        int[] near = new[] {999, 999, 999};
        for (int colour_index = 0; colour_index < current.Length; colour_index++) {
            var n = current[colour_index];
            int raw = theme[colour_index];

            var r = (byte)((n >> 16) & 0xFF);
            var g = (byte)((n >> 8) & 0xFF);
            var b = (byte)(n & 0xFF);

            for (int i = 0; i < data.Length; i++) {
                if (data[i].a == 0) continue;
                near[0] = System.Math.Min(System.Math.Abs(data[i].r - r), near[0]);
                near[1] = System.Math.Min(System.Math.Abs(data[i].g - g), near[1]);
                near[2] = System.Math.Min(System.Math.Abs(data[i].b - b), near[2]);
                if (data[i].r == r && data[i].g == g && data[i].b == b) {
                    Debug.LogFormat(" << {0} {1} {2} {3}", data[i].a, data[i].r, data[i].g, data[i].b);
                    data[i] = new Color32(
                        (byte)((raw >> 16) & 0xFF),
                        (byte)((raw >> 8) & 0xFF),
                        (byte)(raw & 0xFF),
                        255
                    );
                }
            }
        }
        Debug.LogFormat("Applied {0} {1} {2}",  near[0], near[1], near[2]);
    }

}


public class DudeGenerator : MonoBehaviour
{
    public Texture2D template;
    public Texture2D parts;

    Sprite slice(Texture2D img)
    {
        // TODO the parameters to this function are just nonsense at the moment
        return Sprite.Create(img, new Rect(0, 0, 16, 16), new Vector2(0, 0));
    }

    public Sprite FromWebSeed(string e)
    {
        return slice(new Dude(template, parts, Dude.parseParamString(e)).texture);
    }

    public Sprite Random()
    {
        return slice(new Dude(template, parts, Dude.randomParamString()).texture);
    }

}