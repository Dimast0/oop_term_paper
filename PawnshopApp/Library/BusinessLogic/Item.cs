using System;

namespace BusinessLogic
{
    public class Item
    {
        public int ID { get; }
        public string Title { get; set; }
        public Categories Category { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Loan { get; set; }
        public decimal ItemIncome { get; set; } = 0;
        public decimal Interest { get; } //Interest percentage per day
        public int ArrivalDay { get; set; }

        private Pawnshop Pawnshop;
        private Statuses _status;
        private decimal _loanwithinterest;
        private decimal _pricetosell;

        public Statuses Status
        {
            get
            {
                if (_status == Statuses.ToPay)
                {
                    if (LoanPeriod - (Date.Today() - ArrivalDay) < 0)
                    {
                        _status = Statuses.ToSell;
                    }
                }
                return _status;
            }
            set { _status = value; }
        }

        public decimal LoanWithInterest
        {
            get
            {
                int Days = Date.Today() - ArrivalDay;
                _loanwithinterest = Loan;
                for (int i = 0; i < Days - 10; i++)
                    _loanwithinterest += _loanwithinterest * Interest / 100;
                return decimal.Round(_loanwithinterest, 2, MidpointRounding.ToPositiveInfinity);
            }
        }

        public decimal PriceToSell
        {
            get
            {
                if (Status == Statuses.ToSell || Status == Statuses.Sold)
                {
                    if (_pricetosell == 0) return Price * (decimal)1.4;
                    else return _pricetosell;
                }
                else throw new Exception("The item is not for sale");
            }
            set { _pricetosell = value; }
        }

        public int LoanPeriod
        {
            get
            {
                int days = 10;
                decimal EstimatedLoan = Loan;
                while (EstimatedLoan <= Price)
                {
                    days += 1;
                    EstimatedLoan += EstimatedLoan * Interest / 100;
                }
                return days;
            }
        }

        public Item(Pawnshop Pawnshop, int ID, string Name, Categories Category, string Description, decimal Price, decimal Loan, int Date, decimal Interest)
        {
            if (Name == null || Description == null || Pawnshop == null)
                throw new NullReferenceException();
            else if (Price <= 0)
                throw new Exception("The price should be > $0");
            else if (Loan > Price * (decimal)0.7)
                throw new Exception("The initial loan cannot exceed maximum possible loan which is 70% of the product price.");
            else if (Loan < 0)
                throw new Exception("The loan should be > $0");

            Title = Name;
            ArrivalDay = Date;
            this.ID = ID;
            this.Category = Category;
            this.Description = Description;
            this.Pawnshop = Pawnshop;
            this.Price = Price;
            this.Loan = Loan;
            this.Interest = Interest;
            if (Loan == 0)
            {
                Status = Statuses.ToSell;
                Pawnshop.ChangeIncome(-Price);
            }
            else
            {
                Status = Statuses.ToPay;
                Pawnshop.ChangeIncome(-Loan);
            }
        }

        public void PayInFull()
        {
            if (Status == Statuses.ToPay)
            {
                ItemIncome += LoanWithInterest - Loan;
                Pawnshop.ChangeIncome(LoanWithInterest);
                Status = Statuses.PaidOff;
                Loan = 0;
            }
            else throw new Exception($"The loan cannot be paid off due to its status ({Status})");
        }

        public void PayInPart(decimal sum)
        {
            if (sum > LoanWithInterest) throw new Exception($"The sum > than the Loan. You can pay the loan in full.");
            else if (sum <= 0) throw new Exception($"Sum should be > 0");

            if (Status == Statuses.ToPay)
            {
                ItemIncome += LoanWithInterest - Loan;
                Loan = LoanWithInterest - sum;
                Pawnshop.ChangeIncome(sum);
                ArrivalDay = Date.Today();
            }
            else throw new Exception($"The loan cannot be paid off because the status is not 'ToPay'. (Current: {Status})");
        }

        public void Sell()
        {
            if (Status == Statuses.ToSell)
            {
                ItemIncome += PriceToSell - Price;
                Pawnshop.ChangeIncome(PriceToSell);
                Status = Statuses.Sold;
            }
            else throw new Exception($"The item cannot be sold because the status is not 'ToSell'. Current status: {Status}");
        }
    }

}
