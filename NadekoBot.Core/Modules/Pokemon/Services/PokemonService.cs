﻿using NLog;
using NadekoBot.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NadekoBot.Modules.Pokemon.Common;
using Newtonsoft.Json;
using NadekoBot.Core.Services.Impl;
using NadekoBot.Common;

namespace NadekoBot.Modules.Pokemon.Services
{
    public class PokemonService : INService
    { 
        public readonly List<PokemonSpecies> pokemonClasses = new List<PokemonSpecies>();
        public readonly List<PokemonType> pokemonTypes = new List<PokemonType>();

        public const string PokemonClassesFile = "data/pokemon/pokemonBattlelist.json";
        public const string PokemonTypesFile = "data/pokemon_types.json";

        private readonly IBotConfigProvider _bc;
        private readonly CommandHandler _cmd;
        private readonly IImageCache _images;
        private readonly Logger _log;
        private readonly NadekoRandom _rng;
        private readonly ICurrencyService _cs;
        public readonly string TypingArticlesPath = "data/typing_articles3.json";
        private readonly CommandHandler _cmdHandler;
        public static PokemonService pokemonInstance;

        public PokemonService(CommandHandler cmd, IBotConfigProvider bc, NadekoBot bot,
            NadekoStrings strings, IDataCache data, CommandHandler cmdHandler,
            ICurrencyService cs)
        {
            _bc = bc;
            _cmd = cmd;
            _images = data.LocalImages;
            _cmdHandler = cmdHandler;
            _log = LogManager.GetCurrentClassLogger();
            _rng = new NadekoRandom();
            _cs = cs;
            pokemonInstance = this;  

            if (File.Exists(PokemonClassesFile))
            {
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                            {
                                if (System.Diagnostics.Debugger.IsAttached)
                                {
                                    System.Diagnostics.Debugger.Break();
                                }
                            }
                };
                pokemonClasses = JsonConvert.DeserializeObject<List<PokemonSpecies>>(File.ReadAllText(PokemonClassesFile),settings);
            }
            else
            {
                pokemonClasses = new List<PokemonSpecies>();
                _log.Warn(PokemonClassesFile + " is missing. Pokemon Classes not loaded.");
            }
            if (File.Exists(PokemonTypesFile))
            {
                pokemonTypes = JsonConvert.DeserializeObject<List<PokemonType>>(File.ReadAllText(PokemonTypesFile));
            }
            else
            {
                pokemonTypes = new List<PokemonType>();
                _log.Warn(PokemonClassesFile + " is missing. Pokemon types not loaded.");
            }
        }
        public string GetRandomTrainerImage()
        {
            
            var rng = new NadekoRandom();
            var cur = _images.ImageUrls.PokemonUrls;
            return cur[rng.Next(0, cur.Length)];
        }
    }
}
