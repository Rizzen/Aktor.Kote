using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Aktor.Kote.Utils
{
    public class KoteNameGenerator
    {
        public static readonly KoteNameGenerator Default = new KoteNameGenerator(AppDomain.CurrentDomain.BaseDirectory + "\\names.json");
        
        private readonly string[] _names;
        private readonly Random _random = new Random();
        
        public KoteNameGenerator(string fileName)
        {
            _names = JArray.Parse(File.ReadAllText(fileName)).ToObject<string[]>();
        }

        public string GetKoteName()
        {
            return $"{_names[_random.Next(_names.Length - 1)]}{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
        }
    }
}