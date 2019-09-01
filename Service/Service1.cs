using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace Service {
    public partial class Service1 : ServiceBase {

        Timer timer = new Timer();


        public Service1() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {

            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 300000; //number in milisecinds  
            timer.Interval = 3000;

            timer.Enabled = true;


        }

        protected override void OnStop() {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e) {
            WriteToFile("Service is recall at " + DateTime.Now);


            Point PrevMousePos = new Point();
            DateTime time = DateTime.Now;
            getPrevSettings(ref PrevMousePos , ref time );

            DirectoryInfo d = new DirectoryInfo(settings.getFolder());
            FileInfo[] Files = d.GetFiles("*.mrsteve"); //Getting Text files
    
            foreach (FileInfo file in Files) {
                string res = Path.Combine(settings.getFolder(), file.Name);
                settings s = settings.load(res);
                s.play(PrevMousePos, time, true );
                WriteToFile("play" + file.Name);
            }

            setPrevMousePos(HID.MousePos , DateTime.Now);
        }


        private void  getPrevSettings(ref  Point   Cur , ref DateTime   time ) {

       
            string filePath = Path.Combine(settings.getFolder(), "mrsteve_settings.json");

            if ( File.Exists(filePath)) {
                List<object> data = new List<object>();
                data =  JsonConvert.DeserializeObject<List<object>>(File.ReadAllText(filePath) );

                Cur =(Point) data[0];
                time = (DateTime)  data[1];

            }
         

        }
        private void  setPrevMousePos(Point Cur, DateTime time) {
            string filePath = Path.Combine(settings.getFolder(), "mrsteve_settings.json");
            File.Delete(filePath);

            List<object> data = new List<object>();
            data.Add(Cur);
            data.Add(time);
            string json = JsonConvert.SerializeObject(data);
            System.IO.File.WriteAllText(filePath, json);
        }


        private string Root { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        public void WriteToFile(string Message) {
            string filePath = Path.Combine(settings.getFolder(), "mrsteve_log.txt");
            if (!File.Exists(filePath)) {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filePath)) {
                    sw.WriteLine(Message);
                }
            } else {
                using (StreamWriter sw = File.AppendText(filePath)) {
                    sw.WriteLine(Message);
                }
            }
        }


    }
}
