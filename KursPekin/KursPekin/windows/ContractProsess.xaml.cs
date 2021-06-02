using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KursPekin.windows
{
  
    /// <summary>
    /// Логика взаимодействия для ContractProsess.xaml
    /// </summary>
    public partial class ContractProsess : Window, INotifyPropertyChanged
    {
        public ContractProsess()
        {
            InitializeComponent();
            this.DataContext = this;
            ContracList2 = Core.DB.Contract.ToList();
        }
        private List<Contract> _ContractList2;

        public List<Contract> ContracList2
        {
            get
            {

                var FilteredServiceList = _ContractList2.FindAll(item =>
                    item.DiscountFloat >= CurrentDiscountFilter.Item1 &&
                      item.DiscountFloat < CurrentDiscountFilter.Item2
                      );

                if (SearchFilter != "")
                    FilteredServiceList = FilteredServiceList.Where(item =>
                        item.Userman.FullName.IndexOf(SearchFilter, StringComparison.OrdinalIgnoreCase) != -1).ToList();


                if (SortPriceAscending)
                {

                    return FilteredServiceList.OrderBy(item => (item.Summa))
                .ToList();

                }
                else
                {

                    return FilteredServiceList.OrderByDescending(item => (item.Summa))
                .ToList();
                }
            }

            set
            {
                _ContractList2 = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ContracList2"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ServicesCount"));
                    PropertyChanged(this, new PropertyChangedEventArgs("FilteredServicesCount"));
                }
            }
        }
        private Boolean _SortPriceAscending = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public Boolean SortPriceAscending
        {
            get { return _SortPriceAscending; }
            set
            {
                _SortPriceAscending = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ContracList2"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ServicesCount"));
                    PropertyChanged(this, new PropertyChangedEventArgs("FilteredServicesCount"));
                }
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SortPriceAscending = (sender as RadioButton).Tag.ToString() == "1";
        }
        public int ServicesCount
        {
            get
            {
                return _ContractList2.Count;
            }

        }
        public int FilteredServicesCount
        {
            get
            {
                return ContracList2.Count;
            }
        }
        public List<string> FilterByDiscountNamesList
        {
            get
            {
                return FilterByDiscountValuesList
                    .Select(item => item.Item1)
                    .ToList();
            }




        }
        private Tuple<double, double> _CurrentDiscountFilter = Tuple.Create(double.MinValue, double.MaxValue);

        public Tuple<double, double> CurrentDiscountFilter
        {
            get
            {
                return _CurrentDiscountFilter;
            }
            set
            {
                _CurrentDiscountFilter = value;
                if (PropertyChanged != null)
                {
                    // при изменении фильтра список перерисовывается
                    PropertyChanged(this, new PropertyChangedEventArgs("ContracList2"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ServicesCount"));
                    PropertyChanged(this, new PropertyChangedEventArgs("FilteredServicesCount"));
                }
            }
        }

        private void DiscountFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DiscountFilterComboBox.SelectedIndex >= 0)
                CurrentDiscountFilter = Tuple.Create(
                    FilterByDiscountValuesList[DiscountFilterComboBox.SelectedIndex].Item2,
                    FilterByDiscountValuesList[DiscountFilterComboBox.SelectedIndex].Item3

                );
        }

        private List<Tuple<string, double, double>> FilterByDiscountValuesList =
         new List<Tuple<string, double, double>>() {
        Tuple.Create("Все записи", 0d, 1000000d),
        Tuple.Create("от 10000 до 50000", 10000d, 30000d),
         Tuple.Create("от 60000 до 100000", 30000d, 60000d),
        Tuple.Create("от 60000 до 100000", 60000d, 100000d),

    };
        private string _SearchFilter = "";
        public string SearchFilter
        {
            get { return _SearchFilter; }
            set
            {
                _SearchFilter = value;
                if (PropertyChanged != null)
                {
                    // при изменении фильтра список перерисовывается
                    PropertyChanged(this, new PropertyChangedEventArgs("ContracList2"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ServicesCount"));
                    PropertyChanged(this, new PropertyChangedEventArgs("FilteredServicesCount"));

                }
            }
        }
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SearchFilter = SearchFilterTextBox.Text;
        }

        private void ConrtactWindow_Click(object sender, RoutedEventArgs e)
        {
            var NewService = new Contract();

            var NewServiceWindow = new ContractWindow(NewService);
            if ((bool)NewServiceWindow.ShowDialog())
            {
                //список услуг нужно перечитать с сервера
                ContracList2= Core.DB.Contract.ToList();
                PropertyChanged(this, new PropertyChangedEventArgs("ContracList2"));
                PropertyChanged(this, new PropertyChangedEventArgs("ServicesCount"));
                PropertyChanged(this, new PropertyChangedEventArgs("FilteredServicesCount"));
            }
        }

        private void EdditConract_Click(object sender, RoutedEventArgs e)
        {
            var SelectedService = ProductListView.SelectedItem as Contract;
            var EditServiceWindow = new ContractWindow(SelectedService);
            if ((bool)EditServiceWindow.ShowDialog())
            {
                // при успешном завершении не забываем перерисовать список услуг
                PropertyChanged(this, new PropertyChangedEventArgs("ContracList2"));
                PropertyChanged(this, new PropertyChangedEventArgs("ServicesCount"));
                PropertyChanged(this, new PropertyChangedEventArgs("FilteredServicesCount"));
                // и еще счетчики - их добавьте сами
            }
        }

        private void DeleeteContract_Click(object sender, RoutedEventArgs e)
        {
            var item = ProductListView.SelectedItem as Contract;
            Core.DB.Contract.Remove(item);
            Core.DB.SaveChanges();
            ContracList2 = Core.DB.Contract.ToList();
        }

        private void ExitContractButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
