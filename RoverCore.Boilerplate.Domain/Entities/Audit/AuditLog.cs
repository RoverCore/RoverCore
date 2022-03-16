using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoverCore.Boilerplate.Domain.Entities.Audit
{
	public class AuditLog
	{
		[Key]
		public int AuditId { get; set; }
		public string TableName { get; set; }
		public string TablePK { get; set; }
		public string Title { get; set; }
		public string EntityType { get; set; }
		public string AuditAction { get; set; }
		public string AuditUser { get; set; }
		public string AuditData { get; set; }
		public DateTime AuditDate { get; set; }
	}
}