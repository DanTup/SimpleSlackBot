using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSlackBot
{
	static class RandomMessages
	{
		static Random rnd = new Random();
		static T RandomOf<T>(params T[] items)
		{
			return items[rnd.Next(items.Length)];
		}

		public static string Hello() => RandomOf("Hello!", "I'm back!");
	}
}
