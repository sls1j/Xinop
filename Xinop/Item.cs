using System;
using System.Collections.Generic;

namespace Xinop
{
    public class Item : Thing
    {
        public string Owner;
        public bool IsPortable;

        public override string LocationId => Owner;

        public Item()
        {            
        }
    }
}