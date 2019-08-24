using mr_steve;
using System.Collections.Generic;
using System.Windows.Forms;

public class settings {

    public Launch Launch { get; set; }
    public httpGET httpGet { get; set; }

    public Proccess Proccess { get; set; }

    public List<HID> HID { get; set; }
}



public abstract class HID {
    public long Time { get; set; }
}


public class Mouse : HID {
    public MouseStatus Status { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
   
}

public class Key : HID {
    public Keys KeyValue { get; set; }
    public KeyStatus Status { get; set; }
   
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