const API_BASE_URL = '/api';

let currentDeviceId = null;
let currentEditDeviceId = null;

// Initialize app
document.addEventListener('DOMContentLoaded', () => {
    loadDevices();
    setupEventListeners();
});

function setupEventListeners() {
    document.getElementById('device-form').addEventListener('submit', handleDeviceSubmit);
    document.getElementById('heartbeat-form').addEventListener('submit', handleHeartbeatSubmit);
}

// Tab management
function showTab(tabName, clickedButton) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
    
    document.getElementById(`${tabName}-tab`).classList.add('active');
    if (clickedButton) {
        clickedButton.classList.add('active');
    } else {
        // Find and activate the corresponding button
        document.querySelectorAll('.tab-button').forEach(btn => {
            if (btn.textContent.trim() === (tabName === 'devices' ? 'Devices' : 'All Heartbeats')) {
                btn.classList.add('active');
            }
        });
    }
    
    if (tabName === 'devices') {
        loadDevices();
    } else if (tabName === 'heartbeats') {
        loadAllHeartbeats();
    }
}

// Device functions
async function loadDevices() {
    try {
        const response = await fetch(`${API_BASE_URL}/devices`);
        if (!response.ok) throw new Error('Failed to load devices');
        
        const devices = await response.json();
        displayDevices(devices);
    } catch (error) {
        console.error('Error loading devices:', error);
        document.getElementById('devices-list').innerHTML = 
            '<div class="empty-state"><div class="empty-state-icon">⚠️</div><p>Error loading devices. Please try again.</p></div>';
    }
}

function displayDevices(devices) {
    const container = document.getElementById('devices-list');
    
    if (devices.length === 0) {
        container.innerHTML = '<div class="empty-state"><div class="empty-state-icon">📱</div><p>No devices found. Add your first device!</p></div>';
        return;
    }
    
    container.innerHTML = devices.map(device => {
        // Validate device.id
        if (!device.id) {
            console.error('Device missing ID:', device);
            return '';
        }
        const deviceId = device.id;
        return `
        <div class="card">
            <div class="card-header">
                <div>
                    <div class="card-title">${escapeHtml(device.name)}</div>
                    <div class="card-subtitle">${escapeHtml(device.ownerEmail)}</div>
                </div>
            </div>
            <div class="card-body">
                <div class="card-info">
                    <div class="info-item">
                        <span class="info-label">Created:</span>
                        <span class="info-value">${formatDate(device.createdAt)}</span>
                    </div>
                </div>
            </div>
            <div class="card-actions">
                <button class="btn btn-primary btn-small" onclick="viewDeviceHeartbeats('${deviceId}')" data-device-id="${deviceId}">
                    View Heartbeats
                </button>
                <button class="btn btn-secondary btn-small" onclick="editDevice('${deviceId}')" data-device-id="${deviceId}">
                    Edit
                </button>
                <button class="btn btn-danger btn-small" onclick="deleteDevice('${deviceId}')" data-device-id="${deviceId}">
                    Delete
                </button>
            </div>
        </div>
    `;
    }).filter(html => html !== '').join('');
}

async function viewDeviceHeartbeats(deviceId) {
    // Validate deviceId before proceeding
    if (!deviceId || deviceId === 'undefined' || deviceId === 'null') {
        console.error('Invalid deviceId passed to viewDeviceHeartbeats:', deviceId);
        alert('Invalid device ID. Please try again.');
        return;
    }
    
    console.log('Loading heartbeats for device:', deviceId);
    currentDeviceId = deviceId;
    try {
        const response = await fetch(`${API_BASE_URL}/devices/${deviceId}/heartbeats`);
        if (!response.ok) throw new Error('Failed to load heartbeats');
        
        const heartbeats = await response.json();
        showDeviceHeartbeatsModal(deviceId, heartbeats);
    } catch (error) {
        console.error('Error loading heartbeats:', error);
        alert('Error loading heartbeats. Please try again.');
    }
}

