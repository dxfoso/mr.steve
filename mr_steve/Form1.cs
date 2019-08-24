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
            var filepath = Interaction.InputBox("Please Enter File name", "File Name", "");

            if (string.IsNullOrEmpty(textBox5.Text)) {
                MessageBox.Show("please enter file name ");
                return;
            }


            settings res = new settings();
            res.Launch = new Launch();
            //sleep 
            res.Launch.Sleep = new LaunchSleep();
            res.Launch.Sleep.Enable = checkBox2.Checked;
            res.Launch.Sleep.Minutes = (int)numericUpDown1.Value;
            //time
            res.Launch.Timing = new LaunchTiming();
            res.Launch.Timing.Enable = checkBox1.Checked;
            res.Launch.Timing.Hour = (int)numericUpDown4.Value;
            res.Launch.Timing.Minute = (int)numericUpDown3.Value;
            res.Launch.Timing.Second = (int)numericUpDown2.Value;
            //http 
            res.httpGet = new httpGET();
            res.httpGet.Enable = checkBox3.Checked;
            res.httpGet.URL = textBox1.Text;
            res.httpGet.Operator = (ComparisonOperator)comboBox1.SelectedIndex;
            res.httpGet.value = textBox2.Text;
            //proccess 
            res.Proccess = new Proccess();
            res.Proccess.Close = textBox4.Text;

            //HID 
            res.HID = new List<HID>();
            for (int i = 1; i < dataGridView1.Rows.Count-1; i++) {
                string posy =(string) dataGridView1.Rows[i].Cells["posy"].Value ;
                HID temp = null ; 
                if (string.IsNullOrEmpty(posy)) {//keyboard
                    Key k = new Key();
                    k.KeyValue =(Keys) dataGridView1.Rows[i].Cells["posx"].Value; 
                    k.Status = (KeyStatus) dataGridView1.Rows[i].Cells["status"].Value;
                    temp = k;
                } else {//mouse 
                    Mouse m = new Mouse();
                    m.PosX = int.Parse((string)dataGridView1.Rows[i].Cells["posx"].Value);
                    m.PosY = int.Parse((string)dataGridView1.Rows[i].Cells["posy"].Value);
                    m.Status  = (MouseStatus)dataGridView1.Rows[i].Cells["status"].Value;
                    temp = m;
                }
                temp.Time = (long)dataGridView1.Rows[i].Cells["time"].Value;
                res.HID.Add(temp);
            }




        }

        private void Form1_Load(object sender, EventArgs e) {
            documentPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "Mr_Steve");


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


