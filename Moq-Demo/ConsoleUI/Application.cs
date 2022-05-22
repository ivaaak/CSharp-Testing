using DemoLibrary.Logic;
using DemoLibrary.Models;
using System;
using System.Collections.Generic;

namespace ConsoleUI
{
    public class Application : IApplication
    {
        IPersonProcessor _personProcessor;

        public Application(IPersonProcessor personProcessor)
        {
            _personProcessor = personProcessor;
        }

        public void Run()
        {
            IdentifyNextStep();
        }

        private void IdentifyNextStep()
        {
            string selectedAction = "";

            do
            {
                selectedAction = GetActionChoice();
                
                Console.WriteLine();

                switch (selectedAction)
                {
                    case "1":
                        DisplayPeople(_personProcessor.LoadPeople());
                        break;
                    case "2":
                        AddPerson();
                        break;
                    case "3":
                        Console.WriteLine("Ending.");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Hit enter to retry.");
                        break;
                }

                Console.WriteLine("Hit return to continue...");
                Console.ReadLine();

            } while (selectedAction != "3");            
        }

        private void AddPerson()
        {
            Console.Write("What is the person's first name: ");
            string firstName = Console.ReadLine();
            Console.Write("What is the person's last name: ");
            string lastName = Console.ReadLine();
            Console.Write("What is the person's height: ");
            string height = Console.ReadLine();

            var person = _personProcessor.CreatePerson(firstName, lastName, height);
            _personProcessor.SavePerson(person);
        }

        private void DisplayPeople(List<PersonModel> people)
        {
            foreach (var p in people)
            {
                Console.WriteLine(p.FullName);
            }
        }

        private string GetActionChoice()
        {
            string output = "";

            Console.Clear();
            Console.WriteLine("Menu Options".ToUpper());
            Console.WriteLine("1 - Load People");
            Console.WriteLine("2 - Create and Save Person");
            Console.WriteLine("3 - Exit");
            Console.Write("Press 1-3 to continue: ");
            output = Console.ReadLine();

            return output;
        }
    }
}
