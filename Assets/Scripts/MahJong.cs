using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MahJong : Combination
{
    public List<List<Combination>> closedCombinations = new List<List<Combination>>();
    public List<Combination> openedCombinations = new List<Combination>();
    public List<Tile> flowers;

    public MahJong(List<List<Combination>> comb)
    {
        closedCombinations = comb;
    }

    public int CalculateMahJongPoints(string wind,bool playerTurn,bool isFreeTile,int order)
    {
        int score = 20;
        int twice = 1;

        if (playerTurn) score += 2;

        bool noChow = true;
        bool noPoints = true;

        //!!
        bool winds = true;
        bool dragons = true;


        for (int i = 0; i < openedCombinations.Count; i++)
        {

                if (openedCombinations[i].Name == "Chow") noChow = false;
                else if (openedCombinations[i].Name == "Pung" ||
                    openedCombinations[i].Name == "Kong") noPoints = false;

                score += openedCombinations[i].CalculatePoints(wind);
                twice *= openedCombinations[i].doubling;
            
        }

        for (int i = 0; i < closedCombinations.Count; i++)
        {
            for (int j = 0; j < closedCombinations[i].Count; j++)
            {
                if (closedCombinations[i][j].Name == "Chow") noChow = false;
                else if (closedCombinations[i][j].Name == "Pung" ||
                    closedCombinations[i][j].Name == "Kong") noPoints = false;

                closedCombinations[i][j].opened = false;

                score += closedCombinations[i][j].CalculatePoints(wind);
                twice *= closedCombinations[i][j].doubling;
            }
        }

        if (noChow) twice *= 2;
        else if (noPoints) twice *= 2;

        //last tile was free
        if (playerTurn && isFreeTile) twice *= 2;

        //burglary of opened kong
        if (!playerTurn && GameManager.instance.kongDeclarator != null) twice *= 2;

        twice *= CountRestOfWinnerDoubles();

        Debug.Log(score);

        score *= twice;


        Debug.Log(twice + "twice");
        Debug.Log(score + "twice score");
        //flowers!!


        score += GetFlowerPoints(flowers, order);
        Debug.Log(score + "total");

        return score;

    }

    static int GetFlowerPoints(List<Tile> flowers,int order)
    {
        int flowerPoints = 0;

        flowers.Sort();

        List<Tile> seasons = new List<Tile>();
        List<Tile> flows = new List<Tile>();

        for (int i = 0; i < flowers.Count; i++)
        {
            if (flowers[i].name[1].ToString() == (order + 1).ToString())
                flowerPoints += 4;
            else
                flowerPoints += 2;
            if (int.Parse(flowers[i].name[1].ToString()) < 5)
                seasons.Add(flowers[i]);
            else flows.Add(flowers[i]);

        }
        if (seasons.Count == 4)
            flowerPoints *= 2;
        if (flows.Count == 4)
            flowerPoints *= 2;
        return flowerPoints;
    }

    int CountRestOfWinnerDoubles()
    {
        int twice = 1;

        bool clearWithWindsAndDragons = true;
        bool clear = true;
        bool only19WindsDragons = true;
        bool onlyWindsAndDragons = true;

        string suit = "";


        for (int i = 0; i < openedCombinations.Count; i++)
        {

            if (char.IsLower(openedCombinations[i].tileList[0].name[0]))
            {
                onlyWindsAndDragons = false;
                if (suit == "") suit = openedCombinations[i].tileList[0].name[0].ToString();
                
                else if (openedCombinations[i].tileList[0].name[0].ToString() != suit)
                {
                    clear = false;
                    clearWithWindsAndDragons = false;
                }
                Debug.Log(suit);
                for (int j = 0; j < openedCombinations[i].tileList.Count; j++)
                {
                    if (openedCombinations[i].tileList[j].name[1] != '1' && openedCombinations[i].tileList[j].name[1] != '9')
                        only19WindsDragons = false;
                }
                
            }
            else
                clear = false;

        }

        if (clearWithWindsAndDragons && suit!="")
        {
            Debug.Log("check clear wd");

            int suitNum = -1;
            suitNum= (suit[0] == 'b') ?0 : (suit[0] == 'd') ? 1:2;

            Debug.Log(suitNum);

            for (int i = 0; i < closedCombinations.Count; i++)
            {
                Debug.Log(closedCombinations[i].Count);

                if(i!=suitNum && closedCombinations[i].Count>0)
                {
                    Debug.Log("not clear");
                    clear = false;
                    clearWithWindsAndDragons = false;
                }
                if (i > 2 && closedCombinations[i].Count > 0)
                    clear = false;
            }
        }
        if (onlyWindsAndDragons)
        {
            for (int i = 0; i < 3; i++)
            {
                if (closedCombinations[i].Count > 0)
                {
                    onlyWindsAndDragons = false;
                    break;
                }
            }
        }

        if (only19WindsDragons)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < closedCombinations[i].Count; j++)
                {
                    for (int k = 0; k < closedCombinations[i][j].tileList.Count; k++)
                    {
                        if (closedCombinations[i][j].tileList[k].name[1] != '1' && closedCombinations[i][j].tileList[k].name[1] != '9')

                        {
                            only19WindsDragons = false;
                            break;
                        }
                    }
                }
            }
        }
        //0-bamboos,1-dots,2-symbols,3-winds,4-dragons



        if (clearWithWindsAndDragons)
        {
            twice *= 2;
            Debug.Log("clear with winds");

        }
        if (clear)
        {
            twice *= 8;
            Debug.Log("clear");
        }
        if (onlyWindsAndDragons)
        {
            twice *= 8;
            Debug.Log("winds and dragons");
        }

        if (only19WindsDragons)
        {
            twice *= 2;
            Debug.Log("1 9 winds");
        }

        return twice;

    }

    public static int CountNotWinnerDoubles(List<Combination> opened,List<Tile> closed)
    {
        int twice = 1;

        bool clearWithWindsAndDragons = true;
        bool clear = true;
        bool only19WindsDragons = true;
        bool onlyWindsAndDragons = true;

        string suit="";

        for (int i = 0; i < opened.Count; i++)
        {
            if (char.IsLower(opened[i].tileList[0].name[0]))
            {
                onlyWindsAndDragons = false;
                if (suit == "") suit = opened[i].tileList[0].name[0].ToString();
                else if (opened[i].tileList[0].name[0].ToString() != suit)
                {
                    clear = false;
                    clearWithWindsAndDragons = false;
                }

                for (int j = 0; j < opened[i].tileList.Count; j++)
                {
                    if (opened[i].tileList[j].name[1] != '1' && opened[i].tileList[j].name[1] != '9')
                        only19WindsDragons = false;
                }
            }
            else
                clear = false;
        }

        for (int i = 0; i < closed.Count; i++)
        {
            if (char.IsLower(closed[i].name[0]))
            {
                onlyWindsAndDragons = false;
                if (suit == "") suit = closed[i].name[0].ToString();
                else if (closed[i].name[0].ToString() != suit)
                {
                    clear = false;
                    clearWithWindsAndDragons = false;
                }

                if (closed[i].name[1] != '1' && closed[i].name[1] != '9')
                    only19WindsDragons = false;

                
            }
            else
                clear = false;
        }

        if (clearWithWindsAndDragons)
        {
            twice *= 2;
            Debug.Log("clear with winds");

        }
        else if (clear)
        {
            twice *= 8;
            Debug.Log("clear");
        }else if (onlyWindsAndDragons)
        {
            twice *= 8;
            Debug.Log("winds and dragons");
        }

        if (only19WindsDragons)
        {
            twice *= 2;
            Debug.Log("1 9 winds");
        }

        return twice;
    }

    public static int CountNotWinnerScore(ref List<Combination> closedComb,List<Combination> opened,
        List<Tile> closed,List<Tile> flowers,string wind,int order)
    {
        int twice = 1;

        int score = 0;

        for (int i = 0; i < opened.Count; i++)
        {
            score+=opened[i].CalculatePoints(wind);
            twice *= opened[i].doubling;
        }

        for (int i = 0; i < closed.Count; i++)
        {
            List<Tile> lst=closed.FindAll(x => x.name == closed[i].name);
            if (lst.Count == 2)
            {

                closedComb.Add(new Pair(lst[0], lst[1]) { opened = false });
                score += closedComb[closedComb.Count - 1].CalculatePoints(wind);
                
                i++;

            }else if(lst.Count==3 || lst.Count == 4)
            {
                closedComb.Add(new Pung(lst[0], lst[1],lst[2]) { opened = false });
                score += closedComb[closedComb.Count - 1].CalculatePoints(wind);

                twice *= closedComb[closedComb.Count - 1].doubling;

                i +=2;
                if (lst.Count == 4) i++;
            }
        }

        twice *= CountNotWinnerDoubles(opened, closed);
        Debug.Log(score);

        score *= twice;


        Debug.Log(twice + "twice");
        Debug.Log(score + "twice score");

        //flowers!!!
        score += GetFlowerPoints(flowers, order);

        return score;
    }



    public override int CalculatePoints(string wind)
    {
        return 0;


    }

}

