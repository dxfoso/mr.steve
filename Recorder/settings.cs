using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mr_steve {
    public partial class Form1 : Form{



        public void settings_to_UI(settings res) {

            //sleep 

            checkBox2.Checked = res.Launch.Sleep.Enable;
            numericUpDown1.Value = res.Launch.Sleep.Minutes;
            //time

            checkBox1.Checked = res.Launch.Timing.Enable;
            numericUpDown4.Value = res.Launch.Timing.Hour;
            numericUpDown3.Value = res.Launch.Timing.Minute;
            numericUpDown2.Value = res.Launch.Timing.Second;
            //http 

            checkBox3.Checked = res.httpGet.Enable;
            textBox1.Text = res.httpGet.URL;
            comboBox1.SelectedIndex = (int)res.httpGet.Operator;
            textBox2.Text = res.httpGet.value;
            //proccess 
            textBox4.Text = res.Proccess.Close;
            checkBox4.Checked = res.Proccess.Enable;

            dataGridView1.Rows.Clear();



            for (int i = 0; i < res.HID.Count; i++) {


                if (res.HID[i].GetType() == typeof(HIDMouseInput)) {
                    HIDMouseInput m = (HIDMouseInput)res.HID[i];
                    this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, m.Status, m.PosX.ToString(), m.PosY.ToString(), m.Time);

                } else if (res.HID[i].GetType() == typeof(HIDKeyInput)) {
                    HIDKeyInput k = (HIDKeyInput)res.HID[i];
                    this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count, k.Status, k.Key, string.Empty, k.Time);
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
            res.Proccess.Enable =  checkBox4.Checked ;
            res.Proccess.Close = textBox4.Text;

            //HID 
            res.HID = new List<HIDInput>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++) {
                HIDInput temp = dataGridRow_to_HIDInput(dataGridView1, i);
                if (temp == null) continue;
                res.HID.Add(temp);
            }
            return res;
        }

        private static HIDInput dataGridRow_to_HIDInput(DataGridView dv, int row) {

            if (dv.Rows[row].Cells["posx"].Value == null) return null;

            string posy = (string)dv.Rows[row].Cells["posy"].Value;
            HIDInput temp = null;
            if (string.IsNullOrEmpty(posy)) {//keyboard
                HIDKeyInput k = new HIDKeyInput();
                k.Key = (Keys)dv.Rows[row].Cells["posx"].Value;
                k.Status = (KeyStatus)dv.Rows[row].Cells["status"].Value;
                temp = k;
            } else {//mouse 
                HIDMouseInput m = new HIDMouseInput();
                m.PosX = int.Parse((string)dv.Rows[row].Cells["posx"].Value);
                m.PosY = int.Parse((string)dv.Rows[row].Cells["posy"].Value);
                m.Status = (MouseStatus)dv.Rows[row].Cells["status"].Value;
                temp = m;
            }
            temp.Time =int .Parse(dv.Rows[row].Cells["time"].Value.ToString());
            return temp;
        }
    }
}
