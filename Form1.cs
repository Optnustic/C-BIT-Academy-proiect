using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Data.Common;

namespace Agenda_Telefonica
{
    public partial class frmMainWindow : Form
    {
        
        
        private BindingSource bindingSource1 = new BindingSource();
        private SqlDataAdapter dataAdapter = new SqlDataAdapter();
        public frmMainWindow()
        {
            InitializeComponent();
            FrmPreferinteWindow frmPreferinte = new FrmPreferinteWindow(this);
            

        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string message = "Sunteti sigur ca doriti sa iesiti?";
            const string caption = "Exit";
            
            var result = MessageBox.Show(message, caption,
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }

            
        }

        private void FrmMainWindow_Load(object sender, EventArgs e)
        {
            dgdAgenda.DataSource = bindingSource1;
            GetData("select * from Agenda");
            Properties.Settings.Default.Upgrade();
            BackColor = Properties.Settings.Default.BackgroundColor;
            Font = Properties.Settings.Default.Font;


        }

        private void BtnIncarcareDate_Click(object sender, EventArgs e)
        {
            try
            {
                GetData(dataAdapter.SelectCommand.CommandText);
                if (dgdAgenda.Rows.Count == 1)
                {
                    MessageBox.Show("Nici un abonat găsit în bază.");
                }
            }
            catch(SqlException eql)
            {
                MessageBox.Show(eql.Message.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
                   
        }

        private void BtnSalvareDate_Click(object sender, EventArgs e)
        {
            try
            {
                dataAdapter.Update((DataTable)bindingSource1.DataSource);
                MessageBox.Show("Date salvate cu succes în baza de date");
            }
            catch (SqlException eql)
            {
                MessageBox.Show(eql.Message.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        private void GetData(string selectCommand)
        {
            try
            {
                string dp = ConfigurationManager.AppSettings["provider"].ToString();
                String connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

                DbProviderFactory df = DbProviderFactories.GetFactory(dp);
               
                using (DbConnection cn = df.CreateConnection())
                {
                    cn.ConnectionString = connectionString;
                    cn.Open();
                    dataAdapter = new SqlDataAdapter(selectCommand, connectionString);

                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                    DataTable table = new DataTable
                    {
                        Locale = System.Globalization.CultureInfo.InvariantCulture
                    };
                    dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    dataAdapter.Fill(table);
                    bindingSource1.DataSource = table;

                    dgdAgenda.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    cn.Close();
                }
                
                
            }
            catch (SqlException)
            {
                MessageBox.Show("Nu sa stabilit conectiunea la data de baze.");
            }
        }

       

        

        private void SerializareInformatiiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (dgdAgenda.Rows.Count > 1)
            {
                sfdXML.Title = "Salvare fisier serializare";
                sfdXML.Filter = "XML - File | *.xml";
                sfdXML.InitialDirectory = Directory.GetCurrentDirectory();
                DateTime moment = new DateTime();
                moment = DateTime.Now;
                String timestamp = moment.ToString("yyyyMMddHHmmss");
                String filename = string.Format("Abonati_{0}.xml", timestamp);
                sfdXML.FileName = filename;
                if (sfdXML.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = new DataTable
                    {
                        TableName = "abonat"
                    };
                    foreach (DataGridViewColumn col in dgdAgenda.Columns)
                    {
                        dt.Columns.Add(col.Name);
                    }

                    foreach (DataGridViewRow row in dgdAgenda.Rows)
                    {
                        DataRow dRow = dt.NewRow();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            dRow[cell.ColumnIndex] = cell.Value;
                        }
                        dt.Rows.Add(dRow);
                    }
                    try
                    {
                        dt.WriteXml(sfdXML.FileName, XmlWriteMode.WriteSchema);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Nimic de serializat.");
            }


        }

        private void DgdAgenda_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string headerText =
            dgdAgenda.Columns[e.ColumnIndex].HeaderText;

            if (headerText.Equals("Nume"))
            {
                if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    MessageBox.Show("Campul nume trebuie completat!");
                    
                }
            }

            if (headerText.Equals("Prenume"))
            {
                if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    MessageBox.Show("Campul prenume trebuie completat!");
                   
                }
            }

            if (headerText.Equals("Numar_telefon"))
            {
                if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    MessageBox.Show("Campul numar telefon trebuie completat!");
                    
                }
                else
                {
                    if (!int.TryParse(Convert.ToString(e.FormattedValue), out int i))
                    {
                        MessageBox.Show("Va rugam introduceti un numar de telefon format din cifre!");
                        
                    }
                }

            }
        }

        private void PreferinteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPreferinteWindow myWindow = new FrmPreferinteWindow(this);
            myWindow.Show();
        }

        private void CautaPersoanaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmCauta = new Form
            {
                Size = new Size(500, 500),
                Text = "Cautare"
            };
            TextBox txtCauta = new TextBox
            {
                Location = new Point(30, 30),
                Size = new Size(200, 30)
            };
            frmCauta.Controls.Add(txtCauta);
            Button btnCauta = new Button
            {
                Location = new Point(30,70),
                Text = "Cauta"
            };
            frmCauta.Controls.Add(btnCauta);
            DataGridView dgdCauta = new DataGridView
            {
                Location = new Point(30, 100),
                Size = new Size(420,320)
            };
            frmCauta.Controls.Add(dgdCauta);

            btnCauta.Click += new EventHandler(button_Click);

            void button_Click(object s, System.EventArgs ev)
            {
                Button button = sender as Button;

                try
                {
                    string cs = ConfigurationManager.AppSettings["connectionString"].ToString();
                    SqlConnection con;
                    SqlDataAdapter adapt;
                    DataTable dt;
                    con = new SqlConnection(cs);
                    con.Open();
                    //adapt = new SqlDataAdapter("select * from tbl_Employee where FirstName like '" + txt_SearchName.Text + "%'", con);
                    adapt = new SqlDataAdapter("select * from Agenda where Nume like '" + txtCauta.Text + "%' or Prenume like '" + txtCauta.Text + "%' or Numar_telefon like '" + txtCauta.Text + "%' ", con);
                    dt = new DataTable();
                    adapt.Fill(dt);
                    dgdCauta.DataSource = dt;
                    dgdCauta.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    con.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
               
            }

            
            frmCauta.Show();
        }
    }
}
