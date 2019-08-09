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
            var allMatchups = new List<(Type, Type, Type, Effectiveness)>();
            var whitelist = new List<(Type, Type?, Type?, Effectiveness)>();
            var blacklist = new List<(Type, Type, Type?, Effectiveness)>();

            for (Type defender1 = Type.normal; defender1 <= Type.fairy; defender1++)
            {
                for (Type defender2 = Type.normal; defender2 <= Type.fairy; defender2++)
                {
                    var emptyC = GetEmptyEffectiveness(defender1, defender2);

                    // Testing determined it's less css to simply output all the "None" cases
                    // rather than trying to do single/compound logic like the type matchups
                    foreach (var effectiveness in emptyC)
                    {
                        whitelist.Add((defender1, defender2, null, effectiveness));
                        if (defender1 == defender2)
                        {
                            whitelist.Add((defender1, null, null, effectiveness));
                        }
                    }

                    for (Type attacker = Type.normal; attacker <= Type.fairy; attacker++)
                    {
                        Effectiveness effect = Classify(defender1, defender2, attacker);
                        switch (effect)
                        {
                            case Effectiveness.normal: allMatchups.Add((defender1, defender2, attacker, Effectiveness.normal)); break;
                            case Effectiveness.weak: allMatchups.Add((defender1, defender2, attacker, Effectiveness.weak)); break;
                            case Effectiveness.immune: allMatchups.Add((defender1, defender2, attacker, Effectiveness.immune)); break;
                            case Effectiveness.resist: allMatchups.Add((defender1, defender2, attacker, Effectiveness.resist)); break;
                            default: throw new InvalidOperationException();
                        }
                    }
                }
            }

            foreach (var matchup in allMatchups.Where(m => m.Item1 != m.Item2))
            {
                var match1 = Classify(matchup.Item1, matchup.Item1, matchup.Item3);
                var match2 = Classify(matchup.Item2, matchup.Item2, matchup.Item3);
                if (match1 != matchup.Item4)
                {
                    blacklist.Add((matchup.Item1, matchup.Item2, matchup.Item3, match1));
                }
                if (match2 != matchup.Item4)
                {
                    blacklist.Add((matchup.Item1, matchup.Item2, matchup.Item3, match2));
                }
                if (match1 != matchup.Item4 && match2 != matchup.Item4)
                {
                    whitelist.Add((matchup.Item1, matchup.Item2, matchup.Item3, matchup.Item4));
                }
            }

            var singleMatchups = new List<(Type, Type, Effectiveness)>(
                allMatchups
                    .Where(m => m.Item1 == m.Item2)
                    .Select(m => (m.Item1, m.Item3, m.Item4))
            );

            Write(singleMatchups, whitelist, blacklist);
        }

        private static List<Effectiveness> GetEmptyEffectiveness(Type defender1, Type defender2)
        {
            var results = new List<Effectiveness> { Effectiveness.normal, Effectiveness.weak, Effectiveness.immune, Effectiveness.resist };
            for (Type attacker = Type.normal; attacker <= Type.fairy; attacker++)
            {
                results.Remove(Classify(defender1, defender2, attacker));
                if (!results.Any())
                {
                    return results;
                }
            }
            return results;
        }

        private static void Write(
            List<(Type, Type, Effectiveness)> singleMatchups,
            List<(Type, Type?, Type?, Effectiveness)> whitelist,
            List<(Type, Type, Type?, Effectiveness)> blacklist)
        {
            var hideRules = new List<string>();
            var showRules = new List<string>();

            foreach (var m in singleMatchups)
            {
                string defender = DefenderTypePrefix + ToTypeString(m.Item1);
                string attacker = AttackerTypePrefix + ToTypeString(m.Item2);
                string classification = ToEffectivenessString(m.Item3);

                showRules.Add($".{TypeEffectivenessContainer} input[value=\"{defender}\"]:checked~.{classification} .{attacker}");
            }

            foreach (var m in whitelist)
            {
                string defender1 = DefenderTypePrefix + ToTypeString(m.Item1);
                string defender2 = DefenderTypePrefix + ToTypeString(m.Item2);
                string attacker = AttackerTypePrefix + ToTypeString(m.Item3);
                string classification = ToEffectivenessString(m.Item4);

                showRules.Add($".{TypeEffectivenessContainer} input[value=\"{defender1}\"]:checked~input[value=\"{defender2}\"]:checked~.{classification} .{attacker}");
            }

            foreach (var m in blacklist)
            {
                string defender1 = DefenderTypePrefix + ToTypeString(m.Item1);
                string defender2 = DefenderTypePrefix + ToTypeString(m.Item2);
                string attacker = AttackerTypePrefix + ToTypeString(m.Item3);
                string classification = ToEffectivenessString(m.Item4);

                hideRules.Add($".{TypeEffectivenessContainer} input[value=\"{defender1}\"]:checked~input[value=\"{defender2}\"]:checked~.{classification} .{attacker}");
            }

            Console.Write("@charset \"UTF-8\";\n");
            Console.Write(".te .types>*{display:none}\n");

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

        private static double Matchup(Type defender1, Type defender2, Type attacker)
        {
            if (defender1 == defender2)
            {
                return SingleMatchup[(int)attacker, (int)defender1];
            }
            return SingleMatchup[(int)attacker, (int)defender1] * SingleMatchup[(int)attacker, (int)defender2];
        }

        private static Effectiveness Classify(Type defender1, Type defender2, Type attacker)
        {
            double r = Matchup(defender1, defender2, attacker);
            switch (r)
            {
                case 0: return Effectiveness.immune;
                case 0.25:
                case 0.5: return Effectiveness.resist;
                case 1: return Effectiveness.normal;
                case 2:
                case 4: return Effectiveness.weak;
                default: throw new InvalidOperationException();
            }
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
