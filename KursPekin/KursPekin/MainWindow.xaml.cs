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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KursPekin
{
    public partial class Userman
    {
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName + " " + Patronomyc;
            }
        }
        public Uri ImagePreviewClient
        {
            get
            {
                var imageName = System.IO.Path.Combine(Environment.CurrentDirectory, photo ?? "");
                return System.IO.File.Exists(imageName) ? new Uri(imageName) : new Uri("pack://application:,,,/Фото сотрудника/picture.jpg");
            }
        }
    }
    public partial class Contract
    {
        public Boolean MinSalary
        {
            get
            {
                    return prosess == "";
                
            }
        }
        public double DiscountFloat
        {
            get
            {
                return Convert.ToSingle(Summa);
            }
        }
        public string CostString
        {
            get
            {
                // тут должно быть понятно - преобразование в строку с нужной точностью
                return Summa.ToString("#.##");
            }
        }
        public Uri ImagePreview
        {
            get
            {
                var imageName = System.IO.Path.Combine(Environment.CurrentDirectory, Avatar ?? "");
                return System.IO.File.Exists(imageName) ? new Uri(imageName) : new Uri("pack://application:,,,/img/picture.jpg");
            }
        }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        
        public MainWindow()
        {
            this.DataContext = this;
            ContracList = Core.DB.Contract.ToList();
            InitializeComponent();
        }
        private List<Contract> _ContractList;

        public List<Contract> ContracList {
            get
            {
                if (SortPriceAscending)
                    return _ContractList
                        .OrderBy(item =>(item.Summa))
                        .ToList();
                else
                    return _ContractList
                        .OrderByDescending(item => (item.Summa))
                        .ToList();
            }
            
            set
            {
                 _ContractList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ContracList"));
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
                    PropertyChanged(this, new PropertyChangedEventArgs("ContracList"));
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
                return _ContractList.Count;
            }

        }
        public int FilteredServicesCount
        {
            get
            {
                return ContracList.Count;
            }
        }

        private void ContrantShow_Click(object sender, RoutedEventArgs e)
        {
            
            var OpenContractShow = new windows.ContractProsess();
           // OpenContractShow.ShowDialog();
            if ((bool)OpenContractShow.ShowDialog())
            {
                // при успешном завершении не забываем перерисовать список услуг
                PropertyChanged(this, new PropertyChangedEventArgs("ContracList"));
                PropertyChanged(this, new PropertyChangedEventArgs("ServicesCount"));
                PropertyChanged(this, new PropertyChangedEventArgs("FilteredServicesCount"));
                // и еще счетчики - их добавьте сами
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
