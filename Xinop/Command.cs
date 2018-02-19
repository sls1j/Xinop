using System;
using System.Collections.Generic;
using System.Linq;

namespace Xinop
{
    public class Command
    {
        private string[] _workTokens;
        public Command(string Verb, string[] Words)
        {
            verb = Verb;
            _workTokens = Words;
            this.Words = string.Join(" ", Words);
        }

        public string verb { get; private set; }
        public string GetWord(int index)
        {
            if (index < 0 || index >= _workTokens.Length)
                return string.Empty;

            return _workTokens[index];
        }

        public string Words { get; private set; }

        public static bool TryParse(string line, out Command command)
        {
            line = line.ToLower();
            try
            {
                string[] allwords = line.Split(' ');
                string[] words = allwords.Skip(1).ToArray();
                command = new Command(allwords[0], words);
                return true;
            }
            catch (Exception)
            {
                command = null;
                return false;
            }
        }
    }    
}