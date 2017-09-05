using Gma.System.MouseKeyHook;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Phantom_Clicker
{
    public partial class Form1 : MaterialForm
    {
        int truefalsetest = 0;

        bool newHeld = false;
        bool fakeHeld = false;

        string on = "On";
        string off = "Off";

        bool constant = false;
        bool clickedup = false;
        int clicks = 0;
        string HWID = null;
        string ver = "1.2.3 ";
        string togglekey;
        string explodekey;
        bool explode;
        bool justmc = true;
        bool rightclick = true;
        bool toggled;
        bool holdingright;
        MaterialSkinManager skinManager = MaterialSkinManager.Instance;
        private IKeyboardMouseEvents m_GlobalHook;

        enum VirtualKeyStates : int
        {
            VK_LBUTTON = 0x01,
            VK_RBUTTON = 0x02,
        }

            [DllImport("user32.dll")]
        static extern short GetKeyState(VirtualKeyStates nVirtKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public Form1()
        {
            MouseHook.Start();
            MouseHook.MouseAction += new EventHandler(Event);
            InitializeComponent();
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            //skinManager.ColorScheme = new ColorScheme(Primary.Orange800, Primary.DeepOrange900, Primary.DeepOrange500, Accent.Orange200, TextShade.WHITE);
            globalHooks();
        }
        string licensedto;

        private void Form1_Load(object sender, EventArgs e)
        {
                button1.Focus();
            metroLabel10.Text = "v" + ver;

            string drive = "C";
            if (drive == string.Empty)
            {
                //Find first drive
                foreach (DriveInfo compDrive in DriveInfo.GetDrives())
                {
                    if (compDrive.IsReady)
                    {
                        drive = compDrive.RootDirectory.ToString();
                        break;
                    }
                }
            }

            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            dsk.Get();
            string volumeSerial = dsk["VolumeSerialNumber"].ToString();
            HWID = volumeSerial;

            string response = get("https://phantomclicker.us/check.php?hwid=" + HWID);
            if (response == null)
            {
                metroLabel9.Text = "Unable to connect to PhantomClicker.us." + response;
                this.Close();
            }
            if (response != "?DENY")
            {

                licensedto = response;
            }
            
            if (response == "?DENY")
            {
                MessageBox.Show("Invalid HWID/IP combination. Update your license and redownload.");
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " +
                               Application.ExecutablePath;
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
                Close();
            }
            applySettingsString(get("http://phantomclicker.us/getSettings.php?hwid=" + HWID));
            updateLanguage();

        }

        private void metroLabel2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void globalHooks()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyDown += GlobalHookKeyDown;
            m_GlobalHook.MouseDown += GlobalHookMouseDown;
            m_GlobalHook.MouseUp += GlobalHookMouseUp;
            m_GlobalHook.MouseDoubleClick += GlobalHookMouseDoubleClick;
            m_GlobalHook.MouseDragStarted += GlobalHookMouseDown;
            m_GlobalHook.MouseDragFinished += GlobalHookMouseUp;

        }
        public void Unsubscribe()
        {
            m_GlobalHook.KeyDown -= GlobalHookKeyDown;
            m_GlobalHook.MouseDown -= GlobalHookMouseDown;
            m_GlobalHook.MouseUp -= GlobalHookMouseUp;
            m_GlobalHook.MouseDoubleClick -= GlobalHookMouseDoubleClick;
            m_GlobalHook.MouseDragStarted -= GlobalHookMouseDown;
            m_GlobalHook.MouseDragFinished -= GlobalHookMouseUp;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }

        private void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == materialRaisedButton2.Text)
            {
                metroToggle1.Checked = !metroToggle1.Checked;
                if (metroToggle1.Checked)
                {
                    timer1.Start();
                }else
                {
                    timer1.Stop();
                }
            }
            if (e.KeyCode.ToString() == materialRaisedButton3.Text)
            {
                if (explode)
                {
                    Explode();
                }
            }
        }

        string explodeMessage;
        private void Explode()
        {
            try{
                if (materialCheckBox4.Checked)
                {
                    DialogResult dlg = MessageBox.Show(explodeMessage, "Phantom Client", MessageBoxButtons.YesNo);
                    if (dlg == DialogResult.Yes)
                    {

                    }
                    if (dlg == DialogResult.No)
                    {
                        return;
                    }
                }

                if (materialCheckBox3.Checked)
                {
                    ProcessStartInfo Info = new ProcessStartInfo();
                    Info.Arguments = "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del " + Application.ExecutablePath;
                    Info.WindowStyle = ProcessWindowStyle.Hidden;
                    Info.CreateNoWindow = true;
                    Info.FileName = "cmd.exe";
                    Process.Start(Info);
                }
                if (materialCheckBox5.Checked)
                {
                    string[] allFiles = System.IO.Directory.GetFiles("C:\\Windows\\Prefetch");
                    foreach (string file in allFiles)
                    {
                        if (file.ToUpper().Contains(Path.GetFileName(Application.ExecutablePath).ToUpper()))
                        {
                            ProcessStartInfo Info = new ProcessStartInfo();
                            Info.Arguments = "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del " + file;
                            Info.WindowStyle = ProcessWindowStyle.Hidden;
                            Info.CreateNoWindow = true;
                            Info.FileName = "cmd.exe";
                            Process.Start(Info);
                        }
                    }
                    string nel = get("http://phantomclicker.us/updateClicks.php?hwid=" + HWID + "&clicks=" + clicks);
                }

                if (materialCheckBox6.Checked)
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();

                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{9E04CAB2-CC14-11DF-BB8C-A2F1DED72085}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");

                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{A3D53349-6E61-4557-8FC7-0028EDCEEBF6}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");
                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{B267E3AD-A825-4A09-82B9-EEC22AA3B847}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");
                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{BCB48336-4DDD-48FF-BB0B-D3190DACB3E2}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");
                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{CAA59E3C-4792-41A5-9909-6A6A8D32490E}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");
                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{CEBFF5CD-ACE2-4F4F-9178-9926F41749EA}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");
                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{F2A1CB5A-E3CC-4A2E-AF9D-505A7009D442}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");
                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{F4E57C4B-2036-45F0-A9AB-443BCFE33D9F}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");
                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\{FA99DFC7-6AC2-453A-A5E2-5E2AFF4507BD}\\Count\" /v \"" + Transform(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\" /f");
                    Console.WriteLine("1");
                    cmd.StandardInput.WriteLine("reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Compatibility Assistant\\Store\" /v \"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\" /f");
                    //cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                }
                if (materialCheckBox9.Checked)
                {
                    string yel = get("https://phantomclicker.us/updateHWID.php?hwid=" + HWID);
                }
                this.Close();
            }
                catch (Exception)
            {
                Console.Write("Error exploding.");
            }
        }


        private void GlobalHookMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!constant)
            {

                if (e.Button == MouseButtons.Left)
                {
                    //timer1.Stop();
                    //timer5.Start();     
                }
            }
        }



        private void GlobalHookMouseDown(object sender, MouseEventArgs e)
        {
            if (!constant) {
                if (e.Button == MouseButtons.Left)
                {
                    /// MouseDown();
                }
                        }
            if (e.Button == MouseButtons.Right)
            {
                holdingright = true;
            }
        }

        private void Event(object sender, EventArgs e) {

            if (!constant)
            {
                MouseDown();
                            }



            }

       
        private void MouseDown()
        {
            Console.Write("down");
            fakeHeld = true;
            timer1.Start();
            newHeld = true;

          //  metroLabel1.Text = "true";
          //  metroLabel2.Text = "true";

        }
        private void MouseUp()
        {
            Console.Write("up");
            if(newHeld == false)
            {
                fakeHeld = false;
            //    metroLabel2.Text = "false";

            }
            newHeld = false;
            if (!clickedup)
            {
                resetFatigue();
            }
            else
            {
              //  metroLabel1.Text = "false";
                clickedup = false;
                timer8.Stop();
               
            }

            //metroLabel1.Text = "false";

        }



        private const int KEY_PRESSED = 0x8000;

        public bool IsPressed()
        {

            return Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LBUTTON) & KEY_PRESSED);
        }

        private void GlobalHookMouseUp(object sender, MouseEventArgs e)
        {
            if (!constant)
            {
                if (e.Button == MouseButtons.Left)
                {
                    MouseUp();
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                holdingright = false;
            }
            
        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            panel2.BackColor = ColorTranslator.FromHtml("#3D474C");
            pictureBox1.BackColor = ColorTranslator.FromHtml("#3D474C");
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            panel2.BackColor = Color.Transparent;
            pictureBox1.BackColor = Color.Transparent;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            panel2.BackColor = ColorTranslator.FromHtml("#3D474C");
            pictureBox1.BackColor = ColorTranslator.FromHtml("#3D474C");
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            panel2.BackColor = Color.Transparent;
            pictureBox1.BackColor = Color.Transparent;
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            hideApplication();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            hideApplication();
        }
        string notifyLore;
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        private void hideApplication()
        {
            notifyIcon1.Visible = true;
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.BalloonTipTitle = CreatePassword((new Random()).Next(1, 16));
            notifyIcon1.BalloonTipText = CreatePassword((new Random()).Next(16, 32));
            this.Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
        }

        private void metroTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (metroTrackBar2.Value >= metroTrackBar1.Value)
            {
                metroTrackBar2.Value = metroTrackBar1.Value;
            }
            if (metroTrackBar1.Value == 0)
            {
                metroTrackBar1.Value = 1;
            }
            metroLabel4.Text = ((float)metroTrackBar2.Value / 10f).ToString();
            metroLabel3.Text = ((float)metroTrackBar1.Value / 10f).ToString();
        }

        private void metroTrackBar2_Scroll(object sender, ScrollEventArgs e)
        {
            if (metroTrackBar2.Value >= metroTrackBar1.Value)
            {
                metroTrackBar1.Value = metroTrackBar2.Value;
            }
            if (metroTrackBar1.Value == 0)
            {
                metroTrackBar1.Value = 1;
            }
            metroLabel4.Text = ((float)metroTrackBar2.Value / 10f).ToString();
            metroLabel3.Text = ((float)metroTrackBar1.Value / 10f).ToString();
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {


            //ddsadsads
        }

        private string GetActiveWindowTitle()
        {
            StringBuilder text = new StringBuilder(256);
            if (Form1.GetWindowText(Form1.GetForegroundWindow(), text, 256) > 0)
                return text.ToString();
            return (string)null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try {
                if (toggled)
                {
                    if (justmc && this.GetActiveWindowTitle() != null && (this.GetActiveWindowTitle().Contains("Minecraft") || this.GetActiveWindowTitle().Contains("CosmicClient")))
                    {
                        if (this.metroTrackBar1.Value != 0 && this.metroTrackBar2.Value != 0)
                        {
                            this.timer1.Interval = modifyInterval(new Random().Next(1000 / this.metroTrackBar1.Value * 10, 1000 / this.metroTrackBar2.Value * 10));
                        }
                        if (newHeld && fakeHeld || constant) { 
                        clicks++;

                        if (materialCheckBox8.Checked) {
                            playClickSound();
                        }

                            Console.WriteLine("OMFG YOUR HOLDING DOWN LEFT CLICK!!!!");
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                      //      timer5.Start();
                            //  metroLabel1.Text = truefalsetest.ToString();
                        }
                        if (rightclick)
                        {
                            if (holdingright)
                            {
                                timer2.Interval = new Random().Next(10, 500);
                                timer2.Start();
                            }
                        }
                    }
                    if (!justmc)
                    {
                        if (this.metroTrackBar1.Value != 0 && this.metroTrackBar2.Value != 0)
                        {
                            this.timer1.Interval = modifyInterval(new Random().Next(1000 / this.metroTrackBar1.Value * 10, 1000 / this.metroTrackBar2.Value * 10));
                        }
                        if (newHeld && fakeHeld || constant)
                        {

                            clicks++;
                            truefalsetest = truefalsetest + 3;
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 1, 1, 1, 1);
                      //      timer5.Start();
                        }
                        // metroLabel1.Text = truefalsetest.ToString(); if (rightclick)
                        {
                            if (holdingright)
                            {
                                timer2.Start();
                            }
                        }
                    }

                }
            } catch
            {
                timer1.Stop();
            }
        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            toggled = !toggled;

            if (metroToggle1.Checked)
            {
                metroLabel11.Text = on;
                timer1.Start();
            }else
            {
                metroLabel11.Text = off;
                timer1.Stop();
            }
        }

        private void materialCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            justmc = !justmc;
        }

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            rightclick = !rightclick;
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            timer5.Stop();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try {
                timer2.Stop();
                timer3.Start();
                timer2.Interval = new Random().Next(10, 500);
                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            } catch
            {
                timer2.Stop();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                timer3.Stop();
            } catch
            {
                timer3.Stop();
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton2_KeyDown(object sender, KeyEventArgs e)
        {
            materialRaisedButton2.Text = e.KeyCode.ToString();
            togglekey = materialRaisedButton2.Text;
            metroLabel1.Focus();
        }

        private void materialRaisedButton2_Enter(object sender, EventArgs e)
        {
            togglekey = materialRaisedButton2.Text;
            materialRaisedButton2.Text = ">" + materialRaisedButton2.Text + "<";
        }

        private void materialRaisedButton2_Leave(object sender, EventArgs e)
        {
            materialRaisedButton2.Text = togglekey;
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton3_KeyDown(object sender, KeyEventArgs e)
        {
            materialRaisedButton3.Text = e.KeyCode.ToString();
            explodekey = materialRaisedButton3.Text;
            label1.Focus();
        }

        private void materialRaisedButton3_Enter(object sender, EventArgs e)
        {
            explodekey = materialRaisedButton3.Text;
            materialRaisedButton3.Text = ">" + materialRaisedButton3.Text + "<";
        }

        private void materialRaisedButton3_Leave(object sender, EventArgs e)
        {
            materialRaisedButton3.Text = explodekey;
        }

        private void metroToggle2_CheckedChanged(object sender, EventArgs e)
        {
            explode = !explode;
            if (metroToggle2.Checked)
            {
                metroLabel12.Text = on;
            }
            else
            {
                metroLabel12.Text = off;
            }
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            Explode();
        }
        protected string get(string url)
        {
            try
            {
                string rt;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Phantom/" + ver.ToString() + " (PhantomClicker Client; " + DateTime.Now + ")";

                WebResponse response = request.GetResponse();

                Stream dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);

                rt = reader.ReadToEnd();

                Console.WriteLine(rt);

                reader.Close();
                response.Close();

                return rt;
            }

            catch (Exception)
            {
                return null;
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }



        private void button2_Click(object sender, EventArgs e)
        {

        }

        public static string Transform(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void materialCheckBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Unsubscribe();





            if (materialCheckBox12.Checked) {
                    try
                    {

                        string pel = get("http://phantomclicker.us/setSettings.php?hwid=" + HWID + "&settings=" + getSettingsString());
                Console.Write(getSettingsString());
                }
                catch (Exception)
                {
                    Console.Write("Error closing.1");
                }
            }
            try {
            string nel = get("http://phantomclicker.us/updateClicks.php?hwid=" + HWID + "&clicks=" + clicks.ToString());
            }
            catch (Exception)
            {
                Console.Write("Error closing.2");
            }
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            clickedup = true;
            metroLabel1.Text = "true";
            timer7.Stop();
            timer7.Start();
            timer6.Stop();
        }

        private void timer7_Tick(object sender, EventArgs e)
        {
            clickedup = true;
            timer7.Stop();
        }

        private void timer8_Tick(object sender, EventArgs e)
        {
            clickedup = false;
            timer8.Stop();
        }

        private void materialCheckBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckBox8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckBox11_CheckedChanged(object sender, EventArgs e)
        {
            constant = !constant;
            if (constant)
            {
                timer1.Start();
            }
            if (!constant)
            {
                timer1.Stop();
            }
        }

        private void materialCheckBox9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckBox9_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void materialTabSelector1_Click(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            string notification = get("https://phantomclicker.us/getNotification.php?ver=" + ver);
            if (notification != null)
                MessageBox.Show(notification.Replace("|", Environment.NewLine));
        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
        }

        private string getSettingsString()
        {
            string a = Convert.ToInt32(materialCheckBox1.Checked).ToString();
            string b = Convert.ToInt32(materialCheckBox2.Checked).ToString();
            string c = Convert.ToInt32(materialCheckBox9.Checked).ToString();
            string d = Convert.ToInt32(materialCheckBox4.Checked).ToString();
            string e = Convert.ToInt32(materialCheckBox5.Checked).ToString();
            string f = Convert.ToInt32(materialCheckBox6.Checked).ToString();
            string g = Convert.ToInt32(materialCheckBox3.Checked).ToString();
            string h = Convert.ToInt32(materialCheckBox1.Checked).ToString();
            string i = Convert.ToInt32(metroToggle2.Checked).ToString();
            string j = Convert.ToInt32(materialCheckBox7.Checked).ToString();
            string k = Convert.ToInt32(materialCheckBox8.Checked).ToString();
            string l = Convert.ToInt32(materialCheckBox10.Checked).ToString();
            string m = Convert.ToInt32(materialCheckBox11.Checked).ToString();
            string n = Convert.ToInt32(materialCheckBox2.Checked).ToString();
            string o = Convert.ToInt32(materialCheckBox1.Checked).ToString();

            return a + b + c + d + e + f + g + h + i + j + k + l + m + n + o + "|" + metroTrackBar1.Value + "|" + metroTrackBar2.Value + "|" + explodekey + "|" + togglekey + "|" + Language;
        }

        private void applySettingsString(String s)
        {
            Console.Write("["  + s + "]");
            try
            {

           
            string[] parts = s.Split('|');
                if (parts[0] != null)
                {
                    char[] numbers = parts[0].ToCharArray();
                    Language = Int32.Parse(parts[5]);
                    updateLanguage();
                    materialCheckBox1.Checked = getFrom01(numbers[0]);
                    materialCheckBox2.Checked = getFrom01(numbers[1]);
                    justmc = getFrom01(numbers[1]);
                    materialCheckBox9.Checked = getFrom01(numbers[2]);
                    materialCheckBox4.Checked = getFrom01(numbers[3]);
                    materialCheckBox5.Checked = getFrom01(numbers[4]);
                    materialCheckBox6.Checked = getFrom01(numbers[5]);
                    materialCheckBox3.Checked = getFrom01(numbers[6]);
                    materialCheckBox1.Checked = getFrom01(numbers[7]);
                    metroToggle1.Checked = getFrom01(numbers[8]);
                    if (numbers[9] != null) ;
                    materialCheckBox7.Checked = getFrom01(numbers[9]);
                    if (numbers[10] != null) ;
                    materialCheckBox8.Checked = getFrom01(numbers[10]);
                    if (numbers[11] != null) ;
                    materialCheckBox10.Checked = getFrom01(numbers[11]);
                    if(numbers[12] != null) {
                    materialCheckBox11.Checked = getFrom01(numbers[12]);
                    constant = getFrom01(numbers[12]);

                    if (constant)
                    {
                        timer1.Start();
                    }
                    if (!constant)
                    {
                        timer1.Stop();
                    }
                    }
                    materialCheckBox2.Checked = getFrom01(numbers[13]);
                    materialCheckBox1.Checked = getFrom01(numbers[14]);
                }
                if (parts[1] != null)
                {
                    metroTrackBar1.Value = Int32.Parse(parts[1]);
                }
                if (parts[2] != null)
                {
                    metroTrackBar2.Value = Int32.Parse(parts[2]);
                }
            
                    if (parts[4] != null) { 
            materialRaisedButton2.Text = parts[4];
                    togglekey = parts[4];
                }
                    if(parts[3] != null) { 
                materialRaisedButton3.Text = parts[3];
                    explodekey = parts[3];
                }

                if (materialRaisedButton2.Text == "") { 
                    if(Language == 0) {
                    materialRaisedButton2.Text = "NONE";
                    } else if(Language == 1)
                    {
                        materialRaisedButton2.Text = "AUCUN";
                    }
                }
                if (materialRaisedButton3.Text == "") {
                    materialRaisedButton3.Text = "NONE";

                    if (metroToggle2.Checked)
                    {
                        metroLabel12.Text = "On";
                    }
                    else
                    {
                        metroLabel12.Text = "Off";
                    }
                    if (metroToggle1.Checked)
                    {
                        metroLabel11.Text = "On";
                    }
                    else
                    {
                        metroLabel11.Text = "Off";
                    }
                }
                else if (Language == 1)
                {
                    materialRaisedButton2.Text = "AUCUN";
                }
                metroLabel4.Text = ((float)metroTrackBar2.Value / 10f).ToString();  
                metroLabel3.Text = ((float)metroTrackBar1.Value / 10f).ToString();
            }
            catch (Exception)
            {
            }
        }


        private bool getFrom01(Char c)
        {
            if (c == '1')
                return true;
            if (c == '0')
                return false;
            return false;
        }

        private void materialRaisedButton2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (materialCheckBox10.Checked) {
            string notification = get("https://phantomclicker.us/getNotification.php?ver=" + ver);
            if (notification != null)
                MessageBox.Show(notification.Replace("|", Environment.NewLine));
            }
        }
        private void playClickSound()
        {
            Random rand = new Random();
            int rando = rand.Next(0, 100);
            if (rando <= 5)
            {
                SoundPlayer audio = new SoundPlayer(pcl.Properties.Resources._1);
                audio.Play();
            }
            else if (rando <= 10)
            {
                SoundPlayer audio = new SoundPlayer(pcl.Properties.Resources._2);
                audio.Play();
            }
            else if (rando <= 20)
            {
                SoundPlayer audio = new SoundPlayer(pcl.Properties.Resources._3);
                audio.Play();
            }
            else if (rando <= 35)
            {
                SoundPlayer audio = new SoundPlayer(pcl.Properties.Resources._4);
                audio.Play();
            }
            else if (rando <= 50)
            {
                SoundPlayer audio = new SoundPlayer(pcl.Properties.Resources._5);
                audio.Play();
            }
            else if (rando <= 75)
            {
                SoundPlayer audio = new SoundPlayer(pcl.Properties.Resources._6);
                audio.Play();
            }
            else if (rando <= 90)
            {
                SoundPlayer audio = new SoundPlayer(pcl.Properties.Resources._7);
                audio.Play();
            }
            else if (rando <= 100)
            {
                SoundPlayer audio = new SoundPlayer(pcl.Properties.Resources._8);
                audio.Play();
            }
        }
        private int modifyInterval(int i)
        {
            int maxInt = 1000 / this.metroTrackBar1.Value * 10;
            int minInt = 1000 / this.metroTrackBar2.Value * 10;

            int fatiguecheck = new Random().Next(1, 20);
            if (fatiguecheck >= 18)
            {
                if (!bounceupState) {
                    if(fatigue <= 20) {
                        fatigue = fatigue + 1;
                        if (new Random().Next(1, 150) >= 148)
                        {
                            bounceupState = !bounceupState;
                        }
                    }
                    else
                    {
                        if (new Random().Next(1, 10) >= 5)
                        {
                            bounceupState = !bounceupState;
                        }
                    }
                }
                else
                {
                    if (fatigue >= 0)
                    {
                        fatigue = fatigue - 1;
                        if (new Random().Next(1, 150) >= 148)
                        {
                            bounceupState = !bounceupState;
                        }
                    }
                    else
                    {
                        if(new Random().Next(1,10) >= 5)
                        {
                            bounceupState = !bounceupState;
                        }
                    }
                }

            }
            int minIntchangeAmount = minInt - i;

            int addedFatigue = minIntchangeAmount * ((fatigue * 10) / 100);
            return i + addedFatigue;
        }
        bool bounceupState = false;
        int fatigue;
        private void resetFatigue()
        {
            fatigue = 0;
        }

        private void tabPage4_Click_1(object sender, EventArgs e)
        {
            button1.Focus();
        }

        private void tabPage2_Click_1(object sender, EventArgs e)
        {
            button1.Focus();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            button1.Focus();
        }

        private void tabPage2_Paint(object sender, PaintEventArgs e)
        {
            button1.Focus();
        }

        int Language = 0;
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Language = 1;
            updateLanguage();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Language = 0;
            updateLanguage();
        }

        private void updateLanguage()
        {
            if (Language == 1)
            {
                //FRENCH
                materialLabel1.Text = "Général";
                metroLabel5.Text = "Activé:";
                metroLabel1.Text = "CPS Max.";
                metroLabel2.Text = "CPS Min.";
                materialCheckBox2.Text = "Seulement dans la fenêtre MC.";
                materialCheckBox1.Text = "Clique-droit activé";
                metroLabel6.Text = "Activation:";
                tabPage4.Text = "Misc";
                tabPage2.Text = "Explosion";
                tabPage3.Text = "Général";
                materialCheckBox9.Text = "Désactiver l'HWID (temporairement).";
                materialCheckBox4.Text = "Avertissement d'explosion.";
                materialCheckBox5.Text = "Retirer du Prefetch.";
                materialCheckBox6.Text = "Retirer du userassist.";
                materialCheckBox3.Text = "Supprimer à l'explosion.";
                metroLabel7.Text = "Touche d'activation:";
                materialLabel2.Text = "Explosion";
                metroLabel8.Text = "Touche d'explosion:";
                materialRaisedButton1.Text = "Explosion";
                materialLabel3.Text = "Misc";
                materialCheckBox7.Text = "Mode Hit&Block. (1.1.3)";
                materialCheckBox8.Text = "Faux bruits de clics";
                materialCheckBox10.Text = "Notifications au démarrage.";
                materialCheckBox11.Text = "Clic constant";
                materialCheckBox12.Text = "Syncr. les paramètres à la fermeture.";
                materialRaisedButton4.Text = "Montrer les notifications";
                metroLabel9.Text = "Licence à " + licensedto;
                explodeMessage = "Êtes-vous sûr de vouloir self-destruct?";
                notifyLore = "Cliquez pour afficher.";

                if (materialRaisedButton2.Text == "NONE" || materialRaisedButton2.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton2.Text = "VIDE";
                }
                if (materialRaisedButton3.Text == "NONE" || materialRaisedButton3.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton3.Text = "VIDE";
                }
                on = "Sur";
                off = "De";
                if (metroToggle2.Checked)
                {
                    metroLabel12.Text = on;
                }
                else
                {
                    metroLabel12.Text = off;
                }
                if (metroToggle1.Checked)
                {
                    metroLabel11.Text = on;
                }

                pictureBox16.Hide();
                pictureBox14.Hide();
                pictureBox12.Hide();
                pictureBox9.Show();
                pictureBox8.Hide();
                pictureBox10.Hide();
                materialTabSelector2.Refresh();
                
            }
            if (Language == 0)
            {
                //ENGLISH
                materialLabel1.Text = "Main";
                metroLabel5.Text = "Clicker:";
                metroLabel1.Text = "CPS Max.";
                metroLabel2.Text = "CPS Min.";
                materialCheckBox2.Text = "Only in MC window.";
                materialCheckBox1.Text = "Right-click enabled.";
                metroLabel6.Text = "Toggle:";
                tabPage4.Text = "Misc";
                tabPage2.Text = "Explode";
                tabPage3.Text = "Main";
                materialCheckBox9.Text = "Change HWID (temp-disable).";
                materialCheckBox4.Text = "Destruct warning.";
                materialCheckBox5.Text = "Remove from prefetch.";
                materialCheckBox6.Text = "Anti-userassist.";
                materialCheckBox3.Text = "Delete on explode.";
                metroLabel7.Text = "Key Bind:";
                materialLabel2.Text = "Explode";
                metroLabel8.Text = "Explode key:";
                materialRaisedButton1.Text = "Explode";
                materialLabel3.Text = "Misc";
                materialCheckBox7.Text = "Blockhit mode. (1.3.0)";
                materialCheckBox8.Text = "Fake click sounds.";
                materialCheckBox10.Text = "Startup notifications.";
                materialCheckBox11.Text = "Constant click.";
                materialCheckBox12.Text = "Sync settings on exit.";
                materialRaisedButton4.Text = "Show notification";
                metroLabel9.Text = "Licensed to " + licensedto;
                explodeMessage = "Are you sure you want to self destruct?";
                notifyLore = "Click to show.";

                if (materialRaisedButton2.Text == "NONE" || materialRaisedButton2.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton2.Text = "NONE";
                }
                if (materialRaisedButton3.Text == "NONE" || materialRaisedButton3.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton3.Text = "NONE";
                }
                on = "On";
                off = "Off";
                if (metroToggle2.Checked)
                {
                    metroLabel12.Text = on;
                }
                else
                {
                    metroLabel12.Text = off;
                }
                if (metroToggle1.Checked)
                {
                    metroLabel11.Text = on;
                }

                pictureBox16.Hide();
                pictureBox14.Hide();
                pictureBox12.Hide();
                pictureBox8.Show();
                pictureBox9.Hide();
                pictureBox10.Hide();
                materialTabSelector2.Refresh();
            }
            if (Language == 2)
            {
                //DUTCH
                materialLabel1.Text = "Hoofd";
                metroLabel5.Text = "Klikker:";
                metroLabel1.Text = "CPS Max.";
                metroLabel2.Text = "CPS Min.";
                materialCheckBox2.Text = "Alleen in een MC venster";
                materialCheckBox1.Text = "Rechter-Click aangezet";
                metroLabel6.Text = "Ontkoppelknop:";
                tabPage4.Text = "Diversen";
                tabPage2.Text = "Explode";
                tabPage3.Text = "Hoofd";
                materialCheckBox9.Text = "Verander HWID (Tijdenlijk uit).";
                materialCheckBox4.Text = "Vernietigings waarschuwing.";
                materialCheckBox5.Text = "Verwijderd van voorvoeding.";
                materialCheckBox6.Text = "Anti-Gebruikers assistentie.";
                materialCheckBox3.Text = "Verwijder op zelfvernietiging.";
                metroLabel7.Text = "Toets Binding:";
                materialLabel2.Text = "Explode";
                metroLabel8.Text = "Explode sleutel::";
                materialRaisedButton1.Text = "Explode";
                materialLabel3.Text = "Diversen";
                materialCheckBox7.Text = "Afweer mode (1.3.0)";
                materialCheckBox8.Text = "Neppe click geluiden.";
                materialCheckBox10.Text = "Opstart notificaties.";
                materialCheckBox11.Text = "Constante click.";
                materialCheckBox12.Text = "Gesynchroniseerde opties op sluiten.";
                materialRaisedButton4.Text = "Toon notificatie";
                metroLabel9.Text = "Gelicenceerd naar " + licensedto;
                explodeMessage = "Weet je het zeker dat je wilt zelf vernietigen?";
                notifyLore = "Click om te toonen.";

                if (materialRaisedButton2.Text == "NONE" || materialRaisedButton2.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton2.Text = "GEEN";
                }
                if (materialRaisedButton3.Text == "NONE" || materialRaisedButton3.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton3.Text = "GEEN";
                }

                on = "Op";
                off = "Uit";
                if (metroToggle2.Checked)
                {
                    metroLabel12.Text = on;
                }
                else
                {
                    metroLabel12.Text = off;
                }
                if (metroToggle1.Checked)
                {
                    metroLabel11.Text = on;
                }
                pictureBox16.Hide();
                pictureBox14.Hide();
                pictureBox12.Hide();
                pictureBox10.Show();
                pictureBox9.Hide();
                pictureBox8.Hide();
                materialTabSelector2.Refresh();
            }
            if (Language == 3)
            {
                //GERMAN
                materialLabel1.Text = "Hoofd";
                metroLabel5.Text = "Klikker:";
                metroLabel1.Text = "CPS Max.";
                metroLabel2.Text = "CPS Min.";
                materialCheckBox2.Text = "Alleen in een MC venster";
                materialCheckBox1.Text = "Rechter-Click aangezet";
                metroLabel6.Text = "Ontkoppelknop:";
                tabPage4.Text = "Diversen";
                tabPage2.Text = "Explode";
                tabPage3.Text = "Hoofd";
                materialCheckBox9.Text = "Verander HWID (Tijdenlijk uit).";
                materialCheckBox4.Text = "Vernietigings waarschuwing.";
                materialCheckBox5.Text = "Verwijderd van voorvoeding.";
                materialCheckBox6.Text = "Anti-Gebruikers assistentie.";
                materialCheckBox3.Text = "Verwijder op zelfvernietiging.";
                metroLabel7.Text = "Toets Binding:";
                materialLabel2.Text = "Explode";
                metroLabel8.Text = "Explode sleutel::";
                materialRaisedButton1.Text = "Explode";
                materialLabel3.Text = "Diversen";
                materialCheckBox7.Text = "Afweer mode (1.3.0)";
                materialCheckBox8.Text = "Neppe click geluiden.";
                materialCheckBox10.Text = "Opstart notificaties.";
                materialCheckBox11.Text = "Constante click.";
                materialCheckBox12.Text = "Gesynchroniseerde opties op sluiten.";
                materialRaisedButton4.Text = "Toon notificatie";
                metroLabel9.Text = "Gelicenceerd naar " + licensedto;
                explodeMessage = "Weet je het zeker dat je wilt zelf vernietigen?";
                notifyLore = "Click om te toonen.";

                if (materialRaisedButton2.Text == "NONE" || materialRaisedButton2.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton2.Text = "KEINER";
                }
                if (materialRaisedButton3.Text == "NONE" || materialRaisedButton3.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton3.Text = "KEINER";
                }

                on = "Op";
                off = "Uit";
                if (metroToggle2.Checked)
                {
                    metroLabel12.Text = on;
                }
                else
                {
                    metroLabel12.Text = off;
                }
                if (metroToggle1.Checked)
                {
                    metroLabel11.Text = on;
                }
                pictureBox16.Hide();
                pictureBox14.Hide();
                pictureBox12.Show();
                pictureBox10.Hide();
                pictureBox9.Hide();
                pictureBox8.Hide();
                materialTabSelector2.Refresh();
            }
            if (Language == 4)
            {
                //RUSSIAN
                materialLabel1.Text = "Hoofd";
                metroLabel5.Text = "Klikker:";
                metroLabel1.Text = "CPS Max.";
                metroLabel2.Text = "CPS Min.";
                materialCheckBox2.Text = "Alleen in een MC venster";
                materialCheckBox1.Text = "Rechter-Click aangezet";
                metroLabel6.Text = "Ontkoppelknop:";
                tabPage4.Text = "Diversen";
                tabPage2.Text = "Explode";
                tabPage3.Text = "Hoofd";
                materialCheckBox9.Text = "Verander HWID (Tijdenlijk uit).";
                materialCheckBox4.Text = "Vernietigings waarschuwing.";
                materialCheckBox5.Text = "Verwijderd van voorvoeding.";
                materialCheckBox6.Text = "Anti-Gebruikers assistentie.";
                materialCheckBox3.Text = "Verwijder op zelfvernietiging.";
                metroLabel7.Text = "Toets Binding:";
                materialLabel2.Text = "Explode";
                metroLabel8.Text = "Explode sleutel::";
                materialRaisedButton1.Text = "Explode";
                materialLabel3.Text = "Diversen";
                materialCheckBox7.Text = "Afweer mode (1.3.0)";
                materialCheckBox8.Text = "Neppe click geluiden.";
                materialCheckBox10.Text = "Opstart notificaties.";
                materialCheckBox11.Text = "Constante click.";
                materialCheckBox12.Text = "Gesynchroniseerde opties op sluiten.";
                materialRaisedButton4.Text = "Toon notificatie";
                metroLabel9.Text = "Gelicenceerd naar " + licensedto;
                explodeMessage = "Weet je het zeker dat je wilt zelf vernietigen?";
                notifyLore = "Click om te toonen.";
                on = "Op";
                off = "Uit";
                if (metroToggle2.Checked)
                {
                    metroLabel12.Text = on;
                }
                else
                {
                    metroLabel12.Text = off;
                }
                if (metroToggle1.Checked)
                {
                    metroLabel11.Text = on;
                }
                else
                {
                    metroLabel11.Text = off;
                }


                if (materialRaisedButton2.Text == "NONE" || materialRaisedButton2.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton2.Text = "НИКТО";
                }
                if (materialRaisedButton3.Text == "NONE" || materialRaisedButton3.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton3.Text = "НИКТО";
                }
                pictureBox16.Hide();
                pictureBox14.Show();
                pictureBox12.Hide();
                pictureBox10.Hide();
                pictureBox9.Hide();
                pictureBox8.Hide();
                materialTabSelector2.Refresh();
            }
            if (Language == 5)
            {
                //LATVIAN
                materialLabel1.Text = "Hoofd";
                metroLabel5.Text = "Klikker:";
                metroLabel1.Text = "CPS Max.";
                metroLabel2.Text = "CPS Min.";
                materialCheckBox2.Text = "Alleen in een MC venster";
                materialCheckBox1.Text = "Rechter-Click aangezet";
                metroLabel6.Text = "Ontkoppelknop:";
                tabPage4.Text = "Diversen";
                tabPage2.Text = "Explode";
                tabPage3.Text = "Hoofd";
                materialCheckBox9.Text = "Verander HWID (Tijdenlijk uit).";
                materialCheckBox4.Text = "Vernietigings waarschuwing.";
                materialCheckBox5.Text = "Verwijderd van voorvoeding.";
                materialCheckBox6.Text = "Anti-Gebruikers assistentie.";
                materialCheckBox3.Text = "Verwijder op zelfvernietiging.";
                metroLabel7.Text = "Toets Binding:";
                materialLabel2.Text = "Explode";
                metroLabel8.Text = "Explode sleutel::";
                materialRaisedButton1.Text = "Explode";
                materialLabel3.Text = "Diversen";
                materialCheckBox7.Text = "Afweer mode (1.3.0)";
                materialCheckBox8.Text = "Neppe click geluiden.";
                materialCheckBox10.Text = "Opstart notificaties.";
                materialCheckBox11.Text = "Constante click.";
                materialCheckBox12.Text = "Gesynchroniseerde opties op sluiten.";
                materialRaisedButton4.Text = "Toon notificatie";
                metroLabel9.Text = "Gelicenceerd naar " + licensedto;
                explodeMessage = "Weet je het zeker dat je wilt zelf vernietigen?";
                notifyLore = "Click om te toonen.";

                if (materialRaisedButton2.Text == "NONE" || materialRaisedButton2.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton2.Text = "NAV";
                }
                if (materialRaisedButton3.Text == "NONE" || materialRaisedButton3.Text == "VIDE" || materialRaisedButton3.Text == "GEEN" || materialRaisedButton3.Text == "KEINER" || materialRaisedButton3.Text == "НИКТО" || materialRaisedButton3.Text == "NAV")
                {
                    materialRaisedButton3.Text = "NAV";
                }
                on = "Op";
                off = "Uit";
                if (metroToggle2.Checked)
                {
                    metroLabel12.Text = on;
                }
                else
                {
                    metroLabel12.Text = off;
                }
                if (metroToggle1.Checked)
                {
                    metroLabel11.Text = on;
                }
                else
                {
                    metroLabel11.Text = off;
                }

                pictureBox16.Show();
                pictureBox14.Hide();
                pictureBox12.Hide();
                pictureBox10.Hide();
                pictureBox9.Hide();
                pictureBox8.Hide();
                materialTabSelector2.Refresh();
            }
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            Language = 2;
            updateLanguage();
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This language will be implemented soon! Look out for an update.");
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This language will be implemented soon! Look out for an update.");
        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This language will be implemented soon! Look out for an update.");
        }

    }

    public static class MouseHook
    {
        public static event EventHandler MouseAction = delegate { };

        public static void Start()
        {
            _hookID = SetHook(_proc);


        }
        public static void stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                  GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
          int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseAction(null, new EventArgs());
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


    }
}
