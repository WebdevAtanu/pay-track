function toggleForm() {
    const loginForm = document.getElementById("loginForm");
    const signupForm = document.getElementById("signupForm");
    const formTitle = document.getElementById("formTitle");
    const toggleText = document.getElementById("toggleText");

    loginForm.classList.toggle("active");
    signupForm.classList.toggle("active");

    if (loginForm.classList.contains("active")) {
        formTitle.innerText = "Login";
        toggleText.innerHTML = `Don't have an account?
            <span class="toggle-link" onclick="toggleForm()">Sign Up</span>`;
    } else {
        formTitle.innerText = "Sign Up";
        toggleText.innerHTML = `Already have an account?
            <span class="toggle-link" onclick="toggleForm()">Login</span>`;
    }
}

//==================================== data table =====================================
function format(data) {

    let dept = data.EmployeeDepartment
        ? `<p><strong>Department:</strong> ${data.EmployeeDepartment.DeptName}</p>`
        : `<p>No Department Found</p>`;

    let attendanceRows = "";
    if (data.EmployeeAttendances.length > 0) {
        data.EmployeeAttendances.forEach(a => {
            attendanceRows += `
                    <tr>
                        <td>${a.Date ? a.Date.split('T')[0] : ''}</td>
                        <td>${a.Status}</td>
                    </tr>`;
        });
    }

    else {
        attendanceRows = `
                    <tr>
                        <td>No Date</td>
                        <td>No Status</td>
                    </tr>`;
    }

    let salaryRows = "";
    if (data.EmployeeSalaries) {
        data.EmployeeSalaries.forEach(s => {
            salaryRows += `
                    <tr>
                        <td>${s.Month}</td>
                        <td>${s.Year}</td>
                        <td>${s.NetSalary}</td>
                        <td>${s.Status}</td>
                    </tr>`;
        });
    }

    return `
            <div style="padding:8px;">
                <h6>Department</h6>
                ${dept}

                <h6>Attendance</h6>
                <table class="table table-sm">
                    <thead>
                        <tr><th>Date</th><th>Status</th></tr>
                    </thead>
                    <tbody>${attendanceRows}</tbody>
                </table>

                <h6>Salary</h6>
                <table class="table table-sm">
                    <thead>
                        <tr><th>Month</th><th>Year</th><th>Net</th><th>Status</th></tr>
                    </thead>
                    <tbody>${salaryRows}</tbody>
                </table>
            </div>
        `;
}


$(document).ready(function () {

    var table = $('#employeeTable').DataTable();

    $('#employeeTable tbody').on('click', 'td.details-control', function () {

        var tr = $(this).closest('tr');
        var row = table.row(tr);

        let jsonData = $(this).data('json');

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            row.child(format(jsonData)).show();
            tr.addClass('shown');
        }
    });

});

//==================================== confirm delete modal =====================================

var deleteModal = document.getElementById('deleteModal');

deleteModal.addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    var id = button.getAttribute('data-id');

    var form = document.getElementById('deleteForm');
    form.action = '/Employee/Employee/Delete/' + id;
});