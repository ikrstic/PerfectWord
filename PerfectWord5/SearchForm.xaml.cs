using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;
using MahApps.Metro.Controls;

namespace PerfectWord5
{
    /// <summary>
    /// Interaction logic for SearchForm.xaml
    /// </summary>
    public partial class SearchForm : MetroWindow
    {
        public SearchForm()
        {
            InitializeComponent();
        }
        //Iniciranje pokretanja metode ListWords po otvaranju forme
        private void OnLoad(object sender, System.EventArgs e)
        {
            ListWords();
        }
        //Ucitavanje reci u formu za upotrebu pri pretrazi
        private void ListWords()
        {
            sqlCon = new SqlConnection();
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            try
            {
                sqlCon.Open();
                cmd = new SqlCommand();
                cmd.Connection = sqlCon;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT WordId, Original, Prevod, Opis FROM Words";
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string original = dr.GetString(1);
                    cmbOriginal.Items.Add(original);
                }
                //cmd.Dispose();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        //Pokretanje ucitavanje liste reci iz baze podataka u glavnoj formi 
        //pri promeni nastaloj u formi AddWords, npr pri unosu nove reci, 
        //kako bi se promena prikazala na glavnom prozoru bez potrebe za ponovnim porektanjem programa
        public delegate void methodHandler();
        public methodHandler OnRunMethod;
        
        //postavljenje geter a i setera za konekciju i upotrebu podataka iz baze
        public SqlConnection sqlCon { get; set; }

        public SqlCommand cmd { get; set; }

        public SqlDataAdapter sqlDa { get; set; }

        public DataSet ds { get; set; }


        //Klikom na dugme Close zatvara se prozor AddWords ali se pre toga pokrece obnavljanje liste reci iz baze
        // koji se prikazuju u glavnom programu
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (OnRunMethod != null)
                OnRunMethod();
            this.Close();
        }

        //pokretanje prikaza reci u poljima prevod i opis ukoliko je rec uneta u polje original pronadjena u bazi
        private void cmbOriginal_DropDownClosed(object sender, EventArgs e)
        {
            sqlCon = new SqlConnection();
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            try
            {
                sqlCon.Open();
                cmd = new SqlCommand();
                cmd.Connection = sqlCon;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Original, Prevod, Opis FROM Words where Original='" + cmbOriginal.Text + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                //proverava da li je rec pronadjena u bazi
                //ukoliko postoji unos polja prevod i opis ostaju neaktivna
                //kao i dugme save, u suprotnom tj. za nove unose se polja aktiviraju
                if (!(dr.HasRows))
                {
                    txtbPrevod.IsEnabled = true;
                    txtbOpis.IsEnabled = true;
                    btnSearch.IsEnabled = true;
                }
                else
                {
                    while (dr.Read())
                    {
                        //string sID = dr.GetString(0);
                        //string sOriginal = dr.GetString(0);
                        string sPrevod = dr.GetString(1);
                        string sOpis = dr.GetString(2);


                        //txtbID.Text = sID;
                        txtbPrevod.Text = sPrevod;
                        txtbOpis.Text = sOpis;
                    }
                }
                //cmd.Dispose();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //klikom na dugme save u bazu se unose podaci iz polja u formi i cuvaju
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //var ID = Guid.NewGuid().ToString();
            sqlCon = new SqlConnection();
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            try
            {
                sqlCon.Open();
                cmd = new SqlCommand();
                cmd.Connection = sqlCon;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Original, Prevod, Opis FROM Words WHERE Original='" + cmbOriginal.Text + "' OR Prevod='" + txtbPrevod.Text + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                //proverava da li je rec pronadjena u bazi
                //ukoliko postoji unos polja prevod i opis ostaju neaktivna
                //kao i dugme save, u suprotnom tj. za nove unose se polja aktiviraju
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        //string sID = dr.GetString(0);
                        string sOriginal = dr.GetString(0);
                        string sPrevod = dr.GetString(1);
                        string sOpis = dr.GetString(2);


                        cmbOriginal.Text = sOriginal;
                        txtbPrevod.Text = sPrevod;
                        txtbOpis.Text = sOpis;
                    }

                }
                else
                {
                    txtbPrevod.IsEnabled = false;
                    txtbOpis.IsEnabled = false;
                    MessageBox.Show("Word was not faound in PerfectWord DataBase, Please Use AddWords Form to save your perfect word!");
                }
                //cmd.Dispose();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //kontrola klika na dugme enter i tab za prikaz podataka u polja prevod i opis ukoliko se nalaze u bazi
        private void cmbOriginal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                sqlCon = new SqlConnection();
                sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                try
                {
                    sqlCon.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlCon;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT Original, Prevod, Opis FROM Words where Original='" + cmbOriginal.Text + "'";
                    SqlDataReader dr = cmd.ExecuteReader();
                    
                    //proverava da li je rec pronadjena u bazi
                    //ukoliko postoji unos polja prevod i opis ostaju neaktivna
                    //kao i dugme save, u suprotnom tj. za nove unose se polja aktiviraju
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            //string sID = dr.GetString(0);
                            //string sOriginal = dr.GetString(0);
                            string sPrevod = dr.GetString(1);
                            string sOpis = dr.GetString(2);


                            //txtbID.Text = sID;
                            txtbPrevod.Text = sPrevod;
                            txtbOpis.Text = sOpis;
                        }
                        
                    }
                    else
                    {
                        txtbPrevod.IsEnabled = false;
                        txtbOpis.IsEnabled = false;
                        MessageBox.Show("Word was not faound in PerfectWord DataBase, Please Use AddWords Form to save your perfect word!");
                    }
                    //cmd.Dispose();
                    sqlCon.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
