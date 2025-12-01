/// <summary>
/// Client-side AJAX handlers and utility functions for ClickUp Clone
/// Handles real-time updates, drag-drop, and interactive features
/// </summary>

// ===== UTILITY FUNCTIONS =====

/**
 * Generic AJAX helper using Fetch API
 * @param {string} endpoint - The API endpoint
 * @param {string} method - HTTP method (GET, POST, PUT, DELETE)
 * @param {object} data - Request body data
 * @returns {Promise} Response JSON
 */
async function apiCall(endpoint, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': document.querySelector('[name="__RequestVerificationToken"]')?.value || ''
        }
    };

    if (data) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(endpoint, options);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const result = await response.json();
        return result;
    } catch (error) {
        console.error('API Error:', error);
        showNotification(`Error: ${error.message}`, 'error');
        throw error;
    }
}

/**
 * Upload file via FormData
 * @param {string} endpoint - The upload endpoint
 * @param {FormData} formData - FormData containing file and metadata
 * @returns {Promise} Response JSON
 */
async function apiCallMultipart(endpoint, formData) {
    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            body: formData,
            headers: {
                'X-CSRF-TOKEN': document.querySelector('[name="__RequestVerificationToken"]')?.value || ''
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Upload Error:', error);
        showNotification(`Upload failed: ${error.message}`, 'error');
        throw error;
    }
}

/**
 * Show notification message to user
 * @param {string} message - Message to display
 * @param {string} type - Notification type (success, error, info, warning)
 */
function showNotification(message, type = 'info') {
    const typeMap = {
        'success': 'alert-success',
        'error': 'alert-danger',
        'warning': 'alert-warning',
        'info': 'alert-info'
    };

    const alertClass = typeMap[type] || 'alert-info';

    const html = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    const container = document.querySelector('#notification-container') 
        || document.querySelector('.container-fluid')
        || document.body;
    
    const alertElement = document.createElement('div');
    alertElement.innerHTML = html;
    container.insertBefore(alertElement.firstElementChild, container.firstChild);

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        const alert = container.querySelector('.alert');
        if (alert) {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }
    }, 5000);
}

// ===== TASK OPERATIONS =====

/**
 * Update task status via AJAX
 * @param {number} taskId - Task ID
 * @param {string} newStatus - New status value
 */
