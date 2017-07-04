using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ChatValues.PayPal
{
    public class PayPalManager
    {
		public string GenerateButton(TransactionType transactionType, decimal amount,int quantity,int? transactionID)
		{
			ButtonGenerator generator = new ButtonGenerator(new PayPalCredentials(), amount, transactionType, quantity, transactionID);
			return RequestButton(generator.Build());
		}

		private string RequestButton(string param)
		{
			//System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			var paypalApiServerUrl = "https://api-3t.sandbox.paypal.com/nvp";
			var wrWebRequest = (HttpWebRequest)WebRequest.Create(paypalApiServerUrl);
			wrWebRequest.Method = "POST";
			using (var requestWriter = new StreamWriter(wrWebRequest.GetRequestStreamAsync().Result))
			{
				requestWriter.Write(param);
			}

			string responseData = string.Empty;
			using (var responseReader = new StreamReader(wrWebRequest.GetResponseAsync().Result.GetResponseStream()))
			{

				responseData = responseReader.ReadToEnd();
			}
			var result = WebUtility.UrlDecode(responseData);
			var button = ExtractHtmlFromResponse(result);
			return button;
		}

		private static string ExtractHtmlFromResponse(string result)
		{
			var pairs = result.Split('&');
			foreach (var pair in pairs)
			{
				var keyAndValue = pair.Split('=');
				var key = keyAndValue[0];
				var value = string.Join("=", GetSubArray(keyAndValue, 1, keyAndValue.Length - 1));
				if (key == "WEBSITECODE") return value;
			}

			throw new Exception("No button found in PayPal response");
		}


		private static string[] GetSubArray(string[] array, int index, int length)
		{
			var result = new string[length];
			Array.Copy(array, index, result, 0, length);
			return result;
		}
	}
}
