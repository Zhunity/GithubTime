using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GithubTime
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// 取得当前系统时间
			DateTime dateTime = DateTime.Now;

			dateTime = dateTime.AddDays(7);

			SYSTEMTIME systemTime = DateTimeToSystemTime(dateTime);

			bool flag = Win32API.SetLocalTime(ref systemTime);
			SYSTEMTIME localTime = new SYSTEMTIME();
			try
			{
				Win32API.GetLocalTime(ref localTime);
				MessageBox.Show(SystemTimeToDateTime(localTime).ToString() + "  " + flag.ToString());
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			
		}

		public string Execute(string exe, string arg, string workDir = "")
		{
			System.Diagnostics.ProcessStartInfo start =
				new System.Diagnostics.ProcessStartInfo(exe);
			start.Arguments = arg; //确定程式命令行
			if (!string.IsNullOrEmpty(workDir))
			{
				start.WorkingDirectory = workDir;
			}

			start.UseShellExecute = false; //Shell的使用
			start.RedirectStandardInput = true; //重定向输入
			start.RedirectStandardOutput = true; //重定向输出
			start.RedirectStandardError = true; //重定向输出错误
			start.CreateNoWindow = true; //设置置不显示示窗口

			var proc = System.Diagnostics.Process.Start(start);

			//输出出流取得命令行结果
			if (proc != null && !start.UseShellExecute)
			{
				string ret = proc.StandardOutput.ReadToEnd();
				if (!string.IsNullOrEmpty(ret))
				{
					//Debug.Log(ret);
				}

				ret = proc.StandardError.ReadToEnd();
				if (!string.IsNullOrEmpty(ret))
				{
					//Debug.LogError(ret);
					return ret;
				}
			}
			return null;
		}

		public void FromDateTime(out SYSTEMTIME systemTime, DateTime dateTime)
		{
			systemTime.wYear = (ushort)dateTime.Year;
			systemTime.wMonth = (ushort)dateTime.Month;
			systemTime.wDayOfWeek = (ushort)dateTime.DayOfWeek;
			systemTime.wDay = (ushort)dateTime.Day;
			systemTime.wHour = (ushort)dateTime.Hour;
			systemTime.wMinute = (ushort)dateTime.Minute;
			systemTime.wSecond = (ushort)dateTime.Second;
			systemTime.wMilliseconds = (ushort)dateTime.Millisecond;
		}

		public DateTime ToDateTime(SYSTEMTIME systemTime)
		{
			return new DateTime(systemTime.wYear, systemTime.wMonth, systemTime.wDay, systemTime.wHour,
				systemTime.wMinute, systemTime.wSecond, systemTime.wMilliseconds);
		}

		public DateTime SystemTimeToDateTime(SYSTEMTIME time)
		{
			return ToDateTime(time);
		}

		public SYSTEMTIME DateTimeToSystemTime(DateTime time)
		{
			SYSTEMTIME systemTime = new SYSTEMTIME();
			FromDateTime(out systemTime, time);
			return systemTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SYSTEMTIME
		{
			public ushort wYear;
			public ushort wMonth;
			public ushort wDayOfWeek;
			public ushort wDay;
			public ushort wHour;
			public ushort wMinute;
			public ushort wSecond;
			public ushort wMilliseconds;
		}

		private class Win32API
		{
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern bool SetLocalTime(ref SYSTEMTIME time);

			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern bool GetLocalTime(ref SYSTEMTIME time);

			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern bool SetSystemTime(ref SYSTEMTIME time);

			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern bool GetSystemTime(ref SYSTEMTIME time);
		}
	}
}