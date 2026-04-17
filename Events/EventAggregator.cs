using System;

namespace SmartSaving.Events
{
    public static class EventAggregator
    {
        public static event Action? TransactionChanged;
        public static event Action? CategoryChanged;
        public static event Action? TransactionWindowClosed;

        public static void PublishTransactionChanged()
        {
            TransactionChanged?.Invoke();
        }

        public static void PublishCategoryChanged()
        {
            CategoryChanged?.Invoke();
        }

        public static void PublishTransactionWindowClosed()
        {
            TransactionWindowClosed?.Invoke();
        }
    }
}