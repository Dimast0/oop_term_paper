using System;
using BusinessLogic;

namespace PawnshopApp
{
    public static class ClientActions
    {
        public static void OpenClientPage(Pawnshop Pawnshop)
        {
            ControlPanel.ListClients(Pawnshop);
            Console.WriteLine("\nEnter a ClientID to work with:");
            int UserID = Convert.ToInt32(Console.ReadLine());
            if (UserID > Pawnshop.Clients.Count)
                throw new IndexOutOfRangeException("Client ID is out-of-range. Try another ID.");
            UserActions(UserID);

            void UserActions(int UserID)
            {
                Console.Clear();
                Console.WriteLine($"Day: {Date.Today()} | Liquidity: ${Pawnshop.GetCurrentLiquidity()} | Income: ${Pawnshop.GetIncome()}");
                Console.WriteLine($"\n------------CLIENT PAGE------------\n");
                Console.WriteLine($"Name      : {Pawnshop.Clients[UserID].Name}");
                Console.WriteLine($"Phone     : +{Pawnshop.Clients[UserID].Phone}");
                Console.WriteLine($"Additional: {Pawnshop.Clients[UserID].AdditionalInfo}");
                Console.WriteLine($"Interest  : {Pawnshop.Clients[UserID].ClientInterest}% per day");
                Console.WriteLine($"We earned : ${Pawnshop.Clients[UserID].IncomeFromClient}\n");
                Console.WriteLine($"Short Items list:");
                foreach (Item _item in Pawnshop.Clients[UserID].Items)
                {
                    if (_item.Status == Statuses.ToPay)
                        Console.WriteLine($"ID: {_item.ID} | {_item.Status} | {_item.Title} ({_item.LoanPeriod - (Date.Today() - _item.ArrivalDay)} days left)");
                    else
                        Console.WriteLine($"ID: {_item.ID} | {_item.Status} | {_item.Title}");
                }
                Console.WriteLine($"\n-----------------------------------\n");
                Console.WriteLine("Set your Action:\n\n1 - New item  | 3 - List Items   | 5 - Pay in full | 7 - Go to the future \n2 - Sell item | 4 - Add TestItem | 6 - Pay in part | 8 - Return\n");
                int Action = 0;
                while (Action < 1 || Action > 8)
                {
                    try
                    {
                        Console.WriteLine("Enter a number from 1 to 7:");
                        Action = Convert.ToInt32(Console.ReadLine());
                    }
                    catch { }
                }
                Console.Clear();
                Client CurrentClient = Pawnshop.Clients[UserID];
                switch (Action)
                {
                    case 1:
                        AddItem(Pawnshop, CurrentClient);
                        ControlPanel.PressToContinue();
                        break;
                    case 2:
                        SellItem(CurrentClient);
                        ControlPanel.PressToContinue();
                        break;
                    case 3:
                        ListItems(CurrentClient);
                        ControlPanel.PressToContinue();
                        break;
                    case 4:
                        AddExampleItem(Pawnshop, CurrentClient);
                        break;
                    case 5:
                        PayInFull(CurrentClient);
                        ControlPanel.PressToContinue();
                        break;
                    case 6:
                        PayInPart(CurrentClient);
                        ControlPanel.PressToContinue();
                        break;
                    case 7:
                        Date.Increment(10);
                        break;
                    case 8:
                        ControlPanel.Actions(Pawnshop);
                        break;
                }
                UserActions(UserID);
            }
        }

