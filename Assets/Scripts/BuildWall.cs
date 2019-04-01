using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BuildWall : MonoBehaviour {

    Vector3 startPosition1 = new Vector3(20,2.2f,17);
    Vector3 startPosition2 = new Vector3(17,2.2f,-19);
    Vector3 startPosition3 = new Vector3(-20,2.2f,-17);
    Vector3 startPosition4 = new Vector3(-17,2.2f,19);

    public GameObject tilePrefab;

    //public List<Tile> [] tiles=new List<Tile>[4];
    public List<List<WallPair>> tiles = new List<List<WallPair>>(4);

    //for texture
   public List<Vector2> coordinates = new List<Vector2>();
     List<Tile> availableTiles = new List<Tile>(144);
    List<int> availableTilesIndexes = new List<int>();

    public  List<int> indexes = new List<int>();

    public static BuildWall instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);

        for (int i = 0; i < 144; i++)
        {
            availableTilesIndexes.Add(i);
        }
    }

    //all possible texture coordinates
    void FillCoordinates()
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
                    availableTiles.Add(new Tile(Instantiate(availableTiles[availableTiles.Count-1].tile), true, coordinates[i]));
                }
            }

        }
    }

    public void Build(List<int> ind)
    {

        for (int i = 0; i < 4; i++)
        {
            // tiles[i] = new List<Tile>(36);
            tiles.Add(new List<WallPair>(18));
            
        }

        if(coordinates.Count==0)
            FillCoordinates();

        CreateTileVariants(tilePrefab);


        CreateWall(1, tilePrefab,ref ind);
        CreateWall(2, tilePrefab,ref ind);
        CreateWall(3, tilePrefab,ref ind);
        CreateWall(4, tilePrefab,ref ind);

        Wall.instance.Initialize(tiles);
    }

    public void FillIndexes()
    {
        //for (int i = 143; i >= 0; i--)
        //{
        //    indexes.Add(Random.Range(0, i + 1));
        //}

        for (int i = 0; i < 144; i++)
        {
            int index = Random.Range(0, availableTilesIndexes.Count);
            indexes.Add(availableTilesIndexes[index]);

            availableTilesIndexes.RemoveAt(index);
           
        }
    }


    public void CreateWall(int num,GameObject tile,ref List<int> ind)
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

            Tile upper = availableTiles[ind[0]];

            //availableTiles.RemoveAt(ind[0]);
            ind.RemoveAt(0);

            
            upper.tile.transform.position = upperPosition;
            upper.tile.transform.rotation = Quaternion.Euler(0, rotation, 0);

            //first row(odd)

          
            Tile lower = availableTiles[ind[0]];

           // availableTiles.RemoveAt(ind[0]);
            ind.RemoveAt(0);

           
            lower.tile.transform.position = lowerPosition;
            lower.tile.transform.rotation = Quaternion.Euler(0, rotation, 0);

            tiles[num - 1].Add(new WallPair(upper, lower));


        }

    }
}
