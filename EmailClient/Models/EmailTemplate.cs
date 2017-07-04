using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.EmailClient.Models
{
    public enum EmailTemplate
    {
		Welcome,
        EmailVerification,
		JobExpiry,
		Recommendation,
		NewMessage,
		NewVoiceMessage,
		Invitation,
		JobPurchase , 
        ForgotPass 
    }
}
