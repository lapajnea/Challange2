using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Challange2
{
    public class FileStructure
    {
        public string MainFolder { get; set; }
        public Dictionary<string,long> SubFolders { get; set; }
        public Dictionary<string,long> Files { get; set; }

       
    }

    
}
