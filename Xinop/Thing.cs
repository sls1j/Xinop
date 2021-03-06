﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinop
{
    public class Thing
    {
        private Dictionary<string, object> _properties;

        public string Name;
        public int State;
        public int ExtraState;
        public List<DescriptionDef> Descriptions;
        public ExecuteCommand ExecuteCommand;
        public BehaviorMethod Behavior;

        public string GetDescription(bool useLong)
        {
            var de = Descriptions.First(d => d.State == State);
            if (useLong)
                return de.LongDescription ?? de.Description;
            else
                return de.Description;
        }

        public virtual string LocationId { get { return null; } }

        public T Get<T>(string name)
        {
            object v;
            if (_properties.TryGetValue(name, out v))
                return (T)v;
            else
                return default(T);
        }

        public void Set<T>(string name, T value)
        {
            _properties[name] = value;
        }

        public void AddDescription(int state, string description, string longDescription = null)
        {
            var desc = new DescriptionDef()
            {
                State = state, Description = description, LongDescription = longDescription
            };

            this.Descriptions.Add(desc);
        }

        public Thing()
        {
            State = 0;
            _properties = new Dictionary<string, object>();
            Descriptions = new List<DescriptionDef>();
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} {this.Name}";
        }
    }

    public delegate bool ExecuteCommand(Command command, Thing thing, World world);
    public delegate void BehaviorMethod(Thing thing, World world);
}
