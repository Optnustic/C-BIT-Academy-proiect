using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Agenda_Telefonica
{
    public partial class FrmPreferinteWindow : Form
    {
        frmMainWindow frmMain;
        public FrmPreferinteWindow(frmMainWindow parent)
        {
            InitializeComponent();
            frmMain = parent;
        }

        public FrmPreferinteWindow()
        {
            InitializeComponent();
        }
               
    

       

        private void BtnFont_Click(object sender, EventArgs e)
        {
            DialogResult result = fontDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {

                frmMain.Font = fontDialog1.Font;
                Properties.Settings.Default.Font = fontDialog1.Font;
                Properties.Settings.Default.Save();
                frmMain.dgdAgenda.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }


        }

        private void BtnCuloare_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                
                frmMain.BackColor = colorDialog1.Color;
                Properties.Settings.Default.BackgroundColor = colorDialog1.Color;
                Properties.Settings.Default.Save();
            }
        }
    }
}
