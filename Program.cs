using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using SeewoPCEnhancedAssistant;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;

public enum LogLevel
{
    Info,
    Error,
    Warning
}
public enum LogKind
{
    Form,
    Thread,
    Process,
    Service
}
public class TaskSchedulerHelper
{
    public static void CreateTask(string taskName, string exePath, string scheduleType, string timestart)
    {
        // 获取当前时间，格式为 HH:mm
        // schtasks 命令参数
        string args = $"/Create /TN \"{taskName}\" /TR \"{exePath}\" /SC {scheduleType} /ST {timestart} /F";

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "schtasks",
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process process = Process.Start(processInfo))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Logger.WriteLog(LogLevel.Info, $"Scheduled task created successfully:{output}");

            if (!string.IsNullOrWhiteSpace(error))
            {
                Logger.WriteLog(LogLevel.Error, $"Error: {error}");
            }
        }
    }
}
public class Logger
{
    // 定义日志文件名和路径（当前目录下的 SeewoPCEnhancedAssistant.log 文件）
    private static readonly string logFileName = "SeewoPCEnhancedAssistant.log";
    private static readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), logFileName);
    //
    public static void WriteLog(LogLevel logLevel, LogKind logKind, string message)
    {
        // 在写日志之前先检查并处理文件重命名

        // 设置颜色
        if (logLevel == LogLevel.Info)
        {
            Console.ForegroundColor = ConsoleColor.Green;// 设置绿色
            Console.Write($"[{logLevel}] ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"{logKind}: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{message}\n");
            Console.ResetColor();

        }
        else if (logLevel == LogLevel.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[{logLevel}] ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"{logKind}: ");
            Console.ForegroundColor = ConsoleColor.Red; // 设置绿色
            Console.Write($"{message}\n");
            Console.ResetColor();
        }
        else if (logLevel == LogLevel.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{logLevel}] ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"{logKind}: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow; // 设置绿色
            Console.Write($"{message}\n");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"[{logLevel}] {logKind}: {message}");
        }

        // 打印日志到控制台
        Console.ResetColor();
        // 记录日志到文件
        LogToFile(logLevel, message);
    }
    public static void WriteLog(LogLevel logLevel, string message)
    {
        // 在写日志之前先检查并处理文件
        // 设置颜色
        if (logLevel == LogLevel.Info)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{logLevel}]");
            Console.ForegroundColor = ConsoleColor.DarkYellow; // 设置绿色
            Console.Write($": {message}\n");
            Console.ResetColor();
        }
        else if (logLevel == LogLevel.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[{logLevel}]");
            Console.ForegroundColor = ConsoleColor.Red; // 设置绿色
            Console.Write($": {message}\n");
            Console.ResetColor();
        }
        else if (logLevel == LogLevel.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{logLevel}] ");
            Console.ForegroundColor = ConsoleColor.DarkYellow; // 设置绿色
            Console.Write($"{message}\n");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"[{logLevel}] {message}");
        }

        // 打印日志到控制台

        // 记录日志到文件
        LogToFile(logLevel, message);
    }
    public static void PrintLog(LogLevel logLevel, LogKind logKind, string message)
    {
        LogToFile(logLevel, message);
    }
    public static void PrintLog(LogLevel logLevel, string message)
    {
        LogToFile(logLevel, message);
    }

    private static void LogToFile(LogLevel logLevel, string message)
    {
        // 创建日志信息
        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}]: {message}";

        try
        {
            // 如果日志文件不存在，则创建
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Close();
            }

            // 以追加方式写入日志内容
            using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
            {
                writer.WriteLine(logMessage);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] Error writing to log file: {ex.Message}");
            Console.ResetColor();
        }
    }

    public static void ClearFile(string filePath)
    {
        try
        {
            // 打开文件并清空内容
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
            {
                fs.SetLength(0); // 设置文件长度为0，即清空文件内容
            }

            WriteLog(LogLevel.Info, "Clear log file");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            WriteLog(LogLevel.Error, $"fail to clear log file: {ex.Message}");
            Console.ResetColor();
        }

    }
}
class Program
{
    static void ExecuteOperation()
    {
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        Process.Start(exePath, "-sh");
        Environment.Exit(0);
    }
    static void AddTask() // 2
    {
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        Process.Start(exePath, "-auto");
        Environment.Exit(0);
    }
    static void Clean() // 2
    {
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        Process.Start(exePath, "-task");
        Environment.Exit(0);
    }
    static void ChangeServiceStartup(string serviceName, bool enableAutoStart)
    {
        try
        {
            ServiceController service = new ServiceController(serviceName);

            // 打开服务
            service.Refresh();
            if (service.Status == ServiceControllerStatus.Running)
            {
                Logger.WriteLog(LogLevel.Info, LogKind.Service, $"Stopping Service: {serviceName}");
                service.Stop(); // 如果服务正在运行，停止服务
                service.WaitForStatus(ServiceControllerStatus.Stopped);
            }

            // 设置服务的启动类型
            SetServiceStartupType(serviceName, enableAutoStart);
            Logger.WriteLog(LogLevel.Info, LogKind.Service, $"Service {serviceName} has been set {(enableAutoStart ? "自动启动" : "手动启动")}");
            Logger.WriteLog(LogLevel.Info, $"{serviceName} disabled start successful");
        }
        catch (Exception ex)
        {
            Logger.WriteLog(LogLevel.Error, LogKind.Service, $"Error: {ex.Message}");
        }
    }

