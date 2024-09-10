function addNewtaskToList() {
    taskListViewModel.tasks.push(new taskElementListViewModel({ id: 0, title: '' }));

    //$("[name=title-task]").last().focus();
}

async function handleFocusTitletask(task) {

    const title = task.title();

    if (!title) {
        taskListViewModel.task.pop();
        return;
    }
    
    const data = JSON.stringify(title);
    
    const response = await fetch(url, {
        method: 'POST',
        body: data,
        headers: {
            'content-type': 'application/json',
        }
    });

    
    if (response.ok) {
        const json = await response.json();
        task.id(json.id);
    } else {
        // ,mostramos mensaje de error
        handleErrorApi(response);
    }


}

async function getTasks() {
    taskListViewModel.spinnerLoading(true);

    const response = await fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    })

  
    if (!response.ok) {
        await handleErrorApi(response);
        return;
    }
        

    const json = await response.json();
    taskListViewModel.tasks([]);

    json.forEach((value) => {
        taskListViewModel.tasks.push(new taskElementListViewModel(value));
    })

    taskListViewModel.spinnerLoading(false);

}

async function updateOrderTasks() {
    const ids = getIdsTasks();

    await sendTasksIds(ids);

    const arrayOrdered = taskListViewModel.tasks.sorted(function(a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString())
    });

    taskListViewModel.tasks([]);
    taskListViewModel.tasks(arrayOrdered);
    
}

function getIdsTasks() {
    const ids = $("[name=title-task]").map(function() {
        return $(this).attr('data-id');
    }).get();

    return ids;
}

async function sendTasksIds(ids) {

    var data = JSON.stringify(ids);

    await fetch(`${url}/order`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json',
        }
    });
}

async function handleClickTask(task) {
    if (task.isNew()) {
        return;
    }

    const response = await fetch(`${url}/${task.id()}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    });

    if (!response.ok) {
        handleErrorApi(response);
        return;
    }

    console.log(response)

    const json = await response.json();
    console.log(json)

    taskEditVM.steps([]);

    json.steps.forEach((step) => {
        const stepviewmodel = new stepViewModel({ ...step, modeEdit: false })
        taskEditVM.steps.push(stepviewmodel);
    })

    taskEditVM.id = json.id;
    taskEditVM.title(json.title);
    taskEditVM.description(json.description);


    console.log(taskEditVM);

    modalEditTaskBootstrap.show();
}

async function handleChangeEditTask() {
    const obj = {
        id: taskEditVM.id,
        title: taskEditVM.title(),
        description: taskEditVM.description(),
    }

    if (!obj.title)
        return;

    await editTask(obj)

    const index = taskListViewModel.tasks().findIndex(t => t.id() == obj.id)
    const task = taskListViewModel.tasks()[index]
    task.title(obj.title)

}

async function editTask(task) {
    const data = JSON.stringify(task);

    const res = await fetch(`${url}/${task.id}`, {
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!res.ok) {
        handleErrorApi(res)
        throw "error";
    }
}

function intentDeleteTask(task) {

    modalEditTaskBootstrap.hide();

    confirmAction({
        callBackAccept: () => {
            deleteTask(task)
        },
        callBackCancel: () => {
            modalEditTaskBootstrap.show()
        },
        title: `Desea borrar la tarea ${task.title()}?`,
    });
   
}

async function deleteTask(task) {
    const idTask = task.id;

    const response = await fetch(`${url}/${idTask}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const index = getIndexTaskEdit();
        taskListViewModel.tasks.splice(index, 1);
    }
}

function getIndexTaskEdit(){
    return taskListViewModel.tasks().findIndex(t => t.id() === taskEditVM.id);
}

function getTaskInEdition() {
    const index = getIndexTaskEdit();

    return taskListViewModel.tasks()[index];
}

// funcion para ejecutar el arrastre de los elementos de la lista, para reordenarlos como querramos
$(function () {
    $("#rearrange").sortable({
        axis: 'y',
        stop: async function () {
            //alert('patando')
            await updateOrderTasks();
        }
    });
});