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
using HelixToolkit.Wpf.SharpDX;
using System.Linq.Expressions;

namespace SafeAccueil
{
    /// <summary>
    /// Logique d'interaction pour TableauDeBord.xaml
    /// </summary>
    public partial class TableauDeBord : Window
    {
        string query = "SELECT id_agent, prenom_agent,nom_agent,nom_user," +
                           "password_agent,type_agent FROM agent";

        string queryAirport = "SELECT id_airport,code_aeroport,nom_aeroport,code_pays FROM aeroport";

        string querySigne = "SELECT id_signaux, nom_signaux FROM signaux";

        public TableauDeBord()
        {
            InitializeComponent();
            LoadAgent();
            LoadAirport();
            LoadSignaux();
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


        private DataTable GetData(string requete)
        {
            DataTable dataTable = new DataTable();

            ConnexionDB con = new ConnexionDB();
            con.OpenConnexion();
            SqlCommand command = new SqlCommand(requete, con.OpenConnexion());
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dataTable);

            return dataTable;
        }

        //méthode pour charger le contenu de la table agent dans le tableau
        private void LoadAgent()
        {
            DataTable agentData = GetData(query);
            userTab.ItemsSource = agentData.DefaultView;
        }

        private void LoadAirport()
        {
            DataTable agentData = GetData(queryAirport);
            airportTab.ItemsSource = agentData.DefaultView;
        }

        private void LoadSignaux()
        {
            DataTable agentData = GetData(querySigne);
            SignauxTab.ItemsSource = agentData.DefaultView;
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

        private void SigneGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SignauxTab.SelectedItem != null)
            {
                DataRowView ligne = (DataRowView)SignauxTab.SelectedItem;


                nomSigne.Text = ligne["nom_signaux"].ToString();


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
        public void InitAirport()
        {
            codeAirport.Text = "";
            nomAirport.Text = "";
            paysAirport.Text = "";
        }

        //méthode pour actualiser le tableau
        private void Datactualiser(object sender, RoutedEventArgs e)
        {
            LoadAgent();
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
                    && !con.VerifExist(nom_user))
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
                            LoadAgent();
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
                    if (con.VerifExist(nom_user))
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
                        LoadAgent();
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
            if (ligne == null)
                MessageBox.Show("Selectionner une ligne!");
            else
            {
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
                                LoadAgent();
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
            }

        }

        //Méthode pour ajouter un aéroport
        private void AddAirport(object sender, RoutedEventArgs e)
        {
            ConnexionDB con = new ConnexionDB();
            con.OpenConnexion();
            string code_airport = codeAirport.Text.Trim().ToUpper();
            string nom_airport = nomAirport.Text.Trim();
            string pays = paysAirport.Text.Trim().ToUpper();
            if (code_airport.Length==3 && pays.Length==2 && nom_airport.Length>1)
            {
                try
                {
                    string query = "INSERT INTO aeroport(code_aeroport,nom_aeroport,code_pays) VALUES " +
                                  "(@code_airport, @nom_airport, @pays)";
                    SqlCommand cmd = new SqlCommand(query, con.OpenConnexion());
                    cmd.Parameters.AddWithValue("@code_airport",code_airport);
                    cmd.Parameters.AddWithValue("@nom_airport", nom_airport);
                    cmd.Parameters.AddWithValue("@pays", pays);
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        MessageBox.Show("Ajout réussi!");
                        LoadAirport();
                        InitAirport();
                        
                    }
                        
                    else
                        MessageBox.Show("Ajout non réussie!");
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Erreur!"+ex.Message);
                }
            }else
            {
                if (code_airport.Length!=3)
                    MessageBox.Show("Le code aeroport doit égal à 3 caractères!");
                if (nom_airport.Length<1)
                    MessageBox.Show("Le nom de l' aeroport ne doit pas être vide!");
                if (pays.Length != 2)
                    MessageBox.Show("Le code pays doit égal à 2 caractères!");
            }

        }

