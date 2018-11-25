using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM
{
    class Program
    {
        static void Main(string[] args)
        {
            FiniteState machine = new FiniteState("../../task1.txt", false);

            Console.WriteLine(machine.ToString());

            var result = machine.MaxString("ba", 0);

            Console.WriteLine(result);

            //FiniteState fsm = new FiniteState("../../task2.txt", true);

            //fsm.CountNumbers("+.759e+15", 0);

            //fsm.CountNumbers("+7e.+9", 0);
            //fsm.CountNumbers("+5+2365427.63547e+72637.72536e-.5e5e3e5a34.4ghhhgf.gfhgf", 0);
        }
    }
}