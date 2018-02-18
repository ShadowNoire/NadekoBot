﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NadekoBot.Core.Services.Database.Models;
using NadekoBot.Modules.Pokemon2.Common;
using NadekoBot.Modules.Pokemon2.Extentions;
using NadekoBot.Modules.Pokemon2.Services;

namespace NadekoBot.Modules.Pokemon2.Common
{
    class PokemonAttack
    {
        public PokemonSprite Attacker;
        public PokemonSprite Defender;
        public PokemonSpecies AttackSpecies;
        public PokemonSpecies DefendSpecies;
        public List<PokemonType> AttackerTypes { get; set; }
        public List<PokemonType> DefenseTypes { get; set; }
        public int Damage { get; }
        public KeyValuePair<string, string> move { get; }
        Random rng { get; set; } = new Random();
        public bool IsCritical { get; set; } = false;
        /// <summary>
        /// How effective the move is;
        /// 1: somewhat effective,
        /// less than 1: not effective
        /// more than 1: super effective
        /// </summary>
        public double Effectiveness { get; set; } = 1;
        public PokemonAttack(PokemonSprite attacker, PokemonSprite defender, KeyValuePair<string, string> move)
        {
            Attacker = attacker;
            Defender = defender;
            AttackSpecies = attacker.GetSpecies();
            DefendSpecies = defender.GetSpecies();
            AttackerTypes = AttackSpecies.GetPokemonTypes();
            DefenseTypes = DefendSpecies.GetPokemonTypes();
            this.move = move;
            Damage = CalculateDamage();

        }


        private int CalculateDamage()
        {
            //use formula in http://bulbapedia.bulbagarden.net/wiki/Damage
            double attack = Attacker.Attack;
            double defense = Defender.Defense;

            double basePower = rng.Next(40, 120);
            double toReturn = ((2 * (double)Attacker.Level + 10) / 250) * (attack / defense) * basePower + 2;
            toReturn = toReturn * GetModifier();
            return (int)Math.Floor(toReturn);
        }

        private double GetModifier()
        {
            var stablist = AttackerTypes.Where(x => x.Name == move.Value);
            double stab = 1;
            if (stablist.Any())
                stab = 1.5;
            var typeEffectiveness = setEffectiveness();
            double critical = 1;
            if (rng.Next(0, 100) < 10)
            {
                IsCritical = true;
                critical = 2;
            }
            double other = /*rng.NextDouble() * 2*/1;
            double random = (double)rng.Next(85, 100) / 100;
            double mod = stab * typeEffectiveness * critical * other * random;
            return mod;
        }

        private double setEffectiveness()
        {


            var moveTypeString = move.Value.ToUpperInvariant();
            var moveType = moveTypeString.StringToPokemonType();
            var dTypeStrings = DefenseTypes.Select(x => x.Name);
            var mpliers = moveType.Multipliers.Where(x => dTypeStrings.Contains(x.Type));
            foreach (var mplier in mpliers)
            {
                Effectiveness = Effectiveness * mplier.Multiplication;
            }
            return Effectiveness;
        }



        public string AttackString()
        {
            var str = $"**{Attacker.NickName}** attacked **{Defender.NickName}**\n" +
                $"{Defender.NickName} received {Damage} damage!\n";
            if (IsCritical)
            {
                str += "It's a critical hit!\n";
            }
            if (Effectiveness > 1)
            {
                str += "It's super effective!\n";
            }
            else if (Effectiveness < 1)
            {
                str += "It's ineffective...\n";
            }
            else
            {
                str += "It's somewhat effective\n";
            }



            return str;
        }
    }
}
