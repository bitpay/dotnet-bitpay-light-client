using System.Collections.Generic;
using System.Linq;
using Xunit;
using BitPayLight;
using System.Threading.Tasks;
using BitPayLight.Models;
using BitPayLight.Models.Bill;
using BitPayLight.Models.Invoice;
using InvoiceStatus = BitPayLight.Models.Invoice.Status;
using BillStatus = BitPayLight.Models.Bill.Status;

namespace BitPay_xUnit_Test
{
    public class Tests
    {
        private BitPay _bitpay;
        
        private static readonly string environment = Env.Test;
        private static readonly string token = "Hn2WCJfDcVKT5kzS8YST33WghmUV3n7WDRKh5mhk4bW6";
        
        public Tests()
        {
            
            // Initialize the BitPay object to be used in the following tests
            _bitpay = new BitPay(token, environment);
        }
        
        [Fact]
        public async Task TestShouldGetInvoiceId() 
        {
            // create an invoice and make sure we receive an id - which means it has been successfully submitted
            var invoice = new Invoice(30.0, Currency.USD);
            var basicInvoice = await _bitpay.CreateInvoice(invoice);
            Assert.NotNull(basicInvoice.Id);
        }

        [Fact]
        public async Task TestShouldGetInvoiceUrl() {
            // create an invoice and make sure we receive an invoice url - which means we can check it online
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, Currency.USD));
            Assert.NotNull(basicInvoice.Url);
        }

        [Fact]
        public async Task TestShouldGetInvoiceStatus() {
            // create an invoice and make sure we receive a correct invoice status (new)
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, Currency.USD));
            Assert.Equal(InvoiceStatus.New, basicInvoice.Status);
        }

        [Fact]
        public async Task TestShouldGetInvoiceBtcPrice() {
            // create an invoice and make sure we receive values for the Bitcoin Cash and Bitcoin fields, respectively
            var basicInvoice = await _bitpay.CreateInvoice(new Invoice(10.0, Currency.USD));
            Assert.NotNull(basicInvoice.PaymentSubtotals.Btc);
            Assert.NotNull(basicInvoice.PaymentSubtotals.Bch);
        }

        [Fact]
        public async Task TestShouldCreateInvoiceOneTenthBtc() {
            // create an invoice and make sure we receive the correct price value back (under 1 BTC)
            var invoice = await _bitpay.CreateInvoice(new Invoice(0.1, Currency.BTC));
            Assert.Equal(0.1, invoice.Price);
        }

        [Fact]
        public async Task TestShouldCreateInvoice100Usd() {
            // create an invoice and make sure we receive the correct price value back (USD)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.USD));
            Assert.Equal(100.0, invoice.Price);
        }

        [Fact]
        public async Task TestShouldCreateInvoice100Eur() {
            // create an invoice and make sure we receive the correct price value back (EUR)
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.EUR));
            Assert.Equal(100.0, invoice.Price);
        }

        [Fact]
        public async Task TestShouldGetInvoice() {
            // create an invoice then retrieve it through the get method - they should match
            var invoice = await _bitpay.CreateInvoice(new Invoice(100.0, Currency.EUR));
            var retrievedInvoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.Equal(invoice.Id, retrievedInvoice.Id);
        }

        [Fact]
        public async Task TestShouldCreateInvoiceWithAdditionalParams() {
            // create an invoice and make sure we receive the correct fields values back
            var buyerData = new Buyer();
            buyerData.Name = "Satoshi";
            buyerData.Address1 = "street";
            buyerData.Address2 = "911";
            buyerData.Locality = "Washington";
            buyerData.Region = "District of Columbia";
            buyerData.PostalCode = "20000";
            buyerData.Country = "USA";
//            buyerData.Email = "";
//            buyerData.Phone = "";
            buyerData.Notify = true;
            
            var invoice = new Invoice(100.0, Currency.USD)
            {
                Buyer = buyerData,
                PosData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890",
                PaymentCurrencies = new List<string> {
                    Currency.BTC,
                    Currency.BCH
                },
                AcceptanceWindow = 480000,
                FullNotifications = true,
//                NotificationEmail = "",
                NotificationUrl = "https://hookb.in/03EBRQJrzasGmGkNPNw9",
                OrderId = "1234",
                Physical = true,
//                RedirectUrl = "",
                TransactionSpeed = "high",
                ItemCode = "bitcoindonation",
                ItemDesc = "dhdhdfgh"
            };
            invoice = await _bitpay.CreateInvoice(invoice);
            Assert.Equal(InvoiceStatus.New, invoice.Status);
            Assert.Equal("Satoshi", invoice.Buyer.Name);
            Assert.Equal("street", invoice.Buyer.Address1);
            Assert.Equal("911", invoice.Buyer.Address2);
            Assert.Equal("Washington", invoice.Buyer.Locality);
            Assert.Equal("District of Columbia", invoice.Buyer.Region);
            Assert.Equal("20000", invoice.Buyer.PostalCode);
            Assert.Equal("USA", invoice.Buyer.Country);
            Assert.True(invoice.Buyer.Notify);
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", invoice.PosData);
            Assert.Equal("1234", invoice.OrderId);
        }

        [Fact]
        public async Task TestShouldGetExchangeRates() {
            // get the exchange rates
            var rates = await _bitpay.GetRates();
            Assert.NotNull(rates.GetRates());
        }

        [Fact]
        public async Task TestShouldGetUsdExchangeRate() {
            // get the exchange rates and check the USD value
            var rates = await _bitpay.GetRates();
            Assert.True(rates.GetRate(Currency.USD) != 0, "Exchange rate not retrieved: USD");
        }

        [Fact]
        public async Task TestShouldGetEurExchangeRate() {
            // get the exchange rates and check the EUR value
            var rates = await _bitpay.GetRates();
            Assert.True(rates.GetRate(Currency.EUR) != 0, "Exchange rate not retrieved: EUR");
        }

        [Fact]
        public async Task TestShouldGetCnyExchangeRate() {
            // get the exchange rates and check the CNY value
            var rates = await _bitpay.GetRates();
            Assert.True(rates.GetRate(Currency.CNY) != 0, "Exchange rate not retrieved: CNY");
        }

        [Fact]
        public async Task TestShouldUpdateExchangeRates() {
            // check the exchange rates after update
            var rates = await _bitpay.GetRates();
            await rates.Update();
            Assert.NotNull(rates.GetRates());
        }

        [Fact]
        public async Task TestShouldGetInvoiceIdOne() {
            // create an invoice and get it by its id
            var invoice = await _bitpay.CreateInvoice(new Invoice(1.0, Currency.USD));
            invoice = await _bitpay.GetInvoice(invoice.Id);
            Assert.NotNull(invoice.Id);
        }
        
        [Fact]
        public async Task TestShouldCreateBillUSD() 
        {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});
            
            // create a bill and make sure we receive an id - which means it has been successfully submitted
            var bill = new Bill()
            {
                Number = "1", 
                Currency = Currency.USD, 
                //Email = "", 
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.NotNull(basicBill.Id);
        }

        [Fact]
        public async Task TestShouldCreateBillEur() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill and make sure we receive an id - which means it has been successfully submitted
            var bill = new Bill()
            {
                Number = "2", 
                Currency = Currency.EUR, 
                //Email = "", 
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.NotNull(basicBill.Id);
        }

        [Fact]
        public async Task TestShouldGetBillUrl() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill and make sure we receive a bill url - which means we can check it online
            var bill = new Bill()
            {
                Number = "3", 
                Currency = Currency.USD, 
                //Email = "", 
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.NotNull(basicBill.Url);
        }

        [Fact]
        public async Task TestShouldGetBillStatus() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill and make sure we receive a correct bill status (draft)
            var bill = new Bill()
            {
                Number = "4", 
                Currency = Currency.USD, 
                //Email = "", 
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.Equal(BillStatus.Draft, basicBill.Status);
        }

        [Fact]
        public async Task TestShouldGetBillTotals() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill and make sure we receive the same items sum as it was sent
            var bill = new Bill()
            {
                Number = "5", 
                Currency = Currency.USD, 
                //Email = "", 
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            Assert.Equal(basicBill.Items.Select(i => i.Price).Sum(), items.Select(i => i.Price).Sum());
        }

        [Fact]
        public async Task TestShouldGetBill() {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill then retrieve it through the get method - they should match
            var bill = new Bill()
            {
                Number = "6", 
                Currency = Currency.USD, 
                //Email = "", 
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            var retrievedBill = await _bitpay.GetBill(basicBill.Id);
            Assert.Equal(bill.Id, retrievedBill.Id);
        }

        [Fact]
        public async Task TestShouldDeliverBill()
        {
            List<Item> items = new List<Item>();
            items.Add(new Item(){Price = 30.0, Quantity = 9, Description = "product-a"});
            items.Add(new Item(){Price = 14.0, Quantity = 16, Description = "product-b"});
            items.Add(new Item(){Price = 3.90, Quantity = 42, Description = "product-c"});
            items.Add(new Item(){Price = 6.99, Quantity = 12, Description = "product-d"});

            // create a bill then retrieve it through the get method - they should match
            var bill = new Bill()
            {
                Number = "7", 
                Currency = Currency.USD, 
                //Email = "", 
                Items = items
            };
            var basicBill = await _bitpay.CreateBill(bill);
            var result = await _bitpay.DeliverBill(basicBill.Id, basicBill.Token);
            // Retrieve the updated bill for status confirmation
            var retrievedBill = await _bitpay.GetBill(basicBill.Id);
            // Check the correct response
            Assert.Equal("Success", result);
            // Confirm that the bill is sent
            Assert.Equal(BillStatus.Sent, retrievedBill.Status);
        }
    }
}