function showDeviceHeartbeatsModal(deviceId, heartbeats) {
    // Validate deviceId
    if (!deviceId || deviceId === 'undefined' || deviceId === 'null') {
        console.error('Invalid deviceId:', deviceId);
        alert('Invalid device ID. Please try again.');
        return;
    }
    
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.style.display = 'block';
    modal.setAttribute('data-device-id', deviceId);
    modal.innerHTML = `
        <div class="modal-content" style="max-width: 800px;">
            <span class="close" onclick="this.closest('.modal').remove()">&times;</span>
            <h2>Device Heartbeats</h2>
            <div style="margin-bottom: 1rem;">
                <button class="btn btn-primary" id="add-heartbeat-btn" data-device-id="${deviceId}">+ Add Heartbeat</button>
            </div>
            <div id="device-heartbeats-list" class="cards-container" style="grid-template-columns: 1fr;">
                ${heartbeats.length === 0 
                    ? '<div class="empty-state"><p>No heartbeats found for this device.</p></div>'
                    : heartbeats.map(hb => `
                        <div class="card heartbeat-card status-${hb.status.toLowerCase()}">
                            <div class="card-header">
                                <div>
                                    <div class="card-title">
                                        <span class="status-badge status-${hb.status.toLowerCase()}">${hb.status}</span>
                                    </div>
                                    <div class="card-subtitle timestamp">${formatDate(hb.timestamp)}</div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="card-info">
                                    <div class="info-item">
                                        <span class="info-label">Message:</span>
                                        <span class="info-value">${escapeHtml(hb.message || 'N/A')}</span>
                                    </div>
                                </div>
                            </div>
                            <div class="card-actions">
                                <button class="btn btn-danger btn-small" onclick="deleteHeartbeat('${hb.id}', '${deviceId}')">
                                    Delete
                                </button>
                            </div>
                        </div>
                    `).join('')
                }
            </div>
        </div>
    `;
    document.body.appendChild(modal);
    
    // Add event listener for the button instead of inline onclick
    const addBtn = modal.querySelector('#add-heartbeat-btn');
    if (addBtn) {
        // Store deviceId in closure to ensure it's available
        const storedDeviceId = deviceId;
        addBtn.addEventListener('click', () => {
            const btnDeviceId = addBtn.getAttribute('data-device-id') || storedDeviceId || modal.getAttribute('data-device-id');
            console.log('Add heartbeat button clicked. DeviceId from:', {
                'button attribute': addBtn.getAttribute('data-device-id'),
                'storedDeviceId': storedDeviceId,
                'modal attribute': modal.getAttribute('data-device-id'),
                'final deviceId': btnDeviceId
            });
            if (!btnDeviceId || btnDeviceId === 'undefined' || btnDeviceId === 'null') {
                console.error('No valid deviceId found when clicking add heartbeat button');
                alert('Device ID is missing. Please try again.');
                return;
            }
            showHeartbeatModal(btnDeviceId);
        });
    } else {
        console.error('Add heartbeat button not found in modal');
    }
}

function showDeviceModal(deviceId = null) {
    currentEditDeviceId = deviceId;
    const modal = document.getElementById('device-modal');
    const form = document.getElementById('device-form');
    const title = document.getElementById('modal-title');
    
    if (deviceId) {
        title.textContent = 'Edit Device';
        loadDeviceForEdit(deviceId);
    } else {
        title.textContent = 'Add Device';
        form.reset();
        clearErrors();
    }
    
    modal.style.display = 'block';
}

function closeDeviceModal() {
    document.getElementById('device-modal').style.display = 'none';
    document.getElementById('device-form').reset();
    clearErrors();
    currentEditDeviceId = null;
}

async function loadDeviceForEdit(deviceId) {
    try {
        const response = await fetch(`${API_BASE_URL}/devices/${deviceId}`);
        if (!response.ok) throw new Error('Failed to load device');
        
        const device = await response.json();
        document.getElementById('device-name').value = device.name;
        document.getElementById('device-email').value = device.ownerEmail;
    } catch (error) {
        console.error('Error loading device:', error);
        alert('Error loading device. Please try again.');
    }
}

