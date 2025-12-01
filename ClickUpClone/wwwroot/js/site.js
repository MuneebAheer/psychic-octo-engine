// Site JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });

    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function(popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});

// Utility function for AJAX calls
async function makeRequest(url, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
        },
    };

    if (data) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(url, options);
        return await response.json();
    } catch (error) {
        console.error('Request failed:', error);
        return null;
    }
}

// Subtask toggle function
function toggleSubtask(subtaskId, taskId) {
    const checkbox = event.target;
    const isChecked = checkbox.checked;

    makeRequest(`/tasks/updatesubtask?subtaskId=${subtaskId}&isCompleted=${isChecked}`, 'POST').then(result => {
        if (result && result.success) {
            // Update UI
            const row = checkbox.closest('tr');
            if (isChecked) {
                row.classList.add('table-success');
            } else {
                row.classList.remove('table-success');
            }
        }
    });
}

// Confirm delete function
function confirmDelete(message = 'Are you sure?') {
    return confirm(message);
}
