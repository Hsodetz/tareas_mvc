using System;
using Microsoft.AspNetCore.Identity;

namespace TareasMVC.Entities
{
	public class Task
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
		public DateTime CreatedAt { get; set; }

		public string UserCreatedId { get; set; }
		public IdentityUser UserCreated { get; set; }


		// Una tarea puede tener muchos pasos, relacion uno a muchos
		public List<Step> Steps { get; set; }

		// una tarea puede tener muchos archivos adjuntos (AttachedFiles)
		public List<AttachedFile> AttachedFiles { get; set; }

	}
}

