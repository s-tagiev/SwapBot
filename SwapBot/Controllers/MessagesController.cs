using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.ProjectOxford.Linguistics;
using Microsoft.ProjectOxford.Linguistics.Contract;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SwapBot.Service.Dictionary;
using SwapBot.Service.Dictionary.Synonym;
using SwapBot.Service.Dictionary.Hardcode;
using SwapBot.Service;

namespace SwapBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        static DictionarySwapper innerSwapper = new DictionarySwapper();
        static IDictionaryBuilder dictionary = innerSwapper;
        static IWordSwapper swapper = new SynonymWordSwapper(innerSwapper);
        Random random = new Random();
        LinguisticsClient client = new LinguisticsClient(ConfigurationManager.AppSettings["MicrosoftLinguistics"]);

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var rep = await Reply(activity.Text);
                Activity reply = activity.CreateReply(rep);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        async Task<string> Reply(string msg)
        {
            var a = msg.ToLower().Split(' '); // (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
            if (a.IsPresent("привет"))
            {
                return "Привет!";
            }
            if (a.IsPresent("спасибо"))
            {
                return "Пожалуйста!";
            }
            if (a.IsPresent("help") || a.IsPresent("помощь"))
            {
                return "Если хочешь, чтобы я изменил английскую фразу, то просто введи её. " +
"Если хочешь пополнить мой словарный запас, используй слово \"научить\" вначале текста. Я его запомню и буду знать новые слова.";
            }
            if (a.IsPresent("кто") && a.IsPresent("ты"))
            {
                return "Я простой бот, который постоянно путает английские слова.";
            }
            if (a.IsPresent("что") && a.IsPresent("делать"))
            {
                return "Можешь играть с друзьями, узнают ли они исходное предложение.";
            }
            if (a.IsPresent("научить"))
            {
                var tree = await GetTree(msg.Replace("научить", ""));
                var leafs = GetLeaf(tree);
                foreach (var leaf in leafs)
                {
                    await dictionary.SaveWord(leaf.Item2, leaf.Item1);
                }
                return "Спасибо!";
            }
            else
            {
                var tree = await GetTree(msg);
                var leafs = GetLeaf(tree);
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (var leaf in leafs)
                {
                    while (i < msg.Length)
                    {
                        if (msg[i] != leaf.Item2[0])
                        {
                            sb.Append(msg[i++]);
                            continue;
                        }
                        else
                        {
                            var words = await swapper.GetWord(leaf.Item2, leaf.Item1);
                            if (words != null && words.Count > 0)
                            {
                                var word = words.ElementAt(random.Next(0, words.Count));
                                sb.Append(word);
                                i += leaf.Item2.Length;

                            }
                            else
                            {
                                var word = msg.Substring(i, leaf.Item2.Length);
                                sb.Append(word);
                                i += leaf.Item2.Length;
                            }
                            break;
                        }
                    }
                }
                while (i < msg.Length)
                {
                    sb.Append(msg[i++]);
                }
                return sb.ToString();
            }

            return null;
        }

        private async Task<string> GetTree(string text)
        {
            var Analyzers = await client.ListAnalyzersAsync();
            var Req = new AnalyzeTextRequest();
            Req.Language = "en";
            Req.Text = text;
            // Req.AnalyzerIds = (from x in Analyzers select x.Id).ToArray();
            Req.AnalyzerIds = new Guid[] { Analyzers[1].Id };

            var Res = await client.AnalyzeTextAsync(Req);
            return Res[0].Result.ToString();
        }

        private List<Tuple<string, string>> GetLeaf(string tree)
        {
            var result = new List<Tuple<string, string>>();
            Regex ItemRegex = new Regex(@"\((\w+?) ([^\(]*?)\)", RegexOptions.Compiled);
            foreach (Match ItemMatch in ItemRegex.Matches(tree))
            {
                result.Add(new Tuple<string, string>(ItemMatch.Groups[1].ToString(), ItemMatch.Groups[2].ToString()));
            }
            return result;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}