async function handleDeviceSubmit(e) {
    e.preventDefault();
    clearErrors();
    
    const name = document.getElementById('device-name').value.trim();
    const email = document.getElementById('device-email').value.trim();
    
    if (!name || !email) {
        showError('device-name-error', 'Name is required');
        showError('device-email-error', 'Email is required');
        return;
    }
    
    if (!isValidEmail(email)) {
        showError('device-email-error', 'Invalid email address');
        return;
    }
    
    try {
        const url = currentEditDeviceId 
            ? `${API_BASE_URL}/devices/${currentEditDeviceId}`
            : `${API_BASE_URL}/devices`;
        
        const method = currentEditDeviceId ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method,
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name, ownerEmail: email })
        });
        
        if (!response.ok) {
            const errorData = await response.json();
            // Handle validation errors
            if (errorData.errors) {
                // If errors is an array
                if (Array.isArray(errorData.errors)) {
                    throw new Error(errorData.errors.join(', ') || 'Validation failed');
                }
                // If errors is an object (ModelState format)
                const errorMessages = [];
                for (const key in errorData.errors) {
                    if (errorData.errors[key]) {
                        if (Array.isArray(errorData.errors[key])) {
                            errorData.errors[key].forEach(msg => errorMessages.push(msg));
                        } else {
                            errorMessages.push(errorData.errors[key]);
                        }
                    }
                }
                throw new Error(errorMessages.join(', ') || 'Validation failed');
            }
            throw new Error(errorData.message || errorData.title || 'Failed to save device');
        }
        
        closeDeviceModal();
        loadDevices();
    } catch (error) {
        console.error('Error saving device:', error);
        console.error('Full error:', error);
        
        // Show more detailed error message
        let errorMessage = 'Failed to save device';
        if (error.message) {
            errorMessage = error.message;
        } else if (typeof error === 'string') {
            errorMessage = error;
        }
        
        alert('Error saving device: ' + errorMessage);
    }
}

async function editDevice(deviceId) {
    showDeviceModal(deviceId);
}

async function deleteDevice(deviceId) {
    if (!confirm('Are you sure you want to delete this device? All associated heartbeats will also be deleted.')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/devices/${deviceId}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) throw new Error('Failed to delete device');
        
        loadDevices();
    } catch (error) {
        console.error('Error deleting device:', error);
        alert('Error deleting device. Please try again.');
    }
}

// Heartbeat functions
function showHeartbeatModal(deviceId) {
    // Check if deviceId is valid (not undefined, null, or the string "undefined")
    if (!deviceId || deviceId === 'undefined' || deviceId === 'null' || deviceId === undefined) {
        console.error('Invalid deviceId passed to showHeartbeatModal:', deviceId, typeof deviceId);
        alert('Device ID is required to add a heartbeat. Please select a device first.');
        return;
    }
    
    // Convert to string to ensure consistency
    const deviceIdStr = String(deviceId);
    console.log('Opening heartbeat modal for device:', deviceIdStr, 'Type:', typeof deviceIdStr);
    
    currentDeviceId = deviceIdStr;
    const modal = document.getElementById('heartbeat-modal');
    const form = document.getElementById('heartbeat-form');
    
    // Store device ID in form data attribute as backup (BEFORE reset)
    form.setAttribute('data-device-id', deviceIdStr);
    
    // Also store in modal for extra backup
    modal.setAttribute('data-device-id', deviceIdStr);
    
    // Reset form AFTER setting the attribute
    form.reset();
    
    // Re-set the attribute after reset (just to be safe, though reset shouldn't affect attributes)
    form.setAttribute('data-device-id', deviceIdStr);
    
    clearHeartbeatErrors();
    modal.style.display = 'block';
    
    // Verify the attribute was set correctly
    console.log('Device ID stored:', {
        'currentDeviceId': currentDeviceId,
        'form attribute': form.getAttribute('data-device-id'),
        'modal attribute': modal.getAttribute('data-device-id')
    });
}

