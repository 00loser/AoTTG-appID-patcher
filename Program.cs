using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Memory;
using System.Diagnostics;
using System.IO;

class Program
{
    static Mem memoryEditor = new Mem();
    static readonly string appID_bytes = "66 00 31 00 66 00 36 00 31 00 39 00 35 00 63 00 2D 00 64 00 66 00 34 00 61 00 2D 00 34 00 30 00 66 00 39 00 2D 00 62 00 61 00 65 00 35 00 2D 00 34 00 37 00 34 00 34 00 63 00 33 00 32 00 39 00 30 00 31 00 65 00 66 00"; //old app ID bytes in hex 
    static readonly string newAppID = "5578b046-8264-438c-99c5-fb15c71b6744";
    static readonly string oldAppID = "f1f6195c-df4a-40f9-bae5-4744c32901ef";

    //this program needs to be runned as admin so that Memory.dll can read/write our process memory
    static void Main(string[] args) => EditMemory(); //pls do it this way so i wont feel like i wasted my time understanding how to hack program's memory

    //static void Main(string[] args) => WriteFromFile(args[0]);

    static void EditMemory()
    {
        Process aottg = (from proc in Process.GetProcesses() where proc.MainWindowTitle == "ATTACK_ON_TITAN" select proc).FirstOrDefault(); //get aottg process by process window title
        while (aottg == null)
        {
            Console.WriteLine("No process found");
            Thread.Sleep(10000); //wait 10s and try again
            aottg = (from proc in Process.GetProcesses() where proc.MainWindowTitle == "ATTACK_ON_TITAN" select proc).FirstOrDefault();
        }
        Console.WriteLine("Process found!");
        memoryEditor.OpenProcess(aottg.Id); //attach our memory editor to the process using its PID

        Task<IEnumerable<long>> scan = memoryEditor.AoBScan(appID_bytes, true, false); //scans for an array of bytes in memory. we want to get old appID address and modify it so we gotta scan for appID bytes
        scan.Wait(); //wait for task to finish
        foreach (long address in scan.Result) //now lets try to write the new appID in the addresses we got
        {
            //string prev = Encoding.Unicode.GetString(memoryEditor.ReadBytes(address.ToHex(), Encoding.Unicode.GetBytes(oldAppID).Length));
            if (!memoryEditor.WriteMemory(address.ToHex(), "string", newAppID, "", Encoding.Unicode))
                Console.WriteLine($"Failed writing address {address.ToHex()}");
            else
                Console.WriteLine($"Done! Written new AppID in address {address.ToHex()}");
                //Console.WriteLine($"Done! Address {address.ToHex()} written\n(old value: {prev} | updated value: {Encoding.Unicode.GetString(memoryEditor.ReadBytes(address.ToHex(), Encoding.Unicode.GetBytes(newAppID).Length))})");
        }
        Console.WriteLine("You should be able to connect to new servers now");
        memoryEditor.CloseProcess();
        Console.ReadKey();
    }

    static void WriteFromFile(string path)
    {
        byte[] toreplace = Encoding.Unicode.GetBytes(oldAppID); //constant string vars in our binary file are encoded using unicode so we gotta get old appID unicode bytes
        byte[] replace = Encoding.Unicode.GetBytes(newAppID); //get unicode bytes of new appID
        File.WriteAllBytes(path, Utils.ReplaceBytes(File.ReadAllBytes(path), toreplace, replace)); //then write those bytes
    }
}
