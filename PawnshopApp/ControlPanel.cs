using System;
using BusinessLogic;

namespace PawnshopApp
{
    public static class ControlPanel
    {
        public static void Actions(Pawnshop Pawnshop)
        {
            while (true)
            {
                Console.WriteLine("------------Welcome to the Control Panel------------\n");
                Console.WriteLine($"Current date: day {Date.Today()}");
                Console.WriteLine($"Total Clients: {Pawnshop.Clients.Count}");
                Console.WriteLine($"Current Liquidity: ${Pawnshop.GetCurrentLiquidity()}");
                Console.WriteLine($"Total Income : ${Pawnshop.GetIncome()}");
                Console.WriteLine($"\n-----------------------------------\n");
                Console.WriteLine("Set your Action:\n\n1 - Select a client | 2 - New client | 3 - List clients\n");
                int Action = 0;
                while (Action < 1 || Action > 3)
                {
                    try
                    {
                        Console.WriteLine("Enter a number from 1 to 3:");
                        Action = Convert.ToInt32(Console.ReadLine());
                    }
                    catch { }
                }
                Console.Clear();
                try
                {
                    switch (Action)
                    {
                        case 1:
                            ClientActions.OpenClientPage(Pawnshop);
                            break;
                        case 2:
                            NewClient(Pawnshop);
                            break;
                        case 3:
                            ListClients(Pawnshop);
                            break;
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
                PressToContinue();
            }
        }

        public static void ListClients(Pawnshop Pawnshop)
        {
            Console.WriteLine($"Day: {Date.Today()} | Liquidity: ${Pawnshop.GetCurrentLiquidity()} | Income: ${Pawnshop.GetIncome()}");
            Console.WriteLine("\nID| Name | Phone\n");
            foreach (Client _client in Pawnshop.Clients)
                Console.WriteLine($"{_client.ClientID} | {_client.Name} | +{_client.Phone}");
        }

        public static void NewClient(Pawnshop Pawnshop)
        {
            Console.WriteLine("Client Name:");
            string Name = Console.ReadLine();
            if (Name.Length <= 3) throw new Exception("The name is too short!");
            Console.Write("Enter phone number: +");
            string Phone = Convert.ToString(Convert.ToInt64(Console.ReadLine()));
            if (Phone.Length != 12) throw new Exception("The number should be 12 numbers long!");
            Console.WriteLine("Enter additional info:");
            string AdditionalInfo = Console.ReadLine();
            Pawnshop.AddClient(Name, Phone, AdditionalInfo);
            Console.WriteLine($"\n{Name} was successfully added!");
        }

        public static void PressToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        public static bool ConfirmOperation()
        {
            Console.Write("\nConfirm? (y/n): ");
            while (true)
            {
                string answer = Console.ReadLine();
                if (answer == "y" || answer == "Y")
                    return true;
                else if (answer == "n" || answer == "N")
                    return false;
                else
                    Console.Write("The answer should be 'y' or 'n'!");
            }
        }

        public static void AddExampleData(Pawnshop Pawnshop)
        {
            Pawnshop.AddClient("Vasya Ivanov", "380974726472", "Drives S-Class");
            Pawnshop.AddClient("Petia Ivanov", "380984645375", "Wears a Red Hat");
            Pawnshop.AddClient("Zuzia Ivanov", "380994726847", "Be careful! Can be angry");
            Pawnshop.Clients[1].AddItem(Pawnshop, "iPhone 12", Categories.Phones, "512GB. Small scratches", 700, 400, Date.Today());
            Pawnshop.Clients[2].AddItem(Pawnshop, "iPhone 13 Pro Max", Categories.Phones, "1024GB. Does it exist?", 900, 500, Date.Today());
            Pawnshop.Clients[0].AddItem(Pawnshop, "iPhone 8 Plus", Categories.Phones, "64GB. Ideal condition", 400, 200, Date.Today());
        }
    }
}
