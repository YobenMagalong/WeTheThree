using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WeTheTree.Models
{
    public class Picture
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int Tree_ID { get; set; }
        public byte[] Img_Blob { get; set; }
    }
}
