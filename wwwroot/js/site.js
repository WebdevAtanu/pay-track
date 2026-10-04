//==================================== data table =====================================
function format(data) {
    // Calculate attendance statistics
    const totalAttendance = data.EmployeeAttendances.length;
    const presentCount = data.EmployeeAttendances.filter(a => a.Status === 'Present').length;
    const absentCount = data.EmployeeAttendances.filter(a => a.Status === 'Absent').length;
    const leaveCount = data.EmployeeAttendances.filter(a => a.Status === 'Leave').length;
    const attendanceRate = totalAttendance > 0 ? Math.round((presentCount / totalAttendance) * 100) : 0;

    // Calculate salary statistics
    const totalSalary = data.EmployeeSalaries.reduce((sum, s) => sum + (s.NetSalary || 0), 0);
    const pendingSalary = data.EmployeeSalaries.filter(s => s.Status === 'Pending').length;
    const paidSalary = data.EmployeeSalaries.filter(s => s.Status === 'Paid').length;

    // Status badge
    const statusBadge = data.IsActive
        ? '<span class="badge bg-success">Active</span>'
        : '<span class="badge bg-danger">Inactive</span>';

    // Department badge
    const deptBadge = data.EmployeeDepartment
        ? `<span class="badge bg-primary">${data.EmployeeDepartment.DeptName}</span>`
        : '<span class="badge bg-secondary">No Department</span>';

    // Attendance table rows
    let attendanceRows = "";
    if (data.EmployeeAttendances.length > 0) {
        data.EmployeeAttendances.slice(0, 5).forEach(a => {
            const statusClass = a.Status === 'Present' ? 'bg-success' :
                a.Status === 'Absent' ? 'bg-danger' :
                    a.Status === 'Leave' ? 'bg-warning' : 'bg-secondary';
            attendanceRows += `
                <tr>
                    <td>${a.Date ? a.Date.split('T')[0] : 'N/A'}</td>
                    <td><span class="badge ${statusClass}">${a.Status}</span></td>
                </tr>`;
        });
        if (data.EmployeeAttendances.length > 5) {
            attendanceRows += `<tr><td colspan="2" class="text-center text-muted">+${data.EmployeeAttendances.length - 5} more records</td></tr>`;
        }
    } else {
        attendanceRows = `<tr><td colspan="2" class="text-center text-muted">No attendance records</td></tr>`;
    }

    // Salary table rows
    let salaryRows = "";
    if (data.EmployeeSalaries && data.EmployeeSalaries.length > 0) {
        data.EmployeeSalaries.slice(0, 3).forEach(s => {
            const statusClass = s.Status === 'Paid' ? 'bg-success' : 'bg-warning';
            salaryRows += `
                <tr>
                    <td>${s.Month} ${s.Year}</td>
                    <td>$${s.NetSalary?.toFixed(2) || '0.00'}</td>
                    <td><span class="badge ${statusClass}">${s.Status}</span></td>
                </tr>`;
        });
        if (data.EmployeeSalaries.length > 3) {
            salaryRows += `<tr><td colspan="3" class="text-center text-muted">+${data.EmployeeSalaries.length - 3} more records</td></tr>`;
        }
    } else {
        salaryRows = `<tr><td colspan="3" class="text-center text-muted">No salary records</td></tr>`;
    }

    return `
        <div class="employee-details" style="padding: 20px; background: #f8f9fa; border-radius: 8px;">
            <!-- Header Section -->
            <div class="row mb-4">
                <div class="col-md-8">
                    <h5 class="mb-2">
                        <i class="bi bi-person-circle me-2"></i>${data.Name}
                    </h5>
                    <div class="d-flex gap-2 flex-wrap">
                        ${statusBadge}
                        ${deptBadge}
                    </div>
                </div>
                <div class="col-md-4 text-end">
                    <div class="d-flex gap-2 justify-content-end">
                        <a href="/Employee/Employee/Edit/${data.EmployeeId}" class="btn btn-sm btn-outline-primary">
                            <i class="bi bi-pencil"></i> Edit
                        </a>
                    </div>
                </div>
            </div>

            <!-- Contact Information -->
            <div class="row mb-4">
                <div class="col-md-6">
                    <h6 class="mb-2">
                        <i class="bi bi-telephone me-2"></i>Contact Information
                    </h6>
                    <p class="mb-1"><strong>Email:</strong> ${data.Email || 'N/A'}</p>
                    <p class="mb-0"><strong>Phone:</strong> ${data.Phone || 'N/A'}</p>
                </div>
                <div class="col-md-6">
                    <h6 class="mb-2">
                        <i class="bi bi-calendar-check me-2"></i>Employment Details
                    </h6>
                    <p class="mb-0"><strong>Joining Date:</strong> ${data.JoiningDate ? data.JoiningDate.split('T')[0] : 'N/A'}</p>
                </div>
            </div>

            <!-- Statistics -->
            <div class="row mb-4">
                <div class="col-md-3">
                    <div class="text-center border">
                        <h6 class="text-muted mb-2">Attendance Rate</h6>
                        <h4 class="text-primary mb-0">${attendanceRate}%</h4>
                        <small class="text-muted">${presentCount}/${totalAttendance} days</small>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="text-center border">
                        <h6 class="text-muted mb-2">Present</h6>
                        <h4 class="text-success mb-0">${presentCount}</h4>
                        <small class="text-muted">Days</small>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="text-center border">
                        <h6 class="text-muted mb-2">Absent</h6>
                        <h4 class="text-danger mb-0">${absentCount}</h4>
                        <small class="text-muted">Days</small>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="text-center border">
                        <h6 class="text-muted mb-2">Total Salary</h6>
                        <h4 class="text-info mb-0">${totalSalary.toFixed(2)}</h4>
                        <small class="text-muted">${paidSalary} Paid, ${pendingSalary} Pending</small>
                    </div>
                </div>
            </div>

            <!-- Detailed Tables -->
            <div class="row">
                <div class="col-md-6">
                    <h6 class="mb-2">
                        <i class="bi bi-calendar-event me-2"></i>Recent Attendance
                    </h6>
                    <table class="table table-sm table-hover">
                        <thead class="table-light">
                            <tr>
                                <th>Date</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody >${attendanceRows}</tbody>
                    </table>
                </div>
                <div class="col-md-6">
                    <h6 class="mb-2">
                        <i class="bi bi-currency-rupee me-2"></i>Recent Salary
                    </h6>
                    <table class="table table-sm table-hover">
                        <thead class="table-light">
                            <tr>
                                <th>Period</th>
                                <th>Amount</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>${salaryRows}</tbody>
                    </table>
                </div>
            </div>
        </div>
    `;
}


