using System;
using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Models
{
	public class TaskEditDTO
	{
		[Required]
		[StringLength(250)]
		public string Title { get; set; }
		public string Description { get; set; }
	}
}

