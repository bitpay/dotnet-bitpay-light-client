﻿using System;

namespace BitPayLight.Exceptions
{
    public class BillCreationException : BitPayException
    {
        private const string BitPayCode = "BITPAY-BILL-CREATE";
        private const string BitPayMessage = "Failed to create bill";
        protected string ApiCode;

        public BillCreationException() : base(BitPayCode, BitPayMessage)
        {
        }

        public BillCreationException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
