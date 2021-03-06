﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Museum
{
    public class Room
    {
        public static readonly string SizeProperty = "size";
        public static readonly string DescriptionProperty = "description";
        public static readonly string EventProperty = "events_id";

        public Room()
        {
            ArtPiecesList = new List<ArtPiece>();
        }
        
        public Room(Dictionary<string,string> rooms)
        {
            ArtPiecesList = new List<ArtPiece>();
        }

        private int id { get; set; }

        public int Id
        {
            get => id;
            set => id = value;
        }

        private int size { get; set; }

        public int Size
        {
            get => size;
            set => size = value;
        }

        private string description { get; set; }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public IList ArtPiecesList { get; set; }

        public void Save()
        {
            var table = "rooms";                                                     
            var keys = new [] {SizeProperty,DescriptionProperty};
            var values = new [] {Size.ToString(),Description};
            var insertRoom = SqlOperations.Instance.Insert(table, keys, values);
            DBConnection.Instance.Execute(insertRoom);
        }

        public void Update(string changeProperties, string changeValues)
        {
            var properties = changeProperties.Split('-');
            var values = changeValues.Split('-');
            var error = false;
            for (var i = 0; i < properties.Length; i++)
                if (properties[i] != SizeProperty && properties[i] != DescriptionProperty &&
                    properties[i] != EventProperty)
                    error = true;
            if (error)
            {
                Console.WriteLine("Nao e possivel efetuar essa operacao!");
            }
            else
            {
                var update = SqlOperations.Instance.Update(Id, "rooms", properties, values);
                DBConnection.Instance.Execute(update);
            }
        }
    }
}