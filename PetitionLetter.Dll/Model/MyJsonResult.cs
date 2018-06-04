using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitionLetter.Dll.Model
{
    public class MyJsonResult
    {
        public string message { get; set; }
        public bool success { get; set; }
        public object data { get; set; }
    }

    public class JsonData
    {
        public int total { get; set; }
        public object rows { get; set; }
        public object other { get; set; }
    }
}