        private static void AddItem(Pawnshop Pawnshop, Client Client)
        {
            try
            {
                (string Name, Categories Category, string Description, decimal Price, decimal Loan) = ObtainProductInfo();
                if (ControlPanel.ConfirmOperation())
                {
                    Client.AddItem(Pawnshop, Name, Category, Description, Price, Loan, Date.Today());
                    Console.WriteLine("---The item is successfully added---");
                }
                else Console.WriteLine("---You canceled the operation---");
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static (string, Categories, string, decimal, decimal) ObtainProductInfo()
        {
            int CategoriesCount = Enum.GetNames(typeof(Categories)).Length - 1;
            void Header(int a = 0)
            {
                string AlignCenter(string Source, int Width)
                {
                    return Source.PadLeft((Width - Source.Length) / 2 + Source.Length).PadRight(Width);
                }
                Console.WriteLine("\nSet the product category and press Enter:\n(Use Right/Left arrows)\n\n                ||\n                \\/");
                for (int i = a; i <= CategoriesCount; i++)
                    Console.Write(AlignCenter($"{(Categories)i} ", 12));
                for (int i = 0; i < a; i++)
                    Console.Write(AlignCenter($"{(Categories)i} ", 12));
                Console.WriteLine("\n                /\\\n                ||\n");
            }
            Categories SetCategory(int Index = 0)
            {
                while (true)
                {
                    Console.Clear();
                    Header(Index);
                    int ResIndex = Index + 1;
                    if (ResIndex > CategoriesCount) ResIndex = 0;
                    ConsoleKeyInfo Pressed = Console.ReadKey(true);
                    if (Pressed.Key == ConsoleKey.Enter)
                        return (Categories)ResIndex;
                    else if (Pressed.Key == ConsoleKey.RightArrow)
                        Index += 1;
                    else if (Pressed.Key == ConsoleKey.LeftArrow)
                        Index -= 1;
                    if (Index > CategoriesCount) Index = 0;
                    if (Index < 0) Index = CategoriesCount;
                }
            }
            Categories Category = SetCategory();
            Console.WriteLine($"Category: {Category}");
            Console.WriteLine("Enter product name:");
            string Name = Console.ReadLine();
            Console.WriteLine("Enter Product Description:");
            string Description = Console.ReadLine();
            Console.WriteLine("Enter Product Price:");
            decimal Price = Convert.ToDecimal(Console.ReadLine());
            if (Price <= 0) throw new Exception("The price should be > $0");
            Console.WriteLine($"Maximum possible loan: {decimal.Round(Price * (decimal)0.7, 0, MidpointRounding.ToNegativeInfinity)}");
            Console.WriteLine("Enter Client's Loan:");
            decimal Loan = Convert.ToDecimal(Console.ReadLine());
            if (Loan > Price * (decimal)0.7) throw new Exception("The initial loan cannot exceed maximum possible loan which is 70% of the product price; Loan should be > $0");
            else if (Loan < 0) throw new Exception("The loan should be > $0");
            return (Name, Category, Description, Price, Loan);
        }

        private static void SellItem(Client Client)
        {
            Console.WriteLine("Available items for sale:");
            foreach (Item _item in Client.Items)
            {
                if (_item.Status == Statuses.ToSell)
                    Console.WriteLine($"ID: {_item.ID} | {_item.Title} | Price to sell: {_item.PriceToSell}");
            }
            try
            {
                Console.WriteLine("\nEnter the ID of the item to be sold:");
                int ID = Convert.ToInt32(Console.ReadLine());
                PrintItemInfo(Client.Items[ID]);
                if (ControlPanel.ConfirmOperation())
                {
                    Client.Items[ID].Sell();
                    Console.WriteLine("---Item is successfully SOLD---");
                }
                else Console.WriteLine("---You canceled the operation---");
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static void PayInFull(Client Client)
        {
            try
            {
                foreach (Item _item in Client.Items)
                {
                    if (_item.Status == Statuses.ToPay)
                        Console.WriteLine($"ID: {_item.ID} | {_item.Title} | To pay: ${_item.LoanWithInterest}");
                }
                Console.WriteLine("\nEnter the Item ID:");
                int ID = Convert.ToInt32(Console.ReadLine());
                PrintItemInfo(Client.Items[ID]);
                Console.WriteLine($"Amount to pay: ${Client.Items[ID].LoanWithInterest}");
                if (ControlPanel.ConfirmOperation())
                {
                    Client.Items[ID].PayInFull();
                    Console.WriteLine("---Item is successfully paid in full---");
                }
                else Console.WriteLine("---You canceled the operation---");
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static void PayInPart(Client Client)
        {
            try
            {
                foreach (Item _item in Client.Items)
                {
                    if (_item.Status == Statuses.ToPay)
                        Console.WriteLine($"ID: {_item.ID} | {_item.Title} | Loan to pay: {_item.LoanWithInterest}");
                }
                Console.WriteLine("\nEnter the Item ID:");
                int ID = Convert.ToInt32(Console.ReadLine());
                PrintItemInfo(Client.Items[ID]);
                Console.WriteLine($"Amount to pay: ${Client.Items[ID].LoanWithInterest}");
                Console.WriteLine("How much to pay?");
                decimal amount = Convert.ToDecimal(Console.ReadLine());
                if (ControlPanel.ConfirmOperation())
                {
                    Client.Items[ID].PayInPart(amount);
                    Console.WriteLine($"---${amount} are successfully paid---");
                    PrintItemInfo(Client.Items[ID]);
                }
                else Console.WriteLine("---You canceled the operation---");
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
        private static void ListItems(Client Client)
        {
            foreach (Item _item in Client.Items)
                PrintItemInfo(_item);
        }

        private static void PrintItemInfo(Item CurrentItem)
        {
            Console.WriteLine($"\n------------{CurrentItem.Title}------------");
            Console.WriteLine($"Item ID    : {CurrentItem.ID}");
            Console.WriteLine($"Category   : {CurrentItem.Category}");
            Console.WriteLine($"Description: {CurrentItem.Description}\n");
            Console.WriteLine($"Arrived at : day {CurrentItem.ArrivalDay}");
            Console.WriteLine($"Status     : {CurrentItem.Status}");

            switch (CurrentItem.Status)
            {
                case Statuses.ToPay:
                    Console.WriteLine($"TakenLoan  : ${CurrentItem.Loan}");
                    Console.WriteLine($"Price      : ${CurrentItem.Price}");
                    Console.WriteLine($"Percentage : {CurrentItem.Interest}%");
                    Console.WriteLine($"Loan Period: {CurrentItem.LoanPeriod} days.\n");
                    Console.WriteLine($"Time left  : {CurrentItem.LoanPeriod - (Date.Today() - CurrentItem.ArrivalDay)} days.");
                    Console.WriteLine($"Loan to pay: ${CurrentItem.LoanWithInterest}\n------------------------------------\n");
                    break;
                case Statuses.ToSell:
                    Console.WriteLine($"PriceToSell: ${CurrentItem.PriceToSell}\n-----\n");
                    break;
                case Statuses.Sold:
                    Console.WriteLine($"Bought for : ${CurrentItem.Price}");
                    Console.WriteLine($"Sold for   : ${CurrentItem.PriceToSell}");
                    Console.WriteLine($"Our income : ${CurrentItem.ItemIncome}\n-----\n");
                    break;
                case Statuses.PaidOff:
                    Console.WriteLine($"Item price : ${CurrentItem.Price}");
                    Console.WriteLine($"Our income : ${CurrentItem.ItemIncome}\n-----\n");
                    break;
            }
        }              

        private static void AddExampleItem(Pawnshop Pawnshop, Client Client)
        {
            switch (new Random().Next(0, 5))
            {
                case 0:
                    Client.AddItem(Pawnshop, "MacBook Air 13 2013", Categories.Computers, "i7. 256GB. Good condition", 500, 300, Date.Today());
                    break;
                case 1:
                    Client.AddItem(Pawnshop, "Apple TV", Categories.TV, "Good condition", 250, 100, Date.Today());
                    break;
                case 2:
                    Client.AddItem(Pawnshop, "HP PC", Categories.Computers, "i7. RTX300. Good condition", 700, 400, Date.Today());
                    break;
                case 3:
                    Client.AddItem(Pawnshop, "Mac Mini 2010", Categories.Computers, "i5. 128GB. Bad condition", 220, 100, Date.Today());
                    break;
                case 4:
                    Client.AddItem(Pawnshop, "Iphone 12 Pro", Categories.Phones, "Good condition", 700, 450, Date.Today());
                    break;
                case 5:
                    Client.AddItem(Pawnshop, "Iphone Xs", Categories.Phones, "Bad condition", 380, 200, Date.Today());
                    break;
            }
        }
    }
}
