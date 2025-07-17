using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Microsoft.VisualBasic;

namespace nexus.common
{
    public class Processes : IDisposable
    {
        public enum ProcStates
        {
            NotInstalled,
            Stopped,
            Starting,
            Running,
            Halted,
            NonResponsive,
            Stopping,
            Killing,
            InError,
            ReStart,
            Updating,
            DLProcInUse
        }

        public int ProcId;

        public ProcStates AppState(string ExecFile)
        {
            string Mess = "";
            return AppState(ExecFile, 0, "", ref Mess);
        }
        public ProcStates AppState(string ExecFile, string Arguments)
        {
            string Mess = "";
            return AppState(ExecFile, 0, Arguments, ref Mess);
        }

        public ProcStates AppState(string ExecFile, int ProcessId, string Arguments)
        {
            string Mess = "";
            return AppState(ExecFile, ProcessId, Arguments, ref Mess);
        }

        public ProcStates AppState(string ExecFile, int ProcessId, string Arguments, ref string Message)
        {
            Message = "";
            if (ProcessId > 0)
            {
                try
                {
                    var appProc = Process.GetProcessById(ProcessId);
                    if (appProc is null)
                    {
                        return ProcStates.Stopped;
                    }
                    else if (!appProc.Responding)
                    {
                        return ProcStates.NonResponsive;
                    }
                    else
                    {
                        return ProcStates.Running;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("not running"))
                    {
                        return ProcStates.Stopped;
                    }
                    else
                    {
                        Message = ex.Message;
                    }
                }
            }
            else
            {
                try
                {
                    if (File.Exists(ExecFile))
                    {
                        string ExecName = Path.GetFileNameWithoutExtension(ExecFile);
                        var appProc = Process.GetProcessesByName(ExecName);
                        if (appProc.Length > 0)
                        {
                            for (int i = 0, loopTo = appProc.GetLength(0) - 1; i <= loopTo; i++)
                            {
                                Process Proc = (Process)appProc.GetValue(i);
                                string ProcName = Path.GetFileNameWithoutExtension(Proc.MainModule.ModuleName.ToUpper());
                                if ((ProcName ?? "") == (ExecName.ToUpper() ?? ""))
                                {
                                    string Args = "";// GetCommandLine(Proc.Id).ToString();
                                    if (Args.ToUpper().Equals(Arguments.ToUpper()) || Arguments=="" )
                                    {
                                        if (!Proc.Responding)
                                        {
                                            return ProcStates.NonResponsive;
                                        }
                                        else
                                        {
                                            ProcId = Proc.Id;
                                            return ProcStates.Running;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return ProcStates.NotInstalled;
                    }
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                }
            }

            return ProcStates.Stopped;
        }

        public int Find(string ExecFile, string Arguments)
        {
            try
            {
                string ExecName = Path.GetFileNameWithoutExtension(ExecFile);
                var appProc = Process.GetProcessesByName(ExecName);
                if (appProc.Length > 0)
                {
                    for (int i = 0, loopTo = appProc.GetLength(0) - 1; i <= loopTo; i++)
                    {
                        Process Proc = (Process)appProc.GetValue(i);
                        string ProcName = Path.GetFileNameWithoutExtension(Proc.MainModule.ModuleName.ToUpper());
                        if ((ProcName ?? "") == (ExecName.ToUpper() ?? ""))
                        {
                            string Args = "";// GetCommandLine(Proc.Id).ToString();
                            if (Args.ToUpper().Equals(Arguments.ToUpper()))
                            {
                                return Proc.Id;
                            }
                        }
                    }
                }
            }
            catch (Exception )
            {
            }

            return -1;
        }

        public bool KillApp(string ExecFile, bool KillIt=true)
        {
            string Mess = "";
            return KillApp(ExecFile, 0, KillIt, ref Mess);
        }

        public bool KillApp(string ExecFile, int ProcessId, bool KillIt)
        {
            string Mess = "";
            return KillApp(ExecFile, ProcessId, KillIt, ref Mess);
        }

        public bool KillApp(string ExecFile, int ProcessId, bool KillIt, ref string Message)
        {
            Message = "";
            bool Processed = false;
            try
            {
                if (ProcessId > 0)
                {
                    var Proc = Process.GetProcessById(ProcessId);
                    Proc.Refresh();
                    if (!Proc.HasExited)
                    {
                        if (KillIt)
                        {
                            Proc.Kill();
                            Proc.WaitForExit(5000);
                        }
                        else
                        {
                            Proc.CloseMainWindow();
                            Proc.WaitForExit(5000);
                        }
                    }
                }
                else
                {
                    string ExecName = Path.GetFileNameWithoutExtension(ExecFile);
                    var appProc = Process.GetProcessesByName(ExecName);
                    if (appProc.Length > 0)
                    {
                        Message += "(" + appProc.GetLength(0) + ")";
                        for (int i = 0, loopTo = appProc.GetLength(0) - 1; i <= loopTo; i++)
                        {
                            try
                            {
                                Process Proc = (Process)appProc.GetValue(i);
                                try
                                {
                                    string ProcName = Path.GetFileNameWithoutExtension(Proc.MainModule.ModuleName.ToUpper());
                                    if ((ProcName ?? "") == (ExecName.ToUpper() ?? ""))
                                    {
                                        Message += "found ";
                                        try
                                        {
                                            Proc.Refresh();
                                            if (!Proc.HasExited)
                                            {
                                                if (KillIt)
                                                {
                                                    Message += "1";
                                                    Proc.Kill();
                                                    Message += "1";
                                                    Proc.WaitForExit(5000);
                                                }
                                                else
                                                {
                                                    Message += "1";
                                                    Proc.CloseMainWindow();
                                                    Message += "1";
                                                    Proc.WaitForExit(5000);
                                                }
                                            }

                                            Processed = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            if (!string.IsNullOrEmpty(Message))
                                                Message += "||";
                                            Message += ex.Message;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message += ex.Message;
            }

            return Processed;
        }


        public void StartBatch(string BatchFile)
        {

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"cmd /C \"{BatchFile}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();

            //return ProcStates.Starting;
        }
        public ProcStates StartApp(string Package)
        {
            return StartApp(Package, "", false);
        }

        public ProcStates StartApp(string Package, string Arguments = "", bool KillAllOthers = true)
        {
            try
            {
                //bool Process = false;
                //bool Processed = false;
                //string WanVersion = "";
                //string LanVersion = "";
                //string DLProc = @"C:\xcite\programs\avUpdater\avDLProc.exe";
                //var wDB = new common.DB(common.EndPoint.Instance.WANConnStr);
                //var lDB = new common.DB(common.EndPoint.Instance.LANConnStr);
                //string Executable = lDB.ExecLookup("Select executable from terminal.dbo.package where id='" + Package + "'").ToString();
                //string ExecFile = Path.Combine(common.EndPoint.Instance.BaseDir + @"\Programs\" + Package, Executable);
                //if (common.EndPoint.Instance.WAN.CanConnect())
                //{
                //    wDB = new common.DB(common.EndPoint.Instance.WANConnStr);
                //    WanVersion = wDB.ExecLookup("Select (select case tp.Release when 'Beta' then p.beta when 'Alpha' then p.alpha else p.production end) as Version from terminal.dbo.termpacks as tp left join terminal.dbo.package as p on tp.package=p.id where tp.terminalid=" + common.EndPoint.Instance.Device.DeviceId.ToString() + " and tp.package='" + Package + "'").ToString();
                //    bool GetDLProc = !File.Exists(DLProc);
                //    if (!GetDLProc)
                //    {
                //        string WanDLVersion = wDB.ExecLookup("Select (select case [Release] when 'Beta' then [beta] when 'Alpha' then [alpha] else production end) as Version from terminal.dbo.package where Id='avUpdater'").ToString();
                //        try
                //        {
                //            var Vers = System.Reflection.AssemblyName.GetAssemblyName(DLProc).Version;
                //            GetDLProc = CompareVersions(Vers.Major.ToString() + "." + Vers.Minor.ToString() + "." + Vers.Build.ToString() + "." + Vers.Revision.ToString(), WanDLVersion);
                //        }
                //        catch (Exception ex)
                //        {
                //            GetDLProc = true;
                //        }
                //    }

                //    if (GetDLProc)
                //    {
                //        // Dim Upd As xcite.common.tcp.Updater = New xcite.common.tcp.Updater
                //        // Upd.Create(EndPoint.Instance.WAN.Server, "Setup")
                //        // If Upd.Download("/Packages/Production", "avDLProc.zip", "C:\xcite\programs\avUpdater\") Then
                //        // Dim HasMan As Boolean
                //        // Upd.ProcessZip("C:\xcite\programs\avUpdater\avDLProc.zip", "C:\xcite\programs\avUpdater", HasMan)
                //        // End If
                //    }
                //}
                //else
                //{
                //    Process = false;
                //}

                //if (File.Exists(ExecFile))
                //{
                //    try
                //    {
                //        var Vers = System.Reflection.AssemblyName.GetAssemblyName(ExecFile).Version;
                //        LanVersion = Vers.Major.ToString() + "." + Vers.Minor.ToString() + "." + Vers.Build.ToString() + "." + Vers.Revision.ToString();
                //    }
                //    catch (Exception ex)
                //    {
                //        Process = true;
                //    }
                //}

                //if (common.EndPoint.Instance.WAN.CanConnect() & (CompareVersions(LanVersion, WanVersion) | Process))
                //{
                //    var psInfo = new ProcessStartInfo(Path.GetFileName(DLProc));
                //    psInfo.WorkingDirectory = @"C:\xcite\programs\avUpdater";
                //    psInfo.Arguments = "Lan=" + common.EndPoint.Instance.LAN.Server + ";Wan=" + common.EndPoint.Instance.WAN.Server + ";Package=" + Package + ";TerminalID=" + common.EndPoint.Instance.Device.DeviceId.ToString() + ";SiteId=" + common.EndPoint.Instance.Venue.VenueId + ";Mode=Replace";
                //    psInfo.WindowStyle = ProcessWindowStyle.Normal;
                //    var myProcess = System.Diagnostics.Process.Start(psInfo);
                //    ProcId = myProcess.Id;
                //    return ProcStates.Updating;
                //}
                //else
                //{
                   // if (KillAllOthers) (Package, true);
                    var psInfo = new ProcessStartInfo(Package);
                    //psInfo.WorkingDirectory = Path.GetDirectoryName(Package);
                    psInfo.Arguments        = Arguments;
                    psInfo.WindowStyle      = ProcessWindowStyle.Normal;
                    psInfo.UseShellExecute  = true;

                    var myProcess = System.Diagnostics.Process.Start(psInfo);
                    ProcId = myProcess.Id;
                    return ProcStates.Starting;
                //}
            }
            catch (Exception ex)
            {
            }

            return default;
        }

        //public bool ServiceRunning(string ServiceName)
        //{
        //    var sc = new ServiceController(Path.GetFileNameWithoutExtension(ServiceName));
        //    try
        //    {
        //        switch (sc.Status)
        //        {
        //            case ServiceControllerStatus.Running:
        //                {
        //                    return true;
        //                }

        //            default:
        //                {
        //                    return false;
        //                }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public bool CompareVersions(string Installed, string Remote)
        {
            bool Ret = false;
            if (string.IsNullOrEmpty(Installed))
            {
                Ret = true;
            }
            else if (!string.IsNullOrEmpty(Remote))
            {
                Array iArr = (Installed + ".0.0.0.0").Split('.');
                Array sArr = (Remote + ".0.0.0.0").Split('.');
                if (helpers.ToInt(sArr.GetValue(0).ToString()) > helpers.ToInt(iArr.GetValue(0).ToString()))
                {
                    Ret = true;
                }
                else if (helpers.ToInt(sArr.GetValue(0).ToString()) == helpers.ToInt(iArr.GetValue(0).ToString()))
                {
                    if (helpers.ToInt(sArr.GetValue(1).ToString()) > helpers.ToInt(iArr.GetValue(1).ToString()))
                    {
                        Ret = true;
                    }
                    else if (helpers.ToInt(sArr.GetValue(1).ToString()) == helpers.ToInt(iArr.GetValue(1).ToString()))
                    {
                        if (helpers.ToInt(sArr.GetValue(2).ToString()) > helpers.ToInt(iArr.GetValue(2).ToString()))
                        {
                            Ret = true;
                        }
                        else if (helpers.ToInt(sArr.GetValue(2).ToString()) == helpers.ToInt(iArr.GetValue(2).ToString()))
                        {
                            if (helpers.ToInt(sArr.GetValue(3).ToString()) > helpers.ToInt(iArr.GetValue(3).ToString()))
                            {
                                Ret = true;
                            }
                        }
                    }
                }
            }

            return Ret;
        }

        //public static object GetCommandLine(int processId)
        //{
        //    string ret = "";
        //    var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + processId);
        //    foreach (ManagementObject cmd in searcher.Get())
        //        ret = (ret + cmd["CommandLine"]).ToString();
        //    Array Arr = ret.Split(' ');
        //    if (Arr.GetLength(0) == 2)
        //        ret = (Arr.GetValue(1)).ToString();
        //    return ret;
        //}


        #region  IDisposable Support 
        private bool disposedValue = false;        // To detect redundant calls
                                                   // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                    // TODO: free managed resources when explicitly called
                }

                // TODO: free shared unmanaged resources
            }

            disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}