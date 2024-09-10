
function handleClickAddStep() {

    // primero verificamos para que no se agregue mas de un input al agregar el paso
    const index = taskEditVM.steps().findIndex(p => p.isNew())
    if (index !== -1)
        return;

    const stepviewmodel = new stepViewModel({ modeEdit: true, done: false });
    taskEditVM.steps.push(stepviewmodel);

    $("[name=textStepDescription]:visible").focus();
}


function handleClickCancelStep(step) {
    if (step.isNew())
        taskEditVM.steps.pop();
    else {
        step.modeEdit(false);
        step.description(step.previousDescription);
    }
        
}

async function handleClickSaveStep(step) {
    step.modeEdit(false);
    const isNew = step.isNew();
    const taskId = taskEditVM.id;
    const data = getBodyRequestStep(step);

    const description = step.description();

    if (!description) {
        if (isNew)
            taskEditVM.steps.pop();

        return
    }


    if (isNew)
        await insertStep(step, data, taskId);
    else
        updateStep(data, step.id());
}

async function insertStep(step, data, taskId) {
    const response = await fetch(`${urlSteps}/${taskId}`, {
        body: data,
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const json = response.json();
        step.id(json.id);

        const task = getTaskInEdition();
        task.stepsTotal(task.stepsTotal() + 1);

        if (stepsTaken())
            task.stepsTaken(task.stepsTaken() + 1);


    } else {
        handleErrorApi(response)
    }
        

}

function getBodyRequestStep(step) {
    return JSON.stringify({
        description: step.description(),
        done: step.done(),
    })
}

function handleClickDescriptionStep(step) {
    step.modeEdit(true);
    step.previousDescription = step.description();
    $("[name=textStepDescription]:visible").focus();
}

async function updateStep(data, id) {
    const response = await fetch(`${urlSteps}/${id}`, {
        body: data,
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        }

    });

    if (!response.ok)
        handleErrorApi(response);
}

function handleClickCheckboxStep(step) {

    if (step.isNew())
        return true;

    const data = getBodyRequestStep(step);
    updateStep(data, step.id());

    const task = getTaskInEdition();

    let stepsTakenActual = task.stepsTaken();

    if (step.done())
        stepsTakenActual++;
    else
        stepsTakenActual--;

    task.stepsTaken(stepsTakenActual);

    return true;
}

function handleClickDeleteStep(step) {

    modalEditTaskBootstrap.hide();

    confirmAction({
        callBackAccept: () => {
            deleteStep(step);
            modalEditTaskBootstrap.show();
        },
        callBackCancel: () => {
            modalEditTaskBootstrap.show();
        },
        title: `Desea borrar este paso?`
    })

}

async function deleteStep(step) {

    const response = await fetch(`${urlSteps}/${step.id()}`, {
        method: 'DELETE'
    });

    if (!response.ok) {
        handleErrorApi(response);
        return;
    }

    taskEditVM.steps.remove((item) => item.id() === step.id());

    const task = getTaskInEdition();
    task.stepsTotal(task.stepsTotal() - 1);

    if (step.done())
        task.stepsTaken(task.stepsTaken() - 1);
        
}
