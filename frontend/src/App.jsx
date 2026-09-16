import { useEffect, useState } from "react";
import api from "./api";
import "./App.css";


function Dashboard({ onLogout }) {
    const [employees, setEmployees] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [departments, setDepartments] = useState([]);
    const [totalEmployees, setTotalEmployees] = useState(0);
    const [pageNumber, setPageNumber] = useState(1);
    const pageSize = 5;

    const [departmentName, setDepartmentName] = useState("");
    const [editingDepartmentId, setEditingDepartmentId] = useState(null);

    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);
    const [editingEmployeeId, setEditingEmployeeId] = useState(null);
    const [error, setError] = useState("");

  

    const [form, setForm] = useState({
        fullName: "",
        emailAddress: "",
        phoneNumber: "",
        salary: "",
        dateOfJoining: "",
        departmentId: "",
    });

    const loadDashboardData = async () => {
        try {
            setError("");

            const [employeeResponse, departmentResponse] = await Promise.all([
                api.get("/Employee", {
                    params: {
                        pageNumber: pageNumber,
                        pageSize: pageSize,
                        sortBy: "name",
                        search:searchTerm,
                    },
                }),
                api.get("/Department"),
            ]);

            setEmployees(employeeResponse.data.data);
            setTotalEmployees(employeeResponse.data.totalRecords);
            setDepartments(departmentResponse.data);
        } catch {
            setError("Could not load dashboard data.");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        const loadData = async () => {
            await loadDashboardData();
        };

        loadData();
    }, [searchTerm,pageNumber]);

    const handleInputChange = (event) => {
        setForm({
            ...form,
            [event.target.name]: event.target.value,
        });
    };

    const handleAddEmployee = async (event) => {
        event.preventDefault();

        setError("");
        setIsSaving(true);

        try {
            if (editingEmployeeId) {
              
                await api.put(`/Employee/${editingEmployeeId}`, {
                    fullName: form.fullName,
                    emailAddress: form.emailAddress,
                    phoneNumber: form.phoneNumber,
                    salary: Number(form.salary),
                    dateOfJoining: form.dateOfJoining,
                    departmentId: Number(form.departmentId),
                });
            } else {
                await api.post("/Employee", {
                    ...form,
                    salary: Number(form.salary),
                    departmentId: Number(form.departmentId),
                });
            }

            setForm({
                fullName: "",
                emailAddress: "",
                phoneNumber: "",
                salary: "",
                dateOfJoining: "",
                departmentId: "",
            });

            setEditingEmployeeId(null);

           
            await loadDashboardData();

        } catch (requestError) {
            console.error(requestError);

            setError(
                requestError.response?.data?.message ||
                "Could not save employee. Please try again."
            );
        } finally {
            setIsSaving(false);
        }
    }; const handleDelete = async (id) => {
        const confirmed = window.confirm(
            "Are you sure you want to delete this employee?"
        );

        if (!confirmed) {
            return;
        }

        try {
            setError("");

            await api.delete(`/Employee/${id}`);

            await loadDashboardData();
        } catch (error) {
            console.error(error);

            setError(
                error.response?.data?.message ||
                "Could not delete employee. Please try again."
            );
        }
    };

    const handleEditDepartment = (department) => {
        setEditingDepartmentId(department.id);
        setDepartmentName(department.departmentName);
    };
  

    const handleDeleteDepartment = async (id) => {
        const confirmed = window.confirm(
            "Are you sure you want to delete this department?"
        );

        if (!confirmed) {
            return;
        }

        try {
            setError("");

            await api.delete(`/Department/${id}`);

            await loadDashboardData();
        } catch (error) {
            console.error(error);

            setError(
                error.response?.data?.message ||
                "Could not delete department. Please try again."
            );
        }
    };
    const handleAddDepartment = async () => {
        if (!departmentName.trim()) {
            setError("Department name is required.");
            return;
        }

        try {
            setError("");

            if (editingDepartmentId) {
                await api.put(`/Department/${editingDepartmentId}`, {
                    departmentName: departmentName.trim(),
                });

                setEditingDepartmentId(null);
            } else {
                await api.post("/Department", {
                    departmentName: departmentName.trim(),
                });
            }

            setDepartmentName("");

            await loadDashboardData();
        } catch (error) {
            console.error(error);

            setError(
                error.response?.data?.message ||
                "Could not save department. Please try again."
            );
        }
    };
  

    const handleEdit = (employee) => {
        setEditingEmployeeId(employee.id);

        setForm({
            fullName: employee.fullName,
            emailAddress: employee.emailAddress,
            phoneNumber: employee.phoneNumber || "",
            salary: employee.salary,
            dateOfJoining: employee.dateOfJoining
                ? employee.dateOfJoining.substring(0, 10)
                : "",
            departmentId: employee.departmentId,
        });
    };

    return (
        <main className="dashboard-page">
            <header className="dashboard-header">
                <strong>EMS</strong>
                <button className="logout-button" onClick={onLogout}>
                    Logout
                </button>
            </header>

            <section className="dashboard-content">
                <p className="eyebrow">ADMIN DASHBOARD</p>
                <h1>Welcome back, Admin</h1>
                <p className="subtitle">
                    Manage your employees and departments from one place.
                </p>

                <div className="stat-grid">
                    <article className="stat-card">
                        <span>Total Employees</span>
                        <strong>{totalEmployees}</strong>
                    </article>

                    <article className="stat-card">
                        <span>Departments</span>
                        <strong>{departments.length}</strong>
                    </article>

                    <article className="stat-card">
                        <span>Security</span>
                        <strong>JWT protected</strong>
                    </article>
                </div>

                <section className="dashboard-panel">
                    <h2>
                        {editingEmployeeId ? "Edit Employee" : "Add Employee"}
                    </h2>

                    <form className="employee-form" onSubmit={handleAddEmployee}>
                        <input
                            name="fullName"
                            placeholder="Full name"
                            value={form.fullName}
                            onChange={handleInputChange}
                            required
                        />

                        <input
                            name="emailAddress"
                            type="email"
                            placeholder="Email address"
                            value={form.emailAddress}
                            onChange={handleInputChange}
                            required
                        />

                        <input
                            name="phoneNumber"
                            placeholder="Phone number"
                            value={form.phoneNumber}
                            onChange={handleInputChange}
                        />

                        <input
                            name="salary"
                            type="number"
                            min="0"
                            placeholder="Salary"
                            value={form.salary}
                            onChange={handleInputChange}
                            required
                        />

                        <input
                            name="dateOfJoining"
                            type="date"
                            value={form.dateOfJoining}
                            onChange={handleInputChange}
                            required
                        />

                        <select
                            name="departmentId"
                            value={form.departmentId}
                            onChange={handleInputChange}
                            required
                        >
                            <option value="">Select department</option>
                            {departments.map((department) => (
                                <option key={department.id} value={department.id}>
                                    {department.departmentName}
                                </option>
                            ))}
                        </select>

                        <button type="submit" disabled={isSaving}>
                            {isSaving
                                ? "Saving..."
                                : editingEmployeeId
                                    ? "Update Employee"
                                    : "Add Employee"}
                        </button>
                        {editingEmployeeId && (
                            <button
                                type="button"
                                onClick={() => {
                                    setEditingEmployeeId(null);

                                    setForm({
                                        fullName: "",
                                        emailAddress: "",
                                        phoneNumber: "",
                                        salary: "",
                                        dateOfJoining: "",
                                        departmentId: "",
                                    });
                                }}
                            >
                                Cancel
                            </button>
                        )}
                    </form>
                </section>
                <section className="dashboard-panel">
                    <h2>Employees by Department</h2>

                   cd
                </section>
                <section className="dashboard-panel">
                    <h2>Departments</h2>
                    <div className="department-form">
                        <input
                            type="text"
                            placeholder="Department name"
                            value={departmentName}
                            onChange={(event) =>
                                setDepartmentName(event.target.value)
                            }
                        />

                        <button onClick={handleAddDepartment}>
                            {editingDepartmentId ? "Update Department" : "Add Department"}
                        </button>
                    </div>

                       

                    <div className="department-list">
                        {departments.map((department) => (
                            <div className="department-item" key={department.id}>
                                <span>{department.departmentName}</span>

                                <button onClick={() => handleEditDepartment(department)}>
                                    Edit
                                </button>
                                <button onClick={() => handleDeleteDepartment(department.id)}>
                                    Delete
                                </button>
                            </div>
                        ))}
                    </div>
                </section>

                <section className="dashboard-panel">
                    <h2>Employees</h2>
                    <input
                        type="text"
                        placeholder="🔍 Search employee by name or email..."
                        value={searchTerm}
                        onChange={(event) => {
                            setSearchTerm(event.target.value);
                            setPageNumber(1);
                        }}
                    />

                    {isLoading && <p>Loading employees...</p>}

                    {error && <p className="message">{error}</p>}

                    {!isLoading && !error && (
                        <div className="table-wrapper">
                            <table>
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Department</th>
                                        <th>Salary</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    {employees.map((employee) => (
                                        <tr key={employee.id}>
                                            <td>{employee.fullName}</td>

                                            <td>{employee.emailAddress}</td>

                                            <td>{employee.departmentName}</td>

                                            <td>
                                                ₹{Number(employee.salary).toLocaleString("en-IN")}
                                            </td>

                                            <td>
                                                <button
                                                    onClick={() => handleEdit(employee)}
                                                >
                                                    Edit
                                                </button>

                                                <button
                                                    onClick={() => handleDelete(employee.id)}
                                                >
                                                    Delete
                                                </button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                                        

                    <div className="pagination">
                        <button
                            onClick={() => setPageNumber(pageNumber - 1)}
                            disabled={pageNumber === 1}
                        >
                            Previous
                        </button>

                        <span>Page {pageNumber}</span>

                        <button
                            onClick={() => setPageNumber(pageNumber + 1)}
                            disabled={pageNumber * pageSize >= totalEmployees}
                        >
                            Next
                        </button>
                    </div>

                
                </section>
            </section>
        </main>
    );
}

function App() {
    const [token, setToken] = useState(localStorage.getItem("token"));
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");
    const [isLoading, setIsLoading] = useState(false);

    const handleLogin = async (event) => {
        event.preventDefault();

        setMessage("");
        setIsLoading(true);

        try {
            const response = await api.post("/Auth/login", {
                userName,
                password,
            });

            localStorage.setItem("token", response.data.token);
            localStorage.setItem("role", response.data.role);

            setToken(response.data.token);
        } catch (error) {
            setMessage(
                error.response?.data?.message || "Login failed. Please try again.",
            );
        } finally {
            setIsLoading(false);
        }
    };

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("role");
        setToken(null);
        setUserName("");
        setPassword("");
        setMessage("");
    };

    if (token) {
        return <Dashboard onLogout={handleLogout} />;
    }

    return (
        <main className="login-page">
            <section className="login-card">
                <p className="brand">EMS</p>
                <h1>Employee Management System</h1>
                <p className="subtitle">Sign in to manage employees and departments.</p>

                <form onSubmit={handleLogin}>
                    <label htmlFor="userName">Username</label>
                    <input
                        id="userName"
                        type="text"
                        placeholder="Enter username"
                        value={userName}
                        onChange={(event) => setUserName(event.target.value)}
                        required
                    />

                    <label htmlFor="password">Password</label>
                    <input
                        id="password"
                        type="password"
                        placeholder="Enter password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                        required
                    />

                    <button type="submit" disabled={isLoading}>
                        {isLoading ? "Signing in..." : "Sign in"}
                    </button>
                </form>

                {message && <p className="message">{message}</p>}
            </section>
        </main>
    );

}

export default App;