async function updateTaskStatus(taskId, newStatus) {
    try {
        const result = await apiCall(`/api/tasks/${taskId}/status`, 'POST', {
            status: newStatus
        });

        if (result.success) {
            showNotification('Task status updated', 'success');
            // Update the status display in the UI
            const statusElement = document.querySelector(`[data-task-id="${taskId}"] .task-status`);
            if (statusElement) {
                statusElement.textContent = newStatus;
            }
            return true;
        } else {
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('Failed to update task status:', error);
        return false;
    }
}

/**
 * Update task priority via AJAX
 * @param {number} taskId - Task ID
 * @param {string} newPriority - New priority value
 */
async function updateTaskPriority(taskId, newPriority) {
    try {
        const result = await apiCall(`/api/tasks/${taskId}/priority`, 'POST', {
            priority: newPriority
        });

        if (result.success) {
            showNotification('Task priority updated', 'success');
            return true;
        } else {
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('Failed to update task priority:', error);
        return false;
    }
}

/**
 * Assign task to user via AJAX
 * @param {number} taskId - Task ID
 * @param {string} assignedToId - User ID to assign to
 */
async function assignTask(taskId, assignedToId) {
    try {
        const result = await apiCall(`/api/tasks/${taskId}/assign`, 'POST', {
            assignedToId: assignedToId || null
        });

        if (result.success) {
            showNotification('Task assigned successfully', 'success');
            return true;
        } else {
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('Failed to assign task:', error);
        return false;
    }
}

// ===== SUBTASK OPERATIONS =====

/**
 * Toggle subtask completion status
 * @param {number} subtaskId - Subtask ID
 */
async function toggleSubtask(subtaskId) {
    try {
        const checkbox = document.querySelector(`[data-subtask-id="${subtaskId}"]`);
        const originalChecked = checkbox?.checked;

        const result = await apiCall(`/api/subtasks/${subtaskId}/toggle`, 'POST');

        if (result.success) {
            // Update checkbox
            if (checkbox) {
                checkbox.checked = result.data.isCompleted;
            }

            // Update UI
            const subtaskRow = checkbox?.closest('[data-subtask-row]');
            if (subtaskRow) {
                if (result.data.isCompleted) {
                    subtaskRow.classList.add('completed');
                } else {
                    subtaskRow.classList.remove('completed');
                }
            }

            showNotification('Subtask updated', 'success');
            return true;
        } else {
            if (checkbox) checkbox.checked = originalChecked;
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('Failed to toggle subtask:', error);
        return false;
    }
}

// ===== COMMENT OPERATIONS =====

/**
 * Add comment to task via AJAX
 * @param {number} taskId - Task ID
 * @param {string} content - Comment content
 */
async function addComment(taskId, content) {
    try {
        if (!content.trim()) {
            showNotification('Comment cannot be empty', 'warning');
            return false;
        }

        const result = await apiCall(`/api/tasks/${taskId}/comments`, 'POST', {
            content: content.trim()
        });

        if (result.success) {
            showNotification('Comment added', 'success');
            // Add comment to DOM
            addCommentToUI(result.data);
            // Clear textarea
            const textarea = document.querySelector(`#comment-textarea-${taskId}`);
            if (textarea) textarea.value = '';
            return true;
        } else {
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('Failed to add comment:', error);
        return false;
    }
}

/**
 * Update existing comment via AJAX
 * @param {number} commentId - Comment ID
 * @param {string} content - New comment content
 */
async function updateComment(commentId, content) {
    try {
        if (!content.trim()) {
            showNotification('Comment cannot be empty', 'warning');
            return false;
        }

        const result = await apiCall(`/api/comments/${commentId}`, 'PUT', {
            content: content.trim()
        });

        if (result.success) {
            showNotification('Comment updated', 'success');
            // Update comment in DOM
            const commentElement = document.querySelector(`[data-comment-id="${commentId}"] .comment-content`);
            if (commentElement) {
                commentElement.textContent = content;
            }
            return true;
        } else {
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('Failed to update comment:', error);
        return false;
    }
}

/**
 * Delete comment via AJAX
 * @param {number} commentId - Comment ID
 */
async function deleteComment(commentId) {
    if (!confirm('Are you sure you want to delete this comment?')) return false;

    try {
        const result = await apiCall(`/api/comments/${commentId}`, 'DELETE');

        if (result.success) {
            showNotification('Comment deleted', 'success');
            // Remove comment from DOM
            const commentElement = document.querySelector(`[data-comment-id="${commentId}"]`);
            if (commentElement) {
                commentElement.remove();
            }
            return true;
        } else {
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('Failed to delete comment:', error);
        return false;
    }
}

/**
 * Add comment HTML to the DOM
 * @param {object} commentData - Comment DTO
 */
function addCommentToUI(commentData) {
    const commentsContainer = document.querySelector('.comments-list');
    if (!commentsContainer) return;

    const commentHtml = `
        <div class="card mb-3" data-comment-id="${commentData.id}">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <h6 class="card-title mb-1">${commentData.userName}</h6>
                        <p class="comment-content text-muted">${escapeHtml(commentData.content)}</p>
                    </div>
                    <small class="text-muted">${formatDate(commentData.createdAt)}</small>
                </div>
                <div class="comment-actions mt-2">
                    <button class="btn btn-sm btn-link" onclick="editCommentUI(${commentData.id})">Edit</button>
                    <button class="btn btn-sm btn-link text-danger" onclick="deleteComment(${commentData.id})">Delete</button>
                </div>
            </div>
        </div>
    `;

    commentsContainer.insertAdjacentHTML('beforeend', commentHtml);
}

/**
 * Show edit mode for comment
 * @param {number} commentId - Comment ID
 */
function editCommentUI(commentId) {
    const commentElement = document.querySelector(`[data-comment-id="${commentId}"]`);
    if (!commentElement) return;

    const content = commentElement.querySelector('.comment-content').textContent;
    const editHtml = `
        <div class="comment-edit mb-2">
            <textarea class="form-control mb-2" id="edit-comment-${commentId}">${content}</textarea>
            <button class="btn btn-sm btn-primary" onclick="updateComment(${commentId}, document.getElementById('edit-comment-${commentId}').value)">Save</button>
            <button class="btn btn-sm btn-secondary" onclick="cancelEditComment(${commentId})">Cancel</button>
        </div>
    `;

    commentElement.querySelector('.comment-actions')?.remove();
    commentElement.querySelector('.comment-content')?.remove();
    commentElement.querySelector('.card-body')?.insertAdjacentHTML('beforeend', editHtml);
}

/**
 * Cancel editing comment
 * @param {number} commentId - Comment ID
 */
function cancelEditComment(commentId) {
    location.reload(); // Simple refresh for now
}

// ===== FILE UPLOAD =====

/**
 * Upload file attachment via AJAX
 * @param {number} taskId - Task ID
 * @param {File} file - File to upload
 */
async function uploadAttachment(taskId, file) {
    try {
        if (!file) {
            showNotification('No file selected', 'warning');
            return false;
        }

        const formData = new FormData();
        formData.append('taskId', taskId);
        formData.append('file', file);

        const result = await apiCallMultipart('/api/attachments', formData);

        if (result.success) {
            showNotification('File uploaded successfully', 'success');
            addAttachmentToUI(result.data);
            return true;
        } else {
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('File upload failed:', error);
        return false;
    }
}

/**
 * Delete attachment file
 * @param {number} attachmentId - Attachment ID
 */
async function deleteAttachment(attachmentId) {
    if (!confirm('Are you sure you want to delete this file?')) return false;

    try {
        const result = await apiCall(`/api/attachments/${attachmentId}`, 'DELETE');

        if (result.success) {
            showNotification('File deleted', 'success');
            const attachmentElement = document.querySelector(`[data-attachment-id="${attachmentId}"]`);
            if (attachmentElement) {
                attachmentElement.remove();
            }
            return true;
        } else {
            showNotification(result.message, 'error');
            return false;
        }
    } catch (error) {
        console.error('Failed to delete attachment:', error);
        return false;
    }
}

/**
 * Add attachment to UI
 * @param {object} attachmentData - Attachment DTO
 */
function addAttachmentToUI(attachmentData) {
    const attachmentsContainer = document.querySelector('.attachments-list');
    if (!attachmentsContainer) return;

    const attachmentHtml = `
        <div class="list-group-item" data-attachment-id="${attachmentData.id}">
            <div class="d-flex justify-content-between align-items-center">
                <div>
                    <a href="${attachmentData.filePath}" target="_blank" class="text-decoration-none">
                        <i class="bi bi-file"></i> ${attachmentData.fileName}
                    </a>
                    <br>
                    <small class="text-muted">${formatFileSize(attachmentData.fileSize)} • Uploaded by ${attachmentData.uploadedByName}</small>
                </div>
                <button class="btn btn-sm btn-danger" onclick="deleteAttachment(${attachmentData.id})">Delete</button>
            </div>
        </div>
    `;

    attachmentsContainer.insertAdjacentHTML('beforeend', attachmentHtml);
}

// ===== DRAG AND DROP =====

let draggedTaskId = null;
let draggedTaskListId = null;

/**
 * Initialize drag for a task element
 * @param {HTMLElement} element - Task element
 */
function makeDraggable(element) {
    element.draggable = true;
    element.addEventListener('dragstart', (e) => {
        draggedTaskId = element.dataset.taskId;
        draggedTaskListId = element.dataset.listId;
        element.classList.add('opacity-50', 'dragging');
    });

    element.addEventListener('dragend', (e) => {
        element.classList.remove('opacity-50', 'dragging');
    });
}

/**
 * Initialize drop zone for a task list
 * @param {HTMLElement} listElement - List container element
 * @param {number} listId - Task list ID
 */
function makeDropZone(listElement, listId) {
    listElement.addEventListener('dragover', (e) => {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
        listElement.classList.add('drop-zone-active');
    });

    listElement.addEventListener('dragleave', (e) => {
        if (e.target === listElement) {
            listElement.classList.remove('drop-zone-active');
        }
    });

    listElement.addEventListener('drop', (e) => {
        e.preventDefault();
        listElement.classList.remove('drop-zone-active');

        if (draggedTaskId && listId) {
            moveTaskToList(draggedTaskId, listId);
        }
    });
}

/**
 * Move task to a different list
 * @param {number} taskId - Task ID
 * @param {number} toListId - Destination list ID
 */
async function moveTaskToList(taskId, toListId) {
    try {
        // For now, just refresh the page
        // In a full implementation, this would call an API endpoint
        location.reload();
    } catch (error) {
        console.error('Failed to move task:', error);
        showNotification('Failed to move task', 'error');
    }
}

// ===== HELPER FUNCTIONS =====

/**
 * Escape HTML entities to prevent XSS
 * @param {string} text - Text to escape
 * @returns {string} Escaped text
 */
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

/**
 * Format date to readable string
 * @param {string} dateString - ISO date string
 * @returns {string} Formatted date
 */
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

/**
 * Format file size for display
 * @param {number} bytes - File size in bytes
 * @returns {string} Formatted size
 */
function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
}

// ===== INITIALIZATION =====

// Initialize on DOM ready
document.addEventListener('DOMContentLoaded', function() {
    // Initialize Bootstrap tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

    // Initialize draggable tasks
    document.querySelectorAll('[data-task-id]').forEach(el => makeDraggable(el));

    // Initialize drop zones
    document.querySelectorAll('[data-list-id]').forEach(el => {
        makeDropZone(el, el.dataset.listId);
    });

    // Auto-dismiss alerts
    document.querySelectorAll('.alert').forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
});
