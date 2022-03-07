using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Web;
using System.Globalization;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace populations
{

	public class Program
	{
		static JObject GimmeJson(string link)
		{
			WebRequest wrq = WebRequest.CreateHttp(link);
			string result;
			wrq.Method = "GET";
			wrq.Headers["x-rapidapi-key"] = "ae97dc7834mshf6b6af0f0ca416ep19aa3bjsnb0d9f4176aef";
			wrq.Headers["x-rapidapi-host"] = link.Split('/')[2];

			try
			{
				using (HttpWebResponse wrsp = wrq.GetResponse() as HttpWebResponse)
				{
					using (Stream sr = wrsp.GetResponseStream())
					{
						using (StreamReader srr = new StreamReader(sr))
						{
							result = srr.ReadToEnd();
						}
					}
				}
			}
			catch (WebException we)
			{
				return null;
			}
			JObject job = JObject.Parse(result);
			return JObject.Parse(job["body"].ToString());
		}

		static long SaveSumOfpeople(string country)
		{
			JObject response = GimmeJson($"https://world-population.p.rapidapi.com/population?country_name={country}");

			if (response == null)
			{
				return patch[country];
			}
			return response["population"].ToObject<long>();
		}

		static private Dictionary<string, long> patch = new Dictionary<string, long>()
		{
            {"Brunei ", 0},
            {"Sao Tome & Principe", 219159},
            {"Saint Kitts & Nevis", 53199},
            {"Saint Pierre & Miquelon", 5794},
            {"Wallis & Futuna", 11239},
            {"St. Vincent & Grenadines", 110940}, 
		};

		public static void Main()
		{
			JObject wpop = GimmeJson("https://world-population.p.rapidapi.com/worldpopulation");
			long weAll = wpop["world_population"].ToObject<long>();
			int arrlen = wpop["total_countries"].ToObject<int>();
			long popSum = -1;

			JObject lands = GimmeJson("https://world-population.p.rapidapi.com/allcountriesname");
			string[] names = lands["countries"].ToObject<string[]>();

			long you = (long)(weAll * (new Random()).NextDouble());
			Console.WriteLine($"your life number is {you}");
			Console.WriteLine("looking for motherland...");

			bool leftEdge, rightEdge;
			foreach (string name in names)
			{
				leftEdge = you >= popSum;
				popSum = popSum + SaveSumOfpeople(name);
				rightEdge = you <= popSum;

				if (leftEdge && rightEdge)
				{
					Console.WriteLine($"you will be spawned back in {name}");
					return;
				}
			}
		}
	}
}