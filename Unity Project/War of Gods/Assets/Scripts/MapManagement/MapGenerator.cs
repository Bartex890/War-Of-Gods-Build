using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct TileSprites
    {
        public Sprite[] tiles;
    }

    public RuleTile[] tiles; // 0 - grass // 1 - water // 2 - mountains // 3 - bridge

    public SpriteRenderer tileBase;

    //old_way
    /*public void GenerateMap(Map map)
    {
        for (int x = 0; x < map.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.tiles.GetLength(1); y++)
            {
                int id = 0;
                //translate tile names to sprites
                switch(map.tiles[x, y])
                {
                    case "": id = 0; break; //grass
                    case "r": id = 1; break; //rivers
                    case "g": id = 2; break; //mountains
                    case "m": id = 3; break; //bridge
                    case "l": id = 4; break; //forest
                    case "o": id = 5; break; //ocean
                    case "s": id = 6; break; //road
                }

                //generate
                tileBase.sprite = tiles[id].tiles[Random.Range(0, tiles[id].tiles.Length)];
                Instantiate(tileBase.gameObject, new Vector3(x, -y, 0), Quaternion.identity).SetActive(true);
            }
        }
    }*/
    public void GenerateMap(Map map)
    {
        for (int x = 0; x < map.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.tiles.GetLength(1); y++)
            {
                int id = 0;
                //translate tile names to sprites
                switch (map.tiles[x, y])
                {
                    case "": id = 0; break; //grass
                    case "r": id = 1; break; //rivers
                    case "g": id = 2; break; //mountains
                    case "m": id = 3; break; //bridge
                    case "l": id = 4; break; //forest
                    case "o": id = 5; break; //ocean
                    case "s": id = 6; break; //road
                }

                if(tiles[id].rules.Length > 0)
                {
                    foreach(RuleTile.Rule rule in tiles[id].rules)
                    {
                        string[,] rules = CsvOperations.CsvToArray(rule.rules);

                        bool isEverythingOk = true;

                        for(int rx = 0; rx < 3; rx++)
                        {
                            for (int ry = 0; ry < 3; ry++)
                            {
                                if (x + rx - 1 < 0 || y + ry - 1 < 0 || x + rx - 1 >= map.tiles.GetLength(0) || y + ry - 1 >= map.tiles.GetLength(1))
                                    continue;
                                bool areRequirementsMet = false;
                                bool neg = false;
                                foreach(char c in rules[rx, ry])
                                {
                                    if (c == '*') { areRequirementsMet = true; continue; }
                                    if (c == '-') { neg = true; continue; }

                                    if ((map.tiles[x + rx - 1, y + ry - 1].Length == 0 && c == ' ') 
                                        || (map.tiles[x + rx - 1, y + ry - 1].Length != 0 && map.tiles[x + rx - 1, y + ry - 1][0] == c))
                                    {
                                        if (neg)
                                        {
                                            areRequirementsMet = false;
                                            break;
                                        }
                                        else areRequirementsMet = true;
                                    }

                                    neg = false;
                                }

                                if (!areRequirementsMet)
                                {
                                    isEverythingOk = false;
                                    goto ruleLoopEnd;
                                }
                            }
                        }
                    ruleLoopEnd:

                        if (isEverythingOk)
                        {
                            tileBase.sprite = rule.sprite[Random.Range(0, tiles[id].defaultSprite.Length)];
                            Instantiate(tileBase.gameObject, new Vector3(x, -y, 0), Quaternion.identity).SetActive(true);
                            goto skipDefault;
                        }
                    }
                }
                tileBase.sprite = tiles[id].defaultSprite[Random.Range(0, tiles[id].defaultSprite.Length)];
                Instantiate(tileBase.gameObject, new Vector3(x, -y, 0), Quaternion.identity).SetActive(true);
                skipDefault:;
            }
        }
    }
}

