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

        const string TypeEffectivenessContainer = Shortened ? ".te" : ".type-effectiveness-";
        const string DefenderTypePrefix = Shortened ? "dt" : "defender-type-";
        const string AttackerTypePrefix = Shortened ? ".at" : ".attacker-type-";

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
            var normal = new List<(Type, Type, Type?)>();
            var weak = new List<(Type, Type, Type?)>();
            var immune = new List<(Type, Type, Type?)>();
            var resist = new List<(Type, Type, Type?)>();

            for (Type defender1 = Type.normal; defender1 <= Type.fairy; defender1++)
            { 
                for (Type defender2 = Type.normal; defender2 <= Type.fairy; defender2++)
                {
                    var normalTmp = new List<(Type, Type, Type?)>();
                    var weakTmp = new List<(Type, Type, Type?)>();
                    var immuneTmp = new List<(Type, Type, Type?)>();
                    var resistTmp = new List<(Type, Type, Type?)>();

                    for (Type attacker = Type.normal; attacker <= Type.fairy; attacker++)
                    {
                        if (IsNormal(attacker, defender1, defender2))
                        {
                            normalTmp.Add((defender1, defender2, attacker));
                        }
                        else if (IsWeak(attacker, defender1, defender2))
                        {
                            weakTmp.Add((defender1, defender2, attacker));
                        }
                        else if (IsImmune(attacker, defender1, defender2))
                        {
                            immuneTmp.Add((defender1, defender2, attacker));
                        }
                        else if (IsResist(attacker, defender1, defender2))
                        {
                            resistTmp.Add((defender1, defender2, attacker));
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    if (normalTmp.Any())
                    {
                        normal.AddRange(normalTmp);
                    }
                    else
                    {
                        normal.Add((defender1, defender2, null));
                    }

                    if (weakTmp.Any())
                    {
                        weak.AddRange(weakTmp);
                    }
                    else
                    {
                        weak.Add((defender1, defender2, null));
                    }

                    if (immuneTmp.Any())
                    {
                        immune.AddRange(immuneTmp);
                    }
                    else
                    {
                        immune.Add((defender1, defender2, null));
                    }

                    if (resistTmp.Any())
                    {
                        resist.AddRange(resistTmp);
                    }
                    else
                    {
                        resist.Add((defender1, defender2, null));
                    }
                }
            }

            Write(normal, Shortened ? ".ten" : ".type-effectiveness-normal");
            Write(weak, Shortened ? ".tew" : ".type-effectiveness-weak");
            Write(immune, Shortened ? ".tei" : ".type-effectiveness-immune");
            Write(resist, Shortened ? ".ter" : ".type-effectiveness-resist");
        }

        private static void Write(List<(Type, Type, Type?)> matchups, string typeContainer)
        {
            var rules = new List<string>();
            foreach (var m in matchups)
            {
                string attacker = AttackerTypePrefix + ToString(m.Item3);
                string def1 = DefenderTypePrefix + ToString(m.Item1);
                string def2 = DefenderTypePrefix + ToString(m.Item2);

                rules.Add($"{TypeEffectivenessContainer} input[value=\"{def1}\"]:checked~input[value=\"{def2}\"]:checked ~ {typeContainer} {attacker}");
                if (m.Item1 == m.Item2)
                {
                    def2 = DefenderTypePrefix + ToString(null);
                    rules.Add($"{TypeEffectivenessContainer} input[value=\"{def1}\"]:checked~input[value=\"{def2}\"]:checked ~ {typeContainer} {attacker}");
                }
            }

            var chunks = rules
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 500)
                .Select(x => x.Select(v => v.Value).ToList());

            foreach (var chunk in chunks)
            {
                Console.Write(string.Join(",\n", chunk));
                Console.WriteLine("\n{display:list-item;}");
            }
        }

        private static double Matchup(Type attacker, Type defender1, Type defender2)
        {
            if (defender1 == defender2)
            {
                return SingleMatchup[(int)attacker, (int)defender1];
            }
            return SingleMatchup[(int)attacker, (int)defender1] * SingleMatchup[(int)attacker, (int)defender2];
        }

        private static bool IsResist(Type attacker, Type defender1, Type defender2)
        {
            double r = Matchup(attacker, defender1, defender2);
            return r == 0.25 || r == 0.5;
        }

        private static bool IsImmune(Type attacker, Type defender1, Type defender2)
        {
            return Matchup(attacker, defender1, defender2) == 0;
        }

        private static bool IsWeak(Type attacker, Type defender1, Type defender2)
        {
            double r = Matchup(attacker, defender1, defender2);
            return r == 2 || r == 4;
        }

        private static bool IsNormal(Type attacker, Type defender1, Type defender2)
        {
            return Matchup(attacker, defender1, defender2) == 1;
        }

        private static string ToString(Type? t)
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
