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
using HelixToolkit.Wpf.SharpDX.Core2D;

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
        string queryVol = "SELECT id_vol, nom_vol, date_vol, h_depart, h_arrivee, airport_d, airport_a FROM vol";

        public TableauDeBord()
        {
            InitializeComponent();
            LoadAgent();
            LoadAirport();
            LoadSignaux();
            LoadNameAirport();
            LoadTimePicker();
            LoadVol();

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
        //Méthode pour charger le contenu de la table des vols
        private void LoadVol()
        {
            DataTable volData = GetData(queryVol);
            volTab.ItemsSource = volData.DefaultView;
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

        //Méthde pour enregistrer un vol

        private void AddVol(object sender, RoutedEventArgs e)
        {
            ConnexionDB con = new ConnexionDB();
            con.OpenConnexion();
            if (!string.IsNullOrEmpty(codeVol.Text))

            {
                
                int id_agent = IdentifiantAgent.identifiant;
                string nom_vol = codeVol.Text.Trim();
                
                DateTime date_vol1 = dateVol.SelectedDate.Value;
                string date_vol = date_vol1.Date.ToString("d");
                string h_depart = hdCombo.SelectedItem.ToString();
                string m_depart = mdCombo.SelectedItem.ToString();
                string h_arrivee = haCombo.SelectedItem.ToString();
                string m_arrivee = maCombo.SelectedItem.ToString();
                string airport_d = nameComboBox1.SelectedItem.ToString();
                string airport_a = nameComboBox2.SelectedItem.ToString();
                string heureDepart = h_depart + "h : " + m_depart + "min";
                string heureArrivee = h_arrivee + "h : " + m_arrivee + "min";

                if (airport_d != airport_a && (nom_vol.Length > 4))

                {
                    try
                    {

                        con.OpenConnexion();
                        string query = "INSERT INTO vol (nom_vol,date_vol,h_depart,h_arrivee,airport_d,airport_a,id_agent)" +
                            " VALUES (@nom_vol, @date_vol, @heureDepart,@heureArrivee,@airport_d,@airport_a,@id_agent)";
                        SqlCommand cmd = new SqlCommand(query, con.OpenConnexion());

                        cmd.Parameters.AddWithValue("@nom_vol", nom_vol);
                        cmd.Parameters.AddWithValue("@date_vol", date_vol);
                        cmd.Parameters.AddWithValue("@h_depart", h_depart);
                        cmd.Parameters.AddWithValue("@m_depart", m_depart);
                        cmd.Parameters.AddWithValue("@h_arrivee", h_arrivee);
                        cmd.Parameters.AddWithValue("@m_arrivee", m_arrivee);
                        cmd.Parameters.AddWithValue("@heureDepart", heureDepart);
                        cmd.Parameters.AddWithValue("@heureArrivee", heureArrivee);
                        cmd.Parameters.AddWithValue("@airport_d", airport_d);
                        cmd.Parameters.AddWithValue("@airport_a", airport_a);
                        cmd.Parameters.AddWithValue("@id_agent", id_agent);
                        int n = cmd.ExecuteNonQuery();
                        if (n > 0)
                        {
                            MessageBox.Show("Le vol a été ajouté avec succés!");
                            LoadVol();
                            InitVol();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur est survenue" + ex.Message);
                    }

                }
                else
                {
                    if (airport_d == airport_a)
                        MessageBox.Show("L'aéroport de départ doit être différent de celui d'arrivée!");
                    if (nom_vol.Length <= 4)
                        MessageBox.Show("Le nom du vol doit être supérieur à 4 lettres.");

                }
            }
            else
            {
                MessageBox.Show("Veuiller remplir tous les champs!");
            }
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
                }
                else
                {
                    MessageBox.Show("Veuiller remplir tous les champs!");
                }
            }

        //Méthode pour supprimer un utilisateur.
        private void DeleteUser(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)userTab.SelectedItem;
            if (ligne == null)
                MessageBox.Show("Veuiller selectionner une ligne!");
            else
            {
                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet élément ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {



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
            }
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
                if (code_airport.Length == 3 && pays.Length == 2 && nom_airport.Length > 1)
                {
                    try
                    {
                        string query = "INSERT INTO aeroport(code_aeroport,nom_aeroport,code_pays) VALUES " +
                                      "(@code_airport, @nom_airport, @pays)";
                        SqlCommand cmd = new SqlCommand(query, con.OpenConnexion());
                        cmd.Parameters.AddWithValue("@code_airport", code_airport);
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

                        MessageBox.Show("Erreur!" + ex.Message);
                    }
                }
                else
                {
                    if (code_airport.Length != 3)
                        MessageBox.Show("Le code aeroport doit égal à 3 caractères!");
                    if (nom_airport.Length < 1)
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

            if (ligne != null)
                MessageBox.Show("Veuiller selection la ligne à supprimer");
            else
            {
                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet élément ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {

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
            }
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
                    nomAirport.Text = ligne["nom_aeroport"].ToString();

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
                            catch (Exception ex)
                            {
                                MessageBox.Show("Erreur" + ex.Message);

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
                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet élément ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
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
        }

        private void ActualiserSignaux(object sender, RoutedEventArgs e)
            {
                LoadSignaux();
                nomSigne.Text = "";
            }

            // Methode pour récupérer les noms des aéroports
            public List<string> GetNameAirport()
            {
                List<string> names = new List<string>();
                try
                {
                    ConnexionDB con = new ConnexionDB();
                    con.OpenConnexion();
                    string query = "SELECT nom_aeroport FROM aeroport";
                    SqlCommand com = new SqlCommand(query, con.OpenConnexion());
                    SqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        names.Add(reader.GetString(0));
                    }
                    reader.Close();



                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur" + ex.Message);
                }

                return names;
            }
            //Chargement des comboBox avec les nom des aéroports
            private void LoadNameAirport(int? defaultName = null)
            {
                List<string> names = GetNameAirport();
                nameComboBox2.ItemsSource = names;
                nameComboBox1.ItemsSource = names;
                if (defaultName.HasValue)
                {
                    nameComboBox2.SelectedValue = defaultName.Value;
                    nameComboBox1.SelectedValue = defaultName.Value;
                }
                nameComboBox1.SelectedIndex = 849;
                nameComboBox2.SelectedIndex = 0;
            }

            //Méthode pour gérer le comboBox des heures et minutes

            private void LoadTimePicker()
            {
                for (int h = 0; h < 24; h++)
                {
                    hdCombo.Items.Add(h.ToString("D2"));
                    haCombo.Items.Add(h.ToString("D2"));
                }
                hdCombo.SelectedIndex = 0;
                haCombo.SelectedIndex = 0;

                for (int m = 0; m < 60; m++)
                {
                    mdCombo.Items.Add(m.ToString("D2"));
                    maCombo.Items.Add(m.ToString("D2"));
                }
                mdCombo.SelectedIndex = 0;
                maCombo.SelectedIndex = 0;
            }
// Méthode pour la gestion d'une selection de ligne dans la table Vol
        private void VolGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (volTab.SelectedItem != null)
            {
                DataRowView ligne = (DataRowView)volTab.SelectedItem;

                codeVol.Text = ligne["nom_vol"].ToString();
                dateVol.Text = ligne["date_vol"].ToString();
                string HDepart = ligne["h_depart"].ToString();
                string h = HDepart[0].ToString() + HDepart[1];
                string m = HDepart[6].ToString() + HDepart[7];
                hdCombo.SelectedIndex = int.Parse(h);
                mdCombo.SelectedIndex = int.Parse(m);

                string HArrivee = ligne["h_arrivee"].ToString();
                string ha = HArrivee[0].ToString() + HArrivee[1];
                string ma = HArrivee[6].ToString() + HArrivee[7];
                haCombo.SelectedIndex = int.Parse(ha);
                maCombo.SelectedIndex = int.Parse(ma);

                nameComboBox1.Text = ligne["airport_d"].ToString();
                nameComboBox2.Text = ligne["airport_a"].ToString();

            }

        }

        private void UpdateVol(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)volTab.SelectedItem;
            if (ligne == null)
            {
                MessageBox.Show("Veiller séléctionner une ligne d'abord.");
            }
            else
            {    
                    string b = ligne["id_vol"].ToString();

                    int a = int.Parse(b);
                    string nom_vol = codeVol.Text.Trim();

                    DateTime date_vol1 = dateVol.SelectedDate.Value;
                    string date_vol = date_vol1.Date.ToString("d");
                    string h_depart = hdCombo.SelectedItem.ToString();
                    string m_depart = mdCombo.SelectedItem.ToString();
                    string h_arrivee = haCombo.SelectedItem.ToString();
                    string m_arrivee = maCombo.SelectedItem.ToString();
                    string airport_d = nameComboBox1.SelectedItem.ToString();
                    string airport_a = nameComboBox2.SelectedItem.ToString();
                    string heureDepart = h_depart + "h : " + m_depart + "min";
                    string heureArrivee = h_arrivee + "h : " + m_arrivee + "min";

                    if (airport_d != airport_a && (nom_vol.Trim().Length > 4))

                    {
                        try
                        {
                            ConnexionDB con = new ConnexionDB();
                            con.OpenConnexion();
                        string query = "UPDATE vol SET nom_vol=@nom_vol, date_vol=@date_vol,h_depart=@heureDepart," +
                                       "h_arrivee=@heureArrivee, airport_d=@airport_d, airport_a=@airport_a " +
                                       "WHERE id_vol=@a" ;
                                
                            SqlCommand cmd = new SqlCommand(query, con.OpenConnexion());

                            cmd.Parameters.AddWithValue("@nom_vol", nom_vol);
                            cmd.Parameters.AddWithValue("@date_vol", date_vol);
                            cmd.Parameters.AddWithValue("@h_depart", h_depart);
                            cmd.Parameters.AddWithValue("@m_depart", m_depart);
                            cmd.Parameters.AddWithValue("@h_arrivee", h_arrivee);
                            cmd.Parameters.AddWithValue("@m_arrivee", m_arrivee);
                            cmd.Parameters.AddWithValue("@heureDepart", heureDepart);
                            cmd.Parameters.AddWithValue("@heureArrivee", heureArrivee);
                            cmd.Parameters.AddWithValue("@airport_d", airport_d);
                            cmd.Parameters.AddWithValue("@airport_a", airport_a);
                            cmd.Parameters.AddWithValue("@a", a);
                            int n = cmd.ExecuteNonQuery();
                            if (n > 0)
                            {
                                MessageBox.Show("Le vol a été mise à jour avec succés!");
                                LoadVol();
                                InitVol();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Une erreur est survenue" + ex.Message);
                        }

                    }
                    else
                    {
                        if (airport_d == airport_a)
                            MessageBox.Show("L'aéroport de départ doit être différent de celui d'arrivée!");
                        if (nom_vol.Length <= 4)
                            MessageBox.Show("Le nom du vol doit être supérieur à 4 lettres.");

                    }
        
               
            }
        }

        public void InitVol()
        {
            codeVol.Text= string.Empty;
            dateVol.TabIndex= 0;
            hdCombo.Text = "00";
            mdCombo.Text = "00";
            haCombo.Text = "00";
            maCombo.Text = "00";
            nameComboBox1.SelectedIndex = 849;
            nameComboBox2.SelectedIndex = 0;
        }

        private void ActualiserVol(object sender, RoutedEventArgs e)
        {
            LoadVol();
            InitVol() ; 
        }

        private void DeleteVol(object sender, RoutedEventArgs e)
        {
            DataRowView ligne = (DataRowView)volTab.SelectedItem;

            if (ligne == null)
                MessageBox.Show("Veuiller séléctionné une ligne.");
            else
            {
                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet élément ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {


                    try
                    {
                        string id = ligne["id_vol"].ToString();
                        ConnexionDB con = new ConnexionDB();
                        con.OpenConnexion();
                        string query = "DELETE FROM vol WHERE id_vol=@id";
                        SqlCommand com = new SqlCommand(query, con.OpenConnexion());
                        com.Parameters.AddWithValue("@id", id);
                        int n = com.ExecuteNonQuery();
                        if (n > 0)
                        {
                            MessageBox.Show("Le vol a été supprimé avec succés!");
                            LoadVol();
                            InitVol();
                        }
                        else
                        {
                            MessageBox.Show("Aucun vol trouvé.");
                        }
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Une erreur est survenue : " + ex.Message);
                    }
                }
            }
        }
    }
}
    

