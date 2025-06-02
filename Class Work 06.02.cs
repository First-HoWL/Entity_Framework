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
            return $"{Id,5} | {Name,10} | {Math.Round(Avg, 2), 6} | {groupe}";
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
            return $"{Id,5} | {Name, 10} | {Curator}";
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


    public class UniversityContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "server=localhost;database=university;user=root;password=";
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



        public static double RandomAvg()
        {
            return rnd.Next(1, 11) + rnd.NextDouble();
        }

        static void Main(string[] args)
        {
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
                
                while (true) {
                    context.Database.EnsureCreated();

                    var groups = context.Groups.ToList();
                    var teachers = context.Teachers.ToList();
                    var students = context.Students.Include(s => s.groupe);
                    Window2.WriteLine("Students:");
                    Window2.Draw();
                    Window2.WriteLine(string.Join("\n", students));
                    Window2.Draw();

                    int choosen = (int) Menu(
                        new string[]
                        {
                            "Add student",
                            "Remove student",
                            "Update student",
                            "Exit"
                        }
                        );
                    Window1.Clear();

                    switch (choosen + 1){ 
                        case 1:  {
                                Window1.WriteLine("Type name:");
                                Window1.Draw();
                                string name = Console.ReadLine();
                                int groupID;
                                double avg;
                                while (true) 
                                {
                                    try
                                    {
                                        Window1.WriteLine("Type group ID:");
                                        Window1.Draw();
                                        groupID = Convert.ToInt32(Console.ReadLine());
                                        break;
                                    }
                                    catch(Exception ex)
                                    {
                                        Window1.WriteLine(ex.ToString()); Window1.Draw();
                                    }
                                }
                                while (true)
                                {
                                    try
                                    {
                                        Window1.WriteLine($"Type {name}\'s AVG:"); Window1.Draw();
                                        avg = Convert.ToDouble(Console.ReadLine());
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        Window1.WriteLine(ex.ToString()); Window1.Draw();
                                    }
                                }
                                context.Students.Add(new Student { Name = name, Avg = avg, groupe = context.Groups.Find(groupID)});
                                break;
                        }
                        case 2:  {
                                var stud = context.Students.ToList();
                                int choosen2 = (int)Menu(getStudentsName(context.Students.ToList()));
                                Window1.Clear(); Window1.Draw();
                                context.Students.RemoveRange(stud[choosen2]);
                                break;
                        }
                        case 3:  {
                                var stud = context.Students.ToList();
                                int choosen2 = (int)Menu(getStudentsName(context.Students.ToList()));
                                Window1.Clear(); Window1.Draw();
                                int choosen3 = (int)Menu(new string[]
                                {
                                    "Name",
                                    "Group",
                                    "Avg"
                                }) + 1;
                                Window1.Clear(); Window1.Draw();
                                switch (choosen3)
                                {
                                    case 1: {
                                            Window1.WriteLine("Type name:"); Window1.Draw();
                                            string name = Console.ReadLine();
                                            context.Students.Find(stud[choosen2].Id).Name = name;
                                            break; 
                                        }
                                    case 2: {
                                            int groupID;
                                            while (true)
                                            {
                                                try
                                                {
                                                    Window1.WriteLine("Type group ID:"); Window1.Draw();
                                                    groupID = Convert.ToInt32(Console.ReadLine());
                                                    break;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Window1.WriteLine(ex.ToString()); Window1.Draw();
                                                }
                                            }
                                            context.Students.Find(stud[choosen2].Id).groupe = context.Groups.Find(groupID);
                                            break; 
                                        }
                                    case 3: {
                                            double avg;
                                            while (true)
                                            {
                                                try
                                                {
                                                    Window1.WriteLine($"Type student\'s AVG:"); Window1.Draw();
                                                    avg = Convert.ToDouble(Console.ReadLine());
                                                    break;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Window1.WriteLine(ex.ToString()); Window1.Draw();
                                                }
                                            }
                                            context.Students.Find(stud[choosen2].Id).Avg = avg;
                                            break; 
                                        }
                                }
                                
                                break;
                        }
                        case 4:  {
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
