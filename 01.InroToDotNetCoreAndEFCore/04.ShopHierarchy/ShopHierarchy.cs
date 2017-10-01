namespace _04.ShopHierarchy
{
    using System;
    using System.Linq;

    public class ShopHierarchy
    {
        public static void Main()
        {
            using (var db = new ShopHierarchyDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                SaveSalesmen(db);
                SaveItems(db);
                ProcessCommands(db);
                PrintNumberOfBigOrders(db);
            }
        }

        private static void SaveSalesmen(ShopHierarchyDbContext db)
        {
            string[] salesmen = Console.ReadLine().Split(';');
            foreach (var salesman in salesmen)
            {
                db.Salesmen.Add(new Salesman
                {
                    Name = salesman
                });
            }

            db.SaveChanges();
        }

        private static void SaveItems(ShopHierarchyDbContext db)
        {
            string line = Console.ReadLine();
            while (line.ToLower() != "end")
            {
                string[] args = line.Split(';');
                string name = args[0];
                decimal price = decimal.Parse(args[1]);
                db.Items.Add(new Item
                {
                    Name = name,
                    Price = price
                });

                line = Console.ReadLine();
            }

            db.SaveChanges();
        }

        private static void SaveCustomer(ShopHierarchyDbContext db, string args)
        {
            string[] customerArgs = args.Split(';');
            string name = customerArgs[0];
            int salesmanId = int.Parse(customerArgs[1]);
            db.Customers.Add(new Customer()
            {
                Name = name,
                SalesmanId = salesmanId
            });

            db.SaveChanges();
        }

        private static void SaveOrder(ShopHierarchyDbContext db, string args)
        {
            string[] orderArgs = args.Split(';');
            int customerId = int.Parse(orderArgs[0]);
            Order order = new Order()
            {
                CustomerId = customerId
            };

            db.Orders.Add(order);
            db.SaveChanges();

            for (int i = 1; i < orderArgs.Length; i++)
            {
                int itemId = int.Parse(orderArgs[i]);
                order.ItemOrders.Add(new ItemOrder()
                {
                    ItemId = itemId,
                    OrderId = order.Id
                });
            }

            db.SaveChanges();
        }

        private static void SaveReview(ShopHierarchyDbContext db, string args)
        {
            string[] reviewArgs = args.Split(';');
            int customerId = int.Parse(reviewArgs[0]);
            int itemId = int.Parse(reviewArgs[1]);
            db.Reviews.Add(new Review()
            {
                CustomerId = customerId,
                ItemId = itemId
            });

            db.SaveChanges();
        }

        private static void ProcessCommands(ShopHierarchyDbContext db)
        {
            string line = Console.ReadLine();
            while (line.ToLower() != "end")
            {
                string[] args = line.Split('-');
                string command = args[0];
                string commandArgs = args[1];

                switch (command)
                {
                    case "register":
                        SaveCustomer(db, commandArgs);
                        break;
                    case "order":
                        SaveOrder(db, commandArgs);
                        break;
                    case "review":
                        SaveReview(db, commandArgs);
                        break;
                }

                line = Console.ReadLine();
            }
        }

        private static void PrintCustomerCountPerSalesman(ShopHierarchyDbContext db)
        {
            var customersPerSalesman = db.Salesmen
                .Select(s => new
                {
                    s.Name,
                    Count = s.Customers.Count
                })
                .OrderByDescending(s => s.Count)
                .ThenBy(s => s.Name);

            foreach (var salesman in customersPerSalesman)
            {
                Console.WriteLine($"{salesman.Name} - {salesman.Count} customers");
            }
        }

        private static void PrintCustomers(ShopHierarchyDbContext db)
        {
            var customers = db.Customers
                .Select(c => new
                {
                    c.Name,
                    OrderCount = c.Orders.Count,
                    ReviewCount = c.Reviews.Count
                })
                .OrderByDescending(c => c.OrderCount)
                .ThenByDescending(c => c.ReviewCount);

            foreach (var customer in customers)
            {
                Console.WriteLine(customer.Name);
                Console.WriteLine($"Orders: {customer.OrderCount}");
                Console.WriteLine($"Reviews: {customer.ReviewCount}");
            }
        }

        private static void PrintCustomerOrdersAndReviews(ShopHierarchyDbContext db)
        {
            int customerId = int.Parse(Console.ReadLine());
            var orders = db.Orders
                .Where(o => o.CustomerId == customerId)
                .Select(o => new
                {
                    o.Id,
                    ItemCount = o.ItemOrders.Count
                })
                .OrderBy(o => o.Id);

            int reviewCount = db.Customers
                .FirstOrDefault(c => c.Id == customerId)
                .Reviews
                .Count;

            foreach (var order in orders)
            {
                Console.WriteLine($"order {order.Id}: {order.ItemCount} items");
            }

            Console.WriteLine($"reviews: {reviewCount}");
        }

        private static void PrintCustomerData(ShopHierarchyDbContext db)
        {
            int customerId = int.Parse(Console.ReadLine());
            var customer = db.Customers
                .FirstOrDefault(c => c.Id == customerId);

            Console.WriteLine($"Customer: {customer.Name}");
            Console.WriteLine($"Orders count: {customer.Orders.Count}");
            Console.WriteLine($"Reviews count: {customer.Reviews.Count}");
            Console.WriteLine($"Salesman: {customer.Salesman.Name}");
        }

        private static void PrintNumberOfBigOrders(ShopHierarchyDbContext db)
        {
            int customerId = int.Parse(Console.ReadLine());
            var ordersCount = db.Orders
                .Where(o => o.CustomerId == customerId)
                .Where(o => o.ItemOrders.Count > 1)
                .Count();

            Console.WriteLine($"Orders: {ordersCount}");
        }
    }
}