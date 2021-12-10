using System;

namespace BitPayLight.Exceptions
{
    public class BitPayLightCommunicationException : BitPayException
    {
        private const string BitPayMessage = "Error when communicating with the BitPay API";
        private static readonly string _bitpayCode = "BITPAY-API";
        protected string ApiCode;

        public BitPayLightCommunicationException() : base(BitPayMessage)
        {
            BitpayCode = _bitpayCode;
        }

        public BitPayLightCommunicationException(Exception ex) : base(BitPayMessage, ex)
        {
            BitpayCode = _bitpayCode;
        }

        public BitPayLightCommunicationException(string message) : base(_bitpayCode, message)
        {
        }

        public BitPayLightCommunicationException(string apiCode, string message) : base(apiCode, message, true)
        {
        }

        public BitPayLightCommunicationException(string bitpayCode, string message, Exception cause, string apiCode = "000000") : base(bitpayCode,
            message, cause, apiCode)
        {
        }

        public String GetApiCode()
        {
            return ApiCode;
        }
    }
}
