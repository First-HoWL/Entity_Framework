using System.Text;
using System.Threading;
using System;
using System.Text.Json;
using System.Net;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game
{

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Group groupe { get; set; } = new Group { Id = 0, Name = "P00" };
        public double Avg { get; set; }
        public override string ToString()
        {
            //return $"{Id,5} | {Name,10} | {Math.Round(Avg, 2),6} | {groupe}";
            return $"{Id,5} | {Name,10} | {groupe}";
        }
    }
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Teacher Curator { get; set; } = new Teacher { Id = 0, Name = "" };

        /* public Group(int Id, string name) 
        {
            this.Id = Id;
            this.Name = name;
        }*/
        public override string ToString()
        {
            // return $"{Id,5} | {Name,10} | {Curator}";
            return $"{Name,5}";
        }
    }

    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public override string ToString()
        {
            return $"{Id,5} | {Name} | {SecondName}";
        }
    }

    public class ProductType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{Id,5} | {Name}";
        }
    }

    public class Product {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProductType ProductType { get; set; }
        public double Price {  get; set; }
        public int inStock { get; set; }
        public override string ToString()
        {
            return $"{Id,5} | {Name, 20} | {Math.Round(Price, 2),6}| {inStock, 6} | {ProductType}";
        }
    }

    public class Basket
    {
        public int Id { get; set; }
        public DateTime Date{ get; set; }
        public double TotalPrice { get; set; }
        public override string ToString()
        {
            return $"{Id,5} | {Date.ToString()} | {Math.Round(TotalPrice, 2), 7}";
        }
    }

    public class ProductsInBasket
    {
        public int Id { get; set; }
        public Product product { get; set; }
        public Basket Basket { get; set; }
    }






    public class UniversityContext : DbContext
    {
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<ProductsInBasket> ProductsInBaskets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "server=localhost;database=shop;user=root;password=";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }



    class ConsoleWindow
    {
        public Point from;
        public Point to;
        public Point CursorPosition = new Point(0, 0);
        public ConsoleColor ForegroundColor;
        public ConsoleColor BackgroundColor;
        List<string> text = new List<string>();
        public object lockObj;
        public object lockMessages;
        public bool Boards { get; set; }
        public ConsoleWindow(Point from, Point to, object lockObj, object lockMessages, ConsoleColor ForegroundColor = ConsoleColor.White, ConsoleColor BackgroundColor = ConsoleColor.Black)
        {
            this.from = from;
            this.to = to;
            this.ForegroundColor = ForegroundColor;
            this.BackgroundColor = BackgroundColor;
            this.lockObj = lockObj;
            this.lockMessages = lockMessages;
        }

        public void DrawBoards()
        {
            lock (lockObj)
            {
                for (int i = from.Y - 1; i < to.Y; i++)
                {
                    Console.SetCursorPosition(from.X - 1, i);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    Console.ResetColor();
                }
                for (int i = from.Y - 1; i < to.Y + 0; i++)
                {
                    Console.SetCursorPosition(to.X + 1, i);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    Console.ResetColor();
                }
                for (int j = from.X - 1; j < to.X + 1; j++)
                {
                    Console.SetCursorPosition(j, from.Y - 1);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    Console.ResetColor();
                }
                for (int j = from.X - 1; j < to.X + 2; j++)
                {
                    Console.SetCursorPosition(j, to.Y);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                    Console.ResetColor();
                }
            }
        }

        public void SetCursorPosition(Point a)
        {
            CursorPosition = a.X! > to.X - from.X && a.X! < 0 && a.Y! > to.Y - from.Y && a.Y! < 0 ? a : new Point(0, 0);
        }

        public void ClearText()
        {
            text.Clear();
        }


        public void Draw()
        {
            lock (lockObj)
                lock (lockMessages)
                {
                    int i = CursorPosition.X;
                    int j = CursorPosition.Y;
                    if (Boards == true)
                        DrawBoards();
                    foreach (var messages in text)
                    {
                        Console.BackgroundColor = BackgroundColor;
                        Console.ForegroundColor = ForegroundColor;
                        for (int h = 0; h < messages.Length; h++)
                        {
                            if (i == to.X - from.X)
                            {
                                i = 0;
                                j++;
                            }

                            if (messages[h] == '\n')
                            {
                                i = 0;
                                j++;
                            }
                            else
                            {
                                Console.SetCursorPosition(from.X + i++, from.Y + j);
                                Console.Write(messages[h]);
                            }
                        }
                        Console.ResetColor();
                    }
                }

        }

        public void DrawBackground()
        {
            lock (lockObj)
            {
                for (int i = from.Y; i < to.Y; i++)
                {
                    for (int j = from.X; j < to.X + 1; j++)
                    {
                        Console.SetCursorPosition(j, i);
                        Console.BackgroundColor = BackgroundColor;
                        Console.Write(" ");
                        Console.ResetColor();
                    }
                }
            }
        }

        public void Clear()
        {
            lock (lockObj)
                lock (lockMessages)
                {
                    text.Clear();
                    DrawBackground();
                }
        }

        public void WriteLine(string message)
        {
            lock (lockMessages)
            {
                if (text.Count() >= to.Y - from.Y - 1)
                {
                    text.Remove(text[0]);
                }
                while (message.Length > to.X - from.X)
                {
                    if (text.Count() >= to.Y - from.Y - 1)
                    {
                        text.Remove(text[0]);
                    }
                    string a = "";
                    for (int i = 0; i < to.X - from.X; i++)
                    {
                        a += message[0];
                        message = message.Remove(0, 1);
                    }
                    text.Add(a);
                    a = "";
                }

                text.Add(message + "\n");
            }
        }

        public void Write(string message)
        {
            lock (lockMessages)
            {

                while (message.Length > to.X - from.X)
                {
                    if (text.Count() > to.Y - from.Y)
                    {
                        text.Remove(text[0]);

                    }
                    string a = "";
                    for (int i = 0; i < to.X - from.X; i++)
                    {
                        a += message[0];
                        message = message.Remove(0, 1);
                    }

                    text.Add(a);
                    a = "";
                }
                if (text.Last().Length + message.Length < to.X - from.X && text.Last()[text.Last().Length - 1] != '\n')
                {
                    text[text.Count() - 1] = text.Last() + message;
                }
                else
                {
                    text.Add(message);
                }
            }
        }
    }


    class Program
    {
        static Group FindGroupByIndex(List<Group> groups, int id)
        {
            foreach (var a in groups)
            {
                if (a.Id == id)
                    return a;
            }
            return null;

        }
        static readonly object lockObj = new object();
        public static uint Menu(IEnumerable<string> Action)
        {
            uint active = 0;
            while (true)
            {
                Window1.ClearText();
                //Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Action.Count(); i++)
                {

                    if (i == active)
                        Window1.WriteLine($" > {Action.ElementAt(i)}");
                    else
                        Window1.WriteLine($"   {Action.ElementAt(i)}");
                }
                Window1.Draw();
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                    active = (active > 0 ? --active : (uint)Action.Count() - 1);
                else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                    active = (active < Action.Count() - 1) ? ++active : 0;
                else if (key == ConsoleKey.Enter)
                {
                    //Console.Clear();
                    return active;
                }

            }

        }

        public static uint MenuV2(IEnumerable<object> Action)
        {
            uint active = 0;
            while (true)
            {
                Window1.ClearText();
                //Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Action.Count(); i++)
                {

                    if (i == active)
                        Window1.WriteLine($" > {Action.ElementAt(i)}");
                    else
                        Window1.WriteLine($"   {Action.ElementAt(i)}");
                }
                Window1.Draw();
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                    active = (active > 0 ? --active : (uint)Action.Count() - 1);
                else if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                    active = (active < Action.Count() - 1) ? ++active : 0;
                else if (key == ConsoleKey.Enter)
                {
                    //Console.Clear();
                    return active;
                }

            }

        }


        static ConsoleWindow Window2 = new ConsoleWindow(new Point(35, 0), new Point(115, 25), lockObj, new object(), ConsoleColor.White, ConsoleColor.Green);
        static ConsoleWindow Window1 = new ConsoleWindow(new Point(0, 0), new Point(33, 25), lockObj, new object(), ConsoleColor.Black, ConsoleColor.Yellow);


        static Random rnd = new Random();

        public static List<string> getStudentsName(List<Student> students)
        {
            List<string> studentsName = new List<string>();
            foreach (Student student in students)
            {
                studentsName.Add(student.Name);
            }
            return studentsName;
        }

        static void AddProductType(UniversityContext context)
        {
            Window1.WriteLine("Type name:");
            Window1.Draw();
            string name = Console.ReadLine();
            context.ProductTypes.Add(new ProductType { Name = name});
        }
        static void AddProduct(UniversityContext context)
        {
            Window1.WriteLine("Type name:");
            Window1.Draw();
            string name = Console.ReadLine();
            int ProductTypeID;
            double Price;
            int InStock;
            while (true)
            {
                try
                {
                    Window1.WriteLine("Type ProductType ID:");
                    Window1.Draw();
                    ProductTypeID = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                catch (Exception ex)
                {
                    Window1.WriteLine(ex.Message); Window1.Draw();
                }
            }
            while (true)
            {
                try
                {
                    Window1.WriteLine($"Type Price:"); Window1.Draw();
                    Price = Convert.ToDouble(Console.ReadLine());
                    break;
                }
                catch (Exception ex)
                {
                    Window1.WriteLine(ex.Message); Window1.Draw();
                }
            }
            while (true)
            {
                try
                {
                    Window1.WriteLine($"Type InStock:"); Window1.Draw();
                    InStock = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                catch (Exception ex)
                {
                    Window1.WriteLine(ex.Message); Window1.Draw();
                }
            }
            context.Products.Add(new Product { Name = name, Price = Price, inStock = InStock, ProductType = context.ProductTypes.Where(s => s.Id == ProductTypeID).First() });

        }
        static void RemoveProduct(UniversityContext context)
        {
            var stud = context.Products.ToList();
            int choosen2 = (int)MenuV2(context.Products.ToList());
            Window1.Clear(); Window1.Draw();
            context.Products.RemoveRange(stud[choosen2]);
        }
        static void UpdateProduct(UniversityContext context)
        {
            var stud = context.Products.ToList();
            int choosen2 = (int)MenuV2(context.Products.ToList());
            Window1.Clear(); Window1.Draw();
            int choosen3 = (int)Menu(new string[]
            {
                                    "Name",
                                    "ProductType",
                                    "Price",
                                    "InStock"
            }) + 1;
            Window1.Clear(); Window1.Draw();
            switch (choosen3)
            {
                case 1:
                    {
                        Window1.WriteLine("Type name:"); Window1.Draw();
                        string name = Console.ReadLine();
                        context.Products.Find(stud[choosen2].Id).Name = name;
                        break;
                    }
                case 2:
                    {
                        int ProductTypeID;
                        while (true)
                        {
                            try
                            {
                                Window1.WriteLine("Type ProductType ID:"); Window1.Draw();
                                ProductTypeID = Convert.ToInt32(Console.ReadLine());
                                break;
                            }
                            catch (Exception ex)
                            {
                                Window1.WriteLine(ex.ToString()); Window1.Draw();
                            }
                        }
                        context.Products.Find(stud[choosen2].Id).ProductType = context.ProductTypes.Find(ProductTypeID);
                        break;
                    }
                case 3:
                    {
                        double Price;
                        while (true)
                        {
                            try
                            {
                                Window1.WriteLine($"Type Product\'s Price:"); Window1.Draw();
                                Price = Convert.ToDouble(Console.ReadLine());
                                break;
                            }
                            catch (Exception ex)
                            {
                                Window1.WriteLine(ex.ToString()); Window1.Draw();
                            }
                        }
                        context.Products.Find(stud[choosen2].Id).Price = Price;
                        break;
                    }
                case 4:
                    {
                        int InStock;
                        while (true)
                        {
                            try
                            {
                                Window1.WriteLine("Type InStock:"); Window1.Draw();
                                InStock = Convert.ToInt32(Console.ReadLine());
                                break;
                            }
                            catch (Exception ex)
                            {
                                Window1.WriteLine(ex.ToString()); Window1.Draw();
                            }
                        }
                        context.Products.Find(stud[choosen2].Id).inStock = InStock;
                        break;
                    }
            }
        }
        static void AddBasket(UniversityContext context)
        {
            context.Baskets.Add(new Basket { Date = DateTime.Now});
        }
        static void AddProductToBasket(UniversityContext context)
        {
            int choosen2 = (int)MenuV2(context.Baskets.ToList());
            Window1.Clear(); Window1.Draw();
            var baskets = context.Baskets.ToList();
            while (true)
            {
                try
                {
                    Window1.WriteLine($"Type ProductID(\'exit\' to exit):"); Window1.Draw();
                    var a = Console.ReadLine();
                    if (a.ToLower() == "exit")
                        break;
                    int ProductID = Convert.ToInt32(a);
                    context.ProductsInBaskets.Add(new ProductsInBasket { Basket = context.Baskets.Find(baskets[choosen2].Id), product = context.Products.Find(ProductID) });
                    context.Baskets.Find(baskets[choosen2].Id).TotalPrice += context.Products.Find(ProductID).Price;
                    context.Products.Find(ProductID).inStock--;
                    break;
                }
                catch (Exception ex)
                {
                    Window1.WriteLine(ex.Message); Window1.Draw();
                }
            }
        }
        static void RemoveBasket(UniversityContext context)
        {
            var Bask = context.Baskets.ToList();
            int choosen2 = (int)MenuV2(context.Baskets.ToList());
            Window1.Clear(); Window1.Draw();
            context.Baskets.RemoveRange(Bask[choosen2]);
            context.ProductsInBaskets.RemoveRange(context.ProductsInBaskets.Where(s => s.Basket == Bask[choosen2]));

        }


        public static double RandomAvg()
        {
            return Math.Round(rnd.Next(1, 11) + rnd.NextDouble(), 2);
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            Console.InputEncoding = UTF8Encoding.UTF8;

            /*new Thread(() => Parallel.Invoke(() =>
            {
                Window1.DrawBackground();
                while (true)
                {
                    Window1.Draw();
                    Thread.Sleep(400);
                }
            },
            () =>
            {
                Window2.DrawBackground();
                while (true)
                {
                    Window2.Draw();
                    Thread.Sleep(400);
                }
            })).Start();*/


            Window1.DrawBackground();
            Window2.DrawBackground();

            using (var context = new UniversityContext())
            {
                context.Database.EnsureCreated();
                /*context.Groups.AddRange(
                    new Group { Name = "P23", Curator = new Teacher {Name = "HoWL", SecondName = "Csharpowich" } },
                    new Group { Name = "P78", Curator = new Teacher { Name = "none", SecondName = "none" } },
                    new Group { Name = "P10", Curator = new Teacher { Name = "jsonReader", SecondName = "notJsonReader" } },
                    new Group { Name = "P55", Curator = new Teacher { Name = "Aboba", SecondName = "Abobowna" } }
                    );*/


                //for (int i = 0; i < 10; i++)
                //  context.Add(GenerateStudent(context));
                context.SaveChanges();


                while (true)
                {
                    context.Database.EnsureCreated();

                    var products = context.Products.ToList();
                    Window2.WriteLine("Products:");
                    Window2.Draw();
                    Window2.WriteLine(string.Join("\n", products));
                    Window2.Draw();
                    var Baskets = context.Baskets.ToList();
                    Window2.WriteLine("Baskets:");
                    Window2.Draw();
                    Window2.WriteLine(string.Join("\n", Baskets));
                    Window2.Draw();
                    var ProductType = context.ProductTypes.ToList();
                    Window2.WriteLine("ProductTypes:");
                    Window2.Draw();
                    Window2.WriteLine(string.Join("\n", ProductType));
                    Window2.Draw();



                    int choosen = (int)MenuV2(new string[]
                    {
                        "AddProductType",
                        "AddProduct",
                        "RemoveProduct",
                        "UpdateProduct",
                        "AddBasket",
                        "AddProductToBasket",
                        "RemoveBasket",
                        "Exit"
                    });


                    
                    Window1.Clear();

                    switch (choosen + 1)
                    {
                        case 1:
                            {
                                AddProductType(context);
                                break;
                            }
                        case 2:
                            {
                                AddProduct(context);
                                break;
                            }
                        case 3:
                            {
                                RemoveProduct(context);
                                break;
                            }
                        case 4:
                            {
                                UpdateProduct(context);
                                break;
                            }
                        case 5:
                            {
                                AddBasket(context);
                                break;
                            }
                        case 6:
                            {
                                AddProductToBasket(context);
                                break;
                            }
                        case 7:
                            {
                                RemoveBasket(context);
                                break;
                            }
                        case 8:
                            {
                                Environment.Exit(0);
                                break;
                            }

                    }
                    Window1.Clear();
                    Window2.Clear();
                    Window1.Draw();
                    Window2.Draw();
                    context.SaveChanges();
                }




                Console.ReadLine();
                Environment.Exit(1);
            }


        }
    }
}
