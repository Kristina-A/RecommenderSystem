using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases.DomainModel
{
    public class MessageShow
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public List<string> Responses { get; set; }

        public MessageShow()
        {
            Responses = new List<string>();
        }
    }
}
