using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinop
{
    public class Hero
    {
        public string Name;
        public HeroState State;
        public List<DescriptionDef> DescriptionDefs;
        public string PlaceId;

        public Hero()
        {
            Name = "George";
            State = 0;
            DescriptionDefs = new List<DescriptionDef>();
            PlaceId = "Start";
        }

        public string HeroId { get; internal set; }
        public int InjuredForMoves { get; internal set; }

        public void HandleCommand(Command command, World world)
        {
        }
    }

    public enum HeroState : int { Normal = 0, Hungry = 1, Injured = 2, Sleepy = 3, ExtraStrong = 4, Dead = 5, Quit = 6 };
}
