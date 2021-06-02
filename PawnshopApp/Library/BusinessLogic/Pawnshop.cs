using System;
using System.Collections.Generic;

namespace BusinessLogic
{
    public class Pawnshop
    {
        private decimal _income = 0;
        private decimal _moneypool;
        public List<Client> Clients = new List<Client>();

        public Pawnshop(decimal MoneyPool)
        {
            this._moneypool = MoneyPool;
        }

        public void AddClient(string Name, string Phone, string AdditionalInfo = "-")
        {
            Clients.Add(new Client(Clients.Count, Name, Phone, AdditionalInfo));
        }
        public decimal GetCurrentLiquidity()
        {
            return _income + _moneypool;
        }
        public decimal GetIncome()
        {
            return _income;
        }
        public void ChangeIncome(decimal amount)
        {
            if (_income + amount + _moneypool < 0)
                throw new Exception("Not enough money on the money pool to proceed the operation");
            _income += amount;
        }
    }
}
