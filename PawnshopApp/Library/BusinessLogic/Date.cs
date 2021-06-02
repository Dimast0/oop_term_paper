using System;

namespace BusinessLogic
{
    public static class Date
    {
        private static int _day = 0;

        public static int Today()
        {
            return _day;
        }
        public static void Increment(int Days)
        {
            if (Days > 0)
                _day += Days;
            else
                throw new Exception("You cannot travel back in time. Use a positive int number");
        }
    }
}
