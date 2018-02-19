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
            place.AddDescription(state: 0, description: "You are at a cross roads. The sky is blue.");
            place.AddDescription(state: 1, description: "You are at a cross roads. The sky is dark and overcast.");
            place.AddDirection(Dir.North, "north on a narrow dirt track that doesn't look well used.", "north_road");
            place.AddDirection(Dir.East, "east on a broad paved road.", "broad_road");
            place.AddDirection(Dir.South, "south a simple road that leads toward a vast forest.", "forest_1");
            place.AddDirection(Dir.Down, "down small black hole in the ground.", "small_black_hole");
            place.ExecuteCommand = (command, thing, world) =>
                {
                    switch (command.verb)
                    {
                        case "jump":
                            if (HeroState.Injured == world.Hero.State)
                                WriteLine("You cannot jump!  You are hurt.");
                            else
                            {
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
                            }
                            return true;
                    }

                    return false;
                };
            place.Behavior = (thing, world) =>
            {
                Random r = new Random();
                if (1 == r.Next(5))
                    thing.State = 1 - thing.State; // toggle the state to change the weather
            };

            this.Places.Add(place);

            // set the default location
            this.Hero.LocationId = place.Id;

            place = new Place();
            place.Id = "north_road";
            place.Name = "North Road";
            place.AddDescription(0, "The road is well traveled.  It is wide with neat gutters along the side.  However, it is rather lonely.");
            place.AddDirection(Dir.South, "south there seems to be some sort of a junction.", "cross-roads");
            this.Places.Add(place);

            place = new Place();
            place.Id = "small_black_hole";
            place.Name = "Small Black Hole";
            place.AddDescription(0, "This hole opens up, and isn't so dark, or so small.  It looks as if it was dug by machine.");
            place.AddDirection(Dir.Up, "up a stairway there is a little light.", "cross-roads");
            this.Places.Add(place);
        }

        private void CreateItems()
        {
            // make flashlight
            var item = new Item();
            item.Id = "flashlight";
            item.Name = "flashlight";
            item.IsPortable = true;
            item.AddDescription(0, "a flashlight turned off.", "a flashlight with the words Acme on the switch, it has a good weight to it. It is off");
            item.AddDescription(1, "a flashlight turned on.", "a flashlight with words Acme on the switch.  It shines with a light of a thousands suns.");
            item.AddDescription(2, "a flashlight turned on, but not shining", "a flashlight with words Acme on the switch. If it had batteries perhaps it would shine with the light of a thousand suns.");
            item.ExecuteCommand = (command, thing, world) =>
                    {
                        if (command.verb == "turn" && command.GetWord(0) == "on" && command.GetWord(1) == thing.Name)
                        {
                            int batteryStrength = thing.Get<int>("battery-strength");
                            if (batteryStrength > 0)
                            {
                                WriteLine("The flashlight turns on with this the light of a thousand suns.");
                                thing.State = 1;
                            }
                            else
                            {
                                WriteLine("Nothing happens.  With a sense of disappointment the flashlight remains dark.");
                                thing.State = 2;
                            }

                            return true;
                        }
                        else if (command.verb == "turn" && command.GetWord(0) == "off" && command.GetWord(1) == thing.Name)
                        {
                            if (thing.State == 1)
                                WriteLine("The brilliant light of the flashlight cuts off.  You feel a little blind while your eyes adjust to the lack of light.");
                            else
                                WriteLine("Click.");

                            thing.State = 0;
                            return true;
                        }

                        return false;
                    };
            item.Set("battery-strength", 10);

            item.Behavior = (thing, world) =>
            {
                if (thing.State == 1)
                {
                    var newBatteryStrength = thing.Get<int>("battery-strength") - 1;
                    if (newBatteryStrength <= 0)
                    {
                        thing.State = 2;
                        WriteLine("The flashlight dims and then goes out.");
                        thing.Set("battery-strength", 0);
                    }
                    else
                        thing.Set("battery-strength", newBatteryStrength);
                }
                else if (thing.State == 2)
                {
                    var newBatteryStrength = thing.Get<int>("battery-strength");
                    if (newBatteryStrength > 0)
                    {
                        WriteLine("Instantly a brillian bright light shines forth with recharged effort.");
                        thing.State = 1;
                    }
                }
            };
            item.Owner = "cross-roads"; // put the flashlight in the starting place

            Items.Add(item);

            item = new Item();
            item.Name = "charger";
            item.Id = "charger";
            item.IsPortable = false;
            item.AddDescription(0, "a nuclear charger.", "a nuclear charger. It has a red button and a green button on it's panel.  There is a socket on it.");
            item.AddDescription(1, "a nuclear charger, humming slightly", "a nuclear charger.  It is on.  It has a red button and a green button on it's panel.  There is a socket on it.  It is humming.");
            item.ExecuteCommand = (command, thing, world) =>
            {
                if (command.verb == "push" && command.GetWord(0) == "red" && command.GetWord(1) == "button")
                {
                    if (thing.State == 0)
                    {
                        WriteLine("Nothing happens.");
                    }
                    else if (thing.State == 1)
                    {
                        WriteLine("There is a great whiring and screeching as the machine shutdowns. It no longer hums.");
                        thing.State = 0;
                    }
                    return true;
                }
                else if (command.verb == "push" && command.GetWord(0) == "green" && command.GetWord(1) == "button")
                {
                    if (thing.State == 0)
                    {
                        WriteLine("A crunching and creaking sound errupt from the machine.  A loud humming starts.  The machine is now running.");
                        thing.State = 1;
                    }
                    else if (thing.State == 1)
                    {
                        WriteLine("Nothing happens.");
                    }
                    return true;
                }
                else if (command.verb == "plug" && command.GetWord(0) == "in" && command.GetWord(1) == "flashlight")
                {
                    var flashlight = world.Items.Find(i => i.Id == "flashlight");
                    if (flashlight.Owner == world.Hero.HeroId)
                    {
                        if (thing.State == 0)
                            WriteLine("Nothing happens.  You put the flashlight back in your pocket.");
                        else
                        {
                            WriteLine("The flashlight becomes warm.  It is now charged. You hold the flashlight.");
                            flashlight.Set("battery-strength", 100);
                            flashlight.Behavior(flashlight, world);
                        }
                    }
                    else
                        WriteLine("You must have a flashlight to plug it in.");

                    return true;
                }

                return false;
            };

            item.Owner = "small_black_hole"; // put the nuclear charger in the ground
            this.Items.Add(item);
        }

        private void CreateCreatures()
        {

        }

    }
}
