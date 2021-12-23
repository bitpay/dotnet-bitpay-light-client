using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitPayLight.Exceptions;
using BitPayLight.Helpers;
using BitPayLight.Models.Invoice;
using BitPayLight.Models.Rate;
using BitPayLight.Models.Bill;
using Newtonsoft.Json;

/**
 * @author Antonio Buedo
 * @date 6.26.2019
 * @version 1.0.1907
 *
 * See https://bitpay.com/api for more information.
 */

namespace BitPayLight
{
    public class BitPay
    {
        private static string _env;
        private static string _token;

        private RestHelper _restHelper;
        
        /// <summary>
        ///     Constructor for use if the keys and SIN are managed by this library.
        /// </summary>
        /// <param name="token">The token generated on the BitPay account.</param>
        /// <param name="environment">The target environment [Default: Production].</param>
        public BitPay(string token, string environment = Env.Prod)
        {
            _token = token;
            _env = environment;
            Init();
        }
        
        /// <summary>
        ///     Initialize this object for the target environment
        /// </summary>
        /// <returns></returns>
        private void Init()
        {
            try
            {
                _restHelper = new RestHelper(_env);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BitPayException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Create an invoice.
        /// </summary>
        /// <param name="invoice">An invoice request object.</param>
        /// <returns>A new invoice object returned from the API.</returns>
        /// <throws>InvoiceCreationException InvoiceCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Invoice> CreateInvoice(Invoice invoice)
        {
            try
            {
                invoice.Token = _token;
                invoice.Guid = Guid.NewGuid().ToString();
                var json = JsonConvert.SerializeObject(invoice);
                var response = await _restHelper.Post("invoices", json).ConfigureAwait(false);
                var responseString = await _restHelper.ResponseToJsonString(response).ConfigureAwait(false);
                JsonConvert.PopulateObject(responseString, invoice);
            }
            catch (BitPayException ex)
            {
                throw new InvoiceCreationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceCreationException(ex);

                throw;
            }

            return invoice;
        }

        /// <summary>
        ///     Retrieve an invoice by id.
        /// </summary>
        /// <param name="invoiceId">The id of the requested invoice.</param>
        /// <returns>A new invoice object returned from the API.</returns>
        /// <throws>InvoiceQueryException InvoiceQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Invoice> GetInvoice(string invoiceId)
        {
            Dictionary<string, string> parameters = null;
            try
            {
                var response = await _restHelper.Get("invoices/" + invoiceId, parameters);
                var responseString = await _restHelper.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Invoice>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new InvoiceQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new InvoiceQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Retrieve the exchange rate table using the public facade.
        /// </summary>
        /// <returns>The rate table as an object retrieved from the server.</returns>
        /// <throws>RatesQueryException RatesQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Rates> GetRates()
        {
            try
            {
                var response = await _restHelper.Get("rates");
                var responseString = await _restHelper.ResponseToJsonString(response);
                var rates = JsonConvert.DeserializeObject<List<Rate>>(responseString);
                return new Rates(rates, this);
            }
            catch (BitPayException ex)
            {
                throw new RatesQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new RatesQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Create a bill.
        /// </summary>
        /// <param name="bill">An invoice request object.</param>
        /// <returns>A new bill object returned from the server.</returns>
        /// <throws>BillCreationException BillCreationException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Bill> CreateBill(Bill bill)
        {
            try
            {
                bill.Token = _token;
                var json = JsonConvert.SerializeObject(bill);
                var response = await _restHelper.Post("bills", json).ConfigureAwait(false);
                var responseString = await _restHelper.ResponseToJsonString(response).ConfigureAwait(false);
                var serializerSettings = new JsonSerializerSettings {ObjectCreationHandling = ObjectCreationHandling.Replace};
                JsonConvert.PopulateObject(responseString, bill, serializerSettings);
            }
            catch (BitPayException ex)
            {
                throw new BillCreationException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BillCreationException(ex);

                throw;
            }

            return bill;
        }

        /// <summary>
        ///     Retrieve a bill by id.
        /// </summary>
        /// <param name="billId">The id of the requested bill.</param>
        /// <returns>A new bill object returned from the API.</returns>
        /// <throws>BillQueryException BillQueryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<Bill> GetBill(string billId)
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>{{"token", _token}};
                var response = await _restHelper.Get("bills/" + billId, parameters);
                var responseString = await _restHelper.ResponseToJsonString(response);
                return JsonConvert.DeserializeObject<Bill>(responseString);
            }
            catch (BitPayException ex)
            {
                throw new BillQueryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BillQueryException(ex);

                throw;
            }
        }

        /// <summary>
        ///     Deliver a bill to the consumer.
        /// </summary>
        /// <param name="billId">The id of the requested bill.</param>
        /// <param name="billToken">The token of the requested bill.</param>
        /// <returns>A response status returned from the API.</returns>
        /// <throws>BillDeliveryException BillDeliveryException class</throws>
        /// <throws>BitPayException BitPayException class</throws>
        public async Task<string> DeliverBill(string billId, string billToken)
        {
            var responseString = "";
            try
            {
                var json = JsonConvert.SerializeObject(new Dictionary<string, string>{{"token", billToken}});
                var response = await _restHelper.Post("bills/" + billId + "/deliveries", json).ConfigureAwait(false);
                responseString = await _restHelper.ResponseToJsonString(response).ConfigureAwait(false);
            }
            catch (BitPayException ex)
            {
                throw new BillDeliveryException(ex, ex.GetApiCode());
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().IsSubclassOf(typeof(BitPayException)) || ex.GetType() == typeof(BitPayException)))
                    throw new BillDeliveryException(ex);

                throw;
            }

            return responseString;
        }
    }
}