using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatValues.PayPal
{
	public class ButtonGenerator
	{


		private readonly Dictionary<string, string> _variables = new Dictionary<string, string>();

		private void AddOrUpdate(string key, string value)
		{
			if (_variables.ContainsKey(key))
			{
				_variables[key] = value;
			}
			else _variables.Add(key, value);
		}

		public ButtonGenerator(PayPalCredentials credentials, decimal amount, TransactionType transaction, int quantity,int? transactionID)
		{

			int bvCount = 0;
			AddOrUpdate("USER", credentials.ApiUsername);
			AddOrUpdate("PWD", credentials.ApiPassword);
			AddOrUpdate("SIGNATURE", credentials.ApiSignature);

			//Api method name and version
			AddOrUpdate("METHOD", "BMCreateButton");
			AddOrUpdate("VERSION", "85.0");

			//method specific parameters
			AddOrUpdate("BUTTONTYPE", "BUYNOW");
			AddOrUpdate("BUTTONSUBTYPE", "PRODUCTS");

			//Buynow button specific parameters
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "lc=EN");
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "button_subtype=PRODUCTS");
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "item_name=Vouchers");
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "amount=5");
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "currency_code=USD");
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "quantity=" + quantity + "");
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "rm=2");
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "return=http://localhost:1128/paypal/purchase/complete/" + transactionID + "");
			bvCount++; AddOrUpdate("L_BUTTONVAR" + bvCount.ToString(), "cancel_return=http://localhost:1128/paypal/purchase/cancel/" + transactionID + "");
			//bvCount = bvCount + 1 : NVP.Add("L_BUTTONVAR" & bvCount, "cmd=_s-xclick")  //DON'T specify the cmd parameter, if you specify it, it wont work, paypal will give you an error

		}

		public string Build()
		{
			var paramBuilder = new StringBuilder();
			foreach (var st in _variables.Select(kv => kv.Key + "=" + kv.Value + "&"))
			{
				paramBuilder.Append(st);
			}

			var parameterString = paramBuilder.ToString();
			parameterString = parameterString.Substring(0, parameterString.Length - 1);
			return parameterString;
		}
	}
}
