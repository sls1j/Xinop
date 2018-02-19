using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinop
{
    public class Place : Thing
    {
        public List<Direction> Directions;
        public override string LocationId => Id;

        public void AddDirection(string name, string shortName, string description, string destinationId )
        {
            var dir = new Direction();
            dir.Name = name;
            dir.ShortName = shortName;
            dir.Description = description;
            dir.DestinationId = destinationId;
            Directions.Add(dir);
        }

        public void AddDirection(Dir dir, string description, string destinationId)
        {
            string name = dir.ToString().ToLower();
            string shortName = name.Substring(0, 1);
            AddDirection(name, shortName, description, destinationId);
        }

        public Place()
        {
            Directions = new List<Direction>();
        }
    }

    public enum Dir { North, East, South, West, Up, Down };
}
