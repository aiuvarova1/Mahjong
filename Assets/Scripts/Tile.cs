using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;


public class Tile:IComparable<Tile>
{
    public string name;
    public GameObject tile;
    int numOfTile;

    public bool isOwned=false;

    

    public Tile(GameObject tile,bool created,Vector2 coords)
    {
        this.tile = tile;
        if(!created)
            CreateFace(coords);
        CreateName(coords);


    }

    public int CompareTo(Tile tileToCompare)
    {
        return numOfTile.CompareTo(tileToCompare.numOfTile);
    }

    public void MakeVisible()
    {
        Material[] materials = tile.GetComponent<Renderer>().materials;

        for (int i = 0; i < materials.Length; i++)
        {
            Color color = materials[i].color;
            color.a = 1;
            materials[i].color = color;
        }
    }

    void CreateName(Vector2 coordinates)
    {
        //y
        switch ((int)coordinates[1])
        {
            case 0:
                name += 'f';
                name += (int)coordinates[0]+1;
                if ((int)coordinates[0] % 4 == 0)
                {
                    tile.GetComponent<TileName>().Name = $"Season {(int)coordinates[0] + 1}";
                    tile.GetComponent<TileName>().RuName = $"Сезон {(int)coordinates[0] + 1}";
                }
                else
                {
                    tile.GetComponent<TileName>().Name = $"Flower {(int)coordinates[0] % 4 + 1}";
                    tile.GetComponent<TileName>().RuName = $"Цветок {(int)coordinates[0] % 4 + 1}";
                }

                numOfTile = 35 + (int)coordinates[0];
                break;
            case 2:
                name += 's';
                name += (int)coordinates[0]+1;
                tile.GetComponent<TileName>().Name = $"Symbols {(int)coordinates[0] + 1}";
                tile.GetComponent<TileName>().RuName = $"Символы {(int)coordinates[0] + 1}";
                numOfTile =19+ (int)coordinates[0]; 
                break;

            case 3:
                name += 'b';
                name += (int)coordinates[0]+1;
                tile.GetComponent<TileName>().Name = $"Bamboo {(int)coordinates[0] + 1}";
                tile.GetComponent<TileName>().RuName = $"Бамбук {(int)coordinates[0] + 1}";
                numOfTile = (int)coordinates[0];
                break;
            case 4:
                name += 'd';
                name += (int)coordinates[0]+1;
                tile.GetComponent<TileName>().Name = $"Dots {(int)coordinates[0] + 1}";
                tile.GetComponent<TileName>().RuName = $"Доты {(int)coordinates[0] + 1}";
                numOfTile = 10+ (int)coordinates[0]; 
                break;
            case 1:
                switch ((int)coordinates[0])
                {
                    case 0:
                        name = "East";
                        numOfTile = 28;
                        tile.GetComponent<TileName>().Name = name;
                        tile.GetComponent<TileName>().RuName = "Восток";
                        break;
                    case 1:
                        name = "South";
                        numOfTile = 29;
                        tile.GetComponent<TileName>().Name = name;
                        tile.GetComponent<TileName>().RuName = "Юг";
                        break;
                    case 2:
                        name = "West";
                        numOfTile = 30;
                        tile.GetComponent<TileName>().Name = name;
                        tile.GetComponent<TileName>().RuName = "Запад";
                        break;
                    case 3:
                        name = "North";
                        numOfTile = 31;
                        tile.GetComponent<TileName>().Name = name;
                        tile.GetComponent<TileName>().RuName = "Север";
                        break;
                    case 4:
                        name = "Red";
                        numOfTile = 32;
                        tile.GetComponent<TileName>().Name = name;
                        tile.GetComponent<TileName>().RuName = "Красный дракон";
                        break;
                    case 5:
                        name = "Green";
                        numOfTile = 33;
                        tile.GetComponent<TileName>().Name = name;
                        tile.GetComponent<TileName>().RuName = "Зелёный дракон";
                        break;
                    case 6:
                        name = "White";
                        numOfTile = 34;
                        tile.GetComponent<TileName>().Name = name;
                        tile.GetComponent<TileName>().RuName = "Белый дракон";
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

    public override string ToString()
    {
        return name;
    }
}

