using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Data;
using TareasMVC.Entities;
using TareasMVC.Models;
using TareasMVC.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TareasMVC.Controllers
{
    [Route("/api/steps")]
    public class StepsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IServiceUser serviceUser;

        public StepsController(ApplicationDbContext dbContext, IServiceUser serviceUser)
        {
            this.dbContext = dbContext;
            this.serviceUser = serviceUser;
        }

        [HttpPost("{taskId:int}")]
        public async Task<ActionResult<Step>> Post(int taskId, [FromBody] StepCreateDTO stepCreateDTO)
        {
            var userId = serviceUser.getUserId();

            var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

            if (task is null)
                return NotFound();

            if (task.UserCreatedId != userId)
                return Forbid();

            var existsSteps = await dbContext.Steps.AnyAsync(p => p.TaskId == taskId);

            var orderMayor = 0;
            if (existsSteps)
                orderMayor = await dbContext.Steps.Where(p => p.TaskId == taskId).Select(p => p.Order).MaxAsync();

            var step = new Step();

            step.TaskId = taskId;
            step.Order = orderMayor + 1;
            step.Description = stepCreateDTO.Description;
            step.Done = stepCreateDTO.Done;

            dbContext.Add(step);
            await dbContext.SaveChangesAsync();

            return step;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] StepCreateDTO stepCreateDTO)
        {
            var userId = serviceUser.getUserId();

            var step = await dbContext.Steps.Include(p => p.Task).FirstOrDefaultAsync(p => p.Id == id);

            if (step is null)
                return NotFound();

            if (step.Task.UserCreatedId != userId)
                return Forbid();

            step.Description = stepCreateDTO.Description;
            step.Done = stepCreateDTO.Done;

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userId = serviceUser.getUserId();

            var step = await dbContext.Steps.Include(p => p.Task).FirstOrDefaultAsync(p => p.Id == id);

            if (step is null)
                return NotFound();

            if (step.Task.UserCreatedId != userId)
                return Forbid();

            dbContext.Remove(step);
            await dbContext.SaveChangesAsync();

            return Ok();
        }


    }
}

