class ContactsTableManager {
    constructor() {
        this.tableBody = document.querySelector('#contactsTable tbody');
        this.originalRows = [];
        this.filteredRows = [];
        this.sortColumn = '';
        this.sortDirection = 'asc';

        this.init();
    }

    init() {
        if (!this.tableBody) return;

        this.originalRows = Array.from(this.tableBody.querySelectorAll('tr'));
        this.filteredRows = [...this.originalRows];

        this.bindEvents();
    }

    bindEvents() {
        this.bindFilterEvents();

        this.bindSortEvents();

        this.bindEditingEvents();

        this.bindDeleteEvents();
    }

    bindFilterEvents() {
        const nameFilter = document.getElementById('nameFilter');
        const marriedFilter = document.getElementById('marriedFilter');
        const minSalary = document.getElementById('minSalary');
        const maxSalary = document.getElementById('maxSalary');
        const clearFilters = document.getElementById('clearFilters');

        if (nameFilter) {
            nameFilter.addEventListener('input', this.debounce(() => this.applyFilters(), 300));
        }
        if (marriedFilter) {
            marriedFilter.addEventListener('change', () => this.applyFilters());
        }
        if (minSalary) {
            minSalary.addEventListener('input', this.debounce(() => this.applyFilters(), 300));
        }
        if (maxSalary) {
            maxSalary.addEventListener('input', this.debounce(() => this.applyFilters(), 300));
        }
        if (clearFilters) {
            clearFilters.addEventListener('click', () => this.clearAllFilters());
        }
    }

    bindSortEvents() {
        document.querySelectorAll('.sortable').forEach(header => {
            header.addEventListener('click', () => {
                const column = header.dataset.column;
                this.sortTable(column);
            });
        });
    }

    bindEditingEvents() {
        this.tableBody.addEventListener('click', (e) => {
            const row = e.target.closest('tr');
            if (!row) return;

            if (e.target.closest('.edit-btn')) {
                this.enableRowEditing(row);
            } else if (e.target.closest('.save-btn')) {
                this.saveRowChanges(row);
            } else if (e.target.closest('.cancel-btn')) {
                this.cancelRowEditing(row);
            }
        });
    }

    bindDeleteEvents() {
        this.tableBody.addEventListener('click', (e) => {
            if (e.target.closest('.delete-btn')) {
                const row = e.target.closest('tr');
                this.deleteContact(row);
            }
        });
    }

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    applyFilters() {
        const nameValue = document.getElementById('nameFilter')?.value.toLowerCase() || '';
        const marriedValue = document.getElementById('marriedFilter')?.value || '';
        const minSalaryValue = parseFloat(document.getElementById('minSalary')?.value) || 0;
        const maxSalaryValue = parseFloat(document.getElementById('maxSalary')?.value) || Number.MAX_SAFE_INTEGER;

        this.filteredRows = this.originalRows.filter(row => {
            const nameCell = row.querySelector('[data-field="name"]');
            const marriedCell = row.querySelector('[data-field="married"]');
            const salaryCell = row.querySelector('[data-field="salary"]');

            const name = nameCell?.dataset.original.toLowerCase() || '';
            const married = marriedCell?.dataset.original || '';
            const salary = parseFloat(salaryCell?.dataset.original) || 0;

            const nameMatch = !nameValue || name.includes(nameValue);
            const marriedMatch = !marriedValue || married === marriedValue;
            const salaryMatch = salary >= minSalaryValue && salary <= maxSalaryValue;

            return nameMatch && marriedMatch && salaryMatch;
        });

        this.renderFilteredRows();
        this.updateCounters();
    }

    clearAllFilters() {
        document.getElementById('nameFilter').value = '';
        document.getElementById('marriedFilter').value = '';
        document.getElementById('minSalary').value = '';
        document.getElementById('maxSalary').value = '';

        this.filteredRows = [...this.originalRows];
        this.renderFilteredRows();
        this.updateCounters();
    }

    sortTable(column) {
        if (this.sortColumn === column) {
            this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortColumn = column;
            this.sortDirection = 'asc';
        }

        this.filteredRows.sort((a, b) => {
            let valueA = this.getCellValue(a, column);
            let valueB = this.getCellValue(b, column);

            if (column === 'dateOfBirth') {
                valueA = new Date(valueA);
                valueB = new Date(valueB);
            } else if (column === 'salary') {
                valueA = parseFloat(valueA) || 0;
                valueB = parseFloat(valueB) || 0;
            } else if (column === 'married') {
                valueA = valueA === 'true';
                valueB = valueB === 'true';
            } else {
                valueA = valueA.toLowerCase();
                valueB = valueB.toLowerCase();
            }

            let result = 0;
            if (valueA < valueB) result = -1;
            else if (valueA > valueB) result = 1;

            return this.sortDirection === 'asc' ? result : -result;
        });

        this.updateSortIcons();
        this.renderFilteredRows();
    }

