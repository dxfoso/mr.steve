using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
namespace mr_steve {
    public partial class Form1 : Form {
        string documentPath; 
        Mutex checking = new Mutex(false);
        AutoResetEvent are = new AutoResetEvent(false);

        public Form1() {
            InitializeComponent();

            DataGridBoolColumn myDataCol = new DataGridBoolColumn();
            myDataCol.HeaderText = "My New Column";
            myDataCol.MappingName = "Current";

            this.dataGridView1.Columns.Add("#", "#");
            this.dataGridView1.Columns.Add("status", "Status");

            this.dataGridView1.Columns.Add("posx", "Mouse X/Keyb");
            this.dataGridView1.Columns.Add("posy", "Mouse Y");
            this.dataGridView1.Columns.Add("time", "Time ( ms )");
            this.dataGridView1.Columns["status"].Width = 200;
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            HIDHook.MouseAction += MouseHook_MouseAction;
            HIDHook.KeyboardAction += HIDHook_KeyboardAction;
        }

        private void HIDHook_KeyboardAction(object sender, KeyboardEventArgs e) {
            if (e.Key == Keys.Escape) {

                HIDHook.stop();

                this.WindowState = FormWindowState.Normal;
                return;
            }

            this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, e.Status, e.Key, string.Empty, e.Time);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        private void MouseHook_MouseAction(object sender, MouseEventArgs e) {
            this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, e.MouseStatus, e.MousePosX.ToString(), e.MousePosY.ToString(), e.Time);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        private void Button1_Click(object sender, EventArgs e) {



            Button b = ((Button)sender);

            this.WindowState = FormWindowState.Minimized;

            HIDHook.Start();


        }
        private void Button2_Click(object sender, EventArgs e) {
            Interaction.InputBox("Please Enter File name", "File Name", "");
        }

        private void Form1_Load(object sender, EventArgs e) {
            documentPath =  Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "Mr_Steve" );


            textBox3.Text = documentPath;
        }

        private void Button5_Click(object sender, EventArgs e) {
            System.IO.Directory.CreateDirectory(documentPath);
            Process.Start(documentPath);
        }

        private void Button4_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}


