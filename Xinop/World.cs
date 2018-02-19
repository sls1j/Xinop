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

        public void Update()
        {
            foreach (var thing in EveryThing)
            {
                if (thing.Behavior != null)
                    thing.Behavior(thing, this);
            }

        }


        public void ExecuteCommand(Command command)
        {
            var place = Places.Find(p => p.Id == Hero.LocationId);

            if (null == place)
                return;

            if (place.ExecuteCommand != null && place.ExecuteCommand(command, place, this))
                return;

            foreach( var creature in Creatures.Where( c => c.PlaceId == Hero.LocationId ))
            {
                if (creature.ExecuteCommand != null && creature.ExecuteCommand(command, creature, this))
                    return;
            }

            foreach (var item in Items.Where(i => i.Owner == Hero.HeroId || i.Owner == Hero.LocationId))
            {
                if (item.ExecuteCommand != null && item.ExecuteCommand(command, item, this))
                    return;
            }

            Hero.ExecuteCommand(command, this);
            
        }          
             

        public string GetPlaceDescription()
        {
            var place = Places.Find(p => p.Id == Hero.LocationId);
            if (null == place)
            {
                Hero.State = HeroState.Dead;
                return "You find yourself in the void, where no time and no place exist.  This is where the creator didn't shine his light and has yet to create.";
            }
            else
            {
                var scene = new StringBuilder();
                scene.AppendLine(place.GetDescription(false));

                // get current place description
                var otherThings = EveryThing.Where(t => !(t is Place) && t.LocationId == Hero.LocationId);
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


    
}
