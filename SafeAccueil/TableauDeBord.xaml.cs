using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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

namespace SafeAccueil
{
    /// <summary>
    /// Logique d'interaction pour TableauDeBord.xaml
    /// </summary>
    public partial class TableauDeBord : Window
    {
        public TableauDeBord()
        {
            InitializeComponent();
            LoadData();
        }


        private void GestionComptes(object sender, RoutedEventArgs e)
        {
            ShowContent(comptes);
        }

        private void GestionVols(object sender, RoutedEventArgs e)
        {
            ShowContent(vols);
        }

        private void GestionAirport(object sender, RoutedEventArgs e)
        {
            ShowContent(airport);
        }

        private void GestionSignaux(object sender, RoutedEventArgs e)
        {
            ShowContent(signaux);
        }

        private void ListDefinitive(object sender, RoutedEventArgs e)
        {
            ShowContent(listDif);
        }

        private void ListSuspect(object sender, RoutedEventArgs e)
        {
            ShowContent(listSuspect);
        }

        private void ShowContent(UIElement content)
        {
            comptes.Visibility = Visibility.Collapsed;
            vols.Visibility = Visibility.Collapsed;
            airport.Visibility = Visibility.Collapsed;
            signaux.Visibility = Visibility.Collapsed;
            listDif.Visibility = Visibility.Collapsed;
            listSuspect.Visibility = Visibility.Collapsed;
            DefaultContent.Visibility = Visibility.Collapsed;

            content.Visibility = Visibility.Visible;
        }


        private DataTable GetAgentData()
        {
            DataTable dataTable = new DataTable();

            ConnexionDB con = new ConnexionDB();
            con.OpenConnexion();

            string query = "SELECT id_agent, prenom_agent,nom_agent,nom_user," +
                "password_agent,type_agent FROM agent";
            SqlCommand command = new SqlCommand(query, con.OpenConnexion());

            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dataTable);



            return dataTable;
        }

        //méthode pour charger le contenu de la table agent dans le tableau
        private void LoadData()
        {
            DataTable agentData = GetAgentData();

            userTab.ItemsSource = agentData.DefaultView;
        }