    // 设置服务的启动类型
    static void SetServiceStartupType(string serviceName, bool enableAutoStart)
    {
        string command = enableAutoStart
            ? $"sc config {serviceName} start= auto"  // 设置为自动启动
            : $"sc config {serviceName} start= demand"; // 设置为手动启动

        Process.Start("cmd.exe", $"/c {command}");
    }

    static void DisableTask(string taskName)
    {
        try
        {
            // 连接到本地任务计划程序
            using (TaskService ts = new TaskService())
            {
                // 查找任务
                Task task = ts.GetTask(taskName);

                if (task != null)
                {
                    // 禁用任务
                    task.Enabled = false;
                    Logger.WriteLog(LogLevel.Info, $"task '{taskName}' has been disabled");
                }
                else
                {
                    Logger.WriteLog(LogLevel.Warning, $"Cannot find task '{taskName}'");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLog(LogLevel.Error, $"failed to disable the task: {ex.Message}");
        }
    }

    static async System.Threading.Tasks.Task Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "-sh")
        {
            var processes = Process.GetProcessesByName("HipsTray");
            if (processes.Length > 0)
            {
                Logger.WriteLog(LogLevel.Warning, "process HipsTray.exe running");
                Console.Clear();
                Console.WriteLine(" 检测到火绒安全运行中,请在右下角关闭\n\n   正在等待关闭....");
                processes[0].WaitForExit();
                Logger.WriteLog(LogLevel.Info, "process HipsTray has been closed");
            }
            var processes2 = Process.GetProcessesByName("360Tray");
            if (processes2.Length > 0)
            {
                Logger.WriteLog(LogLevel.Warning, "process HipsTray.exe running");
                Console.Clear();
                Console.WriteLine(" 检测到360 安全卫士运行中,请在右下角关闭\n\n   正在等待关闭....");
                processes2[0].WaitForExit();
                Logger.WriteLog(LogLevel.Info, "process 360 Tray has been closed");
            }

            goto SchoolAct;
        }

        if (args.Length > 0 && args[0] == "-auto")
        {
            var processes = Process.GetProcessesByName("HipsTray");
            if (processes.Length > 0)
            {
                Logger.WriteLog(LogLevel.Warning, "process HipsTray.exe running");
                Console.Clear();
                Console.WriteLine(" 检测到火绒安全运行中,请在右下角关闭\n\n   正在等待关闭....");
                processes[0].WaitForExit();
                Logger.WriteLog(LogLevel.Info, "process HipsTray has been closed");
            }
            var processes2 = Process.GetProcessesByName("360Tray");
            if (processes2.Length > 0)
            {
                Logger.WriteLog(LogLevel.Warning, "process HipsTray.exe running");
                Console.Clear();
                Console.WriteLine(" 检测到360 安全卫士运行中,请在右下角关闭\n\n   正在等待关闭....");
                processes2[0].WaitForExit();
                Logger.WriteLog(LogLevel.Info, "process 360 Tray has been closed");
            }

            goto Task;
        }

        if (args.Length > 0 && args[0] == "-task")
        {
            goto CacheKiller;
        }

        Logger.ClearFile(Path.Combine(Directory.GetCurrentDirectory(), "SeewoPCEnhancedAssistant.log"));
        Console.Clear();
        Logger.WriteLog(LogLevel.Info, "enter main menu");
        Console.WriteLine("\n   Seewo PC Enhanced Assistant ver 1.5-beta \n   by Rainbow SPY - 2024.11.8\n\n\n" +
                          " 1.) 希沃系列产品优化(舜耕实验学校 电教办公室 出品)\n 2.) 其他功能\n\n 3.) 清理(Debug beta)\n\n将在5秒后执行默认操作......\n" +
                          "更新日志:\r\n+ 添加了自动清理系统\r\n" +
                          "+ 优化了清理方式\r\n");
        //创建 CancellationTokenSource 来取消默认操作
        var cts = new CancellationTokenSource();
        Logger.WriteLog(LogLevel.Info, "Creat CancellationTokenSource");

        // 创建一个默认操作的任务（会等待 5 秒）
        var defaultAction = System.Threading.Tasks.Task.Delay(5000, cts.Token).ContinueWith(_ =>
        {
            if (!cts.Token.IsCancellationRequested)
            {
                // 执行默认操作
                ExecuteOperation();
            }
        });
        Logger.WriteLog(LogLevel.Info, "Creat defaultAction");

        // 监听按键输入（例如按下 "1"）
        var keyPressAction = MonitorKeyPress(cts);
        Logger.WriteLog(LogLevel.Info, "Creat keyPressAction");

        // 等待 5 秒内的操作或按键操作完成
        var completedTask = await System.Threading.Tasks.Task.WhenAny(defaultAction, keyPressAction);

        // 如果按键操作完成，取消默认操作
        if (completedTask == keyPressAction)
        {
            // 按下 "1" 后取消默认操作
            cts.Cancel();
            Logger.WriteLog(LogLevel.Info, "Cancel default action");
        }
        Logger.WriteLog(LogLevel.Info, "Exit(0)");
        Environment.Exit(0);
    SchoolAct:
        {
            string txtPattern = "*.*";
            string[] txtFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp", txtPattern);
            foreach (string currentFile in txtFiles)
            {
                try
                {
                    File.Delete(currentFile);
                    Logger.WriteLog(LogLevel.Info, $"File Delete: {currentFile}");
                }
                catch (Exception ioException)
                {
                    Logger.WriteLog(LogLevel.Error, $"Error: {ioException}");
                }
            }

            if (File.Exists("C:/Program Files (x86)/Kingsoft/WPS Office/11.8.2.11019/office6/ksomisc.exe"))
            {
                Logger.WriteLog(LogLevel.Info, $"found \"C:/Program Files (x86)/Kingsoft/WPS Office/11.8.2.11019/office6/ksomisc.exe\"");
                #region config WPS Office

                Console.Title = "1.) 初始化 WPS Office";
                Form1 form = new Form1(); // 实例化提示窗体
                Logger.WriteLog(LogLevel.Info, LogKind.Form, "Form1 opened");
                Console.WriteLine("   请按提示在打开的 WPS Office 配置工具中修改配置\n   程序不会检测是否关联,仅做提醒");
                Thread formThread = new Thread(() => // 新建线程打开窗体
                {
                    form.ShowDialog();
                    Logger.WriteLog(LogLevel.Info, LogKind.Form, "Form1 Dialog showed");
                });
                formThread.Start(); // 打开线程
                Logger.WriteLog(LogLevel.Info, LogKind.Thread, "Form1 run successfully");

                Process process = new Process();
                process.StartInfo.FileName = "C:/Program Files (x86)/Kingsoft/WPS Office/11.8.2.11019/office6/ksomisc.exe";
                process.Start();
                process.WaitForExit(); // 等待退出
                Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                Logger.WriteLog(LogLevel.Info, LogKind.Process,
                    "Args: C:/Program Files (x86)/Kingsoft/WPS Office/11.8.2.11019/office6/ksomisc.exe");
                form.Close(); // 关闭窗体
                Logger.WriteLog(LogLevel.Info, LogKind.Form, "Form1 closed");
                formThread.Join(); // 停止调用线程
                Logger.WriteLog(LogLevel.Info, LogKind.Thread, "Thread Closed");
                #endregion
            }

            var xcopy = Directory.GetCurrentDirectory() + "\\IO.Copy.exe";
            var temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp";

            #region copy Edge,KGMusic,WeChat Copied

            for (int i = 1; i < 7; i++)
            {
                Process Edge = new Process();
                Edge.StartInfo.FileName = xcopy; // xcopy 命令复制文件
                Edge.StartInfo.Arguments = $".\\Resource.Enhanced.App.00{i}.bin \"{temp}\"";
                Edge.Start();
                Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                Logger.WriteLog(LogLevel.Info, LogKind.Process, $"Args: {Directory.GetCurrentDirectory() + "\\IO.Copy.exe"} .\\Resource.Enhanced.App.00{i}.bin \"{temp}\"");
                Edge.WaitForExit();
                if (Edge.ExitCode != 0)
                {
                    Logger.WriteLog(LogLevel.Error, $"Error: Resource.Enhanced.App.00{i}.bin copied failed \n    ExitCode:{Edge.ExitCode}");
                    Logger.WriteLog(LogLevel.Warning, $"Resource.Enhanced.App.00{i}.bin 复制失败");
                }
                else
                {
                    Logger.WriteLog(LogLevel.Info, $"copy file Resource.Enhanced.App.00{i}.bin");
                    File.Move($"{temp}\\Resource.Enhanced.App.00{i}.bin", $"{temp}\\Resource.Enhanced.App.00{i}");
                }
            }
            Process sevenZ = new Process();
            sevenZ.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\7za.exe";
            sevenZ.StartInfo.Arguments = $" x {temp}\\Resource.Enhanced.App.001 -o\"{temp}\"";
            sevenZ.Start();
            sevenZ.WaitForExit();

            File.Move($"{temp}\\Resource.Microsoft.Edge.bin", $"{temp}\\EdgeBrowserSetup.exe");
            File.Move($"{temp}\\Resource.KuGou.KGMusic.bin", $"{temp}\\KuGouMusicSetup.exe");
            File.Move($"{temp}\\Resource.Tencent.WeChat.bin", $"{temp}\\WeChatSetup.exe");
            Logger.WriteLog(LogLevel.Info, "completed rename");
            #endregion

            // 2 阶段
            Console.Clear();
            Console.Title = "2.) 开始安装常用软件";
            Console.WriteLine(" 开始安装软件...\n 请勿关闭程序,进行完之后会进行下一个安装程序\n\n请按安装程序步骤进行安装\n\n5秒后继续......");
            Thread.Sleep(5000);

            #region setup

            Process process1 = new Process();
            process1.StartInfo.FileName = $"{temp}\\EdgeBrowserSetup.exe";
            process1.StartInfo.Arguments = "/silent /install";
            process1.Start();
            Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
            Logger.WriteLog(LogLevel.Info, LogKind.Process, $"Args: {temp}\\EdgeBrowserSetup.exe");
            process1.WaitForExit();
            if (process1.ExitCode != 0)
            {
                Logger.WriteLog(LogLevel.Error, $"Error: Microsoft Edge install failed \n    ExitCode:{process1.ExitCode}");
                Logger.WriteLog(LogLevel.Warning, "Microsoft Edge 安装失败");
            }
            else
            {
                Process[] processes = Process.GetProcessesByName("msedge");
                foreach (Process p in processes)
                {
                    p.Kill();
                }

                Logger.WriteLog(LogLevel.Info, "Edge浏览器 安装成功");
            }

            Process process2 = new Process();
            process2.StartInfo.FileName = $"{temp}\\KuGouMusicSetup.exe";
            process2.Start();
            Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
            Logger.WriteLog(LogLevel.Info, LogKind.Process, $"Args: {temp}\\KuGouMusicSetup.exe");
            process2.WaitForExit();
            if (process2.ExitCode != 0)
            {
                Logger.WriteLog(LogLevel.Error, $"Error: KGMusic install failed \n    ExitCode:{process2.ExitCode}");
                Logger.WriteLog(LogLevel.Warning, "酷狗音乐 安装失败");
            }
            else
            {
                Logger.WriteLog(LogLevel.Info, "酷狗音乐 安装成功");
            }

            Process process3 = new Process();
            process3.StartInfo.FileName = $"{temp}\\WeChatSetup.exe";
            process3.Start();
            Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
            Logger.WriteLog(LogLevel.Info, LogKind.Process, $"Args: {temp}\\WeChatSetup.exe");
            process3.WaitForExit();
            if (process3.ExitCode != 0)
            {
                Logger.WriteLog(LogLevel.Error, $"Error: WeChat install failed \n    ExitCode:{process3.ExitCode}");
                Logger.WriteLog(LogLevel.Warning, "微信 安装失败");
            }
            else
            {
                Logger.WriteLog(LogLevel.Info, "微信 安装成功");
            }

            #endregion

            //3 阶段
            Console.Clear();
            Console.Title = "3.) 优化";
            Console.WriteLine("将会对系统和应用作进一步优化\n\n5秒后继续...");
            Thread.Sleep(5000);

            string registryPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            // 获取 HKEY_CURRENT_USER 注册表根
            RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath, writable: true);

            if (key != null)
            {
                try
                {
                    // 设置 "Hidden" 键的值为 1
                    key.SetValue("Hidden", 1, RegistryValueKind.DWord);
                    Logger.WriteLog(LogLevel.Info, "Registy: SetValue 1\nKind: Dword");

                    // 设置 "HideFileExt" 键的值为 0
                    key.SetValue("HideFileExt", 0, RegistryValueKind.DWord);
                    Logger.WriteLog(LogLevel.Info, "Registy: SetValue 0\nKind: Dword");
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(LogLevel.Error, $"error: {ex.Message}");
                }
                finally
                {
                    // 关闭注册表键
                    key.Close();
                    Logger.WriteLog(LogLevel.Info, "Registry Closed");
                }
            }
            else
            {
                Logger.WriteLog(LogLevel.Error, "cannot open registry");
            }

            #region Uninstall

            if (File.Exists("C:\\Program Files (x86)\\Tencent\\QQBrowser\\uninst.exe"))
            {
                try
                {
                    Process process4 = new Process();
                    process4.StartInfo.FileName = "C:\\Program Files (x86)\\Tencent\\QQBrowser\\uninst.exe";
                    process4.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "Args: C:\\Program Files (x86)\\Tencent\\QQBrowser\\uninst.exe");
                    process4.WaitForExit();
                    if (process4.ExitCode != 0)
                    {
                        Logger.WriteLog(LogLevel.Error, $"Error: QQ browser uninstall failed \n    ExitCode:{process4.ExitCode}");
                        Logger.WriteLog(LogLevel.Warning, "QQ浏览器 卸载失败");
                    }
                    else
                    {
                        Logger.WriteLog(LogLevel.Info, "QQ浏览器 卸载成功");
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteLog(LogLevel.Warning, $"cannot uninstall QQ浏览器\n  Exception: {exception}");
                }
            }

            if (File.Exists("C:\\Program Files (x86)\\FlashCenter\\FlashCenterUninst.exe"))
            {
                try
                {
                    Process process5 = new Process();
                    process5.StartInfo.FileName = "C:\\Program Files (x86)\\FlashCenter\\FlashCenterUninst.exe";
                    process5.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "Args: C:\\Program Files (x86)\\FlashCenter\\FlashCenterUninst.exe");
                    process5.WaitForExit();
                    Thread.Sleep(10000);
                    Process[] process99 = Process.GetProcessesByName("Un_A");
                    foreach (Process p in process99)
                    {
                        p.WaitForExit();
                        if (p.ExitCode != 0)
                        {
                            Logger.WriteLog(LogLevel.Error, $"Error: Flash Center uninstall failed \n    ExitCode:{p.ExitCode}");
                            Logger.WriteLog(LogLevel.Warning, "Flash中心 卸载失败");
                        }
                        else
                        {
                            Logger.WriteLog(LogLevel.Info, "Flash中心 卸载成功");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteLog(LogLevel.Warning, $"cannot uninstall Flash Center ,\n Exception: {exception}");
                }
            }

            if (File.Exists("C:\\Users\\Administrator\\AppData\\Local\\360Chrome\\Chrome\\Application\\13.5.1060.0\\installer\\setup.exe"))
            {
                try
                {
                    Process process6 = new Process();
                    process6.StartInfo.FileName = "C:\\Users\\Administrator\\AppData\\Local\\360Chrome\\Chrome\\Application\\13.5.1060.0\\installer\\setup.exe";
                    process6.StartInfo.Arguments = "--uninstall";
                    process6.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "Args: C:\\Users\\Administrator\\AppData\\Local\\360Chrome\\Chrome\\Application\\13.5.1060.0\\installer\\setup.exe --uninstall");
                    process6.WaitForExit();
                    if (process6.ExitCode != 0)
                    {
                        Logger.WriteLog(LogLevel.Error, $"Error: 360 browser uninstall failed \n    ExitCode:{process6.ExitCode}");
                        Logger.WriteLog(LogLevel.Warning, "360极速浏览器 卸载失败");
                    }
                    else
                    {
                        Logger.WriteLog(LogLevel.Info, "360极速浏览器 卸载成功");
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteLog(LogLevel.Warning, $"cannot uninstall 360极速浏览器,\n    Exception: {exception} ");
                }
            }

            if (File.Exists("C:\\Program Files (x86)\\Pure Codec\\uninst.exe"))
            {
                try
                {
                    Process process7 = new Process();
                    process7.StartInfo.FileName = "C:\\Program Files (x86)\\Pure Codec\\uninst.exe";
                    process7.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "Args: C:\\Program Files (x86)\\Pure Codec\\uninst.exe");
                    process7.WaitForExit();
                    Thread.Sleep(1000);
                    Process[] process88 = Process.GetProcessesByName("Un_A");
                    foreach (Process p in process88)
                    {
                        p.WaitForExit();
                        if (p.ExitCode != 0)
                        {
                            Logger.WriteLog(LogLevel.Error, $"Error: 完美解码 uninstall failed \n    ExitCode:{p.ExitCode}");
                            Logger.WriteLog(LogLevel.Warning, "完美解码 卸载失败");
                        }
                        else
                        {
                            Logger.WriteLog(LogLevel.Info, "完美解码 卸载成功");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteLog(LogLevel.Warning, $"cannot uninstall 完美解码,\n   Exception: {exception} ");
                }
            }

            if (File.Exists("C:\\Wub1.7\\Wub_x64.exe"))
            {
                try
                {
                    Process process10 = new Process();
                    process10.StartInfo.FileName = "C:\\Wub1.7\\Wub_x64.exe";
                    process10.StartInfo.Arguments = "/D";
                    process10.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "Args: C:\\Wub1.7\\Wub_x64.exe /D");
                    process10.WaitForExit();
                    if (process10.ExitCode != 0)
                    {
                        Logger.WriteLog(LogLevel.Error, $"Error: Windows Update disable failed \n    ExitCode:{process10.ExitCode}");
                        Logger.WriteLog(LogLevel.Warning, "Windows Update 禁用失败");
                    }
                    else
                    {
                        Logger.WriteLog(LogLevel.Info, "已禁用Windows Update");
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteLog(LogLevel.Error, $"Unknow Exception: {exception},cannot disabled Windows Update");
                }
            }

            if (File.Exists("C:\\HEU_KMS_Activator_v19.6.0.exe"))
            {
                try
                {
                    Process process12 = new Process();
                    process12.StartInfo.FileName = "C:\\HEU_KMS_Activator_v19.6.0.exe";
                    process12.StartInfo.Arguments = "/kms38";
                    process12.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "Args: C:\\HEU_KMS_Activator_v19.6.0.exe /kms38");
                    process12.WaitForExit();
                    if (process12.ExitCode != 0)
                    {
                        Logger.WriteLog(LogLevel.Error, $"Error: windows active failed \n    ExitCode:{process12.ExitCode}");
                        Logger.WriteLog(LogLevel.Warning, "Windows 激活失败");
                    }
                    else
                    {
                        Logger.WriteLog(LogLevel.Info, "已激活Windows 至 2038年");
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteLog(LogLevel.Error, $"Unknow Exception: {exception},cannot active Windows");
                }
            }

            #endregion

            Form2 form2 = new Form2();
            Thread formThread2 = new Thread(() =>
            {
                form2.ShowDialog();
                Logger.WriteLog(LogLevel.Info, LogKind.Form, "Form2 dialog showed");
            });
            Logger.WriteLog(LogLevel.Info, LogKind.Thread, "Form2 thread started");
            formThread2.Start();
            if (File.Exists("C:\\Pot-Player64_1.7.21486_Public_20210504\\PotPlayer64\\PotPlayerMini64.exe"))
            {
                try
                {
                    Process process11 = new Process();
                    process11.StartInfo.FileName = "C:\\Pot-Player64_1.7.21486_Public_20210504\\PotPlayer64\\PotPlayerMini64.exe";
                    process11.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process,
                        "Args: C:\\Pot-Player64_1.7.21486_Public_20210504\\PotPlayer64\\PotPlayerMini64.exe");
                    process11.WaitForExit();
                    if (process11.ExitCode != 0)
                    {
                        Logger.WriteLog(LogLevel.Error,
                            $"Error: Pot-Player config failed \n    ExitCode:{process11.ExitCode}");
                        Logger.WriteLog(LogLevel.Warning, "Pot-Player 配置失败");
                    }
                    else
                    {
                        Logger.WriteLog(LogLevel.Warning, "Pot-Player 配置成功");
                        form2.Close();
                        formThread2.Join();
                        Logger.WriteLog(LogLevel.Info, LogKind.Thread, "Form2 thread exited");
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteLog(LogLevel.Error,
                        $"Unknow Exception:{exception},cannot find C:\\Pot-Player64_1.7.21486_Public_20210504\\PotPlayer64\\PotPlayerMini64.exe");
                }
            }

            #region disable services and tasks

            Logger.WriteLog(LogLevel.Info, "Disabling Service: wuauserv");
            ChangeServiceStartup("wuauserv", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: EasiUpdate");
            ChangeServiceStartup("EasiUpdate", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: EasiUpdate3");
            ChangeServiceStartup("EasiUpdate3", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: EasiUpdate3Protect");
            ChangeServiceStartup("EasiUpdate3Protect", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: Flash Helper Service");
            ChangeServiceStartup("Flash Helper Service", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: jhi_service");
            ChangeServiceStartup("jhi_service", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: igccservice");
            ChangeServiceStartup("igccservice", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: LMS");
            ChangeServiceStartup("LMS", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: InstallService");
            ChangeServiceStartup("InstallService", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: cphs");
            ChangeServiceStartup("cphs", false);
            Logger.WriteLog(LogLevel.Info, "Disabling Service: cplspcon");
            ChangeServiceStartup("cplspcon", false);


            Logger.WriteLog(LogLevel.Info, "Disabling Task: Intel PTT EK Recertification");
            DisableTask("Intel PTT EK Recertification");
            Logger.WriteLog(LogLevel.Info, "Disabling Task: QQBrowser Updater Task");
            DisableTask("QQBrowser Updater Task");
            Logger.WriteLog(LogLevel.Info, "Disabling Task: QQBrowser Updater Task(Core)");
            DisableTask("QQBrowser Updater Task(Core)");
            Logger.WriteLog(LogLevel.Info, "Disabling Task: WpsUpdateTask_Administrator");
            DisableTask("WpsUpdateTask_Administrator");

            #endregion

            Console.Clear();
            Console.Title = "4.) 清理部署";
            Console.WriteLine("此阶段是部署的最后阶段\n\n按任意键继续......");
            Console.ReadKey();

            if (File.Exists("C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\sysclean.exe"))
            {
                try
                {
                    Process process8 = new Process();
                    process8.StartInfo.FileName = "C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\sysclean.exe";
                    process8.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "Args: C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\sysclean.exe");
                    process8.WaitForExit();
                    if (process8.ExitCode != 0)
                    {
                        Logger.WriteLog(LogLevel.Warning, $"Warning: sysclean open failed \n    ExitCode:{process8.ExitCode}");
                        Logger.WriteLog(LogLevel.Warning, "火绒垃圾清理 打开失败");
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLog(LogLevel.Error, $"Unknow Exception: {e},cannot find C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\sysclean.exe");
                    Logger.WriteLog(LogLevel.Error, $"Unknow Exception: {e},cannot find C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\sysclean.exe");
                    Logger.WriteLog(LogLevel.Error, $"Unknow Exception: {e},cannot find C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\sysclean.exe");
                }

                try
                {
                    Process process9 = new Process();
                    process9.StartInfo.FileName = "C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\RightClickMan.exe";
                    process9.Start();
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "process started successfully");
                    Logger.WriteLog(LogLevel.Info, LogKind.Process, "Args: C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\RightClickMan.exe");
                    process9.WaitForExit();
                    if (process9.ExitCode != 0)
                    {
                        Logger.WriteLog(LogLevel.Error, $"Error:RightClickMan open failed \n    ExitCode:{process9.ExitCode}");
                        Logger.WriteLog(LogLevel.Warning, "火绒右键菜单管理 打开失败");
                    }
                }
                catch (Exception exception)
                {
                    Logger.WriteLog(LogLevel.Error, $"Unknow Exception: {exception},cannot find C:\\Program Files (x86)\\Huorong\\Sysdiag\\bin\\RightClickMan.exe");
                }
            }

            Console.Title = "Complete!";
            Console.Clear();
            Console.WriteLine("已完成操作,按任意键退出.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    Task:
        {
            Logger.WriteLog(LogLevel.Info, "Create task");
            var xcopy = Directory.GetCurrentDirectory() + "\\IO.Copy.exe";
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string doc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Process.Start(xcopy, $"{exePath} {doc}");
            TaskSchedulerHelper.CreateTask("Seewo PC Enhanced Assistant", $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{Process.GetCurrentProcess().ProcessName}.exe -task", "daily", "17:30");
        }
    CacheKiller:
        {
            string weChatBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WeChat Files");
            string explorerCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Explorer");

            List<string> directories = new List<string>();            // 存储要处理的目录列
            List<string> files = new List<string>();

            foreach (string userDir in Directory.GetDirectories(weChatBasePath, "wxid_*", SearchOption.TopDirectoryOnly))
            {
                // 拼接每个用户的 Sns Cache 路径
                string snsCachePath = Path.Combine(userDir, "FileStorage", "Sns", "Cache");

                if (Directory.Exists(snsCachePath))
                {
                    directories.Add(snsCachePath); // 添加到目录清理列表
                }
            }
            if (Directory.Exists(explorerCachePath))
            {
                //使用正则表达式匹配文件名：iconcache_数字.db 和 thumbcache_数字.db
                Regex regex = new Regex(@"(iconcache_\d+\.db|thumbcache_\d+\.db)", RegexOptions.IgnoreCase);
                foreach (string file in Directory.GetFiles(explorerCachePath, "*.db", SearchOption.TopDirectoryOnly))
                {
                    if (regex.IsMatch(Path.GetFileName(file)))
                    {
                        files.Add(file); // 添加符合条件的文件到列表
                    }
                }
            }
            // 动态添加多个路径
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp")); // %TEMP%
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kingsoft\\office6\\backup"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\kingsoft\\office6\\log"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KuGou8\\CefCache89"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KuGou8\\ImagesCache"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KuGou8\\log"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KuGou8\\network_log"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\Edge\\User Data\\Default\\Cache"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\Edge\\User Data\\Default\\Code Cache"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CrashDumps"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\Windows\\WebCache"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\System32\\LogFiles"));
            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Installer"));
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                    Logger.WriteLog(LogLevel.Info, $"删除文件: {file}");
                }
                catch (Exception ex)
                {
                    Logger.PrintLog(LogLevel.Warning, $"Warning: {ex}");
                }
            }
            foreach (string dirs in directories)
            {
                try
                {
                    bool hasFiles = Directory.GetFiles(dirs).Length > 0;
                    if (!hasFiles)
                    {
                        Logger.WriteLog(LogLevel.Info, $"folder {dirs} is empty");
                        continue;
                    }
                    foreach (string file in Directory.GetFiles(dirs))
                    {
                        try
                        {
                            File.Delete(file);
                            Logger.WriteLog(LogLevel.Info, $"删除文件: {file}");
                        }
                        catch (Exception ex)
                        {
                            Logger.PrintLog(LogLevel.Warning, $"Warning: {ex}");
                        }
                    }

                    // 删除目录下的所有子目录及其内容
                    foreach (string subDir in Directory.GetDirectories(dirs))
                    {
                        try
                        {
                            Directory.Delete(subDir, true);
                            Logger.WriteLog(LogLevel.Info, $"删除文件: {subDir}");
                        }
                        catch (Exception ex)
                        {
                            Logger.PrintLog(LogLevel.Warning, $"Warning: {ex}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.PrintLog(LogLevel.Warning, $"Warning: {ex}");
                }
            }
        }
    }


    // 监听按键输入的方法，确保它异步运行
    static async System.Threading.Tasks.Task MonitorKeyPress(CancellationTokenSource cts)
    {
        var token = cts.Token;

        // 使用任务监听按键输入
        await System.Threading.Tasks.Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true).KeyChar;
                    // 如果输入了 "1"，执行 "1" 的操作（与默认操作相同）
                    if (key == '1')
                    {
                        ExecuteOperation();  // 执行操作
                        cts.Cancel();  // 取消默认操作
                        break;  // 跳出循环，防止继续等待
                    }
                    // 如果输入了 "1"，执行 "1" 的操作（与默认操作相同）
                    if (key == '2')
                    {
                        AddTask();  // 执行操作
                        cts.Cancel();  // 取消默认操作
                        break;  // 跳出循环，防止继续等待
                    }
                    if (key == '3')
                    {
                        Clean();  // 执行操作
                        cts.Cancel();  // 取消默认操作
                        break;  // 跳出循环，防止继续等待
                    }
                }
                await System.Threading.Tasks.Task.Delay(100);  // 使用 await 确保不会阻塞
            }
        }, token);
    }
    // 执行操作的方法
}