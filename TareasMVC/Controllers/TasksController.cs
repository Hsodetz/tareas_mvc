using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Data;
using TareasMVC.Services;
using TareasMVC.Entities;
using TareasMVC.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace TareasMVC.Controllers
{
    [Route("/api/tasks")]
	public class TasksController : ControllerBase
	{
        private readonly ApplicationDbContext dbContext;
        private readonly IServiceUser serviceUser;
        private readonly IMapper mapper;

        public TasksController(ApplicationDbContext dbContext, IServiceUser serviceUser, IMapper mapper)
		{
            this.dbContext = dbContext;
            this.serviceUser = serviceUser;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDTO>>> Get()
        {
            // Simulamos un badrequest para ver el sweet alert en error
            //return BadRequest("No podes hacer es");

            var userid = serviceUser.getUserId();
            var tasks = await dbContext.Tasks
                .Where(t => t.UserCreatedId == userid)
                .OrderBy(t => t.Order)

                // asi hariamos sin usar automapper
                //.Select(t => new
                //{
                //    Id = t.Id,
                //    Title = t.Title,
                //})

                // ahora usando automapper seria asi
                .ProjectTo<TaskDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(tasks);
        }


        [HttpPost]
        public async Task<ActionResult<Entities.Task>> Post([FromBody] string title)
        {
            // obtenemos el id desl usuario desde elservicio que creamos para ello
            var userId = serviceUser.getUserId();

            // verificamos si hay tareas
            bool existTasks = await dbContext.Tasks.AnyAsync(t => t.UserCreatedId == userId);

            var orderMayor = 0;

            if (existTasks)
            {
                orderMayor = await dbContext.Tasks.Where(t => t.UserCreatedId == userId).Select(t => t.Order).MaxAsync();
            }

            // para crear la tarea en memoria y de esta manera se hace el mapeo manual, pero hay una mejor forma q es automapper
            var task = new Entities.Task
            {
                Title = title,
                UserCreatedId = userId,
                CreatedAt = DateTime.UtcNow,
                Order = orderMayor + 1,
                Description = "Tareas",
            };

            dbContext.Add(task);
            await dbContext.SaveChangesAsync();

            return task;

        }

        [HttpPost("order")]
        public async Task<IActionResult> OrderNew([FromBody] int[] ids)
        {
            var userId = serviceUser.getUserId();
            var tasks = await dbContext.Tasks.Where(t => t.UserCreatedId == userId).ToListAsync();

            var tasksIds = tasks.Select(t => t.Id);

            var tasksNotBelongToUser = ids.Except(tasksIds).ToList();

            if (tasksNotBelongToUser.Any())
            {
                return Forbid();
            }

            var tasksToDictionary = tasks.ToDictionary(t => t.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var task = tasksToDictionary[id];
                task.Order = i + 1;
            }

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Entities.Task>> GetAsync(int id)
        {
            var userId = serviceUser.getUserId();

            var task = await dbContext.Tasks.Include(t => t.Steps).FirstOrDefaultAsync(t => t.Id == id && t.UserCreatedId == userId);

            if (task is null)
            {
                return NotFound();
            }

            return task;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Entities.Task>> Edit(int id, [FromBody] TaskEditDTO taskEditDTO)
        {
            var userId = serviceUser.getUserId();

            var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserCreatedId == userId);

            if (task is null)
            {
                return NotFound();
            }

            task.Title = taskEditDTO.Title;
            task.Description = taskEditDTO.Description;

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = serviceUser.getUserId();

            var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserCreatedId == userId);

            if (task is null)
            {
                return NotFound();
            }

            dbContext.Remove(task);

            await dbContext.SaveChangesAsync();

            return Ok();
        }

    }

    


}

