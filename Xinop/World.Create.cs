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

            place = new Place();
            place.Id = "forest_1";
            place.Name = "Forest";
            place.AddDescription(0, "You are in a forest with tall pine trees all around.");
            place.AddDirection(Dir.North, "north on a small trail that leads out of the forest", "cross-roads");
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

            item = new Item();
            item.Id = "book_of_mormon";
            item.Name = "book";
            item.IsPortable = true;
            item.AddDescription(0, "A tattered book", "The title is hard to read but you make out Book of Mormon on the cover");
            item.ExecuteCommand = (command, thing, world) =>
            {
                if ((command.verb == "get" || command.verb == "take") && command.GetWord(0) == "book")
                {
                    WriteLine("You feel a strange expectancy as you pick up this book.  As if you are destined to read it.");
                }
                else if (command.verb == "read" && command.GetWord(0) == "book")
                {
                    if (world.Hero.HeroId == thing.LocationId)
                    {
                        WriteLine("The book reads,\"if ye will enter in by the way, and receive the Holy Ghost, it will show unto you all things what ye should do.\"  You feel enlightened and strengthened");
                        world.Hero.State = HeroState.ExtraStrong;
                    }
                    else
                        WriteLine("You must have the book to read it.");

                    return true;
                }                

                return false;
            };
            item.Owner = "forest_1";
            this.Items.Add(item);

            item = new Item();
            item.Id = "cooking_meritbadge";
            item.Name = "cooking merit badge";
            item.AddDescription(0, "This is a cooking merit badge earned by some serious cooking.");
            this.Items.Add(item);
        }

        private void CreateCreatures()
        {
            Creature c = new Creature();
            c.Id = "monster";
            c.Name = "monster";
            c.State = 0;
            c.AddDescription(0, "a monster with large and pointy teeth, big muscles, and a foul smell.  It looks angry.");
            c.AddDescription(1, "a monster that looks hungry and shows signs of wanting to eat you.  It has big muscles and pointy teeth.  It is looking at you.");
            c.AddDescription(2, "a monster that is biting and chewing on your arm.");
            c.AddDescription(3, "a monster that is no longer hungry as it uses your finger bone to pick its teeth.");
            c.AddDescription(4, "a dead monster, with a bashed in head.");
            c.AddDescription(5, "a beautiful princess.");
            c.Behavior = (thing, world) =>
            {
                // we are in the same room as the monster
                if (world.Hero.LocationId == thing.LocationId)
                {
                    switch (thing.State)
                    {
                        case 0: thing.State = 1; thing.ExtraState = 0; break;
                        case 1 when thing.ExtraState >= 2: thing.State = 2; Hero.InjuredForMoves = 20; Hero.State = HeroState.Injured; break;
                        case 1: thing.ExtraState++; break;
                        case 2: thing.State = 3; Hero.State = HeroState.Dead; break;
                        case 4: break;
                        case 5: break;
                    }
                } 
                else // not with the monster
                {
                    if ( thing.State != 4 )
                        thing.State = 0;
                }
            };

            c.ExecuteCommand = (command, thing, world) =>
            {
                switch (command.verb)
                {
                    case "kiss" when command.Words == thing.Name:
                        if (thing.State != 5)
                        {
                            WriteLine("The monster turns into a beautiful princess. She smiles at you invitingly.");
                            thing.Name = "princess";
                            thing.State = 5;
                        }
                        else
                        {
                            WriteLine("The princess is really a terrible monster.  As you move in close to kiss her she transforms into a monster with pointy teeth.");
                            thing.Name = "monster";
                            thing.State = 1;
                            thing.ExtraState = 3;
                        }
                        return true;
                    case "punch" when command.Words == thing.Name:
                    case "hit" when command.Words == thing.Name:
                    case "kick" when command.Words == thing.Name:
                        if (thing.State == 4)
                            WriteLine($"You {command.verb} a dead monster. It is good practice, but otherwise ineffective");
                        else if (world.Hero.State == HeroState.ExtraStrong)
                        {
                            WriteLine($"You {command.verb} the monster in the face.  He yelps.  You can tell it's pondering whether to attack you or not.");
                            c.State = 0;
                        }
                        else
                        {
                            WriteLine("The monster is not pleased.  He bares his teeth in a large menacing manner.");
                            c.State = 1;
                            c.ExtraState = 1;
                        }
                        return true;
                    case "kick":
                    case "punch":
                    case "hit":
                    case "kiss":
                        WriteLine($"You want to {command.verb} what?  I don't think so.");
                        return true;
                    case "throw" when command.Words == $"flashlight at {thing.Name}":
                        {
                            var flashlight = world.Items.Find(i => i.Name == "flashlight");
                            if (flashlight.Owner == world.Hero.HeroId)
                            {

                                if (thing.State == 4)
                                    WriteLine("Your flashlight hits the dead beast and does nothing.");
                                else
                                {
                                    if (flashlight.State == 1)
                                    {
                                        if (world.Hero.State == HeroState.ExtraStrong)
                                        {
                                            WriteLine("You throw your flashlight with exceeding strength.  It enters the nose of the monster while blazing with the light of one thousand suns.  The brain of the monster is cooked via the light.  You have earned the Cooking Merit Badge and killed the monster.");
                                            thing.State = 4;
                                            var meritBadge = world.Items.Find(i => i.Name == "cooking merit badge");
                                            meritBadge.Owner = world.Hero.HeroId;
                                        }
                                        else
                                        {
                                            WriteLine("Your flashlight dazzals the monster briefly. It then eats the flashlight.");
                                            flashlight.Owner = thing.Id;
                                        }
                                    }
                                    else
                                    {
                                        WriteLine("You miss the monster.  It doesn't seem to notice your attack. You lost your flashlight");
                                        flashlight.Owner = world.Hero.LocationId;
                                    }
                                }
                                return true;
                            }
                            else
                            {
                                WriteLine("You must have the flashlight to throw it.");
                            }
                            return true;
                        }
                }
                return false;
            };


            c.PlaceId = "north_road";
            this.Creatures.Add(c);

        }

    }
}
