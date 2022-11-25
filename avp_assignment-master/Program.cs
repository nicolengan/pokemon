using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PokemonPocket
{
    public class PokemonContext : DbContext
    {
        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<Skill> Skills { get; set; }

        public string dbpath { get; set; }

        public PokemonContext()
        {
            var path = Directory.GetCurrentDirectory();
            dbpath = System.IO.Path.Join(path, "Pokemon.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={dbpath}");

    }


    class Program
    {
        private static int listAllPokemon(PokemonContext db)
        {
            var p_list = db.Pokemons.OrderBy(p => p.Hp).ToList();
            int x = 0;

            foreach (var p in p_list)
            {
                x += 1;
                var check_p = db.Skills
                    .Where(c => c.ID.ToLower() == p.Skill.ToLower())
                    .First();
                Console.Write($"\n({x}). {p.Name} - {p.Type}\n{new string('=', 25)}\nid: {p.ID}\nHP: {p.Hp} Atk: {p.Atk}\nExperience: {p.Experience}/{p.NextLevel}\nLevel: {p.Level}\n");
                Console.Write($"Skill: {p.Skill}\nDescription: {check_p.Desc}\n{new string('=', 25)}\n");
            }
            return x;
        }

        private static int listPokemonBattle(PokemonContext db)
        {
            var p_list = db.Pokemons.OrderBy(p => p.Hp).Where(p => p.Hp > 0);
            int x = 0;
            var no_alive = p_list.Count();
            if (p_list.Count() >= 2)
            {
                foreach (var p in p_list)
                {
                    if (p.Hp > 0)
                    {
                        x += 1;
                        var check_p = db.Skills
                            .Where(c => c.ID.ToLower() == p.Skill.ToLower())
                            .First();
                        Console.Write($"\n({x}). {p.Name} - {p.Type}\n{new string('=', 25)}\nid: {p.ID}\nHP: {p.Hp} Atk: {p.Atk}\nExperience: {p.Experience}/{p.NextLevel}\nLevel: {p.Level}\n");
                        Console.Write($"Skill: {p.Skill}\nDescription: {check_p.Desc}\n{new string('=', 25)}\n");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nNot enough alive pokemon to battle, please add more pokemons and try again.");
            }
            return x;
        }
        private static int listSpecificPokemon(PokemonContext db, string name)
        {
            var p_list = db.Pokemons.Where(p => p.Name == name).OrderBy(p => p.Hp).ToList();
            int x = 0;

            foreach (var p in p_list)
            {
                x += 1;
                var check_p = db.Skills
                    .Where(c => c.ID.ToLower() == p.Skill.ToLower())
                    .First();
                Console.Write($"\n({x}). {p.Name} - {p.Type}\n{new string('=', 25)}\nid: {p.ID}\nHP: {p.Hp} Atk: {p.Atk}\nExperience: {p.Experience}/{p.NextLevel}\nLevel: {p.Level}\n");
                Console.Write($"Skill: {p.Skill}\nDescription: {check_p.Desc}\n{new string('=', 25)}\n");
            }
            return x;
        }

        private static string checkName(PokemonContext db)
        {
            Console.Write("Enter Pokemon's name: ");
            var name = Console.ReadLine().ToLower();

            while (string.IsNullOrEmpty(name) | !db.Pokemons.Any(p => p.Name.ToLower() == $"{name}"))
            {
                Console.Write("Pokemon cannot be found, please try again: ");
                name = Console.ReadLine().ToLower();
            }
            return name;
        }

        private static List<string> checkEvolve(PokemonContext db, List<PokemonMaster> pokemonMasters)
        {
            List<string> Can_Evolves = new List<String>();
            var pikachu_count = db.Pokemons.Where(p => p.Name.ToLower() == "pikachu").ToList().Count >= pokemonMasters.Where(p => p.Name.ToLower() == "pikachu").First().NoToEvolve;
            var eevee_count = db.Pokemons.Where(p => p.Name.ToLower() == "eevee").ToList().Count >= pokemonMasters.Where(p => p.Name.ToLower() == "eevee").First().NoToEvolve;
            var charmander_count = db.Pokemons.Where(p => p.Name.ToLower() == "charmander").ToList().Count >= pokemonMasters.Where(p => p.Name.ToLower() == "charmander").First().NoToEvolve;
            bool x = pikachu_count | eevee_count | charmander_count;
            if (x)
            {
                Console.WriteLine("Pokemons that are able to evolve right now are: ");
                if (pikachu_count)
                {
                    Console.WriteLine("Pikachu ---> Raichu");
                    Can_Evolves.Add("Pikachu");
                }
                if (eevee_count)
                {
                    Console.WriteLine("Eevee ---> Flareon");
                    Can_Evolves.Add("Eevee");
                }
                if (charmander_count)
                {
                    Console.WriteLine("Charmander ---> Charmeleon");
                    Can_Evolves.Add("Charmander");
                }
            }
            else{
                Console.WriteLine("\nNo pokemon are able to evolve right now");
            }
            return Can_Evolves;

        }
        private static int checkID(PokemonContext db)
        {
            var id = 0;
            while (true)
            {
                try
                {
                    Console.Write("Enter pokemon's id: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    var del_p = db.Pokemons
                        .Where(p => p.ID == id)
                        .First();
                    break;
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Invalid ID input, please try again");
                    continue;
                }
            }
            return id;
        }
        private static void checkExp(PokemonContext db)
        {
            var exp_list = db.Pokemons.ToList();
            foreach (var p in exp_list)
            {
                while (p.Experience > p.NextLevel)
                {
                    Console.WriteLine($"Pokemon has levelled up! Your {p.Name} is now level {p.Level}!");
                    p.Experience -= p.NextLevel;
                    p.Hp += p.NextLevel/2;
                    p.Atk += p.Atk/2;
                    p.NextLevel *= 2;
                    p.Level += 1;
                }
                db.SaveChanges();
            }
        }

        private static int checkInt(PokemonContext db, string field){
            var x = 0;
            while (true)
            {
                try
                {
                    Console.Write($"Enter Pokemon's {field}: ");
                    x = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                catch (System.Exception)
                {
                    Console.Write($"Invalid {field} input, please try again\n");
                    continue;
                }
            }
            return x;

        }
        private static int createId(PokemonContext db){
            var id = 0;
            if (db.Pokemons.ToList().Count != 0)
            {
                id = db.Pokemons.ToList().Last().ID + 1;
            }
            else
            {
                id = 1;
            }
            return id;
        }
        // option 1 add pokemon DONE!!
        private static void option1(PokemonContext db, List<PokemonMaster> pokemonMasters)
        {
            var hp = 0;
            var experience = 0;
            var atk = 0;

            Console.Write("Enter Pokemon's name: ");
            var name = Console.ReadLine().ToLower();

            while (string.IsNullOrEmpty(name) | !pokemonMasters.Any(p => p.Name.ToLower() == $"{name}"))
            {
                Console.WriteLine("Name input is wrong, please try again.");
                name = Console.ReadLine().ToLower();
            }

            hp = checkInt(db, "Hp");

            experience = checkInt(db, "Exp");

            atk = checkInt(db, "Atk");
            
            var id = createId(db);

            switch (name)
            {
                case "pikachu":
                    var pikachu = new Pikachu(id, name, hp, atk, experience, 100, "Lightning Bolt", "Lightning", 1);
                    db.Add(pikachu);
                    break;

                case "charmander":
                    var charmander = new Charmander(id, name, hp, atk, experience, 100, "Solar Power", "Fire", 1);
                    db.Add(charmander);
                    break;

                case "eevee":
                    var eevee = new Eevee(id, name, hp, atk, experience, 100, "Run Away", "Normal", 1);
                    db.Add(eevee);
                    break;

                default: // add error handling
                    break;
            }
            db.SaveChanges();
            checkExp(db);

            Console.WriteLine($"\nPokemon: {name}, ID {id}, was added to your pokemon pocket.");
        }
        // option 2 delete pokemon DONE!!!
        private static void option2(PokemonContext db)
        {
            if (db.Pokemons.ToList().Count() != 0)
            {
                listAllPokemon(db);

                var id = checkID(db);
                var del_p = db.Pokemons
                    .Where(p => p.ID == id)
                    .First();
                db.Remove(del_p);
                db.SaveChanges();

                Console.WriteLine($"\nPokemon: {del_p.Name}, ID {id}, was removed from your pokemon pocket.");
            }
            else{
                Console.WriteLine("\nPokemon pocket is empty, please add a pokemon and try again");
            }

        }

        // option 3 list pokemon DONE!!
        private static void option3(PokemonContext db)
        {
            if (db.Pokemons.ToList().Count != 0)
            {
                checkExp(db);
                listAllPokemon(db);
            }
            else
            {
                Console.WriteLine("\nPokemon pocket is empty, please add a pokemon and try again");
            }
        }

        // option 4 DONE!!!
        private static void option4(PokemonContext db, List<PokemonMaster> pokemonMasters)
        {
            // var name = checkName(db);
            if (db.Pokemons.ToList().Count != 0)
            {
                checkEvolve(db, pokemonMasters);
            }
            else
            {
                Console.WriteLine("\nPokemon pocket is empty, please add a pokemon and try again");
            }
        }

        // option 5 evolve pokemon DONE!!!!!!
        private static void option5(PokemonContext db, List<PokemonMaster> pokemonMasters)
        {
            if (db.Pokemons.ToList().Count != 0)
            {
                var evolve_list = checkEvolve(db, pokemonMasters);
                if (evolve_list.Count != 0 )
                {
                    Console.Write("Enter Pokemon's name: ");
                    var name = Console.ReadLine().ToLower();
                    

                    while (string.IsNullOrEmpty(name) | !evolve_list.Any(p => p.ToLower() == $"{name}"))
                    {
                        Console.WriteLine("Pokemon cannot be found, please try again.");
                        name = Console.ReadLine().ToLower();
                    }

                    listSpecificPokemon(db, name);

                    var check_p = db.Pokemons.OrderBy(p => p.Hp).Where(p => p.Name == name).ToList();

                    var id = 0;
                    var NoToEvolve = pokemonMasters.Where(p => p.Name.ToLower() == name).First().NoToEvolve;
                    var atk_list = db.Pokemons.Where(p => p.Name.ToLower() == name).ToList();
                    var atk = 0;

                    foreach (var i in atk_list)
                    {
                        atk += i.Atk / NoToEvolve;
                    }

                    for (int i = 0; i < NoToEvolve; i++)
                    {

                        var del_id = checkID(db);
                        var del_p = db.Pokemons
                            .Where(p => p.ID == del_id && p.Name.ToLower() == name)
                            .First();
                        db.Remove(del_id);
                        db.SaveChanges();
                    }

                    id = createId(db);

                    var evolution = pokemonMasters.Where(p => p.Name.ToLower() == name).First().EvolveTo;

                    switch (name)
                    {
                        case "pikachu":
                            var pikachu = new Pikachu(id, evolution, 0, atk, 0, 100, "Static", "Lightning", 1);
                            pikachu.Skill = "Static";
                            db.Add(pikachu);
                            break;

                        case "charmander":
                            var charmander = new Charmander(id, evolution, 0, atk, 0, 100, "Blaze", "Fire", 1);
                            db.Add(charmander);
                            break;

                        case "eevee":
                            var eevee = new Eevee(id, evolution, 0, atk, 0, 100, "Flash Fire", "Fire", 1);
                            db.Add(eevee);
                            break;

                        default: // add error handling
                            break;
                    }

                    db.SaveChanges();
                    Console.WriteLine($"\nSuccessful evolution! Pokemon: {evolution}, ID {id}, was added to your pokemon pocket.");
                }
            }
            else
            {
                Console.WriteLine("\nPokemon pocket is empty, please add a pokemon and try again");
            }
        }
        private static void option6(PokemonContext db, List<PokemonMaster> pokemonMasters)
        {
            if (db.Pokemons.ToList().Count != 0)
            {
                var x = listPokemonBattle(db);
                if (x >= 2)
                {
                    Console.WriteLine("Select 2 pokemons to battle: ");

                    var p1_id = checkID(db);
                    var p2_id = checkID(db);

                    var p1 = db.Pokemons
                        .Where(p => p.ID == p1_id)
                        .First();

                    var p1_skill = db.Skills
                        .Where(c => c.ID.ToLower() == p1.Skill.ToLower())
                        .First();

                    var p2 = db.Pokemons
                        .Where(p => p.ID == p2_id)
                        .First();

                    var p2_skill = db.Skills
                        .Where(c => c.ID.ToLower() == p2.Skill.ToLower())
                        .First();

                    if (p1.Name.ToLower() == "eevee" | p2.Name.ToLower() == "eevee")
                    {
                        Console.WriteLine("\nUnable to battle eevee, eevee has ran away");
                    }
                    else if (p1.Hp > 0 && p2.Hp > 0)
                    {
                        Console.WriteLine($"\n{p1.Name}\n ID: {p1.ID}, Atk: {p1.Atk}, Hp: {p1.Hp}\n Skill: {p1.Skill} | Dmg: {p1_skill.Dmg}x");
                        Console.WriteLine($"\n{new string('=', 30)}\nFIGHT!\n{new string('=', 30)}");
                        Console.WriteLine($"\n{p2.Name}\n ID: {p2.ID}, Atk: {p2.Atk}, Hp: {p2.Hp}\n Skill: {p2.Skill} | Dmg: {p2_skill.Dmg}x");

                        var dmg_p1 = Convert.ToInt32(p2.Atk * p2_skill.Dmg);
                        var dmg_p2 = Convert.ToInt32(p1.Atk * p1_skill.Dmg);

                        p1.Experience += Convert.ToInt32(p1.Atk + dmg_p1);
                        p2.Experience += Convert.ToInt32(p2.Atk + dmg_p2);

                        p1.Hp = Convert.ToInt32(p1.Hp - dmg_p1);
                        p2.Hp = Convert.ToInt32(p2.Hp - dmg_p2);

                        db.SaveChanges();
                        checkExp(db);
                        
                        Console.WriteLine($"\n{new string('*', 30)}\nBattle results\n{new string('*', 30)}");
                        Console.WriteLine($"\n{p1.Name}\n ID: {p1.ID} took {dmg_p1} damage, {p1.Hp} Hp left, gained {p1.Atk + dmg_p1} experience.");
                        Console.WriteLine($"\n{p2.Name}\n ID: {p2.ID} took {dmg_p2} damage, {p2.Hp} Hp left, gained {p1.Atk + dmg_p1} experience.");
                    }
                    else
                    {
                        Console.WriteLine("\nPokemon has not enough health to battle.");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nPokemon pocket is empty, please add a pokemon and try again");
            }

        }
        //consume pokemon for health + exp + atk
        private static void option7(PokemonContext db)
        {   
            listAllPokemon(db);
            Console.WriteLine("Choose a pokemon to consume");
            var del_id = checkID(db);
        
            var del_pokemon = db.Pokemons.Where(p => p.ID == del_id).First();
            var del_p = db.Pokemons
                .Where(p => p.ID == del_id)
                .First();

            db.Remove(del_p);
            db.SaveChanges();

            listAllPokemon(db);
            Console.WriteLine("Choose a pokemon to add stats to");

            var id_add = checkID(db);

            var add_pokemon = db.Pokemons.Where(p => p.ID == id_add).First();

            add_pokemon.Atk += del_pokemon.Atk;
            add_pokemon.Hp += del_pokemon.Hp;
            add_pokemon.Experience += del_pokemon.Experience;
            db.SaveChanges();


        }
        public static void Main(string[] args)
        {
            var db = new PokemonContext();
            //PokemonMaster list for checking pokemon evolution availability.    
            List<PokemonMaster> pokemonMasters = new List<PokemonMaster>() {
                new PokemonMaster("Pikachu", 2, "Raichu"),
                new PokemonMaster("Eevee", 3, "Flareon"),
                new PokemonMaster("Charmander", 1, "Charmeleon")
            };

                // db.Add(new Skill("Lightning Bolt", "Shoots a lightning bolt", 1.1));
                // db.Add(new Skill("Solar Power", "Shoots a beam of sun", 1.15));
                // db.Add(new Skill("Run Away", "Ends battle", 0));
                // db.Add(new Skill("Static", "Creates a static field around the pokemon", 1.5));
                // db.Add(new Skill("Flash Fire", "Creates a field of fire around the pokemon", 1.55));
                // db.Add(new Skill("Blaze", "Engulfs opponent in fire", 1.6));

            while (true)
            {
                // add while loop
                Console.WriteLine($"\n{new string('*', 30)}\nWelcome to Pokemon Pocket App\n{new string('*', 30)}\n");
                Console.Write("(1). Add pokemon to my pocket\n(2). Delete pokemon in my pocket\n(3). List pokemon(s) in my pocket\n(4). Check if I can evolve pokemon\n(5). Evolve Pokemon\n(6). Battle\n(7). Consume pokemon\nPlease only enter [1, 2, 3, 4, 5, 6 or, 7]. Enter Q to quit: ");
                var user_input = Console.ReadLine();

                //user input
                switch (user_input.ToUpper())
                {
                    case "1":
                        // add pokemon
                        option1(db, pokemonMasters);
                        break;

                    case "2":
                        // delete pokemon
                        option2(db);
                        break;

                    case "3":
                        // list pokemon
                        option3(db);
                        break;

                    case "4":
                        // check evolution
                        option4(db, pokemonMasters);
                        break;

                    case "5":
                        // evolve pokemon
                        option5(db, pokemonMasters);
                        break;

                    case "6":
                        //  battle
                        option6(db, pokemonMasters);
                        break;

                    case "7":
                        //  eat pokemon
                        option7(db);
                        break;

                    case "Q":
                        // code block
                        Environment.Exit(0);
                        break;

                    default:
                        // code block
                        Console.WriteLine($"\n{new string('!', 30)}\nInvalid input please try again.\n{new string('!', 30)}\n");
                        break;
                }
            }
        }
    }
}
