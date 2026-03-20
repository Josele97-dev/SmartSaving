using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartSaving.ViewModels
{
    public List<string> TransactionTypes { get; set; }

    public string SelectedTransactionType { get; set; }

    public ICommand SaveTransactionCommand { get; }


}