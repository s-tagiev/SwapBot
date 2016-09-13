using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SwapBot.Model.Linguistics
{
    public class PartOfSpeachParser
    {
        public static Dictionary<string, PartsOfSpeech> Parser { get; } = new Dictionary<string, PartsOfSpeech>
        {
            {"CC", PartsOfSpeech.Conjuction },
            {"DT", PartsOfSpeech.Determiner },
            {"IN", PartsOfSpeech.Preposition },
            {"JJ", PartsOfSpeech.Adjective },
            {"JJR", PartsOfSpeech.Adjective },
            {"JJS", PartsOfSpeech.Adjective },
            {"NN", PartsOfSpeech.Noun },
            {"NNP", PartsOfSpeech.Noun },
            {"NNPS", PartsOfSpeech.Noun },
            {"NNS", PartsOfSpeech.Noun },
            {"PDT", PartsOfSpeech.Determiner },
            {"PRP", PartsOfSpeech.Pronoun },
            {"PRP$", PartsOfSpeech.Pronoun },
            {"RB", PartsOfSpeech.Adverb },
            {"RBR", PartsOfSpeech.Adverb },
            {"RBS", PartsOfSpeech.Adverb },
            {"RP", PartsOfSpeech.Preposition },
            {"UH", PartsOfSpeech.Interjection },
            {"VB", PartsOfSpeech.Verb },
            {"VBD", PartsOfSpeech.Verb },
            {"VBG", PartsOfSpeech.Verb },
            {"VBN", PartsOfSpeech.Verb },
            {"VBP", PartsOfSpeech.Verb },
            {"VBZ", PartsOfSpeech.Verb },
            {"WDT", PartsOfSpeech.Determiner },
            {"WP", PartsOfSpeech.Pronoun },
            {"WP$", PartsOfSpeech.Pronoun },
            {"WRB", PartsOfSpeech.Adverb }
        };
    }
}