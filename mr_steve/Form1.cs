using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mr_steve {
    public partial class Form1 : Form {

        Mutex checking = new Mutex(false);
        AutoResetEvent are = new AutoResetEvent(false);

        public Form1() {
            InitializeComponent();

            DataGridBoolColumn myDataCol = new DataGridBoolColumn();
            myDataCol.HeaderText = "My New Column";
            myDataCol.MappingName = "Current";

            this.dataGridView1.Columns.Add("#", "#");
            this.dataGridView1.Columns.Add("status", "Status");
            this.dataGridView1.Columns.Add("posx", "Mouse X");
            this.dataGridView1.Columns.Add("posy", "Mouse Y");
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            HIDHook.MouseAction += MouseHook_MouseAction;
            HIDHook.KeyboardAction += HIDHook_KeyboardAction;
        }

        private void HIDHook_KeyboardAction(object sender, KeyboardEventArgs e) {
            this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, e.Status, e.Key,string.Empty);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        private void MouseHook_MouseAction(object sender, MouseEventArgs e) {
            this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, e.MouseStatus, e.MousePosX.ToString(), e.MousePosY.ToString());
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        private void Button1_Click(object sender, EventArgs e) {
            if (((Button)sender).Text == "Record") {
                this.Text = "Stop";
                HIDHook.Start();
                
            } else if (this.Text == "Stop") {
                this.Text = "Record";
                HIDHook.stop();
            }
        }
        private void Button2_Click(object sender, EventArgs e) {

        }
    }
}


