using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.Model;

namespace Test.Method
{
    class Helpers
    {
        public List<Entity> LoadDataFromJson()
        {

            var serializer = new JsonSerializer();
            List<Entity> entities = new List<Entity>();
            var path = System.IO.Directory.GetCurrentDirectory() + "\\board.json";

            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                jsonReader.SupportMultipleContent = true;

                while (jsonReader.Read())
                {
                    entities = serializer.Deserialize<List<Entity>>(jsonReader);

                }
            }

            return entities;

        }

        public void SaveDataToJson(List<Entity> entities)
        {
            var serializer = new JsonSerializer();
            var path = System.IO.Directory.GetCurrentDirectory() + "\\board.json";

            using (var writer = new StreamWriter(path))
            using(var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, entities);
            }
        }

        public void SaveAttackResultToEntityList(List<Entity> entities, Entity sourceEntity, Entity targetEntity)
        {
            
            entities[entities.FindIndex(ind => ind.EntityId == sourceEntity.EntityId)] = sourceEntity;

            entities[entities.FindIndex(ind => ind.EntityId == targetEntity.EntityId)] = targetEntity;

        }

        public void DrawBoard(List<Entity> entities)
        {

            Console.WriteLine("Broj entiteta na ploči: " + entities.Count + "\n");
            int i = 0;

            //Ispiši podatke svakog entiteta
            foreach(Entity entity in entities)
            {
                i++;

                Console.WriteLine("Entitet broj " + i + " (" + entity.EntityId + ")");

                
                if (entity.EntityType == 1)
                    Console.WriteLine("\t Tip: Creature");
                else
                    Console.WriteLine("\t Tip: Avatar");


                Console.WriteLine("\t Zdravlje: " + entity.Health);
                Console.WriteLine("\t Napad: " + entity.Attack);


                if (entity.AttackReady)
                    Console.WriteLine("\t Spreman za napad: DA");
                else
                    Console.WriteLine("\t Spreman za napad: NE");

                
                foreach(Modifiers modifier in entity.Modifiers)
                {
                    if (modifier.ModifierType == 1)
                        Console.WriteLine("\t Armor: " + modifier.Value);
                    else
                        Console.WriteLine("\t Vulnerability: " + modifier.Value);
                }


                Console.WriteLine("\n");
            }
        }


        public Entity SelectSource(List<Entity> entities)
        {
            string key;
            Entity sourceEntity = new Entity();

            while (true)
            {
                Console.WriteLine("Odaberi napadača (upiši id):");

                //Upis korisnika
                key = Console.ReadLine();

                //Traži entitet prema id-u koji je korisnik upisao
                sourceEntity = entities.SingleOrDefault(o => o.EntityId == key.ToLower());

                //Id ne postoji
                if (sourceEntity == null)
                {
                    Console.WriteLine("Krivi id, probaj ponovo");
                }
                //Avatar nije spreman za napad
                else if (sourceEntity.EntityType == 2 && !sourceEntity.AttackReady)
                    Console.WriteLine("Odabrani avatar nije spreman za napad, izaberi drugog.");                
                else
                    break;
            }           

            return sourceEntity;


        }


        public Entity SelectTarget(List<Entity> entities, Entity sourceEntity)
        {
            string key;
            Entity targetEntity = new Entity();

            while (true)
            {
                Console.WriteLine("Odaberi metu (upiši id):");

                //Upis korisnika
                key = Console.ReadLine();

                //Traži entitet prema id-u koji je korisnik upisao
                targetEntity = entities.SingleOrDefault(o => o.EntityId == key.ToLower());

                //Krivi id
                if (targetEntity == null)
                    Console.WriteLine("Krivi id, probaj ponovo");
                //Creature napada avatara, nije dozvoljeno
                else if (targetEntity.EntityType == 2 && sourceEntity.EntityType == 1)
                    Console.WriteLine("Creature ne može napasti avatara, unesi drugu metu");
                //Napada samog sebe
                else if (targetEntity.EntityId == sourceEntity.EntityId)
                    Console.WriteLine("Ne moze napasti samog sebe");
                else
                    break;
            }
            return targetEntity;
        }


        public bool IsRetaliation(Entity sourceEntity, Entity targetEntity)
        {
            //Avatar na avatara, nema retaliationa
            if (sourceEntity.EntityType == 2 && targetEntity.EntityType == 2)
                return false;
            //Avatar na creature, ima retaliation
            else if (sourceEntity.EntityType == 2 && targetEntity.EntityType == 1)
                return true;

            //Creature na avatar ne prolazi kod SelectTargeta, ostaje jedino creature na creature što je true

            return true;
        }

        public int CheckModifiers(List<Modifiers> modifiers)
        {
            int result = 0;

            //Ako je armor (modifier type 1) oduzmi od ukupnog napada, ako je vulnerability (type 2) dodaj napadu
            foreach(Modifiers modifier in modifiers)
            {
                if (modifier.ModifierType == 1)
                    result -= modifier.Value;
                else
                    result += modifier.Value;

            }
            return result;
        }

        public int Attack(Entity sourceEntity, Entity targetEntity)
        {
            //Provjeri modifikatore za napad
            int attackModifier = CheckModifiers(targetEntity.Modifiers);


            //Oduzmi healthe meti
            targetEntity.Health -= sourceEntity.Attack + attackModifier;

            //Provjeri ima li retaliationa
            if(IsRetaliation(sourceEntity, targetEntity))
            {
                //Provjeri modifikatore
                int retaliationAttackModifier = CheckModifiers(sourceEntity.Modifiers);

                //Retaliation
                sourceEntity.Health -= targetEntity.Attack + retaliationAttackModifier;
            }
               

            //Stavi attackReady na false
            sourceEntity.AttackReady = false;

            //Vrati ukupnu vrijednost napada
            return sourceEntity.Attack + attackModifier;


        }


        public void EntityAttackedMessage(string sourceEntityId, string targetEntityId, int value)
        {

            Console.WriteLine("Id napadača: " + sourceEntityId);
            Console.WriteLine("Id mete: " + targetEntityId);
            Console.WriteLine("Vrijednost napada: " + value);
        }


        public void Round()
        {
            //Učitaj entitete iz JSON-a u objekte
            List<Entity> entities = LoadDataFromJson();

            //Ispiši trenutno stanje boarda
            DrawBoard(entities);

            //Odaberi napadača
            Entity sourceEntity = SelectSource(entities);

            //Odaberi metu
            Entity targetEntity = SelectTarget(entities, sourceEntity);

            //Napad
            int attackValue = Attack(sourceEntity, targetEntity);

            //Ispis
            EntityAttackedMessage(sourceEntity.EntityId, targetEntity.EntityId, attackValue);

            //Spremi promjenjene objekte u listu
            SaveAttackResultToEntityList(entities, sourceEntity, targetEntity);

            //Spremi promjene u JSON
            SaveDataToJson(entities);



            Thread.Sleep(3000);
        }

    }

}
