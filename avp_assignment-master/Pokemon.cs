using System;
using System.Collections.Generic;

namespace PokemonPocket
{

    public class PokemonMaster
    {
        public string Name { get; set; }
        public int NoToEvolve { get; set; }
        public string EvolveTo { get; set; }

        public PokemonMaster(string name, int noToEvolve, string evolveTo)
        {
            this.Name = name;
            this.NoToEvolve = noToEvolve;
            this.EvolveTo = evolveTo;
        }

    }

    public class Skill 
    {
        public string ID { get; set; }
        public string Desc { get; set; }
        public double Dmg { get; set; }

        public Skill(string ID, string desc, double dmg)
        {
            this.ID = ID;
            this.Desc = desc;
            this.Dmg = dmg;
        }
    }
    public class Pokemon
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Atk { get; set; }
        public int Experience { get; set; }
        public int NextLevel { get; set; }
        public virtual string Skill { get; set; }
        public virtual string Type { get; set; }
        public virtual int Level { get; set; }

        public Pokemon(int ID, string name, int hp, int atk, int experience, int nextLevel, string skill, string type, int level)
        {
            this.ID = ID;
            this.Name = name;
            this.Hp = hp;
            this.Atk = atk;
            this.Experience = experience;
            this.NextLevel = nextLevel;
            this.Skill = skill;
            this.Type = type;
            this.Level = level;
        }
    }

    public class Pikachu : Pokemon
    {
        public Pikachu(int ID, string name, int hp, int atk, int experience, int nextLevel, string skill, string type, int level) : base(ID, name, hp, atk, experience, nextLevel, skill, type, level)
        {

        }
    }

    public class Eevee : Pokemon
    {
        public Eevee(int ID, string name, int hp, int atk, int experience, int nextLevel, string skill, string type, int level) : base(ID, name, hp, atk, experience, nextLevel, skill, type, level)
        {

        }
    }
    public class Charmander : Pokemon
    {
        public Charmander(int ID, string name, int hp, int atk, int experience, int nextLevel, string skill, string type, int level) : base(ID, name, hp, atk, experience, nextLevel, skill, type, level)
        {

        }
    }

}