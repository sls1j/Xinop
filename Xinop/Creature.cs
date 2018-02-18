using System;
using System.Collections.Generic;

namespace Xinop
{
    public class Creature : Thing
    {
        public string PlaceId;

        public override string LocationId => PlaceId;

        public Creature()
        {
        }    
    }

    
}