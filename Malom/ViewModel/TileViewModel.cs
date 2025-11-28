using System.Windows.Input;
using Malom.Persistence;

namespace Malom.ViewModel
{
    public class TileViewModel : ViewModelBase
    {
        //Fields 
        private readonly int _index;
        private Player _occupier;

        //Properties
        public int Index
        {
            get { return _index; }
        }

        public string Occupier
        {
            get { return _occupier switch { Player.Red => "Red", Player.Blue => "Blue", _ => "Transparent" }; }
            set
            {
                _occupier = value == "Red" ? Player.Red :
                            value == "Blue" ? Player.Blue : Player.Empty;
                OnPropertyChanged();
            }
        }

        public ICommand TileClickCommand { get; }

        //Constructor
        public TileViewModel(int index, Player occupier, Action<TileViewModel> onClick, Predicate<TileViewModel> isClickable)
        {
            _index = index;
            _occupier = occupier;
            TileClickCommand = new DelegateCommand(_ => onClick(this), _ => isClickable(this));
        }


    }
}