function closeHeartbeatModal() {
    document.getElementById('heartbeat-modal').style.display = 'none';
    const form = document.getElementById('heartbeat-form');
    form.reset();
    form.removeAttribute('data-device-id');
    clearHeartbeatErrors();
    currentDeviceId = null;
}

async function handleHeartbeatSubmit(e) {
    e.preventDefault();
    clearHeartbeatErrors();
    
    // Get device ID from multiple sources (priority order)
    const form = document.getElementById('heartbeat-form');
    let deviceId = currentDeviceId || 
                   form.getAttribute('data-device-id') ||
                   (form.closest('.modal') && form.closest('.modal').getAttribute('data-device-id'));
    
    // Also check if there's a device heartbeats modal open
    const deviceHeartbeatsModal = document.querySelector('.modal[data-device-id]');
    if (!deviceId && deviceHeartbeatsModal) {
        deviceId = deviceHeartbeatsModal.getAttribute('data-device-id');
    }
    
    // Validate deviceId - check for undefined, null, or string "undefined"
    if (!deviceId || deviceId === 'undefined' || deviceId === 'null' || deviceId === undefined) {
        console.error('Device ID is missing or invalid. Sources checked:', {
            'currentDeviceId': currentDeviceId,
            'form data-device-id': form.getAttribute('data-device-id'),
            'modal data-device-id': deviceHeartbeatsModal ? deviceHeartbeatsModal.getAttribute('data-device-id') : null,
            'final deviceId': deviceId
        });
        alert('Device ID is missing. Please select a device first.');
        closeHeartbeatModal();
        return;
    }
    
    console.log('Submitting heartbeat for device:', deviceId, 'Sources:', {
        'currentDeviceId': currentDeviceId,
        'form data-device-id': form.getAttribute('data-device-id'),
        'modal data-device-id': deviceHeartbeatsModal ? deviceHeartbeatsModal.getAttribute('data-device-id') : null
    });
    
    const status = document.getElementById('heartbeat-status').value;
    const message = document.getElementById('heartbeat-message').value.trim();
    
    if (!status) {
        showError('heartbeat-status-error', 'Status is required');
        return;
    }
    
    if (!['OK', 'WARN', 'ERROR'].includes(status)) {
        showError('heartbeat-status-error', 'Status must be OK, WARN, or ERROR');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/devices/${deviceId}/heartbeats`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ status, message })
        });
        
        if (!response.ok) {
            const errorData = await response.json();
            // Handle validation errors
            if (errorData.errors) {
                // If errors is an array
                if (Array.isArray(errorData.errors)) {
                    throw new Error(errorData.errors.join(', ') || 'Validation failed');
                }
                // If errors is an object (ModelState format)
                const errorMessages = [];
                for (const key in errorData.errors) {
                    if (errorData.errors[key]) {
                        if (Array.isArray(errorData.errors[key])) {
                            errorData.errors[key].forEach(msg => errorMessages.push(msg));
                        } else {
                            errorMessages.push(errorData.errors[key]);
                        }
                    }
                }
                throw new Error(errorMessages.join(', ') || 'Validation failed');
            }
            throw new Error(errorData.message || errorData.title || 'Failed to save heartbeat');
        }
        
        closeHeartbeatModal();
        
        // Reload if we're in the heartbeats view
        const heartbeatsTab = document.getElementById('heartbeats-tab');
        if (heartbeatsTab.classList.contains('active')) {
            loadAllHeartbeats();
        } else {
            // Reload device heartbeats modal if open
            const modal = document.querySelector('.modal .modal-content h2');
            if (modal && modal.textContent === 'Device Heartbeats') {
                viewDeviceHeartbeats(deviceId);
            }
        }
    } catch (error) {
        console.error('Error saving heartbeat:', error);
        alert('Error saving heartbeat: ' + error.message);
    }
}

async function loadAllHeartbeats() {
    try {
        // First get all devices, then get heartbeats for each
        const devicesResponse = await fetch(`${API_BASE_URL}/devices`);
        if (!devicesResponse.ok) throw new Error('Failed to load devices');
        
        const devices = await devicesResponse.json();
        const allHeartbeats = [];
        
        for (const device of devices) {
            const heartbeatsResponse = await fetch(`${API_BASE_URL}/devices/${device.id}/heartbeats`);
            if (heartbeatsResponse.ok) {
                const heartbeats = await heartbeatsResponse.json();
                heartbeats.forEach(hb => {
                    allHeartbeats.push({ ...hb, deviceName: device.name, deviceEmail: device.ownerEmail });
                });
            }
        }
        
        // Sort by timestamp descending
        allHeartbeats.sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp));
        
        displayAllHeartbeats(allHeartbeats);
    } catch (error) {
        console.error('Error loading heartbeats:', error);
        document.getElementById('heartbeats-list').innerHTML = 
            '<div class="empty-state"><div class="empty-state-icon">⚠️</div><p>Error loading heartbeats. Please try again.</p></div>';
    }
}

function displayAllHeartbeats(heartbeats) {
    const container = document.getElementById('heartbeats-list');
    
    if (heartbeats.length === 0) {
        container.innerHTML = '<div class="empty-state"><div class="empty-state-icon">💓</div><p>No heartbeats found.</p></div>';
        return;
    }
    
    container.innerHTML = heartbeats.map(hb => `
        <div class="card heartbeat-card status-${hb.status.toLowerCase()}">
            <div class="card-header">
                <div>
                    <div class="card-title">
                        <span class="status-badge status-${hb.status.toLowerCase()}">${hb.status}</span>
                        <span style="margin-left: 0.5rem; font-weight: normal; color: var(--text-secondary);">
                            - ${escapeHtml(hb.deviceName)}
                        </span>
                    </div>
                    <div class="card-subtitle timestamp">${formatDate(hb.timestamp)}</div>
                </div>
            </div>
            <div class="card-body">
                <div class="card-info">
                    <div class="info-item">
                        <span class="info-label">Device:</span>
                        <span class="info-value">${escapeHtml(hb.deviceName)} (${escapeHtml(hb.deviceEmail)})</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Message:</span>
                        <span class="info-value">${escapeHtml(hb.message || 'N/A')}</span>
                    </div>
                </div>
            </div>
            <div class="card-actions">
                <button class="btn btn-danger btn-small" onclick="deleteHeartbeat('${hb.id}')">
                    Delete
                </button>
            </div>
        </div>
    `).join('');
}

async function deleteHeartbeat(heartbeatId, deviceId = null) {
    if (!confirm('Are you sure you want to delete this heartbeat?')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/heartbeats/${heartbeatId}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) throw new Error('Failed to delete heartbeat');
        
        // Reload appropriate view
        if (deviceId) {
            viewDeviceHeartbeats(deviceId);
        } else {
            loadAllHeartbeats();
        }
    } catch (error) {
        console.error('Error deleting heartbeat:', error);
        alert('Error deleting heartbeat. Please try again.');
    }
}

// Utility functions
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString('ro-RO', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

function isValidEmail(email) {
    // Match server-side regex: ^[^@\s]+@[^@\s]+\.[^@\s]+$
    return /^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email);
}

function showError(elementId, message) {
    const errorElement = document.getElementById(elementId);
    if (errorElement) {
        errorElement.textContent = message;
    }
}

function clearErrors() {
    document.querySelectorAll('.error').forEach(el => el.textContent = '');
}

function clearHeartbeatErrors() {
    document.getElementById('heartbeat-status-error').textContent = '';
}

// Close modals when clicking outside
window.onclick = function(event) {
    const deviceModal = document.getElementById('device-modal');
    const heartbeatModal = document.getElementById('heartbeat-modal');
    
    if (event.target === deviceModal) {
        closeDeviceModal();
    }
    if (event.target === heartbeatModal) {
        closeHeartbeatModal();
    }
}

