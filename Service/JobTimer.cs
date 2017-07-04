using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatValues.Service
{
	public abstract class JobTimer
	{
		int interval;

		public JobTimer(IConfiguration configuration, string intervalKey)
		{
			interval = int.Parse(configuration.GetSection(intervalKey).Value);
		}

		Timer timer;

		public void Start()
		{
			TimerCallback callback = new TimerCallback(OnTimerElapsedCore);
			timer = new Timer(callback, null, -1, -1);
			EnableTimer(true);
		}

		public void OnTimerElapsedCore(object sender)
		{
			EnableTimer(false);
			try
			{
				OnTimerElapsed();
			}
			catch (Exception x)
			{
				Console.WriteLine(x.Message);
				Console.WriteLine(x.StackTrace);
			}
			EnableTimer(true);
		}

		protected abstract void OnTimerElapsed();

		private void EnableTimer(bool enable)
		{
			if (enable)
			{
				var midnight = DateTime.Now.AddDays(1).Date;
				var interval = (midnight- DateTime.Now).TotalMilliseconds;
				timer.Change((int)interval, -1);
				Console.WriteLine("Timer will start at {0}", midnight.ToString("MMM-dd-yyyy hh:mm:ss"));
			}
			else
				timer.Change(-1, -1);
		}

		public void Stop()
		{
			timer.Dispose();
		}
	}
}