    getCellValue(row, column) {
        const cell = row.querySelector(`[data-field="${column}"]`);
        return cell?.dataset.original || cell?.textContent || '';
    }

    updateSortIcons() {
        document.querySelectorAll('.sortable i').forEach(icon => {
            icon.className = 'fas fa-sort';
        });

        if (this.sortColumn) {
            const currentHeader = document.querySelector(`[data-column="${this.sortColumn}"] i`);
            if (currentHeader) {
                currentHeader.className = this.sortDirection === 'asc'
                    ? 'fas fa-sort-up'
                    : 'fas fa-sort-down';
            }
        }
    }

    renderFilteredRows() {
        this.originalRows.forEach(row => {
            row.style.display = 'none';
        });

        this.filteredRows.forEach(row => {
            row.style.display = '';
        });

        const fragment = document.createDocumentFragment();
        this.filteredRows.forEach(row => {
            fragment.appendChild(row);
        });
        this.tableBody.appendChild(fragment);
    }

    updateCounters() {
        const totalCount = document.getElementById('totalCount');
        const visibleCount = document.getElementById('visibleCount');

        if (totalCount) totalCount.textContent = this.originalRows.length;
        if (visibleCount) visibleCount.textContent = this.filteredRows.length;
    }

    enableRowEditing(row) {
        if (row.classList.contains('editing')) return;

        row.classList.add('editing');
        const editableCells = row.querySelectorAll('.editable');

        editableCells.forEach(cell => {
            const field = cell.dataset.field;
            const originalValue = cell.dataset.original;
            let inputElement;

            switch (field) {
                case 'name':
                case 'phone':
                    inputElement = this.createTextInput(originalValue);
                    break;
                case 'dateOfBirth':
                    inputElement = this.createDateInput(originalValue);
                    break;
                case 'married':
                    inputElement = this.createBooleanSelect(originalValue);
                    break;
                case 'salary':
                    inputElement = this.createNumberInput(originalValue);
                    break;
            }

            if (inputElement) {
                cell.innerHTML = '';
                cell.appendChild(inputElement);
                inputElement.focus();
            }
        });

        this.toggleEditButtons(row, true);
    }

    createTextInput(value) {
        const input = document.createElement('input');
        input.type = 'text';
        input.value = value;
        input.className = 'form-control form-control-sm';
        return input;
    }

    createDateInput(value) {
        const input = document.createElement('input');
        input.type = 'date';
        input.value = value;
        input.className = 'form-control form-control-sm';
        return input;
    }

    createBooleanSelect(value) {
        const select = document.createElement('select');
        select.className = 'form-select form-select-sm';
        select.innerHTML = `
            <option value="true" ${value === 'true' ? 'selected' : ''}>Yes</option>
            <option value="false" ${value === 'false' ? 'selected' : ''}>No</option>
        `;
        return select;
    }

    createNumberInput(value) {
        const input = document.createElement('input');
        input.type = 'number';
        input.step = '0.01';
        input.min = '0';
        input.value = value;
        input.className = 'form-control form-control-sm';
        return input;
    }

