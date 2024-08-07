﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SafeAccueil
{
    public class ConnexionDB
    {

        private string chaineConnexion = "Data Source=.;Initial Catalog=GDpassage_db;Integrated Security=True;TrustServerCertificate=True";

        public SqlConnection OpenConnexion()
        {
            SqlConnection con = new SqlConnection(chaineConnexion);
            try
            {
                con.Open();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de connexion à la base de données"+ex.Message);
            }
            return con;
        }

        public void FermerConnexion(SqlConnection con)
        {
            try
            {
                con.Close();
                Console.WriteLine("Connexion fermée !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion "+ex.Message);
            }
        }
        // Méthode pour vérifier si un user exist déja ou pas
        public bool VerifExist(string nom_user)
        {
            
            string reqete = "SELECT * FROM agent WHERE nom_user=@nom_user";
            SqlCommand com = new SqlCommand(reqete,OpenConnexion());
            com.Parameters.AddWithValue("@nom_user", nom_user);
            int n = com.ExecuteNonQuery();
            if (n >= 1) 
                return true;
            else
                return false;
        }
    }
}