        //Pour gerer les elements du tableau selectionnés
        private void UserGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (userTab.SelectedItem != null)
            {
                DataRowView ligne = (DataRowView)userTab.SelectedItem;

                //idUser.Text = ligne["id_agent"].ToString();
                idUser.Text = ligne["nom_user"].ToString();
                password.Password = ligne["password_agent"].ToString();
                passwordConfirm.Password = ligne["password_agent"].ToString();
                prenom.Text = ligne["prenom_agent"].ToString();
                nom.Text = ligne["nom_agent"].ToString();
                type.Text = ligne["type_agent"].ToString();

            }
        }

        public void init()
        {
            idUser.Text = "";
            prenom.Text = "";
            nom.Text = "";
            password.Password = "";
            passwordConfirm.Password = "";
            type.Text = "Choisir un rôle...";
        }

        //méthode pour actualiser le tableau
        private void Datactualiser(object sender, RoutedEventArgs e)
        {
            LoadData();
            init();
        }

        //Méthode pour ajouter un nouveau utilisateur dans la table agent.
        private void AddUser(object sender, RoutedEventArgs e)
        {
            ConnexionDB con = new ConnexionDB();
            con.OpenConnexion();
            if (!string.IsNullOrEmpty(idUser.Text) && !string.IsNullOrEmpty(prenom.Text) && !string.IsNullOrEmpty(nom.Text)
                && !string.IsNullOrEmpty(password.Password) && !string.IsNullOrEmpty(type.Text) && !string.IsNullOrEmpty(passwordConfirm.Password))
             
            {
                string nom_user = idUser.Text.Trim();
                string motdepass = password.Password.Trim();
                string prenoms = prenom.Text.Trim();
                string noms = nom.Text.Trim();
                string confirm = passwordConfirm.Password.Trim();
                string typ = type.Text.Trim();
                if (motdepass == confirm && (nom_user.Length > 4) && typ != "Choisir un rôle..." && motdepass.Length > 5
                    && con.VerifExist(nom_user))
                {
                    try
                    {
                        
                        con.OpenConnexion();
                        string query = "INSERT INTO agent (prenom_agent,nom_agent,nom_user,password_agent,type_agent)" +
                            " VALUES (@prenoms, @noms, @nom_user,@motdepass,@typ)";
                        SqlCommand cmd = new SqlCommand(query, con.OpenConnexion());

                        cmd.Parameters.AddWithValue("@nom_user", nom_user);
                        cmd.Parameters.AddWithValue("@motdepass", motdepass);
                        cmd.Parameters.AddWithValue("@prenoms", prenoms);
                        cmd.Parameters.AddWithValue("@noms", noms);
                        cmd.Parameters.AddWithValue("@typ", typ);
                        int n = cmd.ExecuteNonQuery();
                        if (n > 0)
                        {
                            MessageBox.Show("L'utilisateur à été ajouté avec succés!");
                            LoadData();
                            init();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur est survenue" + ex.Message);
                    }

                }
                else
                {
                    if (motdepass != confirm)
                        MessageBox.Show("Les deux mots de passe ne correspondent pas!");
                    if (nom_user.Length <= 4)
                        MessageBox.Show("L'identifiant doit être supérieur à 4 lettres.");
                    if (typ == "Choisir un rôle...")
                        MessageBox.Show("Veuiller choisir un rôle!");
                    if (motdepass.Length <= 5)
                        MessageBox.Show("Le mot de pass doit être supérieur à 5 caractère!");
                    if (!con.VerifExist(nom_user))
                        MessageBox.Show("Le nom utilisateur existe déja!");
                }
            } else
            {
                MessageBox.Show("Veuiller remplir tous les champs!");
            }
        }

        //Méthode pour supprimer un utilisateur.
        private void DeleteUser(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)userTab.SelectedItem;
            string iduser = ligne["id_agent"].ToString();
                //idUser.Text.Trim();
            if (iduser.Length > 0)
            {

                try
                {
                    ConnexionDB con = new ConnexionDB();
                    con.OpenConnexion();
                    string query = "DELETE FROM agent WHERE id_agent=@iduser";
                    SqlCommand com = new SqlCommand(query, con.OpenConnexion());
                    com.Parameters.AddWithValue("@iduser", iduser);
                    int n = com.ExecuteNonQuery();
                    if (n > 0)
                    {
                        MessageBox.Show("L'utilisateur a été supprimé avec succés!");
                        LoadData();
                        init();
                    }
                    else
                    {
                        MessageBox.Show("Aucun utilisateur trouvé avec l'ID spécifié.");
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Une erreur est survenue : " + ex.Message);
                }
            }
            else
                MessageBox.Show("Veuiller saisir l'identifiant de l'utilisateur à supprimer");
        }
        //Mise à jour d'utilisateur
        private void UpdateUser(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)userTab.SelectedItem;

            string a = ligne["Id_agent"].ToString();
            if (!string.IsNullOrEmpty(idUser.Text) && !string.IsNullOrEmpty(prenom.Text) && !string.IsNullOrEmpty(nom.Text)
                && !string.IsNullOrEmpty(password.Password) && !string.IsNullOrEmpty(type.Text) && !string.IsNullOrEmpty(passwordConfirm.Password))
            {
                string nom_user = idUser.Text.Trim();
                string motdepass = password.Password.Trim();
                string prenoms = prenom.Text.Trim();
                string noms = nom.Text.Trim();
                string confirm = passwordConfirm.Password.Trim();
                string typ = type.Text.Trim();
                if (motdepass == confirm && (nom_user.Length > 4) && typ != "Choisir un rôle..." && motdepass.Length > 5)
                {
                    try
                    {
                        ConnexionDB con = new ConnexionDB();
                        con.OpenConnexion();
                        string query = "UPDATE agent SET prenom_agent=@prenoms,nom_agent=@noms," +
                                       "nom_user=@nom_user,password_agent=@motdepass, type_agent=@typ WHERE id_agent=@a";
                        SqlCommand cmd = new SqlCommand(query, con.OpenConnexion());
                        cmd.Parameters.AddWithValue("@nom_user", nom_user);
                        cmd.Parameters.AddWithValue("@motdepass", motdepass);
                        cmd.Parameters.AddWithValue("@prenoms", prenoms);
                        cmd.Parameters.AddWithValue("@noms", noms);
                        cmd.Parameters.AddWithValue("@typ", typ);
                        cmd.Parameters.AddWithValue("@a", a);
                        int n = cmd.ExecuteNonQuery();
                        if (n > 0)
                        {
                            MessageBox.Show("Modification réussie!");
                            LoadData();
                            init();
                        }
                        else
                            MessageBox.Show("Modification non réussie!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur est survenue!" + ex.Message);
                    }
                }
                else
                {
                    if (motdepass != confirm)
                        MessageBox.Show("Les deux mots de passe ne correspondent pas!");
                    if (nom_user.Length <= 4)
                        MessageBox.Show("L'identifiant doit être supérieur à 4 lettres.");
                    if (typ == "Choisir un rôle...")
                        MessageBox.Show("Veuiller choisir un rôle!");
                    if (motdepass.Length <= 5)
                        MessageBox.Show("Le mot de pass doit être supérieur à 5 caractère!");
                }
            }
            else
                MessageBox.Show("Veuiller remplir les champs!");
        }
        
        //Méthode pour ajouter un aéroport
        private void AddAirport(object sender, RoutedEventArgs e)
        {

        }

        //Méthode pour mettre à jour un aéroport.
        private void UpdatAirport(object sender, RoutedEventArgs e)
        {

        }

        //Méthode pour supprimer un aéroport.
        private void DeleteAirport(object sender, RoutedEventArgs e)
        {

        }

        //Méthode pour actualiser le tableau des aéroports.
        private void ActualiserAirport(object sender, RoutedEventArgs e)
        {

        }
    }
}

