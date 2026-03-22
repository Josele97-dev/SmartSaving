using CommunityToolkit.Mvvm.Input;
using SmartSaving.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmartSaving.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        private readonly TransactionService _transactionService;
        private readonly int _userId;
        private Transaction _existingTransaction;

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        private TransactionType _type;
        public TransactionType Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        private string _category;
        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        private int _categoryId;
        public int CategoryId
        {
            get => _categoryId;
            set { _categoryId = value; OnPropertyChanged(); }
        }

        public List<TransactionType> Types { get; } = new List<TransactionType>
        {
            TransactionType.Income,
            TransactionType.Expense
        };

        public bool IsEditing => _existingTransaction != null;
        public string Title => IsEditing ? "Editar Movimiento" : "Nuevo Movimiento";

        public ICommand SaveCommand { get; }

        // Constructor para CREAR
        public TransactionViewModel(TransactionService transactionService, int userId)
        {
            _transactionService = transactionService;
            _userId = userId;
            SaveCommand = new AsyncRelayCommand(SaveAsync);
        }

        // Constructor para EDITAR
        public TransactionViewModel(TransactionService transactionService, int userId, Transaction transaction)
        {
            _transactionService = transactionService;
            _userId = userId;
            _existingTransaction = transaction;

            Description = transaction.Description;
            Amount = transaction.Amount;
            Date = transaction.Date;
            Type = transaction.Type;
            CategoryId = transaction.CategoryId;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
        }

        private async Task SaveAsync()
        {
            try
            {
                if (IsEditing)
                {
                    _existingTransaction.Description = Description;
                    _existingTransaction.Amount = Amount;
                    _existingTransaction.Date = Date;
                    _existingTransaction.Type = Type;
                    _existingTransaction.CategoryId = CategoryId;
                    await _transactionService.UpdateTransactionAsync(_existingTransaction);
                    MessageBox.Show("Movimiento actualizado correctamente");
                }
                else
                {
                    var transaction = new Transaction
                    {
                        UserId = _userId,
                        Description = Description,
                        Amount = Amount,
                        Date = Date,
                        Type = Type,
                        CategoryId = CategoryId
                    };
                    await _transactionService.AddTransactionAsync(transaction);
                    MessageBox.Show("Movimiento guardado correctamente");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception)
            {
                MessageBox.Show("Ha ocurrido un error inesperado");
            }
        }
    }
}