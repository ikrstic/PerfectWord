using System;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using MahApps.Metro.Controls;

namespace PerfectWord5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Load_DataGrid();
        }
        public static class ApplicationEvents
        {
            public static event EventHandler DataChanged;

            public static void NotifyDataChanged()
            {
                EventHandler temp = DataChanged;
                if (temp != null)
                {
                    temp(null, EventArgs.Empty);
                }
            }
        }

        private void FillDataGrid()
        {
            ApplicationEvents.DataChanged += new EventHandler(ApplicationEvents_DataChanged);
        }

        void ApplicationEvents_DataChanged(object sender, EventArgs e)
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            string CmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(ConString))
            {
                CmdString = "Select Original, Prevod, Opis FROM Words";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Words");
                sda.Fill(dt);
                grdWords.ItemsSource = dt.DefaultView;
            }
        }

        private void AddMenu_Clicked(object sender, RoutedEventArgs e)
        {
            AddWordsForm AddWords = new AddWordsForm();
            AddWords.OnRunMethod += new PerfectWord5.AddWordsForm.methodHandler(Load_DataGrid);
            AddWords.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExportMenu_Clicked(object sender, RoutedEventArgs e)
        {
            /*
            ExportForm Export = new ExportForm();
            Export.Show();
            */
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "PerfectWordDB";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Xml documents (.xml)|*.xml"; // Filter files by extension 

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    string CmdString = "SELECT Original, Prevod, Opis FROM Words";
                    SqlConnection con;
                    SqlCommand cmd;
                    SqlDataAdapter sda;
                    DataTable dt;
                    //string filename = dlg.FileName;

                    using (con = new SqlConnection(ConString))
                    {
                        cmd = new SqlCommand(CmdString, con);
                        con.Open();
                        dt = new DataTable("Words");
                        sda = new SqlDataAdapter(cmd);
                        sda.Fill(dt);
                        dt.WriteXml(dlg.FileName);
                        con.Close();
                        System.Windows.MessageBox.Show("File Successfully saved in'" + dlg.FileName.ToString() + "'" + "\n" + "PerfectWord DataBase Exported!");
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
                // Save document 

            }
        }

        private void ImportMenu_Clicked(object sender, RoutedEventArgs e)
        {
            /*
            ImportForm Import = new ImportForm();
            Import.Show();
            */

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = "PerfectWordDB";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Xml documents (.xml)|*.xml"; // Filter files by extension 

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                SqlConnection con;
                SqlCommand cmd;
                SqlDataAdapter sda = new SqlDataAdapter();
                DataSet ds = new DataSet();
                XmlReader xmlFile;
                string sql = null;

                //int WordId = 0;
                string Original = null;
                string Prevod = null;
                string Opis = null;
                try
                {
                    using (con = new SqlConnection(ConString))
                    {

                        MessageBox.Show(filename);
                        xmlFile = XmlReader.Create(filename, new XmlReaderSettings());
                        ds.ReadXml(xmlFile);
                        int i = 0;
                        con.Open();
                        for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            //WordId = Int32.Parse(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                            //WordId = ds.Tables[0].Rows[i].ItemArray[0];
                            Original = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                            Prevod = ds.Tables[0].Rows[i].ItemArray[1].ToString();
                            Opis = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                            sql = "SET IDENTITY_INSERT Words ON";
                            sql = "INSERT INTO Words values('" + Original + "','" + Prevod + "','" + Opis + "')";
                            cmd = new SqlCommand(sql, con);
                            sda.InsertCommand = cmd;
                            sda.InsertCommand.ExecuteNonQuery();
                        }
                    }
                    con.Close();
                    Load_DataGrid();
                    MessageBox.Show("Done !! ");
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
        }
        public void Load_DataGrid()
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            string CmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(ConString))
            {
                CmdString = "Select WordId, Original, Prevod, Opis FROM Words";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Words");
                sda.Fill(dt);
                grdWords.ItemsSource = dt.DefaultView;
            }
        }

        private void EditMenu_Clicked(object sender, RoutedEventArgs e)
        {
            EditWordsForm EditWords = new EditWordsForm();
            EditWords.OnRunMethod += new PerfectWord5.EditWordsForm.methodHandler(Load_DataGrid);
            EditWords.Show();
        }

        private void SearchForm_Clicked(object sender, RoutedEventArgs e)
        {
            SearchForm Search = new SearchForm();
            Search.OnRunMethod += new PerfectWord5.SearchForm.methodHandler(Load_DataGrid);
            Search.Show();
        }

        public SqlConnection sqlCon { get; set; }

        public SqlCommand cmd { get; set; }

        private void RefreshMenu_Clicked(object sender, RoutedEventArgs e)
        {
            Load_DataGrid();
        }
    }
}
