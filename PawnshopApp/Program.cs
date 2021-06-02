using System;
using System.Collections.Generic;
using BusinessLogic;

namespace PawnshopApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Pawnshop OurPawnshop = new Pawnshop(10000);
            ControlPanel.AddExampleData(OurPawnshop);
            ControlPanel.Actions(OurPawnshop);
        }
    }
}

