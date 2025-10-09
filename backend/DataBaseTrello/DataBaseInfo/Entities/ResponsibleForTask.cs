using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DataBaseInfo.models;

namespace DataBaseInfo.Entities
{
    public class ResponsibleForTask
    {
        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public virtual _Task Task { get; set; }
        public int MemberOfBoardId { get; set; }
        public virtual MemberOfBoard MemberOfBoard { get; set; }

    }
}
