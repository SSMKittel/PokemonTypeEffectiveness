using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonTypes
{
    class Program
    {
        const TypeStringMode Mode = TypeStringMode.Position;
        const bool Shortened = true;

        const string TypeEffectivenessContainer = Shortened ? "te" : "type-effectiveness-";
        const string DefenderTypePrefix = Shortened ? "dt" : "defender-type-";
        const string AttackerTypePrefix = Shortened ? "at" : "attacker-type-";
        const string InverseBattleTypeModifier = Shortened ? "mi" : "modifier-inverse";

        static double[,] SingleMatchup = new double[,]
        {
            {1, 1, 1, 1, 1, 0.5, 1, 0, 0.5, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {2, 1, 0.5, 0.5, 1, 2, 0.5, 0, 2, 1, 1, 1, 1, 0.5, 2, 1, 2, 0.5},
            {1, 2, 1, 1, 1, 0.5, 2, 1, 0.5, 1, 1, 2, 0.5, 1, 1, 1, 1, 1},
            {1, 1, 1, 0.5, 0.5, 0.5, 1, 0.5, 0, 1, 1, 2, 1, 1, 1, 1, 1, 2},
            {1, 1, 0, 2, 1, 2, 0.5, 1, 2, 2, 1, 0.5, 2, 1, 1, 1, 1, 1},
            {1, 0.5, 2, 1, 0.5, 1, 2, 1, 0.5, 2, 1, 1, 1, 1, 2, 1, 1, 1},
            {1, 0.5, 0.5, 0.5, 1, 1, 1, 0.5, 0.5, 0.5, 1, 2, 1, 2, 1, 1, 2, 0.5},
            {0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5, 1},
            {1, 1, 1, 1, 1, 2, 1, 1, 0.5, 0.5, 0.5, 1, 0.5, 1, 2, 1, 1, 2},
            {1, 1, 1, 1, 1, 0.5, 2, 1, 2, 0.5, 0.5, 2, 1, 1, 2, 0.5, 1, 1},
            {1, 1, 1, 1, 2, 2, 1, 1, 1, 2, 0.5, 0.5, 1, 1, 1, 0.5, 1, 1},
            {1, 1, 0.5, 0.5, 2, 2, 0.5, 1, 0.5, 0.5, 2, 0.5, 1, 1, 1, 0.5, 1, 1},
            {1, 1, 2, 1, 0, 1, 1, 1, 1, 1, 2, 0.5, 0.5, 1, 1, 0.5, 1, 1},
            {1, 2, 1, 2, 1, 1, 1, 1, 0.5, 1, 1, 1, 1, 0.5, 1, 1, 0, 1},
            {1, 1, 2, 1, 2, 1, 1, 1, 0.5, 0.5, 0.5, 2, 1, 1, 0.5, 2, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 0.5, 1, 1, 1, 1, 1, 1, 2, 1, 0},
            {1, 0.5, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5, 0.5},
            {1, 2, 1, 0.5, 1, 1, 1, 1, 0.5, 0.5, 1, 1, 1, 1, 1, 2, 2, 1},
        };

        static void Main(string[] args)
        {
            var allMatchups = new List<(Type defender1, Type defender2, Type attacker, Effectiveness effectiveness, BattleType? battle)>();
            var whitelist = new List<(Type defender1, Type? defender2, Type? attacker, Effectiveness effectiveness, BattleType? battle)>();
            var blacklist = new List<(Type defender1, Type defender2, Type? attacker, Effectiveness effectiveness, BattleType? battle)>();

            for (Type defender1 = Type.normal; defender1 <= Type.fairy; defender1++)
            {
                for (Type defender2 = Type.normal; defender2 <= Type.fairy; defender2++)
                {
                    foreach (var effectiveness in GetEmptyEffectiveness(defender1, defender2, BattleType.normal))
                    {
                        whitelist.Add((defender1, defender2, null, effectiveness, BattleType.normal));
                        if (defender1 == defender2)
                        {
                            whitelist.Add((defender1, null, null, effectiveness, BattleType.normal));
                        }
                    }

                    foreach (var effectiveness in GetEmptyEffectiveness(defender1, defender2, BattleType.inverse))
                    {
                        if (effectiveness == Effectiveness.immune)
                        {
                            // We are hiding all immunities as there are none
                            continue;
                        }
                        whitelist.Add((defender1, defender2, null, effectiveness, BattleType.inverse));
                        if (defender1 == defender2)
                        {
                            whitelist.Add((defender1, null, null, effectiveness, BattleType.inverse));
                        }
                    }

                    for (Type attacker = Type.normal; attacker <= Type.fairy; attacker++)
                    {
                        Effectiveness effectNorm = Classify(defender1, defender2, attacker, BattleType.normal);
                        Effectiveness effectInv = Classify(defender1, defender2, attacker, BattleType.inverse);
                        if (effectNorm == effectInv)
                        {
                            allMatchups.Add((defender1, defender2, attacker, effectNorm, null));
                        }
                        else
                        {
                            allMatchups.Add((defender1, defender2, attacker, effectNorm, BattleType.normal));
                            allMatchups.Add((defender1, defender2, attacker, effectInv, BattleType.inverse));
                        }
                    }
                }
            }

            foreach (var matchup in allMatchups.Where(m => m.defender1 != m.defender2))
            {
                var match1 = Classify(matchup.defender1, matchup.defender1, matchup.attacker, matchup.battle ?? BattleType.normal);
                var match2 = Classify(matchup.defender2, matchup.defender2, matchup.attacker, matchup.battle ?? BattleType.normal);
                if (match1 != matchup.effectiveness)
                {
                    blacklist.Add((matchup.defender1, matchup.defender2, matchup.attacker, match1, matchup.battle));
                }
                if (match2 != matchup.effectiveness)
                {
                    blacklist.Add((matchup.defender1, matchup.defender2, matchup.attacker, match2, matchup.battle));
                }
                if (match1 != matchup.effectiveness && match2 != matchup.effectiveness)
                {
                    whitelist.Add((matchup.defender1, matchup.defender2, matchup.attacker, matchup.effectiveness, matchup.battle));
                }
            }

            var blacklistIrrelevantBattleModifier = blacklist.GroupBy(x => (x.defender1, x.defender2, x.attacker, x.effectiveness))
                .Where(x => x.Count() >= 2)
                .ToList();

            blacklist = blacklist
                .Except(blacklistIrrelevantBattleModifier.SelectMany(x => x))
                .Union(blacklistIrrelevantBattleModifier
                    .Select(x => x.First())
                    .Select(x => (x.defender1, x.defender2, x.attacker, x.effectiveness, (BattleType?) null))
                )
                .ToList();

            var singleMatchups = new List<(Type defender, Type attacker, Effectiveness effectiveness, BattleType? battle)>(
                allMatchups
                    .Where(m => m.defender1 == m.defender2)
                    .Select(m => (m.defender1, m.attacker, m.effectiveness, m.battle))
            );

            Write(singleMatchups, whitelist, blacklist);
        }

        private static List<Effectiveness> GetEmptyEffectiveness(Type defender1, Type defender2, BattleType battle)
        {
            var results = new List<Effectiveness> { Effectiveness.normal, Effectiveness.weak, Effectiveness.immune, Effectiveness.resist };
            for (Type attacker = Type.normal; attacker <= Type.fairy; attacker++)
            {
                results.Remove(Classify(defender1, defender2, attacker, battle));
                if (!results.Any())
                {
                    return results;
                }
            }
            return results;
        }

        private static void Write(
            List<(Type defender, Type attacker, Effectiveness effectiveness, BattleType? battle)> singleMatchups,
            List<(Type defender1, Type? defender2, Type? attacker, Effectiveness effectiveness, BattleType? battle)> whitelist,
            List<(Type defender1, Type defender2, Type? attacker, Effectiveness effectiveness, BattleType? battle)> blacklist)
        {
            var hideRules = new List<string>();
            var showRules = new List<string>();

            foreach (var m in singleMatchups)
            {
                string defender = DefenderTypePrefix + ToTypeString(m.defender);
                string attacker = AttackerTypePrefix + ToTypeString(m.attacker);
                string classification = ToEffectivenessString(m.effectiveness);
                string modifiers = GetModifiersSelector(m.battle);

                showRules.Add($".{TypeEffectivenessContainer} *[value=\"{defender}\"]:checked{modifiers}~.{classification} .{attacker}");
            }

            foreach (var m in whitelist)
            {
                string defender1 = DefenderTypePrefix + ToTypeString(m.defender1);
                string defender2 = DefenderTypePrefix + ToTypeString(m.defender2);
                string attacker = AttackerTypePrefix + ToTypeString(m.attacker);
                string classification = ToEffectivenessString(m.effectiveness);
                string modifiers = GetModifiersSelector(m.battle);

                showRules.Add($".{TypeEffectivenessContainer} *[value=\"{defender1}\"]:checked~*[value=\"{defender2}\"]:checked{modifiers}~.{classification} .{attacker}");
            }

            foreach (var m in blacklist)
            {
                string defender1 = DefenderTypePrefix + ToTypeString(m.defender1);
                string defender2 = DefenderTypePrefix + ToTypeString(m.defender2);
                string attacker = AttackerTypePrefix + ToTypeString(m.attacker);
                string classification = ToEffectivenessString(m.effectiveness);
                string modifiers = GetModifiersSelector(m.battle);

                hideRules.Add($".{TypeEffectivenessContainer} *[value=\"{defender1}\"]:checked~*[value=\"{defender2}\"]:checked{modifiers}~.{classification} .{attacker}");
            }

            Console.Write("@charset \"UTF-8\";\n");
            Console.Write(".te .types>*{display:none}\n");
            Console.Write($"*[value=\"{InverseBattleTypeModifier}\"]:checked~.tei{{display:none}}\n");

            var hideChunks = hideRules
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 500)
                .Select(x => x.Select(v => v.Value).ToList());

            foreach (var chunk in hideChunks)
            {
                Console.Write(string.Join(",\n", chunk));
                Console.Write("\n{display:none}\n");
            }

            var showChunks = showRules
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 500)
                .Select(x => x.Select(v => v.Value).ToList());

            foreach (var chunk in showChunks)
            {
                Console.Write(string.Join(",\n", chunk));
                Console.Write("\n{display:list-item}\n");
            }
        }

        private static string GetModifiersSelector(BattleType? battle)
        {
            string modifiers = "";
            if (battle != null)
            {
                if (battle == BattleType.normal)
                {
                    modifiers += $"~*[value=\"{InverseBattleTypeModifier}\"]:not(:checked)";
                }
                else
                {
                    modifiers += $"~*[value=\"{InverseBattleTypeModifier}\"]:checked";
                }
            }
            return modifiers;
        }

        private static double Matchup(Type defender1, Type defender2, Type attacker)
        {
            if (defender1 == defender2)
            {
                return SingleMatchup[(int)attacker, (int)defender1];
            }
            return SingleMatchup[(int)attacker, (int)defender1] * SingleMatchup[(int)attacker, (int)defender2];
        }

        private static Effectiveness Classify(Type defender1, Type defender2, Type attacker, BattleType battle)
        {
            double r = Matchup(defender1, defender2, attacker);
            Effectiveness result;
            switch (r)
            {
                case 0: result = Effectiveness.immune; break;
                case 0.25:
                case 0.5: result = Effectiveness.resist; break;
                case 1: result = Effectiveness.normal; break;
                case 2:
                case 4: result = Effectiveness.weak; break;
                default: throw new InvalidOperationException();
            }

            if (battle == BattleType.inverse)
            {
                if (result == Effectiveness.resist || result == Effectiveness.immune)
                {
                    result = Effectiveness.weak;
                }
                else if (result == Effectiveness.weak)
                {
                    result = Effectiveness.resist;
                }
            }

            return result;
        }

        private static string ToEffectivenessString(Effectiveness e)
        {
            if (Shortened)
            {
                return "te" + e.ToString()[0];
            }
            else
            {
                return "type-effectiveness-" + e.ToString();
            }
        }

        private static string ToTypeString(Type? t)
        {
            if (Mode == TypeStringMode.Position)
            {
                return t == null ? "-" : ((int)(t.Value)).ToString();
            }
            else if (Mode == TypeStringMode.String)
            {
                return t == null ? "-none" : t.Value.ToString();
            }
            else
            {
                if (t == null)
                {
                    return "-";
                }
                switch (t.Value)
                {
                    case Type.normal: return "✴";
                    case Type.fighting: return "✊";
                    case Type.flying: return "✈";
                    case Type.poison: return "☠";
                    case Type.ground: return "⏚";
                    case Type.rock: return "⛰️";
                    case Type.bug: return "🐛";
                    case Type.ghost: return "👻";
                    case Type.steel: return "✂️";
                    case Type.fire: return "🔥";
                    case Type.water: return "🌊";
                    case Type.grass: return "⚘";
                    case Type.electric: return "⚡";
                    case Type.psychic: return "☯";
                    case Type.ice: return "❄️";
                    case Type.dragon: return "🐉";
                    case Type.dark: return "🌙";
                    case Type.fairy: return "🧚";
                    default: throw new InvalidOperationException();
                }
            }
        }
    }

    enum BattleType
    {
        normal,
        inverse
    }

    enum Effectiveness
    {
        normal,
        weak,
        immune,
        resist,
    }

    enum TypeStringMode
    {
        String,
        Position,
        Emoji
    }

    enum Type
    {
        normal,
        fighting,
        flying,
        poison,
        ground,
        rock,
        bug,
        ghost,
        steel,
        fire,
        water,
        grass,
        electric,
        psychic,
        ice,
        dragon,
        dark,
        fairy
    }
}
