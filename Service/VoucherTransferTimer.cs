using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatValues.Web.Models;

namespace ChatValues.Service
{
	public class VoucherTransferTimer : JobTimer
	{
		public ChatValuesContext _context;
		public VoucherTransferTimer(ChatValuesContext context, IConfiguration configuration) : base(configuration, "JobCardExpiryTimer:Interval")
		{
			_context = context;
		}

		protected override void OnTimerElapsed()
		{
			//for Time being
			DateTime todayDate = DateTime.Now;
			var VoucherToTransferDetails = _context.GrantedVouchers.Where<GrantedVouchers>(c => c.Status == GrantStatus.Pending && ((TimeSpan)(todayDate - c.GrantedOn)).Days > 7).ToList();
			if (VoucherToTransferDetails.Count > 0)
			{
				foreach (var grantedVoucher in VoucherToTransferDetails)
				{
					Voucher transferVoucher = new Voucher();

					transferVoucher.CreateOn = todayDate;
					transferVoucher.ModifiedOn = todayDate;
					transferVoucher.TransactionDate = todayDate;
					transferVoucher.OwnerId = grantedVoucher.GrantedToID;
					transferVoucher.Quantity = grantedVoucher.Vouchers;
					transferVoucher.CreditorId = grantedVoucher.GrantedByID;
					transferVoucher.TransactionTypeId = (int)GrantStatus.Transfered;
					_context.Vouchers.Add(transferVoucher);
					_context.SaveChanges();

					grantedVoucher.Status = GrantStatus.Transfered;
					_context.SaveChanges();
				}
			}

		}
	}

}
