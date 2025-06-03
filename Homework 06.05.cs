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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Game
{

    public class Doctor
    {
        public int id { get; set; }
        public string name { get; set; }
        public Specialization specialization { get; set; }
        public override string ToString()
        {
            return $"{id,5} | {name,20} | {specialization}";
        }
    }
    public class Specialization
    {
        public int id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return $"{id,5} | {name,20}";
        }
    }

    public class UniversityContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "server=localhost;database=hospital;user=root;password=";
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
            CursorPosition = a.X!> to.X - from.X && a.X !< 0 && a.Y !> to.Y - from.Y && a.Y !< 0 ? a : new Point(0, 0);
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
        static ConsoleWindow Window2 = new ConsoleWindow(new Point(35, 0), new Point(115, 25), lockObj, new object(), ConsoleColor.White, ConsoleColor.Green);
        static ConsoleWindow Window1 = new ConsoleWindow(new Point(0, 0), new Point(33, 25), lockObj, new object(), ConsoleColor.Black, ConsoleColor.Yellow);

        static void Main(string[] args)
        {
            Window1.DrawBackground();
            Window2.DrawBackground();

            using (var context = new UniversityContext())
            {
                
                
                context.Database.EnsureCreated();


                //context.Specializations.AddRange(new Specialization { name = "Хирург" }, new Specialization { name = "Кардиолог" });
                //context.Doctors.AddRange(new Doctor { name = "Doctor1", specialization = context.Specializations.Find(1) }, new Doctor { name = "Doctor2", specialization = context.Specializations.Find(2) });

                var Doctors = context.Doctors.Include(s => s.specialization).ToList();
                var Specializations = context.Specializations.ToList();
                Window2.WriteLine("Doctors:");
                Window2.Draw();
                Window2.WriteLine(string.Join("\n", Doctors));
                Window2.Draw();


                Window2.WriteLine("Specializations:");
                Window2.Draw();
                Window2.WriteLine(string.Join("\n", Specializations));
                Window2.Draw();

                context.SaveChanges();
                

                
                Console.ReadLine();
                Environment.Exit(1);
            }


        }
    }
}
