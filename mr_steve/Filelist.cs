using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mr_steve {
    public partial class Filelist : Form {
        private Form1 _form;

        public string fileName { get; set; }
        public Filelist(  Form1 form) {
            InitializeComponent();
            _form = form;
            fileName = string.Empty;
        }

        private void Filelist_Load(object sender, EventArgs e  ) {
            DirectoryInfo d = new DirectoryInfo(Form1.getFolder());

            FileInfo[] Files = d.GetFiles("*.mrsteve"); //Getting Text files
            string str = "";
            listBox1.Items.Clear();

            foreach (FileInfo file in Files) {
                listBox1.Items.Add(file.Name);
            }

           

        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e) {
          
        }

        private void Button1_Click(object sender, EventArgs e) {
            fileName =(string) listBox1.SelectedItem;
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void ListBox1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            fileName = (string)listBox1.SelectedItem;
            this.Close();
        }
    }
}
