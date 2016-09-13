using SwapBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SwapBot.Service.Dictionary.Hardcode
{
    public class DictionarySwapper : IWordSwapper, IDictionaryBuilder
    {
        private static object _syncRoot = new object();
        private readonly Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
        public Task<IReadOnlyCollection<string>> GetWord(string oldWord, string partOfSpeech)
        {
            if (dictionary.ContainsKey(partOfSpeech))
            {
                return Task.FromResult((IReadOnlyCollection<string> )dictionary[partOfSpeech]);
            }
            return Task.FromResult<IReadOnlyCollection<string>>(null);
        }

        public Task SaveWord(string word, string partOfSpeech)
        {
            word = word.ToLower();
            lock (_syncRoot)
            {
                if (dictionary.ContainsKey(partOfSpeech))
                {
                    if (!dictionary[partOfSpeech].Contains(word))
                    {
                        dictionary[partOfSpeech].Add(word);
                    }
                }
                else
                {
                    dictionary.Add(partOfSpeech, new List<string>() { word });
                }
            }
            return Task.FromResult(0);
        }
    }
}