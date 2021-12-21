# BitPay .NET light client

This SDK provides a convenient abstraction of BitPay's [cryptographically-secure API](https://bitpay.com/api) and allows payment gateway developers to focus on payment flow/e-commerce integration rather than on the specific details of client-server interaction using the API.  This SDK optionally provides the flexibility for developers to have control over important details, including the handling of private tokens needed for client-server communication.
- [Dependencies](GUIDE.md#dependencies)
- [Usage](GUIDE.md#usage)
  - [Getting your client token](GUIDE.md#getting-your-client-token)
  - [Getting Started](GUIDE.md#getting-started)
    - [Initializing your BitPay light client](GUIDE.md#initializing-your-bitPay-light-client)
    - [Create an invoice](GUIDE.md#create-an-invoice)
    - [Create an invoice (extended)](GUIDE.md#Create-an-invoice-(extended))
    - [Retrieve an invoice](GUIDE.md#Retrieve-an-invoice)
    - [Get exchange rates](GUIDE.md#Get-exchange-rates)
    - [Create a bill](GUIDE.md#Create-a-bill)
    - [Retrieve a bill](GUIDE.md#Retrieve-a-bill)
    - [Deliver a bill](GUIDE.md#Deliver-a-bill)
  - [Errors](GUIDE.md#Errors)
- [Copyright](GUIDE.md#copyright)

## Dependenciesa

You must have a BitPay merchant account to use this SDK.  It's free to [sign-up for a BitPay merchant account](https://bitpay.com/start).

This library implements .NET Standard which is supported on MS Frameworks 4.6.1 and higher and .NET Core 2 and higher.

## Usage
### Getting your client token

First of all, you need to generate a new POS token on your BitPay's account which will be required to securely connect to the BitPay's API.  
For testing purposes use:  
https://test.bitpay.com/dashboard/merchant/api-tokens

For production use:  
https://bitpay.com/dashboard/merchant/api-tokens

Click on 'Add New Token', give a name on the Token Label input, leave the 'Require Authentication' checkbox unchecked and click on 'Add Token'.
The new token will appear and ready to use.

### Getting Started
#### Initializing your BitPay light client

Once you have the token, you can initialize the client for the desired environment:

```c#
// Testing
using BitPayLight;

BitPay bitpay = new BitPay(token: "H78Yiu78uh78Gjht6g67gjh6767ghj", environment: Env.Test);
```

```c#
// Production [The environment is selected by default]
using BitPayLight;

BitPay bitpay = new BitPay(token: "uh78Gjht6g67gjH78Yiu78h6767ghj");
```

#### Create an invoice

`POST /invoices` 

Facade **`POS`**

Creating Invoices are time-sensitive payment requests addressed to specific buyers. An invoice has a fixed price, typically denominated in fiat currency. It also has an equivalent price in the supported cryptocurrencies, calculated by BitPay, at a locked exchange rate with an expiration time of 15 minutes. 

##### HTTP Request

**Headers**
| Fields | Description | Presence
| ------ | ------ | ------ |
|  X-Accept-Version  | must be set to `2.0.0` for requests to the BitPay API  | **Mandatory** |
| Content-Type | must be set to `application/json` for requests to the BitPay API | **Mandatory** | 

**Body**
| Name | Description |Type | Presence
| ------ | ------ | ----- |------ |
|  token  | The API token can be retrieved from the dashboard (limited to pos facade).| `string` | **Mandatory** |
| price | Fixed price amount for the checkout, in the "currency" of the invoice object. | `number` | **Mandatory** |
| currency | ISO 4217 3-character currency code. This is the currency associated with the price field, supported currencies are available via the Currencies resource.| `string` | **Mandatory** |
| orderId | Can be used by the merchant to assign their own internal Id to an invoice. If used, there should be a direct match between an orderId and an invoice id. | `string` | Optional |
| itemDesc | Invoice description - will be added as a line item on the BitPay checkout page, under the merchant name. | `string` | Optional |
| itemCode | "bitcoindonation" for donations, otherwise do not include the field in the request. | `string` | Optional |
| notificationEmail | Merchant email address for notification of invoice status change. It is also possible to configure this email via the account setting on the BitPay dashboard or disable the email notification| `string` | Optional |
| notificationURL | URL to which BitPay sends webhook notifications. HTTPS is mandatory. | `string` | Optional |
| redirectURL | The shopper will be redirected to this URL when clicking on the Return button after a successful payment or when clicking on the Close button if a separate closeURL is not specified. Be sure to include "http://" or "https://" in the url. | `string` | Optional |
| closeURL | URL to redirect if the shopper does not pay the invoice and click on the Close button instead. Be sure to include "http://" or "https://" in the url. | `string` | Optional |
| autoRedirect | Set to false by default, merchant can setup automatic redirect to their website by setting this parameter to true. This will applied to the following scenarios:When the invoice is paid, it automatically redirects the shopper to the redirectURL indicated When the invoice expires, it automatically redirects the shopper to the closeURL if specified and to the redirectURL otherwiseNote: If automatic redirect is enabled, redirectURL becomes a mandatory invoice parameters.| `boolean` | Optional |
| posData | A passthru variable provided by the merchant during invoice creation and designed to be used by the merchant to correlate the invoice with an order or other object in their system. This passthru variable can be a serialized object, e.g.: "posData": "\"{ \"ref\" : 711454, \"item\" : \"test_item\" }\"" | `string` | Optional |
| transactionSpeed | This is a risk mitigation parameter for the merchant to configure how they want to fulfill orders depending on the number of block confirmations for the transaction made by the consumer on the selected cryptocurrency. | `string` | Optional |
| fullNotifications | This parameter is set to true by default, meaning all standard notifications are being sent for a payment made to an invoice. If you decide to set it to false instead, only 1 webhook will be sent for each invoice paid by the consumer. This webhook will be for the "confirmed" or "complete" invoice status, depending on the transactionSpeed selected. | `boolean` | Optional |
| extendedNotifications | Allows merchants to get access to additional webhooks. For instance when an invoice expires without receiving a payment or when it is refunded. If set to true, then fullNotifications is automatically set to true. When using the extendedNotifications parameter, the webhook also have a payload slightly different from the standard webhooks. | `boolean` | Optional |
| physical | Indicates whether items are physical goods. Alternatives include digital goods and services. | `boolean` | Optional |
| buyer | Allows merchant to pass buyer related information in the invoice object | ` object ` | Optional |
| paymentCurrencies | Allow the merchant to select the cryptocurrencies available as payment option on the BitPay invoice. Possible values are currently "BTC", "BCH", "ETH", "GUSD", "PAX", "BUSD", "USDC", "XRP", "DOGE", "DAI" and "WBTC". For instance "paymentCurrencies": ["BTC"] will create an invoice with only XRP available as transaction currency, thus bypassing the currency selection step on the invoice. | `array` | Optional |
| jsonPayProRequired | If set to true, this means that the invoice will only accept payments from wallets which have implemented the BitPay JSON Payment Protocol | `boolean` | Optional |
```c#

// Setting mandatory parameters in invoice i.e price and currency.
Invoice invoice = new Invoice(100.0, "USD");

// Setting invoice optional parameters
invoice.OrderId = "98e572ea-910e-415d-b6de-65f5090680f6";
invoice.FullNotifications = true;
invoice.ExtendedNotifications = true;
invoice.TransactionSpeed = "medium";
invoice.NotificationURL = "https://hookbin.com/lJnJg9WW7MtG9GZlPVdj";
invoice.RedirectURL = "https://hookbin.com/lJnJg9WW7MtG9GZlPVdj";
invoice.PosData = "98e572ea35hj356xft8y8cgh56h5090680f6";
invoice.ItemDesc = "Ab tempora sed ut.";
invoice.NotificationEmail = "sandbox@bitpay.com";

// Creating invoice
var basicInvoice = bitpay.CreateInvoice(invoice).Result;

// // To get the generated invoice url and status
var invoiceUrl = invoice.Url;
var status = invoice.Status;
Console.WriteLine(invoiceUrl);
Console.WriteLine(status);
Console.Read();
```

#### Create an invoice (extended)

Facade **`POS`**

You can add optional attributes to the invoice.  Attributes that are not set are ignored or given default values.

##### HTTP Request

**Body**
| Name | Description | Type | Presence |
| --- | --- | :---: | :---: |
| buyer | Allows merchant to pass buyer related information in the invoice object | `object` | Optional |
| &rarr; name | Buyer's name | `string` | Optional |
| &rarr; address1 | Buyer's address | `string` | Optional |
| &rarr; address2 | Buyer's appartment or suite number | `string` | Optional |
| &rarr; locality | Buyer's city or locality | `string` | Optional |
| &rarr; region | Buyer's state or province | `string` | Optional |
| &rarr; postalCode | Buyer's Zip or Postal Code | `string` | Optional |
| &rarr; country | Buyer's Country code. Format ISO 3166-1 alpha-2 | `string` | Optional |
| &rarr; email | Buyer's email address. If provided during invoice creation, this will bypass the email prompt for the consumer when opening the invoice. | `string` | Optional |
| &rarr; phone | Buyer's phone number | `string` | Optional |
| &rarr; notify | Indicates whether a BitPay email confirmation should be sent to the buyer once he has paid the invoice | `boolean` | Optional |
```c#

var buyerData = new Buyer();
buyerData.Name = "Satoshi";
buyerData.Address1 = "street";
buyerData.Address2 = "911";
buyerData.Locality = "Washington";
buyerData.Region = "District of Columbia";
buyerData.PostalCode = "20000";
buyerData.Country = "USA";
buyerData.Notify = true;

Invoice invoice = new Invoice(100.0, Currency.USD)
{
    Buyer = buyerData,
    PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890",
    PaymentCurrencies = new List<string> {
        Currency.BTC,
        Currency.BCH
    }
};

invoice = bitpay.createInvoice(invoice).Result;
```

##### HTTP Response

```json
{
  "facade": "merchant/invoice",
  "data": {
     "url": "https://bitpay.com/invoice?id=G3viJEJgE8Jk2oekSdgT2A",
     "status": "new",
     "price": 10,
     "currency": "USD",
     "orderId": "20210511_fghij",
     "invoiceTime": 1620733980748,
     "expirationTime": 1620734880748,
     "currentTime": 1620733980807,
     "id": "G3viJEJgE8Jk2oekSdgT2A",
     "lowFeeDetected": false,
     "amountPaid": 0,
     "displayAmountPaid": "0",
     "exceptionStatus": false,
     "targetConfirmations": 6,
     "transactions": [],
     "transactionSpeed": "medium",
     "buyer": {
        "email": "john@doe.com"
     },
     "redirectURL": "https://merchantwebsite.com/shop/return",
     "refundAddresses": [],
     "refundAddressRequestPending": false,
     "buyerProvidedEmail": "john@doe.com",
     "buyerProvidedInfo": {
        "emailAddress": "john@doe.com"
     },
     "paymentSubtotals": {
        "BTC": 18000,
        "BCH": 739100,
        "ETH": 2505000000000000,
        "GUSD": 1000,
        "PAX": 10000000000000000000,
        "BUSD": 10000000000000000000,
        "USDC": 10000000,
        "XRP": 7015685,
        "DOGE": 1998865000,
        "DAI": 9990000000000000000,
        "WBTC": 18000
     },
     "paymentTotals": {
        "BTC": 29600,
        "BCH": 739100,
        "ETH": 2505000000000000,
        "GUSD": 1000,
        "PAX": 10000000000000000000,
        "BUSD": 10000000000000000000,
        "USDC": 10000000,
        "XRP": 7015685,
        "DOGE": 1998865000,
        "DAI": 9990000000000000000,
        "WBTC": 18000
     },
     "paymentDisplayTotals": {
        "BTC": "0.000296",
        "BCH": "0.007391",
        "ETH": "0.002505",
        "GUSD": "10.00",
        "PAX": "10.00",
        "BUSD": "10.00",
        "USDC": "10.00",
        "XRP": "7.015685",
        "DOGE": "19.988650",
        "DAI": "9.99",
        "WBTC": "0.000180"
     },
     "paymentDisplaySubTotals": {
        "BTC": "0.000180",
        "BCH": "0.007391",
        "ETH": "0.002505",
        "GUSD": "10.00",
        "PAX": "10.00",
        "BUSD": "10.00",
        "USDC": "10.00",
        "XRP": "7.015685",
        "DOGE": "19.988650",
        "DAI": "9.99",
        "WBTC": "0.000180"
     },
     "exchangeRates": {
        "BTC": {
          "USD": 55413.609335,
          "EUR": 45540.39841,
          "BCH": 40.84109737914668,
          "ETH": 13.870219975470258,
          "GUSD": 55413.609335,
          "PAX": 55413.609335,
          "BUSD": 55413.609335,
          "USDC": 55413.609335,
          "XRP": 38758.09372049268,
          "DOGE": 110606.00665668662,
          "DAI": 55359.96552840298,
          "WBTC": 0.9981333606461704
        },
        "BCH": {
          "USD": 1352.90925,
          "EUR": 1111.2150000000001,
          "BTC": 0.02440102556111244,
          "ETH": 0.33863791096704754,
          "GUSD": 1352.90925,
          "PAX": 1352.90925,
          "BUSD": 1352.90925,
          "USDC": 1352.90925,
          "XRP": 946.2690507998013,
          "DOGE": 2700.4176646706587,
          "DAI": 1351.599550036015,
          "WBTC": 0.024369173431532262
        },
        "ETH": {
          "USD": 3992.672665000001,
          "EUR": 3278.9696950000002,
          "BTC": 0.0720117094001833,
          "BCH": 2.9426910658087726,
          "GUSD": 3992.672665000001,
          "PAX": 3992.672665000001,
          "BUSD": 3992.672665000001,
          "USDC": 3992.672665000001,
          "XRP": 2792.6060619837313,
          "DOGE": 7969.406516966069,
          "DAI": 3988.807510522304,
          "WBTC": 0.07191770817497412
        },
        "GUSD": {
          "USD": 1,
          "EUR": 0.821674,
          "BTC": 0.000018035966241721267,
          "BCH": 0.0007370228698196506,
          "ETH": 0.0002503034929852446,
          "PAX": 1,
          "BUSD": 1,
          "USDC": 1,
          "XRP": 0.6994327600316144,
          "DOGE": 1.9960079840319362,
          "DAI": 0.9990319380520276,
          "WBTC": 0.000018012422807762058
        },
        "PAX": {
          "USD": 1,
          "EUR": 0.821674,
          "BTC": 0.000018035966241721267,
          "BCH": 0.0007370228698196506,
          "ETH": 0.0002503034929852446,
          "GUSD": 1,
          "BUSD": 1,
          "USDC": 1,
          "XRP": 0.6994327600316144,
          "DOGE": 1.9960079840319362,
          "DAI": 0.9990319380520276,
          "WBTC": 0.000018012422807762058
        },
        "BUSD": {
          "USD": 1,
          "EUR": 0.821674,
          "BTC": 0.000018035966241721267,
          "BCH": 0.0007370228698196506,
          "ETH": 0.0002503034929852446,
          "GUSD": 1,
          "PAX": 1,
          "USDC": 1,
          "XRP": 0.6994327600316144,
          "DOGE": 1.9960079840319362,
          "DAI": 0.9990319380520276,
          "WBTC": 0.000018012422807762058
        },
        "USDC": {
          "USD": 1,
          "EUR": 0.821674,
          "BTC": 0.000018035966241721267,
          "BCH": 0.0007370228698196506,
          "ETH": 0.0002503034929852446,
          "GUSD": 1,
          "PAX": 1,
          "BUSD": 1,
          "XRP": 0.6994327600316144,
          "DOGE": 1.9960079840319362,
          "DAI": 0.9990319380520276,
          "WBTC": 0.000018012422807762058
        },
        "XRP": {
          "USD": 1.4253776249999999,
          "EUR": 1.17088545,
          "BTC": 0.00002570806272620483,
          "BCH": 0.0010505359077542177,
          "ETH": 0.0003567769983605121,
          "GUSD": 1.4253776249999999,
          "PAX": 1.4253776249999999,
          "BUSD": 1.4253776249999999,
          "USDC": 1.4253776249999999,
          "DOGE": 2.845065119760479,
          "DAI": 1.423997771159746,
          "WBTC": 0.00002567450444222371
        },
        "DOGE": {
          "USD": 0.5002839,
          "EUR": 0.4110702732486,
          "BTC": 0.000009023103531676658,
          "BCH": 0.0003687206757025671,
          "ETH": 0.00012522280765428083,
          "GUSD": 0.5002839,
          "PAX": 0.5002839,
          "BUSD": 0.5002839,
          "USDC": 0.5002839,
          "XRP": 0.3499149489763802,
          "DAI": 0.49979959419322684,
          "WBTC": 0.000009011325130716152
        },
        "DAI": {
          "USD": 1.000968,
          "EUR": 0.822469380432,
          "BTC": 0.000018053425057043255,
          "BCH": 0.0007377363079576361,
          "ETH": 0.00025054578676645436,
          "GUSD": 1.000968,
          "PAX": 1.000968,
          "BUSD": 1.000968,
          "USDC": 1.000968,
          "XRP": 0.7001098109433249,
          "DOGE": 1.9979401197604791,
          "WBTC": 0.000018029858833039973
        },
        "WBTC": {
          "USD": 55466.58,
          "EUR": 45575.44665492,
          "BTC": 1.000393364423732,
          "BCH": 40.88013797068123,
          "ETH": 13.883478717945511,
          "GUSD": 55466.58,
          "PAX": 55466.58,
          "BUSD": 55466.58,
          "USDC": 55466.58,
          "XRP": 38795.14313891434,
          "DOGE": 110711.73652694612,
          "DAI": 55412.88491451783
        }
     },
     "minerFees": {
        "BTC": {
          "satoshisPerByte": 79.151,
          "totalFee": 11600
        },
        "BCH": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "ETH": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "GUSD": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "PAX": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "BUSD": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "USDC": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "XRP": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "DOGE": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "DAI": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "WBTC": {
          "satoshisPerByte": 0,
          "totalFee": 0
        }
     },
     "shopper": {},
     "jsonPayProRequired": false,
     "supportedTransactionCurrencies": {
        "BTC": {
          "enabled": true
        },
        "BCH": {
          "enabled": true
        },
        "ETH": {
          "enabled": true
        },
        "GUSD": {
          "enabled": true
        },
        "PAX": {
          "enabled": true
        },
        "BUSD": {
          "enabled": true
        },
        "USDC": {
          "enabled": true
        },
        "XRP": {
          "enabled": true
        },
        "DOGE": {
          "enabled": true
        },
        "DAI": {
          "enabled": true
        },
        "WBTC": {
          "enabled": true
        }
     },
     "paymentCodes": {
        "BTC": {
          "BIP72b": "bitcoin:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "BIP73": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "BCH": {
          "BIP72b": "bitcoincash:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "BIP73": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "ETH": {
          "EIP681": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "GUSD": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "PAX": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "BUSD": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "USDC": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "XRP": {
          "BIP72b": "ripple:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "BIP73": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "RIP681": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "DOGE": {
          "BIP72b": "dogecoin:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "BIP73": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "DAI": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "WBTC": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        }
     },
     "token": "Svbfdow1xXc6chtQB3GVKqyRVhyLMqn3nhMhyTtf9T9vTCDKrVfdoA7n94nZUECZf"
  }
}
```
**WARNING**: 
If you get the following error when initiating the client for first time:
"500 Internal Server Error` response: {"error":"Account not setup completely yet."}"
Please, go back to your BitPay account and complete the required steps.
More info [here](https://support.bitpay.com/hc/en-us/articles/203010446-How-do-I-apply-for-a-merchant-account-)

#### Retrieve an invoice
`GET /invoices/:invoiceid`  

Facade **`POS`**

##### HTTP Request

**URL Parameters**

| Parameter | Description |Type | Presence
| ------ | ------ | ----- |------ |
|  ?token=  | When fetching an invoice via the merchant or the pos facade, pass the API token as a URL parameter - the same token used to create the invoice in the first place. | `string` | **Mandatory** |

**Headers**

| Fields | Description | Presence
| ------ | ------ | ------ |
|  X-Accept-Version  | must be set to `2.0.0` for requests to the BitPay API  | **Mandatory** |
| Content-Type | must be set to `application/json` for requests to the BitPay API | **Mandatory** | 

To get the generated invoice details, pass the Invoice Id with URL parameter

```c#
Invoice invoice = bitpay.GetInvoice(invoice.Id).Result;
```
##### HTTP Response

```json
{
  "facade": "merchant/invoice",
  "data": {
     "url": "https://bitpay.com/invoice?id=G3viJEJgE8Jk2oekSdgT2A",
     "status": "confirmed",
     "price": 10,
     "currency": "USD",
     "orderId": "20210511_fghij",
     "invoiceTime": 1620733980748,
     "expirationTime": 1620734880748,
     "currentTime": 1620734253073,
     "id": "G3viJEJgE8Jk2oekSdgT2A",
     "lowFeeDetected": false,
     "amountPaid": 739100,
     "displayAmountPaid": "0.007391",
     "exceptionStatus": false,
     "targetConfirmations": 6,
     "transactions": [
        {
          "amount": 739100,
          "confirmations": 1,
          "receivedTime": "2021-05-11T11:54:32.978Z",
          "txid": "0638edcce08856726135e4f464e603706bc12313fd9a9cd046a177e4ffce98dd",
          "exRates": {
             "BCH": 1,
             "USD": 1355.2800000000002,
             "BTC": 0.02446472581430398,
             "EUR": 1112.66,
             "ETH": 0.33972366494876377,
             "GUSD": 1355.2800000000002,
             "PAX": 1355.2800000000002,
             "BUSD": 1355.2800000000002,
             "USDC": 1355.2800000000002,
             "XRP": 948.9227925474189,
             "DOGE": 2725.466119299674,
             "DAI": 1353.985589776174,
             "WBTC": 0.024436520994387978
          },
          "outputIndex": 0
        }
     ],
     "transactionSpeed": "medium",
     "buyer": {
        "email": "john@doe.com"
     },
     "redirectURL": "https://merchantwebsite.com/shop/return",
     "refundAddresses": [],
     "refundAddressRequestPending": false,
     "buyerProvidedEmail": "john@doe.com",
     "buyerProvidedInfo": {
        "selectedWallet": "bitpay",
        "selectedTransactionCurrency": "BCH",
        "emailAddress": "john@doe.com"
     },
     "paymentSubtotals": {
        "BTC": 18000,
        "BCH": 739100,
        "ETH": 2505000000000000,
        "GUSD": 1000,
        "PAX": 10000000000000000000,
        "BUSD": 10000000000000000000,
        "USDC": 10000000,
        "XRP": 7015685,
        "DOGE": 1998865000,
        "DAI": 9990000000000000000,
        "WBTC": 18000
     },
     "paymentTotals": {
        "BTC": 29600,
        "BCH": 739100,
        "ETH": 2505000000000000,
        "GUSD": 1000,
        "PAX": 10000000000000000000,
        "BUSD": 10000000000000000000,
        "USDC": 10000000,
        "XRP": 7015685,
        "DOGE": 1998865000,
        "DAI": 9990000000000000000,
        "WBTC": 18000
     },
     "paymentDisplayTotals": {
        "BTC": "0.000296",
        "BCH": "0.007391",
        "ETH": "0.002505",
        "GUSD": "10.00",
        "PAX": "10.00",
        "BUSD": "10.00",
        "USDC": "10.00",
        "XRP": "7.015685",
        "DOGE": "19.988650",
        "DAI": "9.99",
        "WBTC": "0.000180"
     },
     "paymentDisplaySubTotals": {
        "BTC": "0.000180",
        "BCH": "0.007391",
        "ETH": "0.002505",
        "GUSD": "10.00",
        "PAX": "10.00",
        "BUSD": "10.00",
        "USDC": "10.00",
        "XRP": "7.015685",
        "DOGE": "19.988650",
        "DAI": "9.99",
        "WBTC": "0.000180"
     },
     "exchangeRates": {
        "BTC": {
          "USD": 55413.609335,
          "EUR": 45540.39841,
          "BCH": 40.84109737914668,
          "ETH": 13.870219975470258,
          "GUSD": 55413.609335,
          "PAX": 55413.609335,
          "BUSD": 55413.609335,
          "USDC": 55413.609335,
          "XRP": 38758.09372049268,
          "DOGE": 110606.00665668662,
          "DAI": 55359.96552840298,
          "WBTC": 0.9981333606461704
        },
        "BCH": {
          "USD": 1352.90925,
          "EUR": 1111.2150000000001,
          "BTC": 0.02440102556111244,
          "ETH": 0.33863791096704754,
          "GUSD": 1352.90925,
          "PAX": 1352.90925,
          "BUSD": 1352.90925,
          "USDC": 1352.90925,
          "XRP": 946.2690507998013,
          "DOGE": 2700.4176646706587,
          "DAI": 1351.599550036015,
          "WBTC": 0.024369173431532262
        },
        "ETH": {
          "USD": 3992.672665000001,
          "EUR": 3278.9696950000002,
          "BTC": 0.0720117094001833,
          "BCH": 2.9426910658087726,
          "GUSD": 3992.672665000001,
          "PAX": 3992.672665000001,
          "BUSD": 3992.672665000001,
          "USDC": 3992.672665000001,
          "XRP": 2792.6060619837313,
          "DOGE": 7969.406516966069,
          "DAI": 3988.807510522304,
          "WBTC": 0.07191770817497412
        },
        "GUSD": {
          "USD": 1,
          "EUR": 0.821674,
          "BTC": 0.000018035966241721267,
          "BCH": 0.0007370228698196506,
          "ETH": 0.0002503034929852446,
          "PAX": 1,
          "BUSD": 1,
          "USDC": 1,
          "XRP": 0.6994327600316144,
          "DOGE": 1.9960079840319362,
          "DAI": 0.9990319380520276,
          "WBTC": 0.000018012422807762058
        },
        "PAX": {
          "USD": 1,
          "EUR": 0.821674,
          "BTC": 0.000018035966241721267,
          "BCH": 0.0007370228698196506,
          "ETH": 0.0002503034929852446,
          "GUSD": 1,
          "BUSD": 1,
          "USDC": 1,
          "XRP": 0.6994327600316144,
          "DOGE": 1.9960079840319362,
          "DAI": 0.9990319380520276,
          "WBTC": 0.000018012422807762058
        },
        "BUSD": {
          "USD": 1,
          "EUR": 0.821674,
          "BTC": 0.000018035966241721267,
          "BCH": 0.0007370228698196506,
          "ETH": 0.0002503034929852446,
          "GUSD": 1,
          "PAX": 1,
          "USDC": 1,
          "XRP": 0.6994327600316144,
          "DOGE": 1.9960079840319362,
          "DAI": 0.9990319380520276,
          "WBTC": 0.000018012422807762058
        },
        "USDC": {
          "USD": 1,
          "EUR": 0.821674,
          "BTC": 0.000018035966241721267,
          "BCH": 0.0007370228698196506,
          "ETH": 0.0002503034929852446,
          "GUSD": 1,
          "PAX": 1,
          "BUSD": 1,
          "XRP": 0.6994327600316144,
          "DOGE": 1.9960079840319362,
          "DAI": 0.9990319380520276,
          "WBTC": 0.000018012422807762058
        },
        "XRP": {
          "USD": 1.4253776249999999,
          "EUR": 1.17088545,
          "BTC": 0.00002570806272620483,
          "BCH": 0.0010505359077542177,
          "ETH": 0.0003567769983605121,
          "GUSD": 1.4253776249999999,
          "PAX": 1.4253776249999999,
          "BUSD": 1.4253776249999999,
          "USDC": 1.4253776249999999,
          "DOGE": 2.845065119760479,
          "DAI": 1.423997771159746,
          "WBTC": 0.00002567450444222371
        },
        "DOGE": {
          "USD": 0.5002839,
          "EUR": 0.4110702732486,
          "BTC": 0.000009023103531676658,
          "BCH": 0.0003687206757025671,
          "ETH": 0.00012522280765428083,
          "GUSD": 0.5002839,
          "PAX": 0.5002839,
          "BUSD": 0.5002839,
          "USDC": 0.5002839,
          "XRP": 0.3499149489763802,
          "DAI": 0.49979959419322684,
          "WBTC": 0.000009011325130716152
        },
        "DAI": {
          "USD": 1.000968,
          "EUR": 0.822469380432,
          "BTC": 0.000018053425057043255,
          "BCH": 0.0007377363079576361,
          "ETH": 0.00025054578676645436,
          "GUSD": 1.000968,
          "PAX": 1.000968,
          "BUSD": 1.000968,
          "USDC": 1.000968,
          "XRP": 0.7001098109433249,
          "DOGE": 1.9979401197604791,
          "WBTC": 0.000018029858833039973
        },
        "WBTC": {
          "USD": 55466.58,
          "EUR": 45575.44665492,
          "BTC": 1.000393364423732,
          "BCH": 40.88013797068123,
          "ETH": 13.883478717945511,
          "GUSD": 55466.58,
          "PAX": 55466.58,
          "BUSD": 55466.58,
          "USDC": 55466.58,
          "XRP": 38795.14313891434,
          "DOGE": 110711.73652694612,
          "DAI": 55412.88491451783
        }
     },
     "minerFees": {
        "BTC": {
          "satoshisPerByte": 79.151,
          "totalFee": 11600
        },
        "BCH": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "ETH": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "GUSD": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "PAX": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "BUSD": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "USDC": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "XRP": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "DOGE": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "DAI": {
          "satoshisPerByte": 0,
          "totalFee": 0
        },
        "WBTC": {
          "satoshisPerByte": 0,
          "totalFee": 0
        }
     },
     "shopper": {
        "user": "FeFc5a7hUwmrEnYwVPmrkp"
     },
     "jsonPayProRequired": false,
     "transactionCurrency": "BCH",
     "supportedTransactionCurrencies": {
        "BTC": {
          "enabled": true
        },
        "BCH": {
          "enabled": true
        },
        "ETH": {
          "enabled": true
        },
        "GUSD": {
          "enabled": true
        },
        "PAX": {
          "enabled": true
        },
        "BUSD": {
          "enabled": true
        },
        "USDC": {
          "enabled": true
        },
        "XRP": {
          "enabled": true
        },
        "DOGE": {
          "enabled": true
        },
        "DAI": {
          "enabled": true
        },
        "WBTC": {
          "enabled": true
        }
     },
     "paymentCodes": {
        "BTC": {
          "BIP72b": "bitcoin:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "BIP73": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "BCH": {
          "BIP72b": "bitcoincash:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "BIP73": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "ETH": {
          "EIP681": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "GUSD": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "PAX": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "BUSD": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "USDC": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "XRP": {
          "BIP72b": "ripple:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "BIP73": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "RIP681": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "DOGE": {
          "BIP72b": "dogecoin:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A",
          "BIP73": "https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "DAI": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        },
        "WBTC": {
          "EIP681b": "ethereum:?r=https://bitpay.com/i/G3viJEJgE8Jk2oekSdgT2A"
        }
     },
     "token": "Svbfdow1xXc6chtQB3GVKqyRVhyLMqn3nhMhyTtf9T9vTCDKrVfdoA7n94nZUECZf"
  }
}
```

#### Get exchange rates
Rates are exchange rates, representing the number of fiat currency units equivalent to one BTC.
You can retrieve BitPay's [BBB exchange rates](https://bitpay.com/exchange-rates).

`GET /rates/:basecurrency`

Facade **`PUBLIC`**

##### HTTP Request

**URL Parameters**
| Parameter | Description |Type | Presence
| ------ | ------ | ----- |------ |
|  baseCurrency  | the cryptocurrency for which you want to fetch the rates. Current supported values are BTC and BCH | `string` | **Mandatory** |

**Headers**

| Fields | Description | Presence
| ------ | ------ | ------ |
|  X-Accept-Version  | must be set to `2.0.0` for requests to the BitPay API  | **Mandatory** |
| Content-Type | must be set to `application/json` for requests to the BitPay API | **Mandatory** |
You can retrieve BitPay's [BBB exchange rates](https://bitpay.com/exchange-rates).

```c#
Rates rates = bitpay.GetRates().Result;

double rate = rates.GetRate("USD"); //Always use the included Currency model to avoid typos

rates.Update();
```

##### HTTP Response

**Body**

| Name | Description | Type |
| --- | --- | :---: |
| data | array of currency rates for the requested `baseCurrency`. | `array` |
| &rarr; code | ISO 4217 3-character currency code. | `string` |
| &rarr; name | detailed currency name. | `string` |
| &rarr; rate | rate for the requested `baseCurrency` /`currency` pair. | `number` |

```json
{
    "data":[
        {
        "code":"BTC",
        "name":"Bitcoin",
        "rate":1
        },
        {
        "code":"BCH",
        "name":"Bitcoin Cash",
        "rate":50.77
        },
        {
        "code":"USD",
        "name":"US Dollar",
        "rate":41248.11
        },
        {
        "code":"EUR",
        "name":"Eurozone Euro",
        "rate":33823.04
        },
        {
        "code":"GBP",
        "name":"Pound Sterling",
        "rate":29011.49
        },
        {
        "code":"JPY",
        "name":"Japanese Yen",
        "rate":4482741
        },
        {
        "code":"CAD",
        "name":"Canadian Dollar",
        "rate":49670.85
        },
        {
        "code":"AUD",
        "name":"Australian Dollar",
        "rate":53031.99
        },
        {
        "code":"CNY",
        "name":"Chinese Yuan",
        "rate":265266.57
        },
        ...
    ]
}
```

#### Create a bill
Bills are payment requests addressed to specific buyers. Bill line items have fixed prices, typically denominated in fiat currency.

`POST /bills`

Facade **`POS`**

##### HTTP Request

**Headers**
| Fields | Description | Presence
| ------ | ------ | ------ |
|  X-Accept-Version  | must be set to `2.0.0` for requests to the BitPay API  | **Mandatory** |
| Content-Type | must be set to `application/json` for requests to the BitPay API | **Mandatory** | 

**Body**
| Name | Description |Type | Presence
| ------ | ------ | ----- |------ |
|  number | Bill identifier, specified by merchant | `string` | Optional |
|  currency | ISO 4217 3-character currency code. This is the currency associated with the price field | `string` | **Mandatory** |
|  name | Bill recipient's name | `string` | Optional |
|  address1 | Bill recipient's address | `string` | Optional |
|  address2 | Bill recipient's address | `string` | Optional |
|  city | Bill recipient's city | `string` | Optional |
|  state | Bill recipient's state or province | `string` | Optional |
| zip | Bill recipient's ZIP code | `string` | Optional |
| country | Bill recipient's country | `string` | Optional |
| email | Bill recipient's email address | `string` | **Mandatory** |
| cc | Email addresses to which a copy of the bill must be sent | `array` | Optional |
| phone | Bill recipient's phone number | `string` | Optional |
| dueDate | Date and time at which a bill is due, ISO-8601 format yyyy-mm-ddThh:mm:ssZ. (UTC) | `string` | Optional |
| passProcessingFee | If set to true, BitPay's processing fee will be included in the amount charged on the invoice | `boolean` | Optional |
| items | List of line items | `array` | **Mandatory** |
| &rarr; description | Line item description | `string` | **Mandatory** |
| &rarr; price | Line item unit price for the corresponding `currency` | `number` | **Mandatory** |
| &rarr; quantity | Bill identifier, specified by merchant | `number` | **Mandatory** |
| token | The API token can be retrieved from the dashboard (limited to pos facade).| `string` | **Mandatory** |

```c#
// Create a list of items to add in the bill
List<Item> items = new List<Item>();
items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

Bill bill = new Bill()
{
    Number = "1",
    Currency = Currency.USD,
    Email = "example!@example!.com",
    Items = items
};

bill = bitpay.CreateBill(bill).Result;
```

##### HTTP Response

```json
{
    "facade": "pos/bill",
    "data": {
        "status": "draft",
        "url": "https://bitpay.com/bill?id=X6KJbe9RxAGWNReCwd1xRw&resource=bills",
        "number": "bill1234-ABCD",
        "createdDate": "2021-05-21T09:48:02.373Z",
        "dueDate": "2021-05-31T00:00:00.000Z",
        "currency": "USD",
        "email": "john@doe.com",
        "cc": [
        "jane@doe.com"
        ],
        "passProcessingFee": true,
        "id": "X6KJbe9RxAGWNReCwd1xRw",
        "items": [
        {
            "id": "EL4vx41Nxc5RYhbqDthjE",
            "description": "Test Item 1",
            "price": 6,
            "quantity": 1
        },
        {
            "id": "6spPADZ2h6MfADvnhfsuBt",
            "description": "Test Item 2",
            "price": 4,
            "quantity": 1
        }
        ],
        "token": "qVVgRARN6fKtNZ7Tcq6qpoPBBE3NxdrmdMD883RyMK4Pf8EHENKVxCXhRwyynWveo"
    }
}
```

#### Retrieve a bill

`GET /bills/:billid`

Facade **`POS`**

##### HTTP Request

**URL Parameters**
| Parameter | Description |Type | Presence
| ------ | ------ | ----- |------ |
|  ?token=  | when fetching settlememts, pass a token as a URL parameter . | `string` | **Mandatory** |

**Headers**
| Fields | Description | Presence
| ------ | ------ | ------ |
|  X-Accept-Version  | must be set to `2.0.0` for requests to the BitPay API  | **Mandatory** |
| Content-Type | must be set to `application/json` for requests to the BitPay API | **Mandatory** | 

```c#
Bill bill = bitpay.GetBill(bill.Id).Result;
```

##### HTTP Response

```json
{
    "facade": "pos/bill",
    "data": {
        "status": "draft",
        "url": "https://bitpay.com/bill?id=X6KJbe9RxAGWNReCwd1xRw&resource=bills",
        "number": "bill1234-ABCD",
        "createdDate": "2021-05-21T09:48:02.373Z",
        "dueDate": "2021-05-31T00:00:00.000Z",
        "currency": "USD",
        "email": "john@doe.com",
        "cc": [
        "jane@doe.com"
        ],
        "passProcessingFee": true,
        "id": "X6KJbe9RxAGWNReCwd1xRw",
        "items": [
        {
            "id": "EL4vx41Nxc5RYhbqDthjE",
            "description": "Test Item 1",
            "price": 6,
            "quantity": 1
        },
        {
            "id": "6spPADZ2h6MfADvnhfsuBt",
            "description": "Test Item 2",
            "price": 4,
            "quantity": 1
        }
        ],
        "token": "qVVgRARN6fKtNZ7Tcq6qpoPBBE3NxdrmdMD883RyMK4Pf8EHENKVxCXhRwyynWveo"
    }
}
```

#### Deliver a bill

`GET /bills/:billid/deliveries`

Facade **`POS`**

##### HTTP Request

**URL Parameters**
| Parameter | Description |Type | Presence
| ------ | ------ | ----- |------ |
|  billId  | the id of the bill you want to deliver via email . | `string` | **Mandatory** |

**Headers**
| Fields | Description | Presence
| ------ | ------ | ------ |
|  X-Accept-Version  | must be set to `2.0.0` for requests to the BitPay API  | **Mandatory** |
| Content-Type | must be set to `application/json` for requests to the BitPay API | **Mandatory** | 

**Body**
| Name | Description |Type | Presence
| ------ | ------ | ----- |------ |
|  token | The resource token for the billId you want to deliver via email. You need to retrieve this token from the bill object itself (see section Retrieve a bill). | `string` | **Mandatory** |
```c#
String deliveryResult = bitpay.DeliverBill(bill.Id, bill.Token).Result;
```

##### HTTP Response

**Body**

| Name | Description | Type |
| --- | --- | :---: |
| data | set to `"Success"` once a bill is successfully sent via email. | `string` |

```json
{
    "data": "Success"
}
```

### Errors

**Description and Methodology:**

An error codes framework was created for this project. An error code consists of 6 digits. The first two digits of an error code represent the HTTP
method that was used to call it. The next two digits refer to the resource that was impacted. The last two digits refer to the specific error.

**Error Response Format**
| Field | Description | Type
| ------ | ------ | ------ |
| status | will always be “error” for an error response | `string` |
| code | six digit error code | `string` |
| data | will be null for an error response | `string` |
| error | error message | `string` |

```c#
// example error response
{
"status": "error",
"code": "010207",
"data": null,
"error": "Invalid invoice state for refund"
}
```

**HTTP Method - First two digits**

| Code | Description | Type
| ------ | ------ | ------ |
| 00xxxx | generic, unmapped error | `string` |
| 01xxxx | POST error | `string` |
| 02xxxx | GET error | `string` |
| 03xxxx | PUT error | `string` |
| 04xxxx | DELETE error | `string` |


**Resource and Error - Last four digits**

Unmapped Errors xx00xx

These errors are not mapped to a specific resource

| Code | Error | Type
| ------ | ------ | ------ |
| xx0000 | Generic server error | `string` |
| xx0001 | Resource not found | `string` |
| xx0002 | Invalid params | `string` |
| xx0003 | Missing params | `string` |

**Invoice Errors: xx01xx**

These errors are mapped to the invoice resource

| Code | Error | Type
| ------ | ------ | ------ |
| xx0100 | Generic server error | `string` |
| xx0101 | Invoice not found | `string` |
| xx0102 | Invalid params | `string` |
| xx0103 | Missing params | `string` |
| xx0107 | Invalid invoice state for cancel | `string` |
| xx0108 | Invoice is missing email or sms | `string` |
| xx0109 | Sms is not verified | `string` |
| xx0110 | Invoice price is below minimum threshold | `string` |
| xx0111 | Invoice price is above maximum threshold | `string` |
| xx0112 | Invalid sms number | `string` |
| xx0113 | Error verifying sms | `string` |

## Copyright
Copyright (c) 2019 BitPay

See also the tests project for more examples of API calls.