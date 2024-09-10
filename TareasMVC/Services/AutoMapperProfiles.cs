using System;
using AutoMapper;
using TareasMVC.Models;
using TareasMVC.Entities;

namespace TareasMVC.Services
{
	public class AutoMapperProfiles: Profile
	{
		public AutoMapperProfiles()
		{
			// le decimos para que automapper haga el mapeo desde tarea a tareaDTO
			CreateMap<Entities.Task, TaskDTO>()
				.ForMember(dto => dto.StepsTotal, ent => ent.MapFrom(x => x.Steps.Count()))
				.ForMember(dto => dto.StepsTaken, ent => ent.MapFrom(x => x.Steps.Where(p => p.Done).Count()));
		}
	}
}

