using System;
using System.Diagnostics;
using System.IO;

class Program
{
    /// <summary>
    /// mgrep 启动器入口：转发到 node + mgrep 的 dist/index.js
    /// </summary>
    static int Main(string[] args)
    {
        // exe 所在目录，用于拼接默认的 dist/index.js 路径
        // 这样可以支持 "mgrep.exe 同级目录下有 dist/index.js" 的布局
        string exeDir = AppContext.BaseDirectory;

        // 1. 决定 node 可执行路径
        // 优先使用环境变量 MGREP_NODE，否则退回到 PATH 中的 node
        string nodePath = Environment.GetEnvironmentVariable("MGREP_NODE");
        if (string.IsNullOrWhiteSpace(nodePath))
        {
            nodePath = "node"; // 依赖环境 PATH
        }

        // 2. 决定 mgrep 脚本路径
        // 优先使用环境变量 MGREP_SCRIPT，
        // 否则默认使用 "<exe目录>/dist/index.js"
        string scriptPath = Environment.GetEnvironmentVariable("MGREP_SCRIPT");
        if (string.IsNullOrWhiteSpace(scriptPath))
        {
            scriptPath = Path.Combine(exeDir, "dist", "index.js");
        }

        // 3. 检查脚本是否存在，给出清晰错误提示
        if (!File.Exists(scriptPath))
        {
            Console.Error.WriteLine($"[mgrep-launcher] 找不到 mgrep 脚本：{scriptPath}");
            Console.Error.WriteLine("[mgrep-launcher] 请确保 dist/index.js 存在，或者设置环境变量 MGREP_SCRIPT 指向 dist/index.js 的绝对路径。");
            return 1;
        }

        // 4. 构造进程启动信息：node <scriptPath> + 用户传入的所有参数
        var psi = new ProcessStartInfo
        {
            FileName = nodePath,
            UseShellExecute = false,
            RedirectStandardInput = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
        };

        // 第一个参数是 mgrep 的入口脚本
        psi.ArgumentList.Add(scriptPath);

        // 追加用户传入的所有参数
        foreach (var arg in args)
        {
            psi.ArgumentList.Add(arg);
        }

        try
        {
            using var process = Process.Start(psi);
            if (process == null)
            {
                Console.Error.WriteLine("[mgrep-launcher] 启动 mgrep 失败：Process.Start 返回空实例。");
                return 1;
            }

            process.WaitForExit();
            return process.ExitCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[mgrep-launcher] 启动 mgrep 失败：{ex.Message}");
            Console.Error.WriteLine("请检查：");
            Console.Error.WriteLine("  1）是否已安装 Node，并且在 PATH 中或通过 MGREP_NODE 指定；");
            Console.Error.WriteLine("  2）MGREP_SCRIPT 或默认 dist/index.js 路径是否正确。");
            return 1;
        }
    }
}
