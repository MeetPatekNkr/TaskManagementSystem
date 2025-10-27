// wwwroot/js/projectDetails.js

// Simple email validation
function isValidEmail(email) {
    if (!email) return false;
    
    // Basic email validation
    if (email.indexOf('@') === -1) return false;
    if (email.indexOf('.') === -1) return false;
    if (email.indexOf(' ') !== -1) return false;
    
    return true;
}

// Show message function
function showMessage(message, type) {
    const messageDiv = document.getElementById('messageContainer');
    const alertClass = 'alert alert-' + type + ' alert-dismissible fade show';
    const alertHtml = '<div class="' + alertClass + '" role="alert">' +
        message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>';
    
    messageDiv.innerHTML = alertHtml;
    
    // Auto hide after 5 seconds
    setTimeout(function() {
        const alert = messageDiv.querySelector('.alert');
        if (alert) {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }
    }, 5000);
}

// Search users function
function searchUsers(searchTerm, projectId) {
    fetch('/Projects/SearchUsers?searchTerm=' + encodeURIComponent(searchTerm))
        .then(function(response) {
            return response.json();
        })
        .then(function(users) {
            displayUserResults(users, projectId);
        })
        .catch(function(error) {
            console.error('Error searching users:', error);
        });
}

// Display user search results
function displayUserResults(users, projectId) {
    const userSearchResults = document.getElementById('userSearchResults');
    userSearchResults.innerHTML = '';

    if (users.length === 0) {
        userSearchResults.innerHTML = '<div class="text-muted text-center">No users found</div>';
        userSearchResults.style.display = 'block';
        return;
    }

    users.forEach(function(user) {
        const userElement = document.createElement('div');
        userElement.className = 'user-result-item list-group-item list-group-item-action d-flex justify-content-between align-items-center';
        userElement.innerHTML = '<div><strong>' + user.fullName + '</strong><br><small class="text-muted">' + user.email + '</small></div><button class="btn btn-sm btn-success add-user-btn" data-user-id="' + user.id + '"><i class="fas fa-plus"></i> Add</button>';
        userSearchResults.appendChild(userElement);
    });

    userSearchResults.style.display = 'block';

    // Add event listeners to add buttons
    document.querySelectorAll('.add-user-btn').forEach(function(button) {
        button.addEventListener('click', function() {
            const userId = this.getAttribute('data-user-id');
            addMemberDirectly(projectId, userId, this);
        });
    });
}

// Add member directly function
function addMemberDirectly(projectId, userId, button) {
    const originalText = button.innerHTML;
    button.disabled = true;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';

    fetch('/Projects/AddMemberDirectly', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            projectId: projectId,
            userId: userId
        })
    })
    .then(function(response) {
        return response.json();
    })
    .then(function(data) {
        if (data.success) {
            showMessage(data.message, 'success');
            document.getElementById('userSearch').value = '';
            document.getElementById('userSearchResults').style.display = 'none';
            // Refresh the page after 1 second
            setTimeout(function() {
                location.reload();
            }, 1000);
        } else {
            showMessage('Error: ' + data.message, 'danger');
            button.disabled = false;
            button.innerHTML = originalText;
        }
    })
    .catch(function(error) {
        showMessage('Error adding member. Please try again.', 'danger');
        console.error('Error:', error);
        button.disabled = false;
        button.innerHTML = originalText;
    });
}

// Send invitation function
function sendInvitation(projectId) {
    const emailInput = document.getElementById('inviteEmail');
    const email = emailInput.value.trim();

    if (!email) {
        showMessage('Please enter an email address', 'danger');
        return;
    }

    if (!isValidEmail(email)) {
        showMessage('Please enter a valid email address', 'danger');
        return;
    }

    const submitBtn = document.querySelector('#sendInvitationForm button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Sending...';

    fetch('/Projects/SendInvitation', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            projectId: projectId,
            email: email
        })
    })
    .then(function(response) {
        return response.json();
    })
    .then(function(data) {
        if (data.success) {
            showMessage(data.message, 'success');
            emailInput.value = '';
        } else {
            showMessage('Error: ' + data.message, 'danger');
        }
    })
    .catch(function(error) {
        showMessage('Error sending invitation. Please try again.', 'danger');
        console.error('Error:', error);
    })
    .finally(function() {
        submitBtn.disabled = false;
        submitBtn.innerHTML = originalText;
    });
}

