using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Xinop
{
    public partial class World
    {
        public Hero Hero;       
        
        public List<Place> Places;
        public List<Creature> Creatures;
        public List<Item> Items;
        public List<Thing> EveryThing;

        public World()
        {
            Hero = new Hero();
            Places = new List<Place>();
            Creatures = new List<Creature>();
            Items = new List<Item>();

            CreatePlaces();
            CreateItems();
            CreateCreatures();

            EveryThing = new List<Thing>();
            EveryThing.AddRange(Places);
            EveryThing.AddRange(Creatures);
            EveryThing.AddRange(Items);
        }

        public void ExecuteCommand(Command command)
        {
            var place = Places.Find(p => p.Id == Hero.PlaceId);

            if (place.ExecuteCommand != null && place.ExecuteCommand(command, place, this))
                return;

            foreach( var creature in Creatures.Where( c => c.PlaceId == Hero.PlaceId ))
            {
                if (creature.ExecuteCommand != null && creature.ExecuteCommand(command, creature, this))
                    return;
            }

            foreach (var item in Items.Where(i => i.Owner == Hero.HeroId || i.Owner == Hero.PlaceId))
            {
                if (item.ExecuteCommand != null && item.ExecuteCommand(command, item, this))
                    return;
            }

            switch (command.verb)
            {
                case "go": HandleGoCommand(command); break;
                case "look": HandleLookCommand(command); break;
                case "quit": Hero.State = HeroState.Quit; break;
                case "ax" when command.GetWord(0) == "me":  Hero.State = HeroState.Dead; break;
                default:
                    WriteLine("I'm confused.  Your command is nonsensical.");
                    break;
            }                       
        }        

        private void HandleGoCommand(Command command)
        {
            string direction = command.GetWord(0);
            // check if a creature is preventing your from going
            
            var place = Places.Find(p => p.Id == Hero.PlaceId);
            foreach (var placeDirection in place.Directions)
            {
                if (placeDirection.Name == direction || placeDirection.ShortName == direction)
                {
                    Hero.PlaceId = placeDirection.DestinationPlaceId;
                    return;
                }
            }            
        }

        private void HandleLookCommand(Command command)
        {
            string what = command.GetWord(0);
            if (what == "at")
                what = command.GetWord(1);

            bool foundThing = false;
            // find thing that we are looking at
            foreach (var item in Items.Where(i => i.Owner == Hero.HeroId || i.Owner == Hero.PlaceId))
            {
                if (item.Name == what)
                {
                    WriteLine($"You see a {item.GetDescription(true)}");
                    foundThing = true;
                }
            }

            foreach (var creature in Creatures.Where(c => c.PlaceId == Hero.PlaceId))
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

        private void UpdateCreatures()
        {
            foreach (var thing in EveryThing)
                thing.Behavior(thing, this);
        }

       
       

        public string GetPlaceDescription()
        {
            var scene = new StringBuilder();
            var place = Places.Find(p => p.Id == Hero.PlaceId);
            scene.AppendLine(place.GetDescription(false));

            // get current place description
            var otherThings = EveryThing.Where(t => !(t is Place) && t.LocationId == Hero.PlaceId);
            if (otherThings.Count() > 0)
            {
                scene.Append("You see ");
                foreach (var thing in otherThings)
                    scene.Append(thing.GetDescription(false));
            }
            scene.AppendLine();


            // get avaiable directions
            scene.Append("You can go ");
            foreach (var direction in place.Directions)
                scene.Append(direction.Description);
            scene.AppendLine();

            return scene.ToString();
        }
    }


    
}
