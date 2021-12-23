using System;

namespace BitPayLight.Exceptions
{
    public class RatesQueryException : BitPayException
    {
        private const string BitPayCode = "BITPAY-RATES-GET";
        private const string BitPayMessage = "Failed to retrieve invoice";
        protected string ApiCode;

        public RatesQueryException() : base(BitPayCode, BitPayMessage)
        {
        }

        public RatesQueryException(Exception ex, string apiCode = "000000") : base(BitPayCode, BitPayMessage, ex)
        {
            ApiCode = apiCode;
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
