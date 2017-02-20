using System;
using System.Collections.Generic;
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

namespace DatabaseDebug
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DatabaseDebug.VictoriousTestDatabaseDataSet victoriousTestDatabaseDataSet = ((DatabaseDebug.VictoriousTestDatabaseDataSet)(this.FindResource("victoriousTestDatabaseDataSet")));
            // Load data into the table Tournaments. You can modify this code as needed.
            DatabaseDebug.VictoriousTestDatabaseDataSetTableAdapters.TournamentsTableAdapter victoriousTestDatabaseDataSetTournamentsTableAdapter = new DatabaseDebug.VictoriousTestDatabaseDataSetTableAdapters.TournamentsTableAdapter();
            victoriousTestDatabaseDataSetTournamentsTableAdapter.Fill(victoriousTestDatabaseDataSet.Tournaments);
            System.Windows.Data.CollectionViewSource tournamentsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("tournamentsViewSource")));
            tournamentsViewSource.View.MoveCurrentToFirst();
        }
    }
}
