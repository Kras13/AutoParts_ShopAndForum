namespace OrdersGenerator;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var products = new Dictionary<int, double>
        {
            { 1, 89.99 },
            { 2, 78.99 },
            { 3, 85.55 },
            { 1002, 58.00 },
            { 1003, 13.69 },
            { 1004, 24.91 },
            { 1005, 14.99 },
            { 1006, 57.98 },
            { 1007, 16.49 },
            { 1008, 13.32 }
        };

        var productIds = new List<int>(products.Keys);

        var userIds = new List<string>
        {
            "029cd2b6-48f8-4431-a3f1-86ca31213b9f",
            "2a1b1628-0bb9-45e6-ab2e-dd980a51359d",
            "2f0fd81e-c573-46ab-83dd-21b7160576fd",
            "5b6f6527-7101-42da-949e-59b8ac5f126d",
            "e9cef60c-bf38-4672-b385-9db2c2727313",
            "ec19d706-1659-4c4a-a1b1-58dd53c849ec",
        };

        var numOrders = 500;
        var baseOrderId = 3500;
        var orderProductId = 3986;

        var orderData = new List<string>();
        var orderProductsData = new List<string>();
        var rnd = new Random();

        for (int i = 0; i < numOrders; i++)
        {
            var orderId = baseOrderId + i;
            var userId = userIds[rnd.Next(userIds.Count)];
            var dateCreated = DateTime.Now
                .AddDays(-rnd.Next(366, 732))
                .AddHours(-rnd.Next(0, 24))
                .AddMinutes(-rnd.Next(0, 60));

            var townId = rnd.Next(1, 201);
            var payWay = 0;
            var deliveryMethod = 0;
            var deliveryStreet = "Demo";
            var invoiceAddress = "Demo";
            var invoiceFirstName = "Demo";
            var invoiceLastName = "Demo";
            var isDelivered = 0;
            var dateDelivered = "NULL";
            var onlinePaymentStatus = "NULL";
            var publicToken = Guid.NewGuid().ToString();
            var courierStationId = "NULL";

            var numProducts = rnd.Next(1, 4);
            var selectedProducts = new HashSet<int>();
            
            while (selectedProducts.Count < numProducts)
            {
                selectedProducts.Add(productIds[rnd.Next(productIds.Count)]);
            }

            var overallSum = 0.0;
            
            foreach (var pid in selectedProducts)
            {
                var quantity = rnd.Next(1, 4);
                var price = products[pid];
                overallSum += quantity * price;

                orderProductsData.Add($"INSERT INTO OrdersProducts (Id, OrderId, ProductId, Quantity, SinglePrice) " +
                                      $"VALUES ({orderProductId}, {orderId}, {pid}, {quantity}, {price.ToString("F2", CultureInfo.InvariantCulture)});");
                orderProductId++;
            }

            var dateCreatedFormatted = dateCreated.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            orderData.Add(
                $"INSERT INTO Orders (Id, OverallSum, DateCreated, DateDelivered, IsDelivered, UserId, TownId, " +
                $"InvoicePersonFirstName, InvoicePersonLastName, CourierStationId, DeliveryStreet, DeliveryMethod, " +
                $"InvoiceAddress, PayWay, OnlinePaymentStatus, PublicToken) VALUES " +
                $"({orderId}, {overallSum.ToString("F2", CultureInfo.InvariantCulture)}, '{dateCreatedFormatted}', " +
                $"{dateDelivered}, {isDelivered}, '{userId}', {townId}, '{invoiceFirstName}', '{invoiceLastName}', " +
                $"{courierStationId}, '{deliveryStreet}', {deliveryMethod}, '{invoiceAddress}', {payWay}, {onlinePaymentStatus}, '{publicToken}');");
        }

        var sqlScript = new StringBuilder();
        
        foreach (var order in orderData)
            sqlScript.AppendLine(order);
        
        foreach (var op in orderProductsData)
            sqlScript.AppendLine(op);

        //Console.WriteLine(sqlScript.ToString());
        
        File.WriteAllText("../../../../orders.sql", sqlScript.ToString());
    }
}