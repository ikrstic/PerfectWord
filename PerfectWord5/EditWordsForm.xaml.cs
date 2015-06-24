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
    /// Interaction logic for EditWordsForm.xaml
    /// </summary>
    public partial class EditWordsForm : MetroWindow
    {
        public EditWordsForm()
        {
            InitializeComponent();
        }
        private void OnLoad(object sender, System.EventArgs e)
        {
            ListWords();
        }

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
                cmd.Dispose();
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

        public delegate void methodHandler();
        public methodHandler OnRunMethod;

        public SqlConnection sqlCon { get; set; }

        public SqlCommand cmd { get; set; }

        public SqlDataAdapter sqlDa { get; set; }

        public DataSet ds { get; set; }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (OnRunMethod != null)
                OnRunMethod();
            this.Close();
        }

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
                cmd.CommandText = "SELECT WordId, Original, Prevod, Opis FROM Words where Original='" + cmbOriginal.Text + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    int sID = dr.GetInt32(0);
                    string sOriginal = dr.GetString(1);
                    string sPrevod = dr.GetString(2);
                    string sOpis = dr.GetString(3);

                    txtbID.Text = sID.ToString();
                    txtbPrevod.Text = sPrevod;
                    txtbOpis.Text = sOpis;
                }
                cmd.Dispose();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
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
                cmd.CommandText = "UPDATE Words SET Original = '" + this.cmbOriginal.Text + "', Prevod = '" + this.txtbPrevod.Text + "', Opis = '" + this.txtbOpis.Text + "' WHERE WordId = '" + this.txtbID.Text + "'";
                cmd.ExecuteNonQuery();

                MessageBox.Show("Sucesfully Updated!");
                cmd.Dispose();
                sqlCon.Close();
                txtbID.Clear();
                cmbOriginal.Items.Clear();
                txtbPrevod.Clear();
                txtbOpis.Clear();
                ListWords();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
                    cmd.Dispose();
                    sqlCon.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            sqlCon = new SqlConnection();
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            try
            {
                sqlCon.Open();
                cmd = new SqlCommand();
                cmd.Connection = sqlCon;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Words WHERE Original='" + this.cmbOriginal.Text + "'";
                cmd.ExecuteNonQuery();

                if (OnRunMethod != null)
                    OnRunMethod();
                MessageBox.Show("Entry deleted!");
                
                cmd.Dispose();
                sqlCon.Close();
                cmbOriginal.Items.Clear();
                txtbPrevod.Clear();
                txtbOpis.Clear();
                ListWords();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
