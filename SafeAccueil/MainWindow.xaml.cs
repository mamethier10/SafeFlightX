using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace SafeAccueil
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        //Gestion du button de connexion

        private void ConnexionClick(object sender, RoutedEventArgs e)
        {         
            if (!string.IsNullOrWhiteSpace(chIndentifiant.Text) && !string.IsNullOrWhiteSpace(chPassword.Password))
            {
                string idAgent = chIndentifiant.Text;
                string passwordAgent = chPassword.Password;
                ConnexionDB con = new ConnexionDB();
                con.OpenConnexion();
                SqlCommand commande = new SqlCommand("SELECT COUNT(*) FROM agent WHERE " +
                                "nom_user = @idAgent AND password_agent = @passwordAgent",con.OpenConnexion());
                commande.Parameters.AddWithValue("@idAgent", idAgent);
                commande.Parameters.AddWithValue("@passwordAgent", passwordAgent);
                int nombreResultats = (int)commande.ExecuteScalar();
                if (nombreResultats > 0)
                {
                    TableauDeBord tab = new TableauDeBord();
                    this.Close();
                    tab.Show();
                }
                else
                    MessageBox.Show("L'identifiant ou le mot de passe est incorrect!");

            }
            else
            {
                MessageBox.Show("Les deux Champs sont obligatoires!");
            }

             
        }
        //gestin du button d'annulation
        private void Annulation(object sender, RoutedEventArgs e)
        {
            chIndentifiant.Text = "";
            chPassword.Password = "";
        }
    }
}
