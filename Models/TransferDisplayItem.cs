using SmartSaving.Models;
using System;

namespace SmartSaving.Models
{
    public  class TransferDisplayItem
    {
        public string Date { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public static TransferDisplayItem FromTransaction(Transaction t, bool isSent)
        {
            var description = t.Description ?? string.Empty;
            string name = string.Empty;
            string note = string.Empty;

            
            var sentSeparator = " \u2192 "; 
            var receivedSeparator = " \u2190 "; 

            var separator = isSent ? sentSeparator : receivedSeparator;
            var separatorIndex = description.IndexOf(separator, System.StringComparison.Ordinal);

            if (separatorIndex >= 0)
            {
                note = description.Substring(0, separatorIndex).Trim();
                name = description.Substring(separatorIndex + separator.Length).Trim();
            }
            else
            {
                var prefix = isSent ? "Transfer to " : "Transfer from ";
                if (description.StartsWith(prefix))
                    name = description.Substring(prefix.Length).Trim();
                else
                    note = description;
            }

            return new TransferDisplayItem
            {
                Date = t.Date.ToString("dd/MM/yyyy"),
                PersonName = string.IsNullOrEmpty(name) ? "Unknown" : name,
                Note = note,
                Amount = t.Amount
            };
        }

    }
}
