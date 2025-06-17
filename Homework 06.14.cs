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
        public double salary { get; set; }
        public Specialization specialization { get; set; }
        public override string ToString()
        {
            return $"{id,5} | {name,20} | {Math.Round(salary, 2), 10} | {specialization}";
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

    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Doctor Doctor { get; set; } = new Doctor();

        public override string ToString()
        {
            return $"{Id, 5} | {Name, 20} | {Age, 4} | {Doctor.id, 5} | {Doctor.name, 20}";
        }
    }

    public class UniversityContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        
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

        static readonly object lockObj = new object();


        static ConsoleWindow Window2 = new ConsoleWindow(new Point(35, 0), new Point(115, 25), lockObj, new object(), ConsoleColor.White, ConsoleColor.Green);
        static ConsoleWindow Window1 = new ConsoleWindow(new Point(0, 0), new Point(33, 25), lockObj, new object(), ConsoleColor.Black, ConsoleColor.Yellow);


        static Random rnd = new Random();

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

        static void AddPatient(UniversityContext context)
        {
            Window1.WriteLine("Type name:");
            Window1.Draw();
            string name = Console.ReadLine();
            int age;
            
            while (true)
            {
                try
                {
                    Window1.WriteLine($"Type Age:"); Window1.Draw();
                    age = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                catch (Exception ex)
                {
                    Window1.WriteLine(ex.Message); Window1.Draw();
                }
            }

            var doctors = context.Doctors.ToList();
            var choosen2 = (int)MenuV2(doctors);
            Window1.Clear(); Window1.Draw();

            context.Patients.Add(new Patient { Name = name, Age = age, Doctor = context.Doctors.Find(doctors[choosen2].id) });

        }

        static void UpdatePatient(UniversityContext context)
        {
            var Patients = context.Patients.ToList();
            int choosen2 = (int)MenuV2(context.Patients.ToList());
            Window1.Clear(); Window1.Draw();
            int choosen3 = (int)MenuV2(new string[]
            {
                                    "Name",
                                    "Age",
                                    "Doctor"
            }) + 1;
            Window1.Clear(); Window1.Draw();
            switch (choosen3)
            {
                case 1:
                    {
                        Window1.WriteLine("Type name:"); Window1.Draw();
                        string name = Console.ReadLine();
                        context.Patients.Find(Patients[choosen2].Id).Name = name;
                        break;
                    }
                case 2:
                    {
                        int Age;
                        while (true)
                        {
                            try
                            {
                                Window1.WriteLine("Type Age:"); Window1.Draw();
                                Age = Convert.ToInt32(Console.ReadLine());
                                break;
                            }
                            catch (Exception ex)
                            {
                                Window1.WriteLine(ex.ToString()); Window1.Draw();
                            }
                        }
                        context.Patients.Find(Patients[choosen2].Id).Age = Age;
                        break;
                    }
                case 3:
                    {
                        var doctors = context.Doctors.ToList();
                        var choosen = (int)MenuV2(doctors);
                        Window1.Clear(); Window1.Draw();
                        context.Patients.Find(Patients[choosen2].Id).Doctor = context.Doctors.Find(doctors[choosen].id);

                        break;
                    }
            }
        }

        static void RemovePatient(UniversityContext context)
        {
            var stud = context.Patients.ToList();
            int choosen2 = (int)MenuV2(context.Patients.ToList());
            Window1.Clear(); Window1.Draw();
            context.Patients.RemoveRange(stud[choosen2]);
        }

        static void AddDoctorToPatient(UniversityContext context)
        {

            var patients = context.Patients.ToList();
            var choosen1 = (int)MenuV2(patients);
            Window1.Clear(); Window1.Draw();

            var doctors = context.Doctors.ToList();
            var choosen2 = (int)MenuV2(doctors);
            Window1.Clear(); Window1.Draw();

            context.Patients.Find(patients[choosen1].Id).Doctor = context.Doctors.Find(doctors[choosen2].id);
        }




        static void Main(string[] args)
        {
            Window1.DrawBackground();
            Window2.DrawBackground();

            using (var context = new UniversityContext())
            {
                
                
                context.Database.EnsureCreated();


                //context.Specializations.AddRange(new Specialization { name = "Хирург" }, new Specialization { name = "Кардиолог" });
                //context.Doctors.AddRange(new Doctor { name = "Doctor1", specialization = context.Specializations.Find(1), salary = 20134.2 }, new Doctor { name = "Doctor2", specialization = context.Specializations.Find(2), salary = 18532.8 });
                //context.Patients.AddRange(
                //    new Patient { Name = "Patient1", Age = 20, Doctor = context.Doctors.Find(3) },
                //    new Patient { Name = "Patient2", Age = 25, Doctor = context.Doctors.Find(3) },
                //    new Patient { Name = "Patient3", Age = 55, Doctor = context.Doctors.Find(4) }
                //);
                context.SaveChanges();
                while (true) { 

                    context.Database.EnsureCreated();
                    Window2.Clear();
                    var Doctors = context.Doctors.Include(s => s.specialization).OrderByDescending(s => s.salary).ToList();
                    Window2.WriteLine("Doctors:");
                    Window2.Draw();
                    Window2.WriteLine(string.Join("\n", Doctors));
                    Window2.Draw();

                    var Patients = context.Patients.Include(s => s.Doctor).ToList();
                    Window2.WriteLine("Patients:");
                    Window2.Draw();
                    Window2.WriteLine(string.Join("\n", Patients));
                    Window2.Draw();




                    var a = (int)MenuV2(
                        new List<string>
                        {
                            "Add Patient",
                            "Edit Patient",
                            "Delete Patient",
                            "Add Doctor to Patient",
                            "Exit"
                        });
                    Window1.Clear(); Window1.Draw();

                    switch (a + 1) {
                            case 1:
                            {
                                AddPatient(context);
                                break;
                            }
                            case 2:
                            {
                                UpdatePatient(context);
                                break;
                            }
                            case 3:
                            {
                                RemovePatient(context);
                                break;
                            }
                            case 4:
                            {
                                AddDoctorToPatient(context);
                                break;
                            }
                            case 5:
                            {
                                Environment.Exit(1);
                                break;
                            }

                    }
                    context.SaveChanges();
                }



                Console.ReadLine();
                Environment.Exit(1);
            }


        }
    }
}
