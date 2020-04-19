using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.Forms.MessageBox;
using System.IO;

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

		Random rd = new Random();
		DateTime beginDate ;
		DateTime endDate;
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			beginDate = DateTime.Parse(BeginDate.Text);
			endDate = DateTime.Parse(EndDate.Text);
			Thread thread = new Thread(DatePass);
			thread.Start();
		}

		/// <summary>
		/// 设置系统时间
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		private void SetLocalTime(DateTime dateTime)
		{
			try
			{
				SYSTEMTIME systemTime = SYSTEMTIME.DateTimeToSystemTime(dateTime);

				bool flag = Win32API.SetLocalTime(ref systemTime);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		/// <summary>
		/// 提交Github内容
		/// </summary>
		/// <param name="dateTime"></param>
		private void GitHubCommit(DateTime dateTime)
		{
			Execute(GitPath.Text, "add .", CommitPath.Text);
			Execute(GitPath.Text, "commit -m \"" + dateTime.ToString() + "\"", CommitPath.Text);
		}

		/// <summary>
		/// 添加文本
		/// </summary>
		/// <param name="dateTime"></param>
		private void AppendText(DateTime dateTime)
		{
			string txtPath = CommitPath.Text + "/commit.txt";

			File.AppendAllText(txtPath, "\n" + dateTime.ToString());
		}

		/// <summary>
		/// 日期叠加线程
		/// </summary>
		private void DatePass()
		{
			this.Dispatcher.Invoke(()=>
			{
				TimeSpan span = endDate - beginDate;
				float daySpan = span.Days;
				DateTime date = beginDate;
				for (float nowDay = 0; nowDay <= daySpan; nowDay++)
				{
					CommitLabel.Content = date.ToString();
					CommitProgress.Value = nowDay / daySpan;
					SetLocalTime(date);
					AppendText(date);
					GitHubCommit(date);
					date = date.AddDays(1);
					CommitLabel.Content = "span:" + (endDate - date).ToString() + "   endDate:" + EndDate.Text.ToString() + "   date:" + BeginDate.Text.ToString();
				}
			} );
		}

		public string Execute(string exe, string arg, string workDir = "")
		{
			try
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
						MessageBox.Show(ret);
					}

					ret = proc.StandardError.ReadToEnd();
					if (!string.IsNullOrEmpty(ret))
					{
						MessageBox.Show(ret);
						return ret;
					}
				}
				return null;
			}
			catch(Exception e)
			{
				MessageBox.Show(e.ToString());
				return e.ToString();
			}
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

		FolderBrowserDialog folderDialog = new FolderBrowserDialog();
		private void Commit_Click(object sender, RoutedEventArgs e)
		{
			var result = folderDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.Cancel)
			{
				return;	
			}
			CommitPath.Text = folderDialog.SelectedPath;
			Properties.Settings.Default.CommitPath = folderDialog.SelectedPath;
			Properties.Settings.Default.Save();
		}
	}
}