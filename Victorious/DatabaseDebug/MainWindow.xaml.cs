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
using DataLib;
namespace DatabaseDebug
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //VictoriousDbContext db; 
        VictoriousDatabase db = new VictoriousDatabase();

        private string connection = "Data Source=RYAN-PC;Initial Catalog=VictoriousTestDatabase;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public MainWindow()
        {
            InitializeComponent();
            //db = new VictoriousDbContext();

            //db.Database.Connection.ConnectionString = connection;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Tournament tournament = new Tournament() { Title = "Title", Description = "Desc" };
            //db.Tournaments.Add(tournament);
            //db.SaveChanges();
            //db.Tournaments.d
            db.DeleteTournamentById(1);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

           
        }
    }
}
