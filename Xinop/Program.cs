using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace Xinop
{
    class Program
    {
        static void Main(string[] args)
        {
            World world = new World();

            WriteLine($"Welcome to Xinop -- Text adventure fun and joy.");

            while (true)
            {
                string description = world.GetPlaceDescription();
                WriteLine(description);

                Write("> ");
                string line = ReadLine();
                Command command;
                if (Command.TryParse(line, out command))
                {
                    world.ExecuteCommand(command);
                }
                else
                {
                    WriteLine();
                    WriteLine("I cannot understand what you want!");
                }

                if (world.Hero.State == HeroState.Dead)
                {
                    WriteLine("You have died!");
                    break;
                }
                else if (world.Hero.State == HeroState.Quit)
                {
                    WriteLine("You have quit.");
                    break;
                }
            }

            WriteLine("Press Key to end");
            ReadKey();
        }
    }
}
