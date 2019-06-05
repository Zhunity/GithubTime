using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class GitHubTime : MonoBehaviour
{
	private struct SystemTime
	{
		public ushort wYear;
		public ushort wMonth;
		public ushort wDayOfWeek;
		public ushort wDay;
		public ushort wHour;
		public ushort wMinute;
		public ushort wSecond;
		public ushort wMilliseconds;

		public void FromDateTime(DateTime time)
		{
			wYear = (ushort)time.Year;
			wMonth = (ushort)time.Month;
			wDayOfWeek = (ushort)time.DayOfWeek;
			wDay = (ushort)time.Day;
			wHour = (ushort)time.Hour;
			wMinute = (ushort)time.Minute;
			wSecond = (ushort)time.Second;
			wMilliseconds = (ushort)time.Millisecond;
		}

		public DateTime ToDateTime()
		{
			return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
		}

		public static DateTime SystemTimeToDateTime(SystemTime time)
		{
			return time.ToDateTime();
		}

		public static SystemTime DateTimeToSystemTime(DateTime time)
		{
			SystemTime systemTime = new SystemTime();
			systemTime.FromDateTime(time);
			return systemTime;
		}
	}

	private class Win32API
	{
		[DllImport("Kernel32.dll")]
		public static extern bool SetLocalTime(ref SystemTime time);

		[DllImport("Kernel32.dll")]
		public static extern bool GetLocalTime(ref SystemTime time);
	}

	

    // Start is called before the first frame update
    void Start()
    {
		// 取得当前系统时间
		DateTime dateTime = DateTime.Now;
		Debug.Log(dateTime);

		dateTime = dateTime.AddDays(7);

		SystemTime systemTime = SystemTime.DateTimeToSystemTime(dateTime);
		Debug.Log(systemTime.wDay);

		Debug.Log(Win32API.SetLocalTime(ref systemTime));

    }
}
