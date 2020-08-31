using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageDocuments.Api.ViewModel
{
    public class Document
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public long SizeInBytes { get; set; }
    }
}