// Remove member function
function removeMember(projectId, userId, userName, button) {
    if (confirm('Are you sure you want to remove ' + userName + ' from the project?')) {
        const originalHtml = button.innerHTML;
        button.disabled = true;
        button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';

        fetch('/Projects/RemoveMember', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                projectId: projectId,
                memberId: userId
            })
        })
        .then(function(response) {
            return response.json();
        })
        .then(function(data) {
            if (data.success) {
                showMessage(data.message, 'success');
                setTimeout(function() {
                    location.reload();
                }, 1000);
            } else {
                showMessage('Error: ' + data.message, 'danger');
                button.disabled = false;
                button.innerHTML = originalHtml;
            }
        })
        .catch(function(error) {
            showMessage('Error removing member. Please try again.', 'danger');
            button.disabled = false;
            button.innerHTML = originalHtml;
        });
    }
}

// Send message to team member
function sendMessageToMember(email) {
    window.location.href = 'mailto:' + email + '?subject=Project Collaboration&body=Hello, I wanted to discuss about our project...';
}

// Export team members to CSV
function exportTeamToCSV() {
    // Get all team member data from the table
    const rows = document.querySelectorAll('#teamMembersTable tbody tr');
    let csvContent = 'data:text/csv;charset=utf-8,';
    csvContent += 'Name,Email,Role,Joined Date\n';
    
    rows.forEach(function(row) {
        const cells = row.querySelectorAll('td');
        const name = cells[0].querySelector('strong').textContent.trim();
        const email = cells[1].querySelector('a').textContent.trim();
        const role = cells[2].querySelector('.badge').textContent.trim();
        const joinedDate = 'Just now'; // You can add joined date if available
        
        csvContent += '"' + name + '","' + email + '","' + role + '","' + joinedDate + '"\n';
    });
    
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement('a');
    link.setAttribute('href', encodedUri);
    link.setAttribute('download', 'team_members.csv');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    showMessage('Team members exported to CSV successfully!', 'success');
}

// Initialize tooltips
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Main initialization function
function initializeProjectDetails(projectId) {
    let searchTimeout;

    // Initialize tooltips
    initializeTooltips();

    // User search functionality
    const userSearch = document.getElementById('userSearch');
    const userSearchResults = document.getElementById('userSearchResults');

    if (userSearch) {
        userSearch.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            const searchTerm = this.value.trim();

            if (searchTerm.length < 2) {
                userSearchResults.style.display = 'none';
                return;
            }

            searchTimeout = setTimeout(function() {
                searchUsers(searchTerm, projectId);
            }, 300);
        });
    }

    // Send invitation functionality
    const sendInvitationForm = document.getElementById('sendInvitationForm');
    if (sendInvitationForm) {
        sendInvitationForm.addEventListener('submit', function(e) {
            e.preventDefault();
            sendInvitation(projectId);
        });
    }

    // Remove member functionality
    document.querySelectorAll('.remove-member').forEach(function(button) {
        button.addEventListener('click', function() {
            const userId = this.getAttribute('data-user-id');
            const userName = this.getAttribute('data-user-name');
            removeMember(projectId, userId, userName, this);
        });
    });

    // Send message functionality
    document.querySelectorAll('.send-message-btn').forEach(function(button) {
        button.addEventListener('click', function() {
            const email = this.getAttribute('data-email');
            sendMessageToMember(email);
        });
    });

    // Export CSV functionality
    const exportBtn = document.getElementById('exportTeamBtn');
    if (exportBtn) {
        exportBtn.addEventListener('click', exportTeamToCSV);
    }

    // Close search results when clicking outside
    document.addEventListener('click', function(e) {
        if (userSearch && userSearchResults && !userSearch.contains(e.target) && !userSearchResults.contains(e.target)) {
            userSearchResults.style.display = 'none';
        }
    });

    // Auto-focus on search input
    if (userSearch) {
        userSearch.focus();
    }
}

// Add this to make functions available globally
window.sendMessageToMember = sendMessageToMember;
window.exportTeamToCSV = exportTeamToCSV;