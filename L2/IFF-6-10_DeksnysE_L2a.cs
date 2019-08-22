using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IFF_6_10_DeksnysE_L2a
{
    class Program
    {
        public static List<Group> N_Groups = new List<Group>();
        public static StudentGroupssSortable M_Groups = new StudentGroupssSortable();
        private static StudentGroupssSortable M_Groups_Print = new StudentGroupssSortable();
        public static StudentGroupssSortable Result_Groups = new StudentGroupssSortable();
        public const string DataFile = @"IFF-6-10_DeksnysE_L2a_dat_1.txt";
        public const string ResultFile = @"IFF-6-10_DeksnysE_L2a_rez.txt";

        // Monitorius, skirtas veiksmų sinchronizacijai
        public class MyMonitor
        {
            //Užraktas
            //naudojamas kritinei sekcijai apsaugoti
            private readonly object _locker;

            // Bendras masyvas monitorius, į kurį įterpimo gijos talpina duomenis, o šalinimo gijos iš 
            // jo ima
            private StudentGroupssSortable _GroupsSortable { get; set; }

            public MyMonitor()
            {
                _locker = new object();
                
            }

            //naudojamas komunikuoti gijai su kitomis gijomis,
            //t.y. pranešti apie įterpimą / pašalinimą
            public StudentGroupssSortable GroupsSortable
            {
                get => _GroupsSortable;
                private set
                {
                    _GroupsSortable = value;
                    Monitor.Pulse(_locker);

                }
            }

            //Sinchronizuotas metodas, reikalingas elementų įterpimui į bendrą masyvą
            public void AddStudent(string group, Student student)
            {
                lock (_locker)
                {
                    //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    Program.Result_Groups.AddBirthDate(student.BirthDate, group);
                    GroupsSortable = Program.Result_Groups;
                }

            }

            //Sinchronizuotas metodas, reikalingas šalinimui iš bendro masyvo
            public void Delete(string group, StudentSortable student)
            {
                lock (_locker)
                {
                    //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    while (GroupsSortable == null
                           || (GroupsSortable != null && GroupsSortable.groups == null)
                           || (GroupsSortable != null && GroupsSortable.groups != null && (GroupsSortable.groups.FirstOrDefault(x => x.Group == group) == null))
                           || (GroupsSortable != null && GroupsSortable.groups != null && (GroupsSortable.groups.FirstOrDefault(x => x.Group == group) != null && (GroupsSortable.groups.FirstOrDefault(x => x.Group == group).MStudent == null))))
                    {
                        Console.WriteLine("Laukia");
                        Monitor.Wait(_locker);
                    }

                    if (GroupsSortable != null && GroupsSortable.groups != null)
                    {
                        var sortableGroup = GroupsSortable.groups.FirstOrDefault(x => x.Group == group);

                        if (sortableGroup == null || sortableGroup.MStudent.ToList().FirstOrDefault(x => x == student) == null)
                        {
                            if (student.Counter > 0)
                            {
                                Program.Result_Groups.DeleteBirthDate(student.BirthDate, group);
                                var studentToDecrease = M_Groups.groups.FirstOrDefault(x => x.Group == group).MStudent.FirstOrDefault(x => x == student);
                                studentToDecrease.Counter--;

                                M_Groups.groups.FirstOrDefault(x => x.Group == group).MStudent.FirstOrDefault(x => x == student).Counter = studentToDecrease.Counter;

                                GroupsSortable = Program.Result_Groups;
                            }
                        }
                    }
                }
            }
        }


        public class Group
        {
            public Student[] students { get; set; }
            public string group { get; set; }

        }

        // Klasė, sauganti reikiamų rikiavimo laukų ir jų kiekių objektus
        public class GroupSortable
        {
            public StudentSortable[] MStudent { get; set; }
            public string Group { get; set; }

        }

        // Klasė, skirta gamintojų gaminamų produktų objektams saugoti
        // studento vardas, gimimo data, ir pažymių vidurkis
        public class Student
        {
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }
            public double Average { get; set; }

        }

        // Klasė, sauganti rikiavimo lauką ir jo kiekį
        public class StudentSortable
        {
            public DateTime BirthDate { get; set; }
            public int Counter { get; set; }
        }

        // Klasė, sauganti reikiamų rikiavimo laukų ir jų kiekių objektus
        public class StudentGroupssSortable
        {
            public List<GroupSortable> groups { get; set; }

            //įterpia elementą į bendrą masyvą
            public void AddBirthDate(DateTime birthDate, string group)
            {
                bool birthDateAlreadyAdded = false;
                
                if (groups == null)
                {
                    groups = new List<GroupSortable>();
                }

                var selectedGroup = groups.FirstOrDefault(x => x.Group == group);

                if (selectedGroup == null)
                {
                    //jei rikiuojamo lauko grupė yra neįterpta, tada galime pridėti naują grupę
                    StudentSortable[] arrayOfDates = new List<StudentSortable>{new StudentSortable
                    {
                        BirthDate = birthDate,
                        Counter = 0
                    }}.ToArray();

                    groups.Add(new GroupSortable
                    {
                        Group = group,
                        MStudent = arrayOfDates
                    });

                    return;
                }

                foreach (var student in selectedGroup.MStudent)
                {
                    if (birthDate == student.BirthDate)
                    {
                        //padidaname rikiuojamo lauko kiekį
                        student.Counter++;
                        birthDateAlreadyAdded = true;
                        break;
                    }
                }

                if (!birthDateAlreadyAdded)
                {
                    //pridedame naują rikiuojamą lauką
                    List<StudentSortable> mStudent = selectedGroup.MStudent.ToList();
                    mStudent.Add(new StudentSortable
                    {
                        BirthDate = birthDate,
                        Counter = 0
                    });


                    selectedGroup.MStudent = mStudent.ToArray();
                }
            }


            public void AddBirthDate(DateTime birthDate, string group, int counter)
            {
                bool birthDateAlreadyAdded = false;
                
                if (groups == null)
                {
                    groups = new List<GroupSortable>();
                }

                var selectedGroup = groups.FirstOrDefault(x => x.Group == group);

                if (selectedGroup == null)
                {
                    //jei rikiuojamo lauko grupė yra neįterpta, tada galime pridėti naują grupę
                    StudentSortable[] arrayOfDates = new List<StudentSortable>{new StudentSortable
                    {
                        BirthDate = birthDate,
                        Counter = counter
                    }}.ToArray();

                    groups.Add(new GroupSortable
                    {
                        Group = group,
                        MStudent = arrayOfDates
                    });

                    return;
                }

                foreach (var student in selectedGroup.MStudent)
                {
                    if (birthDate == student.BirthDate)
                    {
                        //padidaname rikiuojamo lauko kiekį
                        student.Counter++;
                        birthDateAlreadyAdded = true;
                        break;
                    }
                }


                if (!birthDateAlreadyAdded)
                {
                    //pridedame naują rikiuojamą lauką
                    List<StudentSortable> mStudent = selectedGroup.MStudent.ToList();
                    mStudent.Add(new StudentSortable
                    {
                        BirthDate = birthDate,
                        Counter = counter
                    });


                    selectedGroup.MStudent = mStudent.ToArray();
                }
            }

            //pašalina elementą iš bendro masyvo
            public void DeleteBirthDate(DateTime birthDate, string group)
            {
                bool removeBirthDate = false;

                var selectedGroup = groups.FirstOrDefault(x => x.Group == group);

                foreach (var student in selectedGroup.MStudent)
                {
                    if (birthDate == student.BirthDate)
                    {
                        if (student.Counter == 0)
                        {
                            removeBirthDate = true;
                            break;
                        }
                        //mažiname rikiuojamo lauko kiekį
                        student.Counter--;
                        break;
                    }
                }

                //naikiname rikiuojamą lauką
                if (removeBirthDate)
                {
                    var studentsList = selectedGroup.MStudent.ToList();
                    studentsList.Remove(studentsList.FirstOrDefault(x => x.BirthDate == birthDate));
                    studentsList.Sort((x, y) => x.BirthDate < y.BirthDate ? 0 : 1);
                    selectedGroup.MStudent = studentsList.ToArray();
                }
            }
        }

        static void Main(string[] args)
        {
            ReadData();
            RunThreads();
            WriteData();
            Console.ReadLine();
        }

        public static void RunThreads()
        {
            var myMonitor = new MyMonitor();

            var deletionThreads = Enumerable.Range(0, 2).Select(i =>
                new Thread(() =>
                {
                    for (int x = 0; x < M_Groups.groups[i].MStudent.Length; x++)
                    {
                        for (int z = 0; z < 100 * 20000; z++)
                        {
                            Math.Pow(z, z);

                        }
                        while (M_Groups.groups[i].MStudent[x].Counter > 0)
                        {
                            myMonitor.Delete(M_Groups.groups[i].Group, M_Groups.groups[i].MStudent[x]);
                            //M_Groups.groups[i].MStudent[x].Counter--;
                        }
                    }

                })).AsParallel();

            //sukuria gijas duomenų įterpimui į bendrą masyvą
            var insertionThread = Enumerable.Range(0, 2).Select(i =>
                new Thread(() =>
                {
                    for (int x = 0; x < N_Groups[i].students.Length; x++)
                    {
                        for (int z = 0; z < 100 * 2000; z++)
                        {
                            Math.Pow(z, z);

                        }
                        myMonitor.AddStudent(N_Groups[i].group, N_Groups[i].students[x]);
                    }


                })).AsParallel();

            //sukuria gijas trinti duomenys iš bendro masyvo




            var threads = deletionThreads.Concat(insertionThread).ToList();

            foreach (var thread in threads) { thread.Start(); }
            foreach (var thread in threads) { thread.Join(); }

        }

        /// <summary>
        /// Nuskaito duomenų failą ir sudeda nuskaitytus duomenys
        /// į N_Groups ir M_Groups masyvus
        /// </summary>
        public static void ReadData()
        {
            using (StreamReader reader = new StreamReader(DataFile))
            {
                var line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "break")
                    {
                        break;
                    }

                    var readData = line.Split(' ');

                    if (readData.Length <= 1)
                    {

                        N_Groups.Add(new Group
                        {
                            students = new Student[0],
                            group = readData[0]

                        });

                        continue;
                    }


                    var name = readData[0];
                    var birthDate = DateTime.Parse(readData[1]);
                    var average = double.Parse(readData[2]);

                    Student readStudent = new Student
                    {
                        Name = name,
                        Average = average,
                        BirthDate = birthDate
                    };

                    var listOfStudent = N_Groups.Last().students.ToList();
                    listOfStudent.Add(readStudent);

                    N_Groups.Last().students = listOfStudent.ToArray();
                }

                var readLines = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    readLines.Add(line);

                }

                string group = "";

                foreach (var readLine in readLines)
                {
                    var splittedReadLine = readLine.Split(' ');
                    if (splittedReadLine.Length > 1)
                    {
                        var birthDate = DateTime.Parse(splittedReadLine[0]);
                        var counter = int.Parse(splittedReadLine[1]);
                        M_Groups.AddBirthDate(birthDate, group, counter);
                        M_Groups_Print.AddBirthDate(birthDate, group, counter);
                    }
                    else
                    {
                        group = readLine;
                    }
                }
            }
        }

        /// <summary>
        /// Spausdina gautus rezultatus lentelėmis
        /// </summary>
        public static void WriteData()
        {
            using (StreamWriter writer = new StreamWriter(ResultFile))
            {
                writer.WriteLine("{0}", "Studentų duomenų rinkiniai");

                int count = N_Groups.Count - 1;
                int counter = 1;
                foreach (var group in N_Groups.ToList())
                {
                    writer.WriteLine();
                    writer.WriteLine("{0} grupė", group.group);
                    writer.WriteLine();
                    writer.WriteLine("{0,-9} {1,-16} {2,-12} {3,-12}", "", "Vardas", "Gimimo data", "Vidurkis");

                    foreach (var student in group.students.ToList())
                    {
                        writer.WriteLine(new string('-', 64));
                        writer.WriteLine((counter) + ") {0,-6} {1,-16} {2,-12} {3, -12}", "", student.Name, student.BirthDate.ToShortDateString(), student.Average);
                        writer.WriteLine(new string('-', 64));
                        counter++;
                    }

                    counter = 1;
                }

                writer.WriteLine();
                writer.WriteLine("{0}", "Rikiavimo struktūros");
                writer.WriteLine();
                counter = 1;
                foreach (var group in M_Groups_Print.groups.ToList())
                {
                    writer.WriteLine();
                    writer.WriteLine("{0} grupė", group.Group);
                    writer.WriteLine();
                    writer.WriteLine("{0,-2} {1,-18} {2,-12}", "", "Rikiuojamas laukas", "Kiekis");
                    foreach (var student in group.MStudent.ToList())
                    {
                        writer.WriteLine(new string('-', 32));
                        writer.WriteLine(counter + ") {0,-22} {1,-12}", student.BirthDate.ToShortDateString(), student.Counter);
                        writer.WriteLine(new string('-', 32));
                        counter++;
                    }

                    counter = 1;
                }

                writer.WriteLine();

                writer.WriteLine("{0}", "Rezultatas - bendras masyvas");
                writer.WriteLine();
                foreach (var group in Result_Groups.groups.ToList())
                {
                    if (group.MStudent.Length <= 0)
                    {
                        writer.WriteLine("{0}i grupei nėra duomenų", group.Group);
                    }
                    else
                    {
                        writer.WriteLine();
                        writer.WriteLine("{0} grupė", group.Group);
                        writer.WriteLine();
                        writer.WriteLine("{0,-2} {1,-18} {2,-12}", "", "Rikiuojamas laukas", "Kiekis");
                    }
                    foreach (var student in group.MStudent.ToList())
                    {
                        writer.WriteLine(new string('-', 32));
                        writer.WriteLine(counter + ") {0,-18} {1,-12}", student.BirthDate.ToShortDateString(), student.Counter + 1);
                        writer.WriteLine(new string('-', 32));
                        counter++;
                    }

                    counter = 1;
                }
            }
        }
    }
}
