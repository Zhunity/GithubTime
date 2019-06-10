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
using Microsoft.Win32;

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
			// Properties.Settings.Default.GitPath 为应用程序时，会被设成只读
			GitPath.Text = Properties.Settings.Default.GitPath;
			CommitPath.Text = Properties.Settings.Default.CommitPath;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// 取得当前系统时间
			DateTime dateTime = DateTime.Now;

			dateTime = dateTime.AddDays(7);

			SYSTEMTIME systemTime = SYSTEMTIME.DateTimeToSystemTime(dateTime);

			bool flag = Win32API.SetLocalTime(ref systemTime);
			SYSTEMTIME localTime = new SYSTEMTIME();
			try
			{
				Win32API.GetLocalTime(ref localTime);
				MessageBox.Show(SYSTEMTIME.SystemTimeToDateTime(localTime).ToString() + "  " + flag.ToString());
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

		

		

		

		OpenFileDialog fileDialog = new OpenFileDialog();
		private void Browse_Click(object sender, RoutedEventArgs e)
		{
			var result = fileDialog.ShowDialog();
			if(result == true)
			{
				GitPath.Text = fileDialog.FileName;
				Properties.Settings.Default.GitPath = fileDialog.FileName;
				Properties.Settings.Default.Save();
			}
		}

		private void Commit_Click(object sender, RoutedEventArgs e)
		{
			var result = fileDialog.ShowDialog();
			if (result == true)
			{
				CommitPath.Text = fileDialog.FileName;
				Properties.Settings.Default.CommitPath = fileDialog.FileName;
				Properties.Settings.Default.Save();
			}
		}
	}
}