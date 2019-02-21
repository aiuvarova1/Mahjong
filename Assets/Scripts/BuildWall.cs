using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWall : MonoBehaviour {

    Vector3 startPosition1 = new Vector3(20,2.2f,17);
    Vector3 startPosition2 = new Vector3(17,2.2f,-19);
    Vector3 startPosition3 = new Vector3(-20,2.2f,-17);
    Vector3 startPosition4 = new Vector3(-17,2.2f,19);
    public GameObject tile;

    public List<Tile> [] tiles=new List<Tile>[4];

    //for texture
    static List<Vector2> coordinates = new List<Vector2>(42);
    static List<Tile> availableTiles = new List<Tile>(144);

    //all possible texture coordinates
    static void FillCoordinates()
    {
        //flowers
        for (int x = 0; x < 8; x++)
        {
            coordinates.Add(new Vector2(x, 0));
        }
        //other tiles
        for (int y = 1; y < 5; y++)
        {
            //winds and dragons
            if (y == 1)
            {
                for (int x = 0; x <= 6; x++)

                    coordinates.Add(new Vector2(x , y));
            }
            else
            {
                for (int x = 0; x < 9; x++)

                    coordinates.Add(new Vector2(x , y));

            }
        }

    }

    //creates list of tiles which are not used in the wall yet
    void CreateTileVariants(GameObject tile)
    {
        for (int i = 0; i < coordinates.Count; i++)
        {
            availableTiles.Add(new Tile(Instantiate(tile), false, coordinates[i]));
            //if not flowers=>3 more copies
            if (i > 7)
            {
                for (int k = 0; k < 3; k++)
                {
                    availableTiles.Add(new Tile(Instantiate(availableTiles[i].tile), true, coordinates[i]));
                }
            }

        }
    }

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 4; i++)
        {
            tiles[i] = new List<Tile>(36);
        }
        FillCoordinates();
        CreateTileVariants(tile);
        CreateWall(1, tile);
        CreateWall(2, tile);
        CreateWall(3, tile);
        CreateWall(4, tile);
    }

    public void CreateWall(int num,GameObject tile)
    {
        //coordinates of first right tile in the second row
        Vector3 start=new Vector3();
        float rotation=90;
        //if coefficient>0,x or z coordinate grows else reduces
        int c = -1;
        switch (num)
        {
            case 1:
                start = startPosition1;
                break;
            case 2:
                start = startPosition2;
                rotation = 180;
                break;
            case 3:
                start = startPosition3;
                rotation = -90;
                c = 1;
                break;
            case 4:
                start = startPosition4;
                rotation = 0;
                c = 1;
                break;
        }
        //fiil the wall from right to left
        for (int i = 0; i < 18; i++)
        {
            Vector3 upperPosition = new Vector3(start[0] + c * 2 * i * ((num + 1) % 2), start[1], start[2] + c * 2 * i * (num % 2));
            Vector3 lowerPosition = new Vector3(start[0] + c * 2 * i * ((num + 1) % 2), start[1] - 1.2f, start[2] + c * 2 * i * (num % 2));
            //second row(even)
            int randomTileIndex = Random.Range(0, availableTiles.Count);
            tiles[num-1].Add(availableTiles[randomTileIndex]);
            availableTiles.RemoveAt(randomTileIndex);
            //tiles[num - 1].Add(new Tile(Instantiate(tile)));
            tiles[num - 1][2*i].tile.transform.position=upperPosition;
            tiles[num - 1][2 * i].tile.transform.rotation = Quaternion.Euler(0,rotation,0);

            //first row(odd)

            randomTileIndex = Random.Range(0, availableTiles.Count);
            tiles[num-1].Add(availableTiles[randomTileIndex]);
            availableTiles.RemoveAt(randomTileIndex);
            //tiles[num - 1].Add(new Tile(Instantiate(tile)));
            tiles[num - 1][2 * i+1].tile.transform.position = lowerPosition;
            tiles[num - 1][2 * i+1].tile.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

    }
}
