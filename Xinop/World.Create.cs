using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Xinop
{
    partial class World
    {
        public void CreateWorld()
        {
            CreatePlaces();
            CreateItems();
            CreateCreatures();
        }

        private void CreatePlaces()
        {
            var place = new Place();
            place.Name = "cross roads";
            place.Id = "cross-roads";
            place.Descriptions = new[]
                {
                    new DescriptionDef(){ State = 0, Description = "You are at a cross roads. The sky is blue." },
                    new DescriptionDef(){ State = 1, Description = "You are at a cross roads. The sky is dark and overcast." }
                };
            place.Directions = new[]{
                    new Direction(){
                        Name = "north",
                        ShortName = "n",
                        Description = "north on a narrow dirt track that doesn't look well used.",
                        DestinationPlaceId = "north_road",
                        },
                    new Direction(){
                        Name = "east",
                        ShortName = "e",
                        Description = "east on a broad paved road.",
                        DestinationPlaceId = "broad_road",
                    },
                    new Direction(){
                        Name = "south",
                        ShortName = "s",
                        Description = "south a simple road that leads toward a vast forest.",
                        DestinationPlaceId = "forest_1",
                    },
                    new Direction(){
                        Name = "down",
                        Description = "down small black hole in the ground.",
                        DestinationPlaceId = "small_black_hole"
                    }
                };
            place.ExecuteCommand = (command, thing, world) =>
                {
                    switch (command.verb)
                    {
                        case "jump":
                            var r = new Random();
                            if (r.Next(10) == 1)
                            {
                                WriteLine("Ouch you sprained your ankle on the hole.  You should not jump here!");
                                world.Hero.State = HeroState.Injured;
                                world.Hero.InjuredForMoves = 3;
                            }
                            else
                            {
                                WriteLine("You nearly sprained your ankle!  You should becareful jumping around a hole in the ground.");
                            }
                            return true;
                    }

                    return false;
                };

            this.Places.Add(place);

            // set the default location
            this.Hero.LocationId = place.Id;


        }

        private void CreateItems()
        {
            // make flashlight
            var item = new Item();
            item.Id = "flashlight_id";
            item.Name = "flashlight";
            item.IsPortable = true;
            item.Descriptions = new DescriptionDef[]{
                    new DescriptionDef(){
                        State = 0, Description = "a flashlight turned off.", LongDescription = "a flashlight with the words Acme on the switch, it has a good weight to it. It is off"
                    },
                    new DescriptionDef(){
                        State = 1, Description = "a flashlight turned on.", LongDescription = "a flashlight with words Acme on the switch.  It shines with a light of a thousands sun."
                    },
                    new DescriptionDef(){
                        State = 2, Description = "a flashlight turned on, but not shining", LongDescription = "a flashlight with words Acme on the switch. If it had batteries perhaps it would shine with the light of a thousand suns."
                    }
                };
            item.ExecuteCommand = (command, thing, world) =>
                    {
                        if (command.verb == "turn" && command.GetWord(0) == "on" && command.GetWord(1) == thing.Name)
                        {
                            int batteryStrength = thing.Get<int>("battery-strength");
                            if (batteryStrength > 0)
                                thing.State = 1;
                            else
                                thing.State = 2;
                            
                            return true;
                        }
                        else if (command.verb == "turn" && command.GetWord(0) == "off" && command.GetWord(1) == thing.Name)
                        {
                            thing.State = 0;
                            return true;
                        }

                        return false;
                    };
            item.Set("battery-strength", 100);

            item.Behavior = (thing, world) =>
            {
                if (thing.State == 1)
                {
                    var newBatteryStrength = item.Get<int>("battery-strength") - 1;
                    if (newBatteryStrength <= 0)
                    {
                        thing.State = 2;
                        WriteLine("The flashlight dims and then goes out.");
                        thing.Set("battery-strength", newBatteryStrength);
                    }
                    else
                        thing.Set("battery-strength", newBatteryStrength);
                }                    
            };

            item.Owner = "cross-roads";

            Items.Add(item);
        }

        private void CreateCreatures()
        {

        }

    }
}
