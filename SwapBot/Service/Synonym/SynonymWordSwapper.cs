using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwapBot.Model;
using SwapBot.Model.Linguistics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SwapBot.Service.Dictionary.Synonym
{
    public class SynonymWordSwapper : IWordSwapper
    {
        private const string ApiUrlTemplate = "http://words.bighugelabs.com/api/2/{0}/{1}/json";
        private const string ApiSynonymKey = "syn";

        private readonly IWordSwapper _innerSwapper;
        private readonly string secret;
        private readonly Dictionary<PartsOfSpeech, string> partToApi = new Dictionary<PartsOfSpeech, string>()
        {
            { PartsOfSpeech.Noun, "noun"},
            { PartsOfSpeech.Verb, "verb"},
            { PartsOfSpeech.Adjective, "adjective"},
            { PartsOfSpeech.Adverb, "adverb"}
        };

        private HttpClient httpClient;

        public SynonymWordSwapper(IWordSwapper swapper)
        {
            _innerSwapper = swapper;
            secret = ConfigurationManager.AppSettings["Bighugelabs"];
            httpClient = new HttpClient();
        }

        public async Task<IReadOnlyCollection<string>> GetWord(string oldWord, string partOfSpeech)
        {
            if (!PartOfSpeachParser.Parser.ContainsKey(partOfSpeech))
                return null;
            var part = PartOfSpeachParser.Parser[partOfSpeech];
            var synonyms = await GetSynonym(oldWord, part);
            if (synonyms == null)
            {
                return await _innerSwapper.GetWord(oldWord, partOfSpeech);
            }

            return synonyms;
        }

        private async Task<IReadOnlyCollection<string>> GetSynonym(string word, PartsOfSpeech partOfSpeech)
        {
            if (!partToApi.ContainsKey(partOfSpeech))
                return null;

            var json = await GetSynonymObject(word);
            if (json != null)
            {
                var selectPart = json.Property(partToApi[partOfSpeech]);
                if (selectPart != null && selectPart.HasValues)
                {
                    var synonymsArrayObject = selectPart.Value.Value<JObject>();
                    var synonymsArray = synonymsArrayObject.Property(ApiSynonymKey);
                    if (synonymsArray != null && synonymsArray.HasValues)
                    {
                        return synonymsArray.Value.ToObject<List<string>>();
                    }
                }
            }
            return null;
        }

        private async Task<JObject> GetSynonymObject(string word)
        {
            try
            {
                string requestUrl = string.Format(ApiUrlTemplate, secret, word);
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                HttpResponseMessage response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = null;
                    if (response.Content != null)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                    }

                    if (!string.IsNullOrWhiteSpace(responseContent))
                    {
                        return JObject.Parse(responseContent);
                    }

                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

    }
}