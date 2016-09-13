using SwapBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SwapBot.Service.Dictionary
{
    public interface IWordSwapper
    {
        Task<IReadOnlyCollection<string>> GetWord(string oldWord, string partOfSpeech);
    }
}