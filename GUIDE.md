## Using the BitPay .NET light client

This SDK provides a convenient abstraction of BitPay's [cryptographically-secure API](https://bitpay.com/api) and allows payment gateway developers to focus on payment flow/e-commerce integration rather than on the specific details of client-server interaction using the API.  This SDK optionally provides the flexibility for developers to have control over important details, including the handling of private tokens needed for client-server communication.

### Dependencies

You must have a BitPay merchant account to use this SDK.  It's free to [sign-up for a BitPay merchant account](https://bitpay.com/start).

This library implements .NET Standard which is supported on MS Frameworks 4.6.1 and higher and .NET Core 2 and higher.

### Getting your client token

First of all, you need to generate a new POS token on your BitPay's account which will be required to securely connect to the BitPay's API.  
For testing purposes use:  
https://test.bitpay.com/dashboard/merchant/api-tokens

For production use:  
https://bitpay.com/dashboard/merchant/api-tokens

Click on 'Add New Token', give a name on the Token Label input, leave the 'Require Authentication' checkbox unchecked and click on 'Add Token'.
The new token will appear and ready to use.


### Initializing your BitPay client

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

### Create an invoice

```c#
Invoice invoice = bitpay.createInvoice(new Invoice(100.0, "USD")).Result;

String invoiceUrl = invoice.Url;

String status = invoice.Status;
```

### Create an invoice (extended)

You can add optional attributes to the invoice.  Atributes that are not set are ignored or given default values.
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

### Retrieve an invoice

```c#
Invoice invoice = bitpay.getInvoice(invoice.Id).Result;
```

### Get exchange rates

You can retrieve BitPay's [BBB exchange rates](https://bitpay.com/exchange-rates).

```c#
Rates rates = bitpay.getRates().Result;

double rate = rates.getRate("USD");

rates.update();
```

### Create a bill

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

### Retrieve a bill

```c#
Bill bill = bitpay.getBill(bill.Id).Result;
```

### Deliver a bill

```c#
String deliveryResult = bitpay.deliverBill(bill.Id, bill.Token).Result;
```


See also the tests project for more examples of API calls.
