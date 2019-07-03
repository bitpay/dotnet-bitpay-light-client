using System;

namespace BitPayLight.Exceptions
{
    public class RatesQueryException : BitPayException
    {
        private const string BitPayCode = "BITPAY-RATES-GET";
        private const string BitPayMessage = "Failed to retrieve invoice";

        public RatesQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RatesQueryException(Exception ex) : base(BitPayCode, BitPayMessage, ex)
        {
        }
    }
}