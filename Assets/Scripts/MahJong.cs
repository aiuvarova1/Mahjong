using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class MahJong : Combination
{
    public List<List<Combination>> closedCombinations = new List<List<Combination>>();
    public List<List<Combination>> openedCombinations = new List<List<Combination>>();

    public MahJong(List<List<Combination>> comb)
    {
        closedCombinations = comb;
    }

    public void CalculateMahJongPoints()
    {

    }

}

