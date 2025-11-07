using System;
using System.ComponentModel;

namespace ReadMore.Models
{
    public class AdminOrderView : INotifyPropertyChanged
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string BookTitles { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        private bool _isProcessed;

        public bool IsProcessed
        {
            get => _isProcessed;
            set
            {
                if (_isProcessed != value)
                {
                    _isProcessed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsProcessed)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
