﻿using System;

namespace BitPayLight.Exceptions
{
    public class InvoiceCreationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-INVOICE-CREATE";
        private const string BitPayMessage = "Failed to create invoice";
        protected string ApiCode;

        public InvoiceCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public InvoiceCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
