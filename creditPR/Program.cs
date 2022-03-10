using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace creditPR
{
    class Program
    {
        //public static bool  working = true;
        static void Main(string[] args)
        {
            var conString = "" +
                "Data source=localhost; " +
                "Initial catalog=PRPR; " +
                "user id=sa; " +
                "password=123456";
            var working = true;
            while (working)
            {
                Console.Write("1. Register\n2. Login\n0. Выход\n Выбрат:");
                int.TryParse(Console.ReadLine(), out var choice);
              
                switch (choice)
                {
                    case 1:
                        {
                                 CreateUser(conString);
                        }
                        break;
                    case 2:
                        Console.Write("Введите логин:\n В нашой системе логин это номер телефон наших клиентов:\n");
                        var login = int.Parse(Console.ReadLine());
                        var retLogin = CheckAccountByPhoneNumber(login, conString);
                        if (retLogin == 0) { Console.WriteLine($"Number { login } not found"); break; }

                        var retCridit = CheckCredit(login, conString);
                        //CheckCreditHistoryNumber(login, conString);
                        if (retCridit == 0) { Console.WriteLine($"Number { login } not found"); break; }
                        HistoryApp(conString);
                        Console.WriteLine("Если хитите посмотреть график погошения нажмите 1:\n если нет нажмите любую цийру");
                        var detail = Console.ReadLine();

                        if (string.IsNullOrEmpty(detail))
                        {
                            Console.WriteLine("Были ради вам помочь");
                            break;
                        }

                        if (detail == "1")
                        {
                            Login(conString, login);
                        }
                        else
                        {
                            Console.WriteLine("Были ради вам помочь");
                        }
                     
                        break;

             
                    case 0:
                        working = false;
                        break;
                    default:
                        Console.WriteLine("Wrong command.");
                        break;
                }
                Console.WriteLine("Press any key...");
                Console.ReadLine();
                Console.Clear();
            }
        }
        public static int Id = 0;
        private static int CheckAccountByPhoneNumber(int number,string conString)
        {
            var accNumber = 0;
            //var userId = 0;
            var connection = new SqlConnection(conString);
            var query = "SELECT [id],[phone] FROM [dbo].[users] WHERE [phone] = @number";
            //var queryID = "SELECT a.userId FROM [PRPR].[dbo].[Users] u inner join Applications a on u.id = a.userId where phone = @number";
            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("@number", number);
            command.CommandText = query;
           // command.CommandText = queryID;

            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Id = (int)reader["id"];
                accNumber = reader.GetInt32(0);
               // HistoryApp(conString);
            }
            connection.Close();
            reader.Close();
             return accNumber;
         }
        private static int CheckCredit(int number,string conString)
        {
            var accNumber = 0;
            //var userId = 0;
            var connection = new SqlConnection(conString);
            //var query = "SELECT [id],[phone] FROM [dbo].[users] WHERE [phone] = @number";
            var queryID = "SELECT a.userId FROM [PRPR].[dbo].[Users] u inner join Applications a on u.id = a.userId where phone = @number";
            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("@number", number);
            command.CommandText = queryID;
           // command.CommandText = queryID;

            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                //Id = (int)reader["id"];
                accNumber = reader.GetInt32(0);
               // HistoryApp(conString);
            }
            connection.Close();
            reader.Close();
             return accNumber;
         }
        private static void HistoryApp(string conString)
        {
            Application[] applications = new Application[0];

            var connection = new SqlConnection(conString);
            //var query = "SELECT [Id] ,[FirstName] ,[LastName] ,[MiddleName] ,[Created_At] ,[Updated_At] FROM [dbo].[Clients]";
            var historyApp = "SELECT [id],[creditSum],[termCredit],[userId],[payedSum],[percent],[score],[creditStatus],[created_at] FROM [PRPR].[dbo].[Applications] where userId  = 2";

            var command = connection.CreateCommand();
            command.CommandText = historyApp;

            connection.Open();

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var application = new Application { };
                application.id = int.Parse(reader["id"].ToString());
                application.Percent = (int)reader["percent"];
                application.CreditSum = (decimal)reader["creditSum"];
                application.TermCredit = (int)reader["termCredit"];
                application.CreditStatus = 1;
                application.CreatedAt = (DateTime)reader["created_at"];
                AddApp(ref applications, application);

            }
            connection.Close();
            foreach (var client in applications)
            {
                Console.WriteLine($"Номер кредита:{client.id}, Сумма кредита:{client.CreditSum}, FirstName:{client.TermCredit}, MiddleName:{client.CreditStatus}, CreatedAt:{client.CreatedAt}");
            }
        }
 
 
        private static void AddApp(ref Application[] applications, Application application)
        {
            if (applications == null)
            {
                return;
            }

            Array.Resize(ref applications, applications.Length + 1);

            applications[applications.Length - 1] = application;
        }

        private static void Login(string conString,int login)
        {
            var conn = new SqlConnection(conString);      
            var QueryLgin = "SELECT u.id,u.name, a.creditSum,a.[percent], a.payedSum,a.termCredit,a.created_at FROM [PRPR].[dbo].[Users] u inner join Applications a on u.id = a.userId where phone = @login";
            var command = conn.CreateCommand();
            command.Parameters.AddWithValue("@login", login);
            //command.CommandText = checkPhone;
             //command.CommandText = Allcredit;
             command.CommandText = QueryLgin;
            conn.Open();
            var reader = command.ExecuteReader();
            decimal CreditSum = 0;
            decimal payedSum = 0;
            var TermCredit = 0;
            var Percent = 0;
            var CreatedAt = DateTime.Now;
            while (reader.Read())
            {
                //Name = reader["Name"].ToString();
                //Surname = reader["surname"].ToString();
                Percent = (int)reader["percent"];
                CreditSum = (decimal)(reader["creditSum"]);
                TermCredit = (int)reader["termCredit"];
                payedSum = (decimal)reader["payedSum"];
                CreatedAt = (DateTime)reader["created_at"];
            }
      
             decimal perSum = 0;
            decimal repAmount = 0;
            //decimal paySum = 0;
            //decimal payedSum = 0;
            //DateTime dt = new(2021, 09, 30);
            Console.WriteLine("Сумма Погашения,Процент,Погашенная Сумма, Остаток, Дни Погашения");
            for (int i = 0; i < TermCredit; i++)
            {
                 repAmount = CreditSum / TermCredit;
                // var perSum = repAmount * app.Percent / 100;
                perSum = Convert.ToDecimal(repAmount * Percent / 100);
                //paySum = payedSum;
                Console.WriteLine($"{repAmount}           {perSum:0.00}       {payedSum}      {repAmount - payedSum}      {  CreatedAt.Date.AddMonths(i):d}");
            }
            Console.WriteLine("");
            Console.WriteLine($"Общая сумма:  Сумма процентов:");
            Console.WriteLine($"{ CreditSum + (perSum * TermCredit) : 0.00}         {perSum * TermCredit}");
        }

        //List<int> AuthorList = new List<int>();
        public static int[] balance = new int[30];
        //int[] balance = new int[];
        public static decimal sal = 6000;    //need to chabge

        public static decimal percentSalary = 0;

        public static string FirstName = "";
        public static string LastName = "";

        public static int userId = 0;
        public static int Phone = 0;

        // public static decimal sal = 5000;    //need to chabge

        private static void CreateForm(string conString)
        {
            var form = new Form();
            //Console.WriteLine("Create Form for client");
            Console.WriteLine("enterage");
            form.Age = int.Parse(Console.ReadLine());

            //sum = decimal.Parse(Console.ReadLine());
            //var salary = (sum / sal) * 100;
           
            switch (form.Age)
            {
                case <= 25:
                    Console.WriteLine("Введите макс(2) и мин(0) балл ");
                    var bal = int.Parse(Console.ReadLine());
                    balance[1] = bal;
                    break;
                case > 26 and <= 35:
                    balance[2] = 1;
                    break;
                case > 36 and <= 62:
                    balance[3] = 2;
                    break;
                case > 63:
                    balance[4] = 1;
                    break;
            }
             Console.WriteLine("enterSalary");
            form.Salary = int.Parse(Console.ReadLine());
            sal = form.Salary;
            Console.WriteLine("entercredit history:\n более 3-ёх закр кредитов: \n 1-2 закр кредита \n нет кред ист");
            var history = int.Parse(Console.ReadLine());
            if (history > 0)
            {
                form.CreditHistory = history;
            }
            else
            {
                var his = Convert.ToString(history);
                if (string.IsNullOrEmpty(his)) 
                {
                    form.CreditHistory = 0;

                }
            }

            //if (string.IsNullOrEmpty(history) || !string.IsNullOrEmpty(history) || history == "")
            //{
            //    form.CreditHistory = 0;
            //}
            //else
            //{
            //    form.CreditHistory = int.Parse(history);
            //}
            var chis = form.CreditHistory;
             switch (chis)
            {
                case > 3:
                    Console.WriteLine("Введите макс(2) и мин(-1) балл ");
                    var bal = int.Parse(Console.ReadLine());
                    balance[5] = bal;
                    break;
                case > 1 and <= 2:
                    balance[6] = 1;
                    break;
                default: 
                    balance[7] = -1;
                    break;
            }

             Console.WriteLine("enterOverdue Credit:\n свыше 7 раз: \n 5-7 раз: \n 4 раза: \n до 3 раз");
            var Overdue = (Console.ReadLine());
            if (string.IsNullOrEmpty(Overdue))
            {
                form.OverdueCredit = 0;
            }
            //else
            //{
            //    form.OverdueCredit = int.Parse(Overdue);
            //}
            var Over = int.Parse(Overdue);
            if (Over > 0)
            {
                form.OverdueCredit = Over;
            }
            else
            {
                var ov = Convert.ToString(Over);
                if (string.IsNullOrEmpty(ov) )
                {
                    form.OverdueCredit = 0;

                }
            }
            //if (string.IsNullOrEmpty(Overdue) || !string.IsNullOrEmpty(Overdue))
            //{
            //    form.OverdueCredit = 0;
            //}
            //else
            //{
            //    form.OverdueCredit = int.Parse(Overdue);
            //}
            //form.OverdueCredit = int.Parse(Console.ReadLine());
            switch (form.OverdueCredit)
            {
                case > 7:
                    Console.WriteLine("Введите макс(0) и мин(-3) балл ");
                    var bal = int.Parse(Console.ReadLine());
                    balance[8] = bal;
                    break;
                case > 5 and <= 7:
                    balance[9] = -2;
                    break;
                case  4 :
                    balance[10] = -1;
                    break;
                case <= 3:
                    balance[11] = 0;
                    break;
            }
            form.CreatedAt = DateTime.Now;
            var connection = new SqlConnection(conString);
            var query = "INSERT INTO [dbo].[Form]([age],[salary],[creditHistory],[overdueCreditHistory],[created_at]) VALUES (@age,@salary, @creditHistory, @overdueCredit, @createdAt)";
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@age", form.Age);
            command.Parameters.AddWithValue("@salary", form.Salary);
            command.Parameters.AddWithValue("@creditHistory", form.CreditHistory);
            command.Parameters.AddWithValue("@overdueCredit", form.OverdueCredit);
            command.Parameters.AddWithValue("@createdAt", form.CreatedAt);
            connection.Open();
            var myQueryId = "SELECT* FROM Users WHERE ID = (SELECT MAX(ID) FROM Users)";
            command.CommandText = myQueryId;
            var LastId = (Int32)command.ExecuteScalar();
            userId = LastId;
            var result = command.ExecuteNonQuery();
             if (result > 0)
            {
                Console.WriteLine($"Клиент: {FirstName} Логин:{Phone} успешно создан.");
            }


            CreateApplication(conString);
            connection.Close();
        }

        private static void CreateUser(string conString)
        {
            {

                var user = new User();
                Console.WriteLine("enter Who is you want to register: \n1. Create Client\n2. Create Administrator");
                int.TryParse(Console.ReadLine(), out var choice);
                switch (choice)
                {
                    case 1:
                        {
                            user.WhoIs = "Client";
                        }
                        break;
                    case 2:
                        {
                            user.WhoIs = "Administrator";
                        }
                        break;
                    default:
                        Console.WriteLine("Wrong Command");
                        CreateUser(conString);
                        // Console.WriteLine("выход");
                        break;
                }
                //   Console.WriteLine("выход");
                var s = true;
                while (s)
                {
                    Console.WriteLine("entername");
                    var Name = Console.ReadLine();
                    if (string.IsNullOrEmpty(Name))
                    {
                        Console.WriteLine("Имя не можеть быть пустым или int");
                    }
                    else
                    {
                        user.Name = Name;
                        FirstName = Name;
                        s = false;
                    }

                }
                var l = true;
                while (l)
                {
                    Console.WriteLine("enter surname");
                    var Surname = Console.ReadLine();
                    if (string.IsNullOrEmpty(Surname))
                    {
                        Console.WriteLine("Фамилия не можеть быть пустым или int");
                    }
                    else
                    {
                        user.Surname = Surname;
                        LastName = Surname;
                        l = false;
                    }
                }
                // user.Surname = Convert.ToString(Surname);
                Console.WriteLine("entermiddlename");
                user.MiddleName = Console.ReadLine();
                Console.WriteLine("enterdate of birth");
                user.DateOfBirth = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("Введите логин:\n В качестве логина принимается номер мобильного телефона");
                Phone = int.Parse(Console.ReadLine());
                user.CreatedAt = DateTime.Now;

                Console.WriteLine("enterGender");

                Console.WriteLine("Choose credit term One or Two: \n1. Male\n2. Female");
                _ = int.TryParse(Console.ReadLine(), out var choiceGender);

                switch (choiceGender)
                {
                    case 1:
                        user.Gender = "Male";
                        Console.WriteLine("Введите макс(2) и мин(1) балл");
                        var bal = int.Parse(Console.ReadLine());
                        balance[22] = bal;
                        //term = false;
                        break;
                    case 2:
                        user.Gender = "Female";
                        balance[23] = 2;
                        //term = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Wrong comand: You can choose 1 or 2\n");
                        break;
                }

                Console.WriteLine("entermarital status");

                Console.WriteLine("Choose marital status: \n1. Single\n2. Married\n3. Divorced\n4. Widower / Widow");
                _ = int.TryParse(Console.ReadLine(), out var choiceMarital);

                switch (choiceMarital)
                {
                    case 1:
                        user.maritalStatus = "Single";
                        Console.WriteLine("Введите макс(2) и мин(1) балл");
                        var bal = int.Parse(Console.ReadLine());
                        balance[24] = bal;
                        //term = false;
                        break;
                    case 2:
                        user.maritalStatus = "Married";
                        balance[25] = 2;
                        //term = false;
                        break;
                    case 3:
                        user.maritalStatus = "Divorced";
                        balance[26] = 1;
                        //term = false;
                        break;
                    case 4:
                        user.maritalStatus = "Widower / Widow";
                        balance[27] = 2;
                        //term = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Wrong comand: You can choose 1,2,3,4\n");
                        break;
                }
                Console.WriteLine("enternationality");
                Console.WriteLine("Choose credit term One or Two: \n1. Tajikistan\n2. Foreigner");
                _ = int.TryParse(Console.ReadLine(), out var choiceNational);

                switch (choiceNational)
                {
                    case 1:
                        user.Nationality = "Tajikistan";
                        Console.WriteLine("Введите макс(1) и мин(0) балл");
                        var bal = int.Parse(Console.ReadLine());
                        balance[28] = bal;
                        //term = false;
                        break;
                    case 2:
                        user.Nationality = "Foreigner";
                        balance[29] = 0;
                        //term = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Wrong comand: You can choose 1 or 2\n");
                        break;
                }
                Console.WriteLine("Введите прописку:");
                user.Registration = Console.ReadLine();
                //s = false;
                // }
                var connection = new SqlConnection(conString);
                var query = "INSERT INTO [dbo].[Users]([name],[surname],[middleName],[phone],[dateOfBirth],[gender],[maritalStatus],[nationality],[registration],[whoIS],[created_at]) VALUES (@name, @surname, @middleName,@phone, @bitrthDate,@gender,@maritalStatus,@nationality,@registration, @whoIs, @createdAt)";
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surname", user.Surname);
                command.Parameters.AddWithValue("@middleName", user.MiddleName);
                command.Parameters.AddWithValue("@bitrthDate", user.DateOfBirth);
                command.Parameters.AddWithValue("@phone", Phone);
                command.Parameters.AddWithValue("@gender", user.Gender);
                command.Parameters.AddWithValue("@maritalStatus", user.maritalStatus);
                command.Parameters.AddWithValue("@nationality", user.Nationality);
                command.Parameters.AddWithValue("@registration", user.Registration);
                command.Parameters.AddWithValue("@whoIs", user.WhoIs);
                command.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                connection.Open();
                var result = command.ExecuteNonQuery();

                //if (result > 0)
                //{
                //    Console.WriteLine("User created successfully.");
                //}

                connection.Close();
            }
            // Console.WriteLine("выход");
            CreateForm(conString);
        }

        private static void CreateApplication(string conString)
        {
            var app = new Application();
            var working = true;
            while (working)
            {
                Console.WriteLine("purpose of the credit : \n1. Appliances\n2. Repair\n3. Phone\n4. Other");
                int.TryParse(Console.ReadLine(), out var choice);
                switch (choice)
                {
                    case 1:
                        app.PurposeCredit = "Appliances";
                        Console.WriteLine("Введите макс(2) и мин(-1) балл ");
                        var bal = int.Parse(Console.ReadLine());
                        balance[12] = bal;
                        working = false;
                        // AuthorList.Add(1);
                        break;
                    case 2:
                        app.PurposeCredit = "Repair";
                        balance[13] = 2;
                        working = false;
                        //  AuthorList.Add(2);
                        break;
                    case 3:
                        app.PurposeCredit = "Phone";
                        balance[14] = 0;
                        working = false;
                        // AuthorList.Add(3);
                        break;
                    case 4:
                        app.PurposeCredit = "Other";
                        //Console.WriteLine("Введите макс и мин балл ");
                        //var bal =  int.Parse(Console.ReadLine());
                        balance[15] = -1;
                        working = false;
                        // AuthorList.Add(4);
                        break;
                    case 0:
                        working = false;
                        break;
                    default:
                        Console.WriteLine("Wrong command.");
                        Console.WriteLine("Press any key...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
            //Console.WriteLine("Choose credit term : \n1. more than 12\n2. up to 12");
            //int.TryParse(Console.ReadLine(), out var choiceTerm);
            //var term = true;
            //while (term)
            //{
            Console.WriteLine("entercredit persent:");
            _ = int.TryParse(Console.ReadLine(), out var persent);
            app.Percent = persent;
            Console.WriteLine("Choose credit term:");
                _ = int.TryParse(Console.ReadLine(), out var choiceTerm);

            if (choiceTerm > 12)
            {
                balance[16] = 1;
                app.TermCredit = choiceTerm;
            }
            else
            {
                balance[17] = 1;
                app.TermCredit = choiceTerm;
            }

            var s = true;
            while (s)
            {
                Console.WriteLine("enterCredit summ");
                //var summ = Console.ReadLine();
                //if (string.IsNullOrEmpty(summ))
                //{
                //    Console.WriteLine("Сумма не можеть быть пустым или строковым");
                //}
                //else
                //{
                //    app.CreditSum = decimal.Parse(summ);
                //    s = false;
                //}
                //}
                decimal summm = 0;
                try
                {
                    summm = decimal.Parse(Console.ReadLine());
                    percentSalary = summm / sal * 100;
                    switch (percentSalary)
                    {
                        case <= 80:
                            balance[18] = 4;
                            break;
                        case > 80 and <= 150:
                            balance[19] = 3;
                            break;     
                        case > 150 and <= 250:
                            balance[20] = 2;
                            break;   
                        case > 250:
                            balance[21] = 4;
                            break;                     
                        //case > 150 or <= 250:
                        //    balance[7] = 2;
                        //    break;
                        //default:
                        //    break;
                    }
                    app.CreditSum = summm;
                    s = false;
                }
                catch
                {
                    Console.WriteLine("Сумма не можеть быть пустым или строковым");
                    s = true;
                };
            }
            app.CreatedAt = DateTime.Now;
            decimal sum = 0;
            Array.ForEach(balance, delegate (int i) { sum += i; });
            Console.WriteLine(sum);
           // var sumScore = (sum * percentSalary) / 100;
            var sumScore = (sum * 70) / 100;
            Console.WriteLine(sumScore);
            var crStatus = 1;

            if (sumScore <= 11)
            {
                Console.WriteLine($"Извините,\nУважаемый(ая) {FirstName} - {LastName }  Мы не можем выдать Вам Кредит");
                crStatus = 0;
            }
                var connection = new SqlConnection(conString);
                var query = "INSERT INTO [dbo].[Applications]([purposeCredit],[creditSum],[termCredit],[userId],[payedSum],[percent],[score],[creditStatus],[created_at]) VALUES (@purposeCredit, @creditSum, @termCredit,@userId,@payedSum,@percent,@score,@creditStatus, @createdAt)";
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@purposeCredit", app.PurposeCredit);
                command.Parameters.AddWithValue("@termCredit", app.TermCredit);
                command.Parameters.AddWithValue("@creditSum", app.CreditSum);
                command.Parameters.AddWithValue("@userId", app.userId = userId);
                command.Parameters.AddWithValue("@payedSum", app.PayedSum);
                command.Parameters.AddWithValue("@percent", app.Percent);
                command.Parameters.AddWithValue("@score", sumScore);
                command.Parameters.AddWithValue("@creditStatus", crStatus);
                command.Parameters.AddWithValue("@createdAt", app.CreatedAt);
                connection.Open();
                var result = command.ExecuteNonQuery();


            //if (result > 0)
            //{
            //    Console.WriteLine("application created successfully");
            //}
            //else 
            if (sumScore > 11)
            {
                Console.WriteLine("Поздравяем ваш заявка на кредит одобрена.");
                var myQueryId = "select u.name,u.surname,u.phone, ap.creditSum, ap.termCredit,ap.payedSum from Applications as ap inner join Users u on ap.userId = u.id where userId = userId";
                    command.CommandText = myQueryId;
                    var reader = command.ExecuteReader();
                    var Name = "";
                    var Surname = "";
                    var Phone = 0;
                    decimal CreditSum = 0;
                    decimal payedSum = 0;
                    var TermCredit = 0;
                    var CreatedAt = DateTime.Now.Date;
                    while (reader.Read())
                    {
                        Name = reader["Name"].ToString();
                        Surname = reader["surname"].ToString();
                        Phone = (int)reader["phone"];
                        CreditSum = (decimal)(reader["creditSum"]);
                        TermCredit = (int)reader["termCredit"];
                        payedSum = (decimal)reader["payedSum"];
                    }
                    connection.Close();

                    //DateTime dt = new (2021, 09, 30);
                    decimal perSum = 0;
                    Console.WriteLine("Сумма Погашения,Процент,Погашенная Сумма,Остаток, Дни Погашения");
                    for (int i = 0; i < TermCredit; i++)
                    {
                        var repAmount = CreditSum / TermCredit;
                        // var perSum = repAmount * app.Percent / 100;
                        perSum = Convert.ToDecimal(repAmount * app.Percent / 100);
                        Console.WriteLine($"{repAmount}           {perSum:0.00}       {payedSum}       {repAmount - payedSum}       {  CreatedAt.AddMonths(i):d}");
                    }
                    Console.WriteLine("");
                    Console.WriteLine($"Общая сумма:  Сумма процентов:");
                    Console.WriteLine($"{ CreditSum + (perSum * TermCredit): 0.00}         {perSum * TermCredit}");
                } 
                //CreateApplication(conString);
                connection.Close();
            
            //else
            //{
            //    Console.WriteLine($"Извините,\nУважаемый(ая) {FirstName} - {LastName }  Мы не можем выдать Вам Кредит");
            //}
        }      
 
        //private static int GetAccountId(string number,string conString)
        //{
        //    var accNumber = 0;
        //    var connection = new SqlConnection(conString);
        //    var query = "SELECT [Id] FROM [dbo].[Accounts] WHERE [Number] = @number";

        //    var command = connection.CreateCommand();
        //    command.Parameters.AddWithValue("@number", number);
        //    command.CommandText = query;

        //    connection.Open();

        //    var reader = command.ExecuteReader();

        //    while (reader.Read())
        //    {
        //        accNumber = reader.GetInt32(0);
        //    }
        //    connection.Close();
        //    reader.Close();

        //    return accNumber;
        //}
        //GetAccountId(conString);


        //    CreateForm(conString);

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string MiddleName { get; set; }
            public int Phone { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string maritalStatus { get; set; }
            public string Nationality { get; set; }
            public string Registration { get; set; }
            public string WhoIs { get; set; }

            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class Application
        {
            public int id { get; set; }
            public string PurposeCredit { get; set; }
            public decimal CreditSum { get; set; }
            public decimal PayedSum { get; set; }
            public int userId { get; set; }
            public int Percent { get; set; }
            public decimal Score { get; set; }
            public int CreditStatus { get; set; }
            public int TermCredit { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class Form
        {
            public int id { get; set; }
            public int Age { get; set; }
            public decimal Salary { get; set; }
            public int CreditHistory { get; set; }
            public int OverdueCredit { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }



    }
}
