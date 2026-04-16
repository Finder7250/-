using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using KeeperPRO.Models;
using Npgsql;

namespace KeeperPRO.Data
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static string GetMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public bool RegisterUser(string email, string password)
        {
            string passwordHash = GetMD5Hash(password);
            string query = "INSERT INTO users (email, password_hash, registration_date) VALUES (@email, @hash, CURRENT_TIMESTAMP)";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@hash", passwordHash);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public User LoginUser(string email, string password)
        {
            string passwordHash = GetMD5Hash(password);
            string query = "SELECT user_id, email, registration_date FROM users WHERE email = @email AND password_hash = @hash";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@hash", passwordHash);
                conn.Open();
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            RegistrationDate = reader.GetDateTime(2)
                        };
                    }
                }
            }
            return null;
        }

        public DataTable GetDepartments()
        {
            DataTable dt = new DataTable();
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT department_id, name FROM departments ORDER BY name", conn))
            {
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        public DataTable GetEmployeesByDepartment(int departmentId)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT employee_id, 
                                    last_name || ' ' || first_name || ' ' || COALESCE(middle_name, '') as full_name
                             FROM employees 
                             WHERE department_id = @deptId
                             ORDER BY last_name";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@deptId", departmentId);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        public int CreateVisitRequest(VisitRequest request)
        {
            string query = @"INSERT INTO visitrequests (user_id, type, start_date, end_date, purpose, department_id, employee_id, status, created_at)
                             VALUES (@userId, @type, @startDate, @endDate, @purpose, @deptId, @empId, 'проверка', CURRENT_TIMESTAMP)
                             RETURNING request_id";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userId", request.UserId);
                cmd.Parameters.AddWithValue("@type", request.Type);
                cmd.Parameters.AddWithValue("@startDate", request.StartDate);
                cmd.Parameters.AddWithValue("@endDate", request.EndDate);
                cmd.Parameters.AddWithValue("@purpose", request.Purpose);
                cmd.Parameters.AddWithValue("@deptId", request.DepartmentId);
                cmd.Parameters.AddWithValue("@empId", request.EmployeeId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public DataTable GetUserRequests(int userId)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT r.request_id, 
                                    CASE WHEN r.type = 'personal' THEN 'Личная' ELSE 'Групповая' END as type,
                                    r.start_date, 
                                    r.end_date, 
                                    r.purpose, 
                                    r.status, 
                                    COALESCE(r.rejection_reason, '') as rejection_reason,
                                    r.created_at,
                                    d.name as department_name,
                                    e.last_name || ' ' || e.first_name || ' ' || COALESCE(e.middle_name, '') as employee_name
                             FROM visitrequests r
                             JOIN departments d ON r.department_id = d.department_id
                             JOIN employees e ON r.employee_id = e.employee_id
                             WHERE r.user_id = @userId
                             ORDER BY r.created_at DESC";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }
    }
}