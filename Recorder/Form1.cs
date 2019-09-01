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

using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;

namespace mr_steve {
    public partial class Form1 : Form {
        private Timer timer;

        private Point PrevMousePos;
        private DateTime PrevMousetime;


        public Form1() {
            InitializeComponent();

            DataGridBoolColumn myDataCol = new DataGridBoolColumn();
            myDataCol.HeaderText = "My New Column";
            myDataCol.MappingName = "Current";


            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            dataGridInit();
            HID.MouseAction += MouseHook_MouseAction;
            HID.KeyboardAction += HIDHook_KeyboardAction;
            PrevMousePos = HID.MousePos;
            PrevMousetime = DateTime.Now;
            this.FormClosing += Form1_FormClosing;
            this.Resize += Form1_Resize;
            notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += NotifyIcon1_MouseDoubleClick;
            toolStripMenuItem1.Click += ToolStripMenuItem1_Click;
        }
        private bool isclose = false;
        private void ToolStripMenuItem1_Click(object sender, EventArgs e) {
            isclose = true;
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (isclose) return;
            e.Cancel = true;
            Hide();
          
            return ;

          var window = MessageBox.Show(
       "Close the window?",
       "Are you sure?",
       MessageBoxButtons.YesNo);

            e.Cancel = (window == DialogResult.No);
        }


        private void NotifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void dataGridInit() {

            this.dataGridView1.Columns.Add("#", "#");
            this.dataGridView1.Columns.Add("status", "Status");
            this.dataGridView1.Columns.Add("posx", "Mouse X/Keyb");
            this.dataGridView1.Columns.Add("posy", "Mouse Y");
            this.dataGridView1.Columns.Add("time", "Time ( ms )");
            this.dataGridView1.Columns["status"].Width = 200;


        }
        private void HIDHook_KeyboardAction(object sender, KeyboardEventArgs e) {
            var E = e.KeyInput;
            if (E.Key == Keys.Escape) {

                HID.stop();

                this.WindowState = FormWindowState.Normal;
                return;
            }

            this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, E.Status, E.Key, string.Empty, E.Time);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        private void MouseHook_MouseAction(object sender, MouseEventArgs e) {

            var E = e.MouseInput;

            this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, E.Status, E.PosX.ToString(), E.PosY.ToString(), E.Time);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        private void Button1_Click(object sender, EventArgs e) {



            Button b = ((Button)sender);

            this.WindowState = FormWindowState.Minimized;

            HID.Start();


        }
        private void Button2_Click(object sender, EventArgs e) {

            if (string.IsNullOrEmpty(textBox5.Text)) {
                MessageBox.Show("please enter file name ");
                return;
            }

            settings s = UI_to_settings();

            string filePath = Path.Combine(settings.getFolder(), textBox5.Text + ".mrsteve");

            if (File.Exists(filePath)) {



                DialogResult dialogResult = MessageBox.Show("Are you Sure", textBox5.Text + " overwrite ? ", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes) {

                } else if (dialogResult == DialogResult.No) {
                    return;
                }




            }

            MessageBox.Show("Save Completed !");
            settings.save(s, filePath);



        }





        private void Form1_Load(object sender, EventArgs e) {

            textBox3.Text = settings.getFolder();
        }

        private void Button5_Click(object sender, EventArgs e) {

            Process.Start(settings.getFolder());
        }

        private void Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void Button3_Click(object sender, EventArgs e) {
            Filelist f = new Filelist(this);
            //   f.ShowDialog();
            f.ShowDialog();
            if (string.IsNullOrEmpty(f.fileName)) return;
            string res = Path.Combine(settings.getFolder(), f.fileName);
            settings s = settings.load(res);
            this.settings_to_UI(s);
            textBox5.Text = f.fileName.Substring(0, f.fileName.Length - 8);
        }

        private void Button6_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            settings s = UI_to_settings();
            DateTime t = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute - 1, 0);


            bool res = s.play(true);
            this.WindowState = FormWindowState.Normal;
        }

        private void Button5_Click_1(object sender, EventArgs e) {
            Process.Start(settings.getFolder());
        }

        private void Button7_Click(object sender, EventArgs e) {
            Button b = (Button)sender;

            if (b.Text == "Run") {

                play();



                b.Text = "Stop";
                this.WindowState = FormWindowState.Minimized;
            } else if (b.Text == "Stop") {
                b.Text = "Run";
                set = new List<settings>();
            }

        }
        private List<settings> set;
        private void play() {
            set = new List<settings>();


            DirectoryInfo d = new DirectoryInfo(settings.getFolder());
            FileInfo[] Files = d.GetFiles("*.mrsteve"); //Getting Text files

            foreach (FileInfo file in Files) {
                string res = Path.Combine(settings.getFolder(), file.Name);
                settings s = settings.load(res);
                s.play(false);
                set.Add(s);
            }

            /*
             if (PrevMousePos != HID.MousePos) {
                 PrevMousePos = HID.MousePos;
                 PrevMousetime = DateTime.Now;
             }*/
        }



    }
}


