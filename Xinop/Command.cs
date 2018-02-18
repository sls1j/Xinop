using System;
using System.Collections.Generic;
using System.Linq;

namespace Xinop
{
    public class Command
    {
        private string[] words;
        public Command(string Verb, string[] Words)
        {
            verb = Verb;
            words = Words;
        }

        public string verb { get; private set; }
        public string GetWord(int index)
        {
            if (index < 0 || index >= words.Length)
                return string.Empty;

            return words[index];
        }

        public static bool TryParse(string line, out Command command)
        {
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