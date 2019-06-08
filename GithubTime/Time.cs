using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GithubTime
{
	class Time
	{
	}

	public class Win32API
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

		public void FromDateTime(DateTime dateTime)
		{
			wYear = (ushort)dateTime.Year;
			wMonth = (ushort)dateTime.Month;
			wDayOfWeek = (ushort)dateTime.DayOfWeek;
			wDay = (ushort)dateTime.Day;
			wHour = (ushort)dateTime.Hour;
			wMinute = (ushort)dateTime.Minute;
			wSecond = (ushort)dateTime.Second;
			wMilliseconds = (ushort)dateTime.Millisecond;
		}

		public DateTime ToDateTime()
		{
			return new DateTime(wYear, wMonth, wDay, wHour,
				wMinute, wSecond, wMilliseconds);
		}

		public static DateTime SystemTimeToDateTime(SYSTEMTIME time)
		{
			return time.ToDateTime();
		}

		public static SYSTEMTIME DateTimeToSystemTime(DateTime time)
		{
			SYSTEMTIME systemTime = new SYSTEMTIME();
			systemTime.FromDateTime(time);
			return systemTime;
		}
	}
}
