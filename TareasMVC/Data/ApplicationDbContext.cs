using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entities;
using Task = TareasMVC.Entities.Task;

namespace TareasMVC.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{

		}

		// Creamos la tabla Tasks a partir de la entidad o modelo
		public DbSet<Task> Tasks { get; set; }
		public DbSet<Step> Steps { get; set; }
		public DbSet<AttachedFile> AttachedFiles { get; set; }



		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            base.OnModelCreating(modelBuilder);

            // Indicandole llave primaria y configuraciones para la tabla Tasks
            modelBuilder.Entity<Task>().HasKey(t => t.Id); // asi le indicamos la llave primaria con la api fluente
			modelBuilder.Entity<Task>().Property(t => t.Title).IsRequired().HasMaxLength(250);


			// Indicandole llave primaria y configuraciones para la tabla Steps, y la relacion uno a muchos
			modelBuilder.Entity<Step>().HasKey(s => s.Id);
            modelBuilder.Entity<Step>().HasOne(t => t.Task).WithMany(s => s.Steps).HasForeignKey(s => s.TaskId);


			// Indicandole llave primaria y configuraciones para la tabla AttachedFiles, y la relacion uno a muchos 
			modelBuilder.Entity<AttachedFile>().HasKey(af => af.Id);
			modelBuilder.Entity<AttachedFile>().HasOne(t => t.Task).WithMany(af => af.AttachedFiles).HasForeignKey(af => af.TaskId);
			modelBuilder.Entity<AttachedFile>().Property(af => af.Url).IsUnicode();


        }
    }
}

