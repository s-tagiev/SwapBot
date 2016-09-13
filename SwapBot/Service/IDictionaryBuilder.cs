using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SwapBot.Service
{
    public interface IDictionaryBuilder
    {
        Task SaveWord(string word, string partOfSpeech);
    }
}