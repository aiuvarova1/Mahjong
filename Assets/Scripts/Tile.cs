using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Tile:MonoBehaviour
{
    public string name;
    public GameObject tile;
    

    public Tile(GameObject tile,bool created,Vector2 coords)
    {
        this.tile = tile;
        if(!created)
            CreateFace(coords);
        CreateName(coords);

    }

    void CreateName(Vector2 coordinates)
    {
        //y
        switch ((int)coordinates[1])
        {
            case 0:
                name += 'f';
                name += (int)coordinates[0];
                break;
            case 2:
                name += 's';
                name += (int)coordinates[0];
                break;
            case 3:
                name += 'b';
                name += (int)coordinates[0];
                break;
            case 4:
                name += 'd';
                name += (int)coordinates[0];
                break;
            case 1:
                switch ((int)coordinates[0])
                {
                    case 0:
                        name = "East";
                        break;
                    case 1:
                        name = "South";
                        break;
                    case 2:
                        name = "West";
                        break;
                    case 3:
                        name = "North";
                        break;
                    case 4:
                        name = "Red";
                        break;
                    case 5:
                        name = "Green";
                        break;
                    case 6:
                        name = "White";
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
    void CreateFace(Vector2 coords)
    {
        Vector2 coordinates = new Vector2(coords[0] * 1 / 9f, coords[1] * 1 / 5f);
        tile.GetComponent<Renderer>().materials[1].SetTextureOffset("_DetailAlbedoMap", coordinates);
    }
}

