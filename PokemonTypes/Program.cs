using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTypes
{
    class Program
    {
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
            for (Type defender1 = Type.normal; defender1 <= Type.fairy; defender1++)
            { 
                for (Type defender2 = Type.normal; defender2 <= Type.fairy; defender2++)
                {
                    var normal = new List<Type>();
                    var weak = new List<Type>();
                    var immune = new List<Type>();
                    var resist = new List<Type>();

                    for (Type attacker = Type.normal; attacker <= Type.fairy; attacker++)
                    {
                        if (IsNormal(attacker, defender1, defender2))
                        {
                            normal.Add(attacker);
                        }
                        else if (IsWeak(attacker, defender1, defender2))
                        {
                            weak.Add(attacker);
                        }
                        else if (IsImmune(attacker, defender1, defender2))
                        {
                            immune.Add(attacker);
                        }
                        else if (IsResist(attacker, defender1, defender2))
                        {
                            resist.Add(attacker);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    string def1 = ((int)defender1).ToString();
                    string def2 = defender1 == defender2 ? "-" : ((int)defender2).ToString();

                    if (normal.Any())
                    {
                        Console.Write(string.Join(",\n", normal.Select(t => $".te.t{def1}.t{def2} .te-n .t{(int)t}")));
                    }
                    else
                    {
                        Console.Write($".te.t{def1}.t{def2} .te-n .t-");
                    }
                    Console.WriteLine(" {\n\tdisplay: list-item;\n}\n");

                    if (weak.Any())
                    {
                        Console.Write(string.Join(",\n", weak.Select(t => $".te.t{def1}.t{def2} .te-w .t{(int)t}")));
                    }
                    else
                    {
                        Console.Write($".te.t{def1}.t{def2} .te-w .t-");
                    }
                    Console.WriteLine(" {\n\tdisplay: list-item;\n}\n");

                    if (immune.Any())
                    {
                        Console.Write(string.Join(",\n", immune.Select(t => $".te.t{def1}.t{def2} .te-i .t{(int)t}")));
                    }
                    else
                    {
                        Console.Write($".te.t{def1}.t{def2} .te-i .t-");
                    }
                    Console.WriteLine(" {\n\tdisplay: list-item;\n}\n");

                    if (resist.Any())
                    {
                        Console.Write(string.Join(",\n", resist.Select(t => $".te.t{def1}.t{def2} .te-r .t{(int)t}")));
                    }
                    else
                    {
                        Console.Write($".te.t{def1}.t{def2} .te-r .t-");
                    }
                    Console.WriteLine(" {\n\tdisplay: list-item;\n}\n");
                }
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
            return r > 0 && r < 1;
        }

        private static bool IsImmune(Type attacker, Type defender1, Type defender2)
        {
            return Matchup(attacker, defender1, defender2) == 0;
        }

        private static bool IsWeak(Type attacker, Type defender1, Type defender2)
        {
            double r = Matchup(attacker, defender1, defender2);
            return r > 1;
        }

        static bool IsNormal(Type attacker, Type defender1, Type defender2)
        {
            return Matchup(attacker, defender1, defender2) == 1;
        }
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
