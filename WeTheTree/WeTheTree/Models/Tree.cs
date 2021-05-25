using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WeTheTree.Models
{
   public class Tree
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Tree_Code { get; set; }
        public string Initial_Identification { get; set; }
        public string Scientific_Name { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Location { get; set; }
        public string Landmarks_of_Location { get; set; }
        public string Height { get; set; }
        public string DMB { get; set; }
    }
}
