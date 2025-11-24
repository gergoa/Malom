using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MalomWPF.ViewModel
{
    internal class TileViewModel : ViewModelBase
    {
        //Fields 
        private readonly int _index;
        private string _occupier;

        //Properties
        public int Index
        {
            get { return _index; }
        }

        public string Occupier
        {
            get { return _occupier; }
            set
            {
                if (_occupier != value)
                {
                    _occupier = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand TileClickCommand { get; }

        //Constructor
        public TileViewModel(int index, string occupier, Action<TileViewModel> onClick, Predicate<object?> isClickable)
        {
            _index = index;
            _occupier = occupier;
            TileClickCommand = new DelegateCommand(_ => onClick(this), isClickable);
        }


    }
}
 