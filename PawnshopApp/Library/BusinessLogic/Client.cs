using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BusinessLogic
{
    public class Client
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string AdditionalInfo { get; set; }
        public int ClientID { get; }

        public List<Item> Items = new List<Item>();
        private decimal _incomefromclient;
        private decimal _clientinterest;

        public decimal IncomeFromClient
        {
            get
            {
                _incomefromclient = 0;
                foreach (Item _item in Items)
                    _incomefromclient += _item.ItemIncome;
                return _incomefromclient;
            }
        }

        public decimal ClientInterest
        {
            get
            {
                if (IncomeFromClient <= 250)
                    _clientinterest = (decimal)1.5;
                else if (IncomeFromClient >= 5000)
                    _clientinterest = (decimal)0.5;
                else
                    _clientinterest = (decimal)1.5 - (IncomeFromClient / 5000);
                return _clientinterest;
            }
        }

        public Client(int ClientID, string Name, string Phone, string AdditionalInfo = "")
        {
            if (Name == null || Phone == null || AdditionalInfo == null)
                throw new NullReferenceException();
            else if (Name.Length <= 3)
                throw new Exception("The name is too short!");
            else if (Regex.IsMatch(Phone, @"^[0-9]{12}$") == false)
                throw new Exception("The number should be 12 numbers long!");

            this.ClientID = ClientID;
            this.Name = Name;
            this.Phone = Phone;
            this.AdditionalInfo = AdditionalInfo;
        }

        public void AddItem(Pawnshop Pawnshop, string Name, Categories Category, string Description, decimal Price, decimal Loan, int Date)
        {
            Items.Add(new Item(Pawnshop, Items.Count, Name, Category, Description, Price, Loan, Date, ClientInterest));
        }
    }
}
