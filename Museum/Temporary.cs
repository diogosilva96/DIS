﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Museum
{
    public class Temporary : Events
    {
        public static readonly string ScheduleProperty = "schedule_id";

        public Temporary()
        {
            scheduleList = new List<Schedule>();
        }
        
        public Temporary(Dictionary<string,string> dictionary)
        {
            
        }

        private int id { get; set; }

        public int Id
        {
            get => id;
            set => id = value;
        }

        private IList scheduleList { get; set; }

        public IList Schedule
        {
            get => scheduleList;
            set => scheduleList = value;
        }

        public override string GetInformation()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            var table = "events";                                                     
            var keys = new [] {DescriptionProperty};
            var values = new [] {Description};
            var insertEvent  = SqlOperations.Instance.Insert(table, keys, values);
            DBConnection.Instance.Execute(insertEvent);
            
            table = "temporaries";                                                     
            keys = new [] {"events_id"};
            values = new [] {base.Id.ToString()};
            var insertTemporaries  = SqlOperations.Instance.Insert(table, keys, values);
            DBConnection.Instance.Execute(insertTemporaries);
        }

        public override void Update(string changeProperties, string changeValues, string table)
        {
            var properties = changeProperties.Split('-');
            var values = changeValues.Split('-');
            var error = false;
            for (var i = 0; i < properties.Length; i++)
                if (table == Event)
                {
                    if (properties[i] != DescriptionProperty) error = true;
                }
                else if (table == Temporary)
                {
                    if (properties[i] != ScheduleProperty) error = true;
                }
                else
                {
                    error = true;
                }

            if (error)
                Console.WriteLine("Nao e possivel efetuar essa operacao!");
            else
                UpdateSequence(table, properties, values);
        }
    }
}