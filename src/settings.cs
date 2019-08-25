using mr_steve;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;








public class settings {

    public settings() {

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
        string json = JsonConvert.SerializeObject(s );
        //write string to file
        System.IO.File.WriteAllText(filePath, json);
    }
    public static settings load(string filePath) {
        return JsonConvert.DeserializeObject<settings>(File.ReadAllText(filePath), new HIDConverter() );
    }
}








public class Proccess {


    public string Close { get; set; }
}


public class httpGET {
    public bool Enable { get; set; }
    public string URL { get; set; }
    public ComparisonOperator Operator { get; set; }
    public string value { get; set; }
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
}


public class LaunchSleep {
    //sleep
    public bool Enable { get; set; }
    public int Minutes { get; set; }

}
public class LaunchTiming {

    //time
    public bool Enable { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Second { get; set; }
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

        res.Time = long.Parse(getValue(jObject, "Time"));
        //,{"KeyValue":32,"Status":257,"Time":1881}]}
        //"Status":513,"PosX":707,"PosY":408,"Time":3286}

        return res;
    }

    private static string getValue(JObject j, string PropertyName) {
        JToken jt;
        if (!j.TryGetValue(PropertyName, out jt)) return string.Empty;
        return jt.Value<string>();
    }



}