    async saveRowChanges(row) {
        const contactId = row.dataset.contactId;
        const updatedData = this.extractRowData(row);

        if (!this.validateRowData(updatedData)) {
            return;
        }

        try {
            this.setRowLoading(row, true);

            const response = await fetch(`/api/contacts/${contactId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify({
                    id: contactId,
                    ...updatedData
                })
            });

            if (response.ok) {
                const updatedContact = await response.json();
                this.updateRowDisplay(row, updatedContact);
                this.showNotification('Contact updated successfully!', 'success');
            } else {
                const errorData = await response.json();
                this.showNotification(errorData.message || 'Failed to update contact', 'error');
                this.cancelRowEditing(row);
            }
        } catch (error) {
            console.error('Error updating contact:', error);
            this.showNotification('An error occurred while updating the contact', 'error');
            this.cancelRowEditing(row);
        } finally {
            this.setRowLoading(row, false);
        }
    }

    extractRowData(row) {
        const data = {};
        const editableCells = row.querySelectorAll('.editable');

        editableCells.forEach(cell => {
            const field = cell.dataset.field;
            const input = cell.querySelector('input, select');

            if (input) {
                switch (field) {
                    case 'name':
                        data.name = input.value.trim();
                        break;
                    case 'dateOfBirth':
                        data.dateOfBirth = input.value;
                        break;
                    case 'married':
                        data.married = input.value === 'true';
                        break;
                    case 'phone':
                        data.phone = input.value.trim();
                        break;
                    case 'salary':
                        data.salary = parseFloat(input.value) || 0;
                        break;
                }
            }
        });

        return data;
    }

    validateRowData(data) {
        if (!data.name) {
            this.showNotification('Name is required', 'error');
            return false;
        }
        if (!data.phone) {
            this.showNotification('Phone is required', 'error');
            return false;
        }
        if (!data.dateOfBirth) {
            this.showNotification('Date of birth is required', 'error');
            return false;
        }
        if (data.salary <= 0) {
            this.showNotification('Salary must be greater than 0', 'error');
            return false;
        }
        return true;
    }

    updateRowDisplay(row, contactData) {
        const cells = row.querySelectorAll('.editable');

        cells.forEach(cell => {
            const field = cell.dataset.field;

            switch (field) {
                case 'name':
                    cell.textContent = contactData.name;
                    cell.dataset.original = contactData.name;
                    break;
                case 'dateOfBirth':
                    const date = new Date(contactData.dateOfBirth);
                    cell.textContent = date.toLocaleDateString('en-US', {
                        year: 'numeric', month: 'short', day: 'numeric'
                    });
                    cell.dataset.original = contactData.dateOfBirth.split('T')[0];
                    break;
                case 'married':
                    cell.textContent = contactData.married ? 'Yes' : 'No';
                    cell.dataset.original = contactData.married.toString().toLowerCase();
                    break;
                case 'phone':
                    cell.textContent = contactData.phone;
                    cell.dataset.original = contactData.phone;
                    break;
                case 'salary':
                    cell.textContent = new Intl.NumberFormat('en-US', {
                        style: 'currency',
                        currency: 'USD'
                    }).format(contactData.salary);
                    cell.dataset.original = contactData.salary.toString();
                    break;
            }
        });

        row.classList.remove('editing');
        this.toggleEditButtons(row, false);
    }

    cancelRowEditing(row) {
        const editableCells = row.querySelectorAll('.editable');

        editableCells.forEach(cell => {
            const field = cell.dataset.field;
            const originalValue = cell.dataset.original;

            switch (field) {
                case 'name':
                case 'phone':
                    cell.textContent = originalValue;
                    break;
                case 'dateOfBirth':
                    const date = new Date(originalValue);
                    cell.textContent = date.toLocaleDateString('en-US', {
                        year: 'numeric', month: 'short', day: 'numeric'
                    });
                    break;
                case 'married':
                    cell.textContent = originalValue === 'true' ? 'Yes' : 'No';
                    break;
                case 'salary':
                    cell.textContent = new Intl.NumberFormat('en-US', {
                        style: 'currency',
                        currency: 'USD'
                    }).format(parseFloat(originalValue));
                    break;
            }
        });

        row.classList.remove('editing');
        this.toggleEditButtons(row, false);
    }

    toggleEditButtons(row, isEditing) {
        const editBtn = row.querySelector('.edit-btn');
        const saveBtn = row.querySelector('.save-btn');
        const cancelBtn = row.querySelector('.cancel-btn');
        const viewEditBtn = row.querySelector('.view-edit-btn');
        const deleteBtn = row.querySelector('.delete-btn');

        if (isEditing) {
            editBtn?.classList.add('d-none');
            viewEditBtn?.classList.add('d-none');
            deleteBtn?.classList.add('d-none');
            saveBtn?.classList.remove('d-none');
            cancelBtn?.classList.remove('d-none');
        } else {
            editBtn?.classList.remove('d-none');
            viewEditBtn?.classList.remove('d-none');
            deleteBtn?.classList.remove('d-none');
            saveBtn?.classList.add('d-none');
            cancelBtn?.classList.add('d-none');
        }
    }

    setRowLoading(row, loading) {
        const buttons = row.querySelectorAll('button');
        buttons.forEach(btn => {
            btn.disabled = loading;
        });

        if (loading) {
            row.style.opacity = '0.6';
        } else {
            row.style.opacity = '1';
        }
    }

    async deleteContact(row) {
        const contactId = row.dataset.contactId;
        const nameCell = row.querySelector('[data-field="name"]');
        const contactName = nameCell?.dataset.original || 'this contact';

        if (!confirm(`Are you sure you want to delete ${contactName}?`)) {
            return
        }

        try {
            this.setRowLoading(row, true);

            const response = await fetch(`/api/contacts/${contactId}`, {
                method: 'DELETE',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            });

            if (response.ok) {
                row.remove();
                this.originalRows = this.originalRows.filter(r => r !== row);
                this.filteredRows = this.filteredRows.filter(r => r !== row);
                this.updateCounters();
                this.showNotification('Contact deleted successfully!', 'success');
            } else {
                const errorData = await response.json();
                this.showNotification(errorData.message || 'Failed to delete contact', 'error');
            }
        } catch (error) {
            console.error('Error deleting contact:', error);
            this.showNotification('An error occurred while deleting the contact', 'error');
        } finally {
            this.setRowLoading(row, false);
        }
    }

    showNotification(message, type) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert ${alertClass} alert-dismissible fade show`;
        alertDiv.role = 'alert';
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        document.body.prepend(alertDiv);

        setTimeout(() => {
            alertDiv.remove();
        }, 3000);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new ContactsTableManager();
});