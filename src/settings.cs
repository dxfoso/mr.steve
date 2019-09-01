
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;


public class settings {
    
    private Timer timer;

    public settings() {
        timer = new Timer();
        timer.Tick += Timer_Tick;
    
        Launch = new Launch();
        //sleep 
        Launch.Sleep = new LaunchSleep();
        //time
        Launch.Timing = new LaunchTiming();
        //http 
        httpGet = new httpGET();
        //proccess 
        Proccess = new Proccess();

    }


    public Launch Launch { get; set; }
    public httpGET httpGet { get; set; }

    public Proccess Proccess { get; set; }

    public List<HIDInput> HID { get; set; }

    public static void save(settings s, string filePath) {
        string json = JsonConvert.SerializeObject(s);
        //write string to file
        System.IO.File.WriteAllText(filePath, json);
    }
    public static settings load(string filePath) {
        return JsonConvert.DeserializeObject<settings>(File.ReadAllText(filePath), new HIDConverter());
    }

    public bool play(bool PlayNow = true) {
      
        if (!PlayNow) {
            Launch.Sleep.reset();
            timer.Interval = Launch.Sleep.Minutes * 60 * 1000;
            timer.Stop();
            timer.Start();
        } else
            _play();

        return true;
    }
    private void _play() {
        this.Proccess.Play();
        foreach (HIDInput i in HID)
            i.Play();

    }
    private void Timer_Tick(object sender, EventArgs e) {

        if (!Launch.Play(timer.Interval)) return;
        if (!httpGet.Play()) return;

        _play();
        this.Launch.Sleep.resetMouse();
    }

    public static string getFolder() {
        string res = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "Mr_Steve");
        if (!File.Exists(res))
            System.IO.Directory.CreateDirectory(res);
        return res;
    }


}

public class Proccess {

    public bool Enable { get; set; }
    public string Close { get; set; }


    public void Play() {

        if (!string.IsNullOrEmpty(Close) && Enable) {
            try {
                Process[] proc = Process.GetProcessesByName(Close);
                proc[0].Kill();
            } catch {
            }
        }
    }
}


public class httpGET {
    public bool Enable { get; set; }
    public string URL { get; set; }
    public ComparisonOperator Operator { get; set; }
    public string value { get; set; }

    public bool Play() {
        if (!Enable) return true;
        try {
            string response = Get(URL);
            switch (Operator) {
                case ComparisonOperator.Equal:
                    return response == value;
                case ComparisonOperator.Larger:
                    return int.Parse(response) > int.Parse(value);
                case ComparisonOperator.Larger_Equal:
                    return int.Parse(response) >= int.Parse(value);
                case ComparisonOperator.Smaller:
                    return int.Parse(response) < int.Parse(value);
                case ComparisonOperator.Smaller_Equal:
                    return int.Parse(response) <= int.Parse(value);
            }
        } catch { }
        return false;
    }

    private string Get(string uri) {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream)) {
            return reader.ReadToEnd();
        }
    }
}


public enum ComparisonOperator {
    Equal = 0,
    Smaller = 1,
    Larger = 2,
    Smaller_Equal = 3,
    Larger_Equal = 4
}
public class Launch {
    public LaunchSleep Sleep { get; set; }
    public LaunchTiming Timing { get; set; }

    public bool Play(int span) {
        bool b1 = Sleep.Play(span);
        bool b2 = Timing.Play();
        return b1 || b2;
    }
}


public class LaunchSleep {

    private Point PrevMousePos;

    private bool hasPlayed;
    public LaunchSleep() {
        hasPlayed = false;
    }
    public bool Enable { get; set; }
    public int Minutes { get; set; }


    public void reset() {
        resetMouse();
        hasPlayed = false;

    }
    public void resetMouse() {
        PrevMousePos.X = HID.MousePos.X;
        PrevMousePos.Y = HID.MousePos.Y;

    }

    public bool Play(int span) {


        bool res = true;

        if (!Enable) res = false;

        else if (PrevMousePos != HID.MousePos) {
            reset();
            res = false;
            Debug.WriteLine("rest  ");

        }

        if (hasPlayed) return false;
        hasPlayed = res; ;


        Debug.WriteLine("run........");
        return res;
    }
}
public class LaunchTiming {

    //time
    public bool Enable { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Second { get; set; }

    public DateTime DateTime {
        get {
            DateTime n = DateTime.Now;
            return new DateTime(n.Year, n.Month, n.Day, Hour, Minute, Second);
        }
    }
    public bool Play( ) {
        if (!Enable) return false;
        DateTime s = this.DateTime;


        return false;
        TimeSpan span = DateTime.Now.Subtract(s);
       
      //  if (totalMinutes <= settings.Interval && totalMinutes >= 0) return true;
        return false;
    }
}



public class HIDConverter : JsonConverter<HIDInput> {

    public override void WriteJson(JsonWriter writer, HIDInput value, JsonSerializer serializer) {
        // base.WriteJson(writer, value, serializer);

        //      if (value.GetType() == typeof(Mouse))
        serializer.Serialize(writer, (HIDMouseInput)value);


        //    else if (value.GetType() == typeof(Key))
        //      serializer.Serialize(writer, (Key)value);





    }
    public override HIDInput ReadJson(JsonReader reader, Type objectType, HIDInput existingValue, bool hasExistingValue, JsonSerializer serializer) {
        HIDInput res = null;

        JObject jObject = JObject.Load(reader);


        if (string.IsNullOrEmpty(getValue(jObject, "PosX"))) {//key
            HIDKeyInput temp = new HIDKeyInput();
            temp.Key = (Keys)int.Parse(getValue(jObject, "Key"));
            temp.Status = (KeyStatus)int.Parse(getValue(jObject, "Status"));
            res = temp;
        } else {//mouse 
            HIDMouseInput temp = new HIDMouseInput();
            temp.PosX = int.Parse(getValue(jObject, "PosX"));
            temp.PosY = int.Parse(getValue(jObject, "PosY"));
            temp.Status = (MouseStatus)int.Parse(getValue(jObject, "Status"));
            res = temp;
        }

        res.Time = int.Parse(getValue(jObject, "Time"));
        return res;
    }

    private static string getValue(JObject j, string PropertyName) {
        JToken jt;
        if (!j.TryGetValue(PropertyName, out jt)) return string.Empty;
        return jt.Value<string>();
    }



}