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
using Microsoft.VisualBasic.Devices;

namespace mr_steve {
    public partial class Form1 : Form {
         
        public Form1() {
            InitializeComponent();

            DataGridBoolColumn myDataCol = new DataGridBoolColumn();
            myDataCol.HeaderText = "My New Column";
            myDataCol.MappingName = "Current";


            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            dataGridInit();
            HID.MouseAction += MouseHook_MouseAction;
            HID.KeyboardAction += HIDHook_KeyboardAction;
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

            string filePath = Path.Combine(getFolder(), textBox5.Text + ".mrsteve");

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

      

        public static string getFolder() {
            string res = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "Mr_Steve");
            if (!File.Exists(res))
                System.IO.Directory.CreateDirectory(res);
            return res;
        }

        private void Form1_Load(object sender, EventArgs e) {


            textBox3.Text = getFolder();
        }

        private void Button5_Click(object sender, EventArgs e) {

            Process.Start(getFolder());
        }

        private void Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void Button3_Click(object sender, EventArgs e) {
            Filelist f = new Filelist(this);
            //   f.ShowDialog();
            f.ShowDialog();
            if (string.IsNullOrEmpty(f.fileName)) return;
            string res = Path.Combine(getFolder(), f.fileName);
            settings s = settings.load(res);
            this.settings_to_UI(s);
        }

        private void Button6_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            settings s = UI_to_settings();
            s.play();
            this.WindowState = FormWindowState.Normal;
        }

    }
}