$(document).ready(function () {

    var table = $('#employeeTable').DataTable({
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search employees..."
        },
        columnDefs: [
            { orderable: false, targets: 0 },
            { width: "30px", targets: 0 }
        ]
    });

    $('#employeeTable tbody').on('click', 'td.details-control', function () {

        var tr = $(this).closest('tr');
        var row = table.row(tr);

        let jsonData = $(this).data('json');

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
            $(this).find('i').removeClass('bi-chevron-down').addClass('bi-chevron-right');
        }
        else {
            row.child(format(jsonData)).show();
            tr.addClass('shown');
            $(this).find('i').removeClass('bi-chevron-right').addClass('bi-chevron-down');
        }
    });

});

//==================================== employee delete modal =====================================

var empDeleteModal = document.getElementById('employeeDeleteModal');
if (empDeleteModal) {
    empDeleteModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;
        var id = button.getAttribute('data-id');
        var empName = button.getAttribute('data-name');
        document.getElementById('empNameText').innerText = empName;

        var form = document.getElementById('empDeleteForm');
        form.action = '/Employee/Employee/Delete/' + id;
    });
}

//==================================== attendance search function =====================================

function clearSearch() {
    let input = document.getElementById("searchInput");
    input.value = "";
    input.dispatchEvent(new Event("keyup")); //It fakes a key press event
}

//==================================== department delete modal =====================================

var deptDeleteModal = document.getElementById('deptDeleteModal');

if (deptDeleteModal) {
    deptDeleteModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;
        var id = button.getAttribute('data-id');
        var deptName = button.getAttribute('data-name');
        document.getElementById('deptNameText').innerText = deptName;

        var form = document.getElementById('deptDeleteForm');
        form.action = '/Admin/Department/Delete/' + id;
    });
}