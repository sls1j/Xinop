using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace Xinop
{
    public class Hero
    {
        public string Name;
        public HeroState State;
        public List<DescriptionDef> DescriptionDefs;
        public string Location;
        public string LastLocationId;        

        public Hero()
        {
            HeroId = "**hero**";
            Name = "George";
            State = 0;
            DescriptionDefs = new List<DescriptionDef>();
            Location = "Start";
        }

        public string HeroId { get; internal set; }
        public int InjuredForMoves { get; internal set; }
        public void Update()
        {
            if (InjuredForMoves > 0)
            {
                InjuredForMoves--;
                if (InjuredForMoves == 0)
                    WriteLine("You have healed and are now whole.  It feels good to be whole again.");
            }
        }

        public void ExecuteCommand(Command command, World world)
        {
            switch (command.verb)
            {
                case "go": HandleGoCommand(world, command); break;
                case "look": HandleLookCommand(world, command); break;
                case "get":
                case "take":
                    HandleGetCommand(world, command); break;
                case "inv":
                case "i":
                    HandleInventory(world); break;
                case "jump":
                    HandleJump(world); break;
                case "quit": State = HeroState.Quit; break;
                case "ax" when command.GetWord(0) == "me": State = HeroState.Dead; break;
                default:
                    WriteLine("I'm confused.  Your command is nonsensical.");
                    break;
            }
        }

        private void HandleJump(World world)
        {
            string[] responses = new[]{
                "You can't quite reach the sky.",
                "Such joy can only be expressed through jumping.",
                "You jump.",
                "A blade swishes out of no where and passes under your feet.",
                "Too bad there is no one here to jump with you.",
                "You look silly jumping."
                };
            Random r = new Random();
            WriteLine(responses[r.Next(responses.Length)]); 
        }

        private void HandleInventory(World world)
        {
            Write("You have ");
            var inventory = world.EveryThing.Where(t => t.LocationId == HeroId).ToArray();

            if (inventory.Length == 0)
                WriteLine("You have nothing.");
            else
            {
                for (int i = 0; i < inventory.Length; i++)
                {
                    var item = inventory[i];
                    if (i == 0 && inventory.Length == 1)
                        WriteLine($"a {item.Name} .");
                    else if (i == 0)
                        Write(item.Name);
                    else if (i == inventory.Length - 1 && inventory.Length == 2)
                        WriteLine($" and a {item.Name}.");
                    else if (i == inventory.Length - 1)
                        WriteLine($", and a {item.Name}.");
                    else
                        Write($", a {item.Name}");
                }
            }
        }

        private void HandleGetCommand(World world, Command command)
        {
            bool foundThing = false;
            string what = command.GetWord(0);
            // find item and pick them up
            var items = world.Items.FindAll(i => i.Name == what && i.LocationId == Location);
            foreach (var item in items)
            {
                if (item.IsPortable)
                    item.Owner = HeroId;
                else
                    WriteLine($"{item.Name} is not portable.  You can't take it.");
                foundThing = true;
            }

            var creatures = world.Creatures.FindAll(c => c.Name == what && c.LocationId == Location);
            foreach (var creature in creatures)
            {
                foundThing = true;
                WriteLine($"Creatures such as {what} belong to themselves you cannot take it.");
            }

            if (!foundThing)
                WriteLine($"I don't see {what} here cannot take it.");
        }

        private void HandleGoCommand(World world, Command command)
        {
            string direction = command.GetWord(0);
            // check if a creature is preventing your from going

            var place = world.Places.Find(p => p.Name == Location);
            foreach (var placeDirection in place.Directions)
            {
                if (placeDirection.Name == direction || placeDirection.ShortName == direction)
                {
                    Location = placeDirection.DestinationId;
                    return;
                }
            }
        }

        private void HandleLookCommand(World world, Command command)
        {
            string what = command.GetWord(0);
            if (what == "at")
                what = command.GetWord(1);

            if (string.IsNullOrEmpty(what))
            {
                var place = world.Places.Find(p => p.Name == Location);
                if (null == place)
                    WriteLine("There is nothing to see.  You are in the void.");
                else
                    WriteLine(place.GetDescription(true));
                return;
            }

            bool foundThing = false;
            // find thing that we are looking at
            foreach (var item in world.Items.Where(i => i.Owner == HeroId || i.Owner == Location))
            {
                if (item.Name == what)
                {
                    WriteLine($"You see a {item.GetDescription(true)}");
                    foundThing = true;
                }
            }

            foreach (var creature in world.Creatures.Where(c => c.PlaceId == Location))
            {
                if (creature.Name == what)
                {
                    WriteLine($"You see a {creature.GetDescription(true)}");
                    foundThing = true;
                }
            }

            if (!foundThing)
                WriteLine($"I don't see a {what}");

            WriteLine();
        }
    }

    public enum HeroState : int { Normal = 0, Hungry = 1, Injured = 2, Sleepy = 3, ExtraStrong = 4, Dead = 5, Quit = 6 };
}
