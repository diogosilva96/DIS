﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Museum
{
    public abstract class Person
    {
        public static readonly string NameProperty = "name";
        public static readonly string PasswordProperty = "password";
        public static readonly string PhoneProperty = "phone";
        public static readonly string MailProperty = "mail";
        public static readonly string Itself = "persons";
        public static readonly string Exhibitor = "exhibitors";
        public static readonly string Employee = "employees";

        private int id { get; set; }

        public int Id
        {
            get => id;
            set => id = value;
        }

        private string name { get; set; }

        public string Name
        {
            get => name;
            set => name = value;
        }

        private string password { get; set; }

        public string Password
        {
            get => password;
            set => password = value;
        }

        private int phone { get; set; }

        public int Phone
        {
            get => phone;
            set => phone = value;
        }

        private string mail { get; set; }

        public string Mail
        {
            get => mail;
            set => mail = value;
        }

        private IList<Message> notifications { get; set; } = new List<Message>();

        public IList<Message> Notifications
        {
            get => notifications;
            set => notifications = value;
        }

        public abstract int RoleId();

        public bool CreateAccountMethod(Dictionary<string,string> values)
        {
            var adapter = new DictonaryAdapter(values);
            if (CheckAvailability(adapter.GetValue(MailProperty)))
            {
                GetData(values);
                return SubmitData();
            }
            else
            {
                return false;
            }
        }

        public void getMessages()
        {
            SqlOperations so = Museum.SqlOperations.Instance;
            DBConnection db = DBConnection.Instance;
            string[] selvals = { "*" };
            string[] tables = { "messages", "persons_has_messages" };
            string[] keys = { "persons_has_messages.persons_id", "persons_has_messages.messages_id" };
            string[] values = { "" + Id + "", "messages.id ORDER BY lastUpdate ASC" };
            string select = so.Select(selvals, tables, keys, values);
            Debug.WriteLine(select);
            IList<Dictionary<string, string>> l = db.Query(select);
            int c = 0;
            bool containsMessage;
            foreach (Dictionary<string, string> dmessages in l)//dicionario com as msgs
            {
                //Label msgtext = addMessageField(50 * c);
                DictonaryAdapter dam = new DictonaryAdapter(dmessages);
                string sender_id = dam.GetValue("sender_id");

                // receiver.Text = "To: " + Person.Name;
                // receiver.Text = receiver.Text + "        From: ";
                if (sender_id != null)
                {
                    string[] selval = { "*" };
                    string[] tab = { "persons" };
                    string[] k = { "persons.id" };
                    string[] v = { sender_id };
                    string sel = so.Select(selval, tab, k, v);
                    Debug.WriteLine(sel);
                    IList<Dictionary<string, string>> li = db.Query(sel);
                    Debug.WriteLine("l:" + l.Count + "li:" + li.Count);
                    foreach (Dictionary<string, string> dperson in li) //dicionario com a pessoa dessa msg
                    {

                        DictonaryAdapter dap = new DictonaryAdapter(dperson);
                        string did = dap.GetValue("id");

                        Person sender = checkRole(did);

                        Museum.Message msg = new Museum.Message(dmessages, sender);
                        DictonaryAdapter dictad = new DictonaryAdapter(dmessages);
                        containsMessage = false;
                        foreach (Museum.Message message in Notifications)
                        {
                            if (message.Id == int.Parse(dictad.GetValue("id"))) // se ja existir essa msg nas messages da pessoa
                            {
                                containsMessage = true;
                            }
                        }
                        if (containsMessage == false) // se a msg nao existir adiciona-a
                        {
                            Notifications.Insert(0, msg);
                        }
                    }
                }
            }

        }


        public static Person Login(string mailInserted, string passwordInserted)
        {
            var mail = mailInserted;
            var password = passwordInserted;

            var properties = new[] { "*" };
            var table = new[] { "persons" };
            var keys = new[] { Person.MailProperty };
            var values = new[] { mail };
            var checkEmailAvailability = SqlOperations.Instance.Select(properties, table, keys, values);
            Debug.WriteLine(checkEmailAvailability);
            var checkEmailAvailabilityResult = DBConnection.Instance.Query(checkEmailAvailability);
            if (checkEmailAvailabilityResult != null)
            {
                if (checkEmailAvailabilityResult.Count > 0)
                {
                    //Debug.WriteLine("Tem n linhas:"+ checkEmailAvailabilityResult.Count);
                    var adapter = new DictonaryAdapter(checkEmailAvailabilityResult[0]);
                    if (adapter.GetValue(Person.PasswordProperty).Equals(password))
                    {
                        properties = new[] { "*" };
                        table = new[] { "exhibitors" };
                        keys = new[] { "persons_id" };
                        values = new[] { adapter.GetValue("id") };
                        var getExhibitorData = SqlOperations.Instance.Select(properties, table, keys, values);
                        var exhibitorResult = DBConnection.Instance.Query(getExhibitorData);

                        //properties = new[] { "*" };
                        //table = new[] { "persons_has_messages", "messages" };
                        //keys = new[] { "id", "persons_id" };
                        //values = new[] { "messages_id", adapter.GetValue("id") };
                        //var messagesSQL = SqlOperations.Instance.Select(properties, table, keys, values);
                        //var messagesDictonary = DBConnection.Instance.Query(messagesSQL);
                        //var messagesList = new List<Message>();

                        //if (messagesDictonary != null)
                        //{
                        //    foreach (var message in messagesDictonary)
                        //    {
                        //        var messagesAdapter = new DictonaryAdapter(messagesDictonary[0]);
                        //        properties = new[] { "employees.id AS employees_id","persons.id AS persons_id",
                        //    Person.NameProperty, Person.PasswordProperty, Person.PhoneProperty, Person.MailProperty,
                        //    Museum.Employee.SalaryProperty };
                        //        table = new[] { "employees", "persons" };
                        //        keys = new[] { "persons_id" };
                        //        values = new[] { messagesAdapter.GetValue("id") };
                        //        var employeeSQL = SqlOperations.Instance.Select(properties, table, keys, values);
                        //        var result = DBConnection.Instance.Query(employeeSQL);
                        //        Person person;
                        //        if (result.Count > 0)
                        //        {
                        //            person = new Employee(result[0]);
                        //        }
                        //        else
                        //        {
                        //            properties = new[] { "exhibitors.id AS exhibitors_id", "persons.id AS persons_id", "name", "password", "phone", "mail", "type" };
                        //            table = new[] { "exhibitors, persons" };
                        //            keys = new[] { "persons_id" };
                        //            values = new[] { messagesAdapter.GetValue("id") };
                        //            var exhibitorsSQL = SqlOperations.Instance.Select(properties, table, keys, values);
                        //            result = DBConnection.Instance.Query(exhibitorsSQL);
                        //            person = new Exhibitor(result[0]);
                        //        }
                        //        Message newMessage = new Message(message, person);
                        //        messagesList.Add(newMessage);
                        //    }
                        //}


                        Person user = null;
                        if (exhibitorResult.Count > 0)
                        {
                            properties = new[] { "exhibitors.id AS exhibitors_id","persons.id AS persons_id",Person.NameProperty,
                            Person.PasswordProperty, Person.PhoneProperty, Person.MailProperty, Museum.Exhibitor.TypeProperty };
                            table = new[] { "exhibitors, persons" };
                            keys = new[] { "mail" };
                            values = new[] { adapter.GetValue("mail") };
                            var exhibitorsSQL = SqlOperations.Instance.Select(properties, table, keys, values);
                            var userData = DBConnection.Instance.Query(exhibitorsSQL);
                            user = new Exhibitor(userData[0]);
                        }
                        else
                        {
                            properties = new[] { "employees.id AS employees_id","persons.id AS persons_id",
                            Person.NameProperty, Person.PasswordProperty, Person.PhoneProperty, Person.MailProperty,
                            Museum.Employee.SalaryProperty };
                            table = new[] { "employees", "persons" };
                            keys = new[] { "mail" };
                            values = new[] { adapter.GetValue("mail") };
                            var employeesSQL = SqlOperations.Instance.Select(properties, table, keys, values);
                            //Debug.WriteLine(employeesSQL);
                            var userData = DBConnection.Instance.Query(employeesSQL);
                            user = new Employee(userData[0]);
                        }
                        return user;
                    }
                    else
                    {
                        Console.WriteLine("The data that you inserted is incorrect!");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("Doesnt exist this email in the system");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Person checkRole(string person_id)
        {
            //Verifica se é um exhibitor ou um employee
            string[] properties = new[] { "*" };
            string[] table = new[] { "exhibitors" };
            string[] k = new[] { "persons_id" };
            string[] v = new[] { person_id };
            var getExhibitorData = SqlOperations.Instance.Select(properties, table, k, v);
            var exhibitorResult = DBConnection.Instance.Query(getExhibitorData);
            Person user = null;
            IFactory personFactory = FactoryCreator.Instance.CreateFactory("PersonFactory");
            if (exhibitorResult.Count > 0)
            {
                properties = new[]
                {
                    "exhibitors.id AS exhibitors_id", "persons.id AS persons_id", Person.NameProperty,
                    Person.PasswordProperty, Person.PhoneProperty, Person.MailProperty, Museum.Exhibitor.TypeProperty
                };
                table = new[] {"exhibitors, persons"};
                k = new[] {"persons.id"};
                v = new[] {person_id};
                var exhibitorsSQL = SqlOperations.Instance.Select(properties, table, k, v);
                Debug.WriteLine(exhibitorsSQL);
                var userData = DBConnection.Instance.Query(exhibitorsSQL);
                user = (Exhibitor)personFactory.ImportData("Exhibitor", userData[0]);
            }
            else
            {
                properties = new[] { "employees.id AS employees_id","persons.id AS persons_id",
                    Person.NameProperty, Person.PasswordProperty, Person.PhoneProperty, Person.MailProperty,
                    Museum.Employee.SalaryProperty };
                table = new[] { "employees", "persons" };
                k = new[] { "persons.id"};
                v = new[] { person_id };
                var employeesSQL = SqlOperations.Instance.Select(properties, table, k, v);
                Debug.WriteLine(employeesSQL);
                var userData = DBConnection.Instance.Query(employeesSQL);
                
                user = (Employee)personFactory.ImportData("Employee",userData[0]);
            }
            return user;
        }

        public int GetMaxMessagesPages()
        { 
            var properties = new [] { "*" };
            var table = new [] { "persons_has_messages" };
            var keys = new [] {"persons_id"};
            var values = new [] {Id.ToString()};
            var messages = SqlOperations.Instance.Select(properties, table, keys, values);
            var result = DBConnection.Instance.Query(messages);
            int quantity = (int)Math.Ceiling((double) result.Count / 5);
            return (int) quantity;
        }

        public List<Process> GetProcesses(int index, string type)
        {
            var startIndex = (index - 1) * 5 + 1;
            var endIndex = (index - 1) * 5 + 5;
            var properties = new [] { "*" };
            var table = new [] { "processes" };
            var values = new [] {RoleId().ToString()};
            var keys = new[] {""};
            if (type == Employee)
                keys = new [] {"employees_id"};
            else if (type == Employee)
                keys = new[] {"exhibitors_id"};
            else
            {
                Console.WriteLine("Efetuou algum erro na atribuicao do tipo da pessoa1");
                return null;
            }
            var processes = SqlOperations.Instance.Select(properties, table, keys, values);
            var chosenProcesses = DBConnection.Instance.Query(processes);
            var processList = new List<Process>();
            foreach (var process in chosenProcesses)
            {
//       TODO         O PROCESSO ESTA COM MUITAS DEPENDENCIAS CUIDADO
//       TODO         Process processInstance = new Process();
//       TODO         processList.Add(processInstance);
            }

            return processList;
        }

        public int GetMaxProcessesPages(string type)
        {
            var properties = new [] { "*" };
            var table = new [] { "processes" };
            var keys = new [] {""};
            var values = new [] {RoleId().ToString()};
            if (type == Employee)
            {
                keys[0] = "employees_id";
            }
            else if (type == Exhibitor)
            {
                keys[0] = "exhibitors_id";
            }
            else
            {
                Console.WriteLine("Ocorreu algum erro na definicao do tipo de pessoa!");
                return 0;
            }
            var processes = SqlOperations.Instance.Select(properties, table, keys, values);
            var result = DBConnection.Instance.Query(processes);
            var quantity = Math.Ceiling((double) result.Count / 5);
            return (int) quantity;
        }

        public void GetData(Dictionary<string,string> values)
        {
            var dictionaryAdapter = new DictonaryAdapter(values);
            Name = dictionaryAdapter.GetValue(NameProperty);
            Password = dictionaryAdapter.GetValue(PasswordProperty);
            Phone = int.Parse(dictionaryAdapter.GetValue(PhoneProperty));
            Mail = dictionaryAdapter.GetValue(MailProperty);
        }

        public abstract bool SubmitData();

        public bool CheckAvailability(string mail)
        {
            var properties = new [] { "*" };
            var table = new [] { "persons" };
            var keys = new [] {MailProperty};
            var values = new [] {mail};
            var person = SqlOperations.Instance.Select(properties, table, keys, values);
            var persons = DBConnection.Instance.Query(person);
            if (persons.Count > 0)
                return false;
            return true;
        }

        public abstract void Update(string properties, string values, string table);

        public void UpdateSequence(string table, string[] properties, string[] values)
        {
            var update = SqlOperations.Instance.Update(Id, table, properties, values);
            DBConnection.Instance.Execute(update);
        }
    }
}