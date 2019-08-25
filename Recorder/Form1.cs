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

           
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            dataGridInit();
            HIDHook.MouseAction += MouseHook_MouseAction;
            HIDHook.KeyboardAction += HIDHook_KeyboardAction;
        }
        private void  dataGridInit() {

            this.dataGridView1.Columns.Add("#", "#");
            this.dataGridView1.Columns.Add("status", "Status");
            this.dataGridView1.Columns.Add("posx", "Mouse X/Keyb");
            this.dataGridView1.Columns.Add("posy", "Mouse Y");
            this.dataGridView1.Columns.Add("time", "Time ( ms )");
            this.dataGridView1.Columns["status"].Width = 200;


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

        public  void  settings_to_UI(settings res) {
 
            //sleep 

           checkBox2.Checked = res.Launch.Sleep.Enable;
         numericUpDown1.Value = res.Launch.Sleep.Minutes ;
            //time

          checkBox1.Checked = res.Launch.Timing.Enable ;
           numericUpDown4.Value = res.Launch.Timing.Hour ;
           numericUpDown3.Value = res.Launch.Timing.Minute ;
          numericUpDown2.Value = res.Launch.Timing.Second ;
            //http 

          checkBox3.Checked = res.httpGet.Enable ;
            textBox1.Text = res.httpGet.URL ;
           comboBox1.SelectedIndex =(int)  res.httpGet.Operator ;
             textBox2.Text = res.httpGet.value ;
            //proccess 
           textBox4.Text = res.Proccess.Close ;



            dataGridView1.Rows.Clear();



            for (int i = 0; i < res.HID.Count; i++) {


                if (res.HID[i].GetType() == typeof(Mouse)) {
                    Mouse m = (Mouse)res.HID[i];
                    this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, m.Status, m.PosX.ToString(), m.PosY.ToString(), m.Time);
                   
                } else if (res.HID[i].GetType() == typeof(Key)) {
                    Key k = (Key)res.HID[i];
                    this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, k.Status, k.KeyValue, string.Empty, k.Time);
                }
          
            }


          
        }


        private settings UI_to_settings() {

            settings res = new settings();

            //sleep 

            res.Launch.Sleep.Enable = checkBox2.Checked;
            res.Launch.Sleep.Minutes = (int)numericUpDown1.Value;
            //time

            res.Launch.Timing.Enable = checkBox1.Checked;
            res.Launch.Timing.Hour = (int)numericUpDown4.Value;
            res.Launch.Timing.Minute = (int)numericUpDown3.Value;
            res.Launch.Timing.Second = (int)numericUpDown2.Value;
            //http 

            res.httpGet.Enable = checkBox3.Checked;
            res.httpGet.URL = textBox1.Text;
            res.httpGet.Operator = (ComparisonOperator)comboBox1.SelectedIndex;
            res.httpGet.value = textBox2.Text;
            //proccess 

            res.Proccess.Close = textBox4.Text;

            //HID 
            res.HID = new List<HID>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++) {

                if (dataGridView1.Rows[i].Cells["posx"].Value == null) continue;

                string posy = (string)dataGridView1.Rows[i].Cells["posy"].Value;
                HID temp = null;
                if (string.IsNullOrEmpty(posy)) {//keyboard
                    Key k = new Key();
                    k.KeyValue = (Keys)dataGridView1.Rows[i].Cells["posx"].Value;
                    k.Status = (KeyStatus)dataGridView1.Rows[i].Cells["status"].Value;
                    temp = k;
                } else {//mouse 
                    Mouse m = new Mouse();
                    m.PosX = int.Parse((string)dataGridView1.Rows[i].Cells["posx"].Value);
                    m.PosY = int.Parse((string)dataGridView1.Rows[i].Cells["posy"].Value);
                    m.Status = (MouseStatus)dataGridView1.Rows[i].Cells["status"].Value;
                    temp = m;
                }
                temp.Time = (long)dataGridView1.Rows[i].Cells["time"].Value;
                res.HID.Add(temp);
            }
            return res;

        }



        public static  string getFolder() {
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
            string res = Path.Combine(getFolder() , f.fileName );
            settings s = settings.load(res);
            this.settings_to_UI(s);
        }
    }
}


