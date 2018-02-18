using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinop
{
    public class Place : Thing
    {
        public Direction[] Directions;
        public override string LocationId => Id;

        public Place()
        {            
        }
    }
}
