
async function handleErrorApi(response) {
    let errorMessage = '';

    if (response.status === 400)
        errorMessage = await response.statusText;
    else if (response.status === 404)
        errorMessage = 'Recurso no encontrado';
    else
        errorMessage = 'Error inesperado!';

    showErrormessage(errorMessage);
}

function showErrormessage(message) {
    Swal.fire({
        icon: 'error',
        title: 'Error...!',
        text: message,
    })
}

function confirmAction({ callBackAccept, callBackCancel, title }) {
    Swal.fire({
        title: title || "Realmente deseas hacer esto?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si',
        focusConfirm: true,
    }).then((result) => {
        if (result.isConfirmed) {
            callBackAccept();
        } else if (callBackCancel) {
            callBackCancel();
        }
    })
}

