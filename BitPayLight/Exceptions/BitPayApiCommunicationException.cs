using System;

namespace BitPayLight.Exceptions
{
    public class BitPayLightCommunicationException : BitPayException
    {
        private const string BitPayMessage = "Error when communicating with the BitPay API";
        private static readonly string _bitpayCode = "BITPAY-API";

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

        public BitPayLightCommunicationException(string bitpayCode, string message) : base(bitpayCode, message)
        {
        }

        public BitPayLightCommunicationException(string bitpayCode, string message, Exception cause) : base(bitpayCode,
            message, cause)
        {
        }
    }
}