        //Méthode pour mettre à jour un aéroport.
        private void UpdatAirport(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)airportTab.SelectedItem;
            if (ligne == null)
                MessageBox.Show("Veuiller selectionné une ligne.");
            else
            {
                string a = ligne["id_airport"].ToString();
                if (!string.IsNullOrEmpty(codeAirport.Text) && !string.IsNullOrEmpty(nomAirport.Text) && !string.IsNullOrEmpty(paysAirport.Text))
                {
                    string nom_airport = nomAirport.Text.Trim();
                    string code_airport = codeAirport.Text.Trim().ToUpper();
                    string pays_airport = paysAirport.Text.Trim().ToUpper();
                    if (code_airport.Length == 3 && pays_airport.Length == 2 && nom_airport.Length > 1)
                    {
                        try
                        {
                            ConnexionDB con = new ConnexionDB();
                            con.OpenConnexion();
                            string query = "UPDATE aeroport SET code_aeroport=@code_airport,nom_aeroport=@nom_airport," +
                                           "code_pays=@pays_airport WHERE id_airport=@a";
                            SqlCommand cmd = new SqlCommand(query, con.OpenConnexion());
                            cmd.Parameters.AddWithValue("@nom_airport", nom_airport);
                            cmd.Parameters.AddWithValue("@code_airport", code_airport);
                            cmd.Parameters.AddWithValue("@pays_airport", pays_airport);
                            cmd.Parameters.AddWithValue("@a", a);
                            int n = cmd.ExecuteNonQuery();
                            if (n > 0)
                            {
                                MessageBox.Show("Modification réussie!");
                                LoadAirport();
                                InitAirport();
                            }
                            else
                                MessageBox.Show("Modification non réussie!");

                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("Erreur" + ex.Message);
                        }
                    }
                    else
                    {
                        if (code_airport.Length != 3)
                            MessageBox.Show("Code aeroport doit egale à 3 caracteres.");
                        if (pays_airport.Length != 2)
                            MessageBox.Show("Code pays doit egal à deuc caractères!");
                        if (nom_airport.Length < 1)
                            MessageBox.Show("Nom de l'aéroport doit pas être vide");
                    }
                }
            }
        }

        //Méthode pour supprimer un aéroport.
        private void DeleteAirport(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)airportTab.SelectedItem;
            string id_airport = ligne["id_airport"].ToString();
            string code = ligne["code_pays"].ToString().Trim();
            int id = int.Parse(id_airport);
            if (code.Length > 0)
            {

                try
                {
                    ConnexionDB con = new ConnexionDB();
                    con.OpenConnexion();
                    string query = "DELETE FROM aeroport WHERE id_airport=@id";
                    SqlCommand com = new SqlCommand(query, con.OpenConnexion());
                    com.Parameters.AddWithValue("@id", id);
                    int n = com.ExecuteNonQuery();
                    if (n > 0)
                    {
                        MessageBox.Show("L'aéroport a été supprimé avec succés!");
                        LoadAirport();
                        InitAirport();
                    }
                    else
                    {
                        MessageBox.Show("Aucun aeroport trouvé.");
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Une erreur est survenue : " + ex.Message);
                }
            }
            else
                MessageBox.Show("Veiller selectionner la ligne a suprimer!");

        }

        //Méthode pour actualiser le tableau des aéroports.
        private void ActualiserAirport(object sender, RoutedEventArgs e)
        {
            LoadAirport();
            InitAirport();
        }

        private void AirportGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (airportTab.SelectedItem != null)
            {
                DataRowView ligne = (DataRowView)airportTab.SelectedItem;

                //idUser.Text = ligne["id_agent"].ToString();
                codeAirport.Text = ligne["code_aeroport"].ToString();
                nomAirport.Text= ligne["nom_aeroport"].ToString();

                paysAirport.Text = ligne["code_pays"].ToString();




            }
        }
        // Méthode pour ajouter un signe
        private void AddSignaux(object sender, RoutedEventArgs e)
        {
            string nom_signe = nomSigne.Text.Trim();
            if (nom_signe.Length > 0)
            {
                try
                {
                    ConnexionDB con = new ConnexionDB();
                    con.OpenConnexion();
                    string req = "INSERT INTO signaux(nom_signaux) VALUES(@nom_signe)";
                    SqlCommand cmd = new SqlCommand(req, con.OpenConnexion());
                    cmd.Parameters.AddWithValue("@nom_signe", nom_signe);
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        MessageBox.Show("Ajout réussi.");
                        LoadSignaux();
                        nomSigne.Text = "";
                    }
                    else
                        MessageBox.Show("Erreur!");
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Erreur" + ex.Message);
                }
            }
            else
                MessageBox.Show("Veuiller saisir un signe...");
        }

        //Méthode pour mettre a jour les signaux
        private void UpdateSignaux(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)SignauxTab.SelectedItem;
            if (ligne == null)
                MessageBox.Show("Veuiller selectionné une ligne.");
            else
            {
                string a = ligne["id_signaux"].ToString();
                if (!string.IsNullOrEmpty(nomSigne.Text))
                {
                    string nom_signaux = nomSigne.Text.Trim();

                    if (nom_signaux.Length > 1)
                    {
                        try
                        {
                            ConnexionDB con = new ConnexionDB();
                            con.OpenConnexion();
                            string query = "UPDATE signaux SET nom_signaux=@nom_signaux" +
                                           " WHERE id_signaux=@a";
                            SqlCommand cmd = new SqlCommand(query, con.OpenConnexion());
                            cmd.Parameters.AddWithValue("@nom_signaux", nom_signaux);

                            cmd.Parameters.AddWithValue("@a", a);
                            int n = cmd.ExecuteNonQuery();
                            if (n > 0)
                            {
                                MessageBox.Show("Modification réussi.");
                                LoadSignaux();
                                nomSigne.Text = "";
                            }
                            else
                                MessageBox.Show("Erreur.");
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Erreur" +ex.Message);

                        }

                    }
                }
            }
        }
        //Méthode pour supprimer un signe
        private void DeleteSigne(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)SignauxTab.SelectedItem;
            
            if (ligne == null)
                MessageBox.Show("Veuiller séléctionné une ligne.");
            else
            {
                try
                {
                    string id = ligne["id_signaux"].ToString();
                    ConnexionDB con = new ConnexionDB();
                    con.OpenConnexion();
                    string query = "DELETE FROM signaux WHERE id_signaux=@id";
                    SqlCommand com = new SqlCommand(query, con.OpenConnexion());
                    com.Parameters.AddWithValue("@id", id);
                    int n = com.ExecuteNonQuery();
                    if (n > 0)
                    {
                        MessageBox.Show("Signe a été supprimé avec succés!");
                        LoadSignaux();
                        nomSigne.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Aucun signe trouvé.");
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Une erreur est survenue : " + ex.Message);
                }
            }
        }

        private void ActualiserSignaux(object sender, RoutedEventArgs e)
        {
            LoadSignaux();
            nomSigne.Text = "";
        }
    }
}

