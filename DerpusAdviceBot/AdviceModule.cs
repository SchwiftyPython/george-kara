using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DerpusAdviceBot
{
    public class AdviceModule : ModuleBase<SocketCommandContext>
    {
        private enum Category
        {
            Gardening,
            Love,
            All
        }

        private readonly Dictionary<Category, string> _categoryStartSymbols = new Dictionary<Category, string>
        {
            {Category.Gardening, "#gardening#"},
            {Category.Love, "#love#"},
            {Category.All, "#origin#"}
        };

        private readonly Dictionary<Category, List<string>> _categoryTriggers = new Dictionary<Category, List<string>>
        {
            {Category.Gardening, GardeningTriggers},
            {Category.Love, LoveTriggers},
        };

        private Dictionary<string, Category> _triggerLookup;

        private const string AllCategoriesTraceryPath = "all_advice_categories.json";

        private static readonly List<string> GardeningTriggers = new List<string> 
        {
            "spade",
            "trowel",
            "pruners",
            "shears",
            "hose",
            "wheelbarrow",
            "plant",
            "plants",
            "dirt",
            "flower",
            "flowers",
            "greenhouse",
            "annuals",
            "perennials",
            "hardy",
            "evergreen",
            "soil",
            "earth",
            "root",
            "seeds",
            "seedling",
            "weed",
            "weeds",
            "mulch",
            "lawn",
            "grass",
            "mow",
            "fruit",
            "vegetables",
            "veggies",
            "yard"
        };

        private static readonly List<string> LoveTriggers = new List<string>
        {
            "break up",
            "divorce",
            "leave him", 
            "leave her",
            "love language",
            "red flag",
            "love",
            "seeing",
            "relationship",
            "girlfriend",
            "boyfriend",
            "wife",
            "husband",
            "significant other",
            "spouse",
            "so",
            "gf",
            "bf",
            "guy",
            "chick",
            "girl",
            "boy",
            "sex",
            "talking",
            "crush",
            "feelings",
            "butterflies",
            "marry",
            "marriage",
            "married",
            "cheating",
            "date",
            "commit"
        };

        private string _traceryText;

        private readonly Random _rand = new Random();

        public AdviceModule()
        {
            Console.WriteLine($"{Prefix.Info} Loading Advice Module...");

            LoadTriggerLookup();
            LoadTraceryText();

            Console.WriteLine($"{Prefix.Info} Advice Module Loaded");
        }

        // !advice How do I grow tomatoes?
        [Command("advice")]
        [Summary("Derpus gives you his best advice.")]
        public async Task GiveAdvice([Remainder] [Summary("A desperate plea for help.")] string dQuestion)
        {
            var cQuestion = CleanInput(dQuestion).ToLower();

            var category = DetermineCategory(cQuestion);

            await Context.Channel.SendMessageAsync(GenerateTraceryResponse(category));

            //todo long question get a prefix of wow that's a lot to unpack or something like that
            //todo or like do you always talk this much?

            //todo trigger a DM mode
            //todo basically a text based dungeon crawler
            //todo build it like the old ones lol
        }

        private string GenerateTraceryResponse(Category cat)
        {
            if (!_categoryStartSymbols.ContainsKey(cat))
            {
                Console.WriteLine("Can't find start symbol for Category " + cat);
                return "Derpus tired. Try again later.";
            }

            var startSymbol = _categoryStartSymbols[cat];

            try
            {
                var grammar = new TraceryNet.Grammar(_traceryText);

                var response = grammar.Flatten(startSymbol);

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generating with tracery" + e);
                throw;
            }
        }

        private void LoadTraceryText()
        {
            _traceryText = LoadJson();

            Console.WriteLine($"{Prefix.Info} Tracery Text Loaded");
        }

        private string LoadJson()
        {
            using (var r = new StreamReader(AllCategoriesTraceryPath))
            {
                return r.ReadToEnd();
            }
        }

        private void LoadTriggerLookup()
        {
            _triggerLookup = new Dictionary<string, Category>();

            var categories = GetEnumList<Category>().Where(c => c != Category.All);

            foreach (var cat in categories)
            {
                AddCategoryTriggers(cat);
            }

            Console.WriteLine($"{Prefix.Info} Trigger Lookups Loaded");
        }

        private void AddCategoryTriggers(Category cat)
        {
            if (_categoryTriggers == null || !_categoryTriggers.ContainsKey(cat))
            {
                return;
            }

            foreach (var trigger in _categoryTriggers[cat])
            {
                if (_triggerLookup.ContainsKey(trigger))
                {
                    continue;
                }

                _triggerLookup.Add(trigger, cat);
            }
        }

        private static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"([^\s\w])", "",
                    RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }

        private Category DetermineCategory(string question)
        {
            if (string.IsNullOrEmpty(question))
            {
                return Category.All;
            }

            var words = question.Split(' ');

            var shuffledWords = Shuffle(words);

            const int maxTries = 3;

            for (var i = 0; i < maxTries; i++)
            {
                if (i >= shuffledWords.Count)
                {
                    return Category.All;
                }

                var word = shuffledWords[i];

                if (!_triggerLookup.ContainsKey(word))
                {
                    continue;
                }

                return _triggerLookup[word];
            }

            return Category.All;
        }

        private List<string> Shuffle(string[] words)
        {
            var wordList = words.ToList();

            for (var i = words.Length - 1; i > 0; i--)
            {
                var n = _rand.Next(0, i + 1);
                var temp = wordList[i];
                wordList[i] = wordList[n];
                wordList[n] = temp;
            }

            return wordList;
        }

        private static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
            return list;
        }
    }
}
