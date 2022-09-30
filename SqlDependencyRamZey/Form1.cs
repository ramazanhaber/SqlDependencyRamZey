using SqlDependencyRamZey.SqlListening;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlDependencyRamZey
{
    public partial class Form1 : Form
    {
        /*
        bunu mssql den açman lazım enable_broker
        alter database [<dbname>] set enable_broker with rollback immediate;
        kontrol için
        SELECT NAME, is_broker_enabled FROM SYS.DATABASES

        kullandığımız paket
        Install-Package SqlTableDependency -Version 8.5.8

         */
        public Form1()
        {
            InitializeComponent();
        }

        public SqlTableDependency<Ogrenci> ogrenciTableDependency;

        string connection_string_people = "Data Source=RAMBO3;Initial Catalog = Deneme; Persist Security Info=True;User ID = sa; Password=19830126";

        private void button1_Click(object sender, EventArgs e)
        {
            refreshTable();
            startOgrenciTableDependency();
        }
        private void refreshTable()
        {
            string sql = "SELECT * FROM Ogrenci";
            SqlConnection connection = new SqlConnection(connection_string_people);
            SqlDataAdapter dataadapter = new SqlDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            connection.Open();
            dataadapter.Fill(ds, "Ogrenci");
            connection.Close();
        }

        public void mesajYaz(string mesaj)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                textBox1.Text = textBox1.Text + System.Environment.NewLine + mesaj;
            }));
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                stopOgrenciTableDependency();
            }
            catch (Exception ex)
            {
                mesajYaz(ex.ToString());
            }
        }

        // start , stop , error, changed 
        private bool startOgrenciTableDependency()
        {
            try
            {
                ogrenciTableDependency = new SqlTableDependency<Ogrenci>(connection_string_people);
                ogrenciTableDependency.OnChanged += ogrenciTableDependency_Changed;
                ogrenciTableDependency.OnError += ogrenciTableDependency_OnError;
                ogrenciTableDependency.Start();
                return true;
            }
            catch (Exception ex)
            {

                mesajYaz(ex.ToString());
            }
            return false;

        }
        private bool stopOgrenciTableDependency()
        {
            try
            {
                if (ogrenciTableDependency != null)
                {
                    ogrenciTableDependency.Stop();

                    return true;
                }
            }
            catch (Exception ex) { mesajYaz(ex.ToString()); }

            return false;

        }
        private void ogrenciTableDependency_OnError(object sender, ErrorEventArgs e)
        {
            mesajYaz(e.Error.Message);
        }
        private void ogrenciTableDependency_Changed(object sender, RecordChangedEventArgs<Ogrenci> e)
        {
            try
            {
                var changedEntity = e.Entity;

                switch (e.ChangeType)
                {
                    case ChangeType.Insert:
                        {


                            mesajYaz("Insert values:\tname:" + changedEntity.ad.ToString() + "\tage:" + changedEntity.yas.ToString());
                            refreshTable();

                        }
                        break;

                    case ChangeType.Update:
                        {
                            mesajYaz("Update values:\tname:" + changedEntity.ad.ToString() + "\tage:" + changedEntity.yas.ToString());
                            refreshTable();

                        }
                        break;

                    case ChangeType.Delete:
                        {
                            mesajYaz("Delete values:\tname:" + changedEntity.ad.ToString() + "\tage:" + changedEntity.yas.ToString());
                            refreshTable();
                        }
                        break;
                };

            }
            catch (Exception ex)
            {
                mesajYaz(ex.Message);
            }

        }


    }
}
