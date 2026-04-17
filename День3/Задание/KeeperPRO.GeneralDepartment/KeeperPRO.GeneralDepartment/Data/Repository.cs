using System;
using System.Data;
using KeeperPRO_GeneralDepartment.Models;
using Npgsql;

namespace KeeperPRO_GeneralDepartment.Data
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Авторизация сотрудника по коду
        public bool LoginEmployee(int code)
        {
            string query = "SELECT COUNT(*) FROM employees WHERE employee_id = @code";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@code", code);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Получение всех подразделений
        public DataTable GetDepartments()
        {
            DataTable dt = new DataTable();
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand("SELECT department_id, name FROM departments ORDER BY name", conn))
            {
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // Получение всех заявок (через представление)
        public DataTable GetAllRequests()
        {
            DataTable dt = new DataTable();
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand("SELECT * FROM ViewListRequests ORDER BY created_at DESC", conn))
            {
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // Фильтрация заявок (через хранимую функцию)
        public DataTable FilterRequests(string type, int? departmentId, string status)
        {
            DataTable dt = new DataTable();
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand("SELECT * FROM FilteringRequests(@type, @deptId, @status)", conn))
            {
                cmd.Parameters.AddWithValue("@type", string.IsNullOrEmpty(type) ? DBNull.Value : (object)type);
                cmd.Parameters.AddWithValue("@deptId", departmentId.HasValue ? (object)departmentId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? DBNull.Value : (object)status);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // Получение заявки по ID
        public DataRow GetRequestById(int requestId)
        {
            DataTable dt = new DataTable();
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand("SELECT * FROM ViewListRequests WHERE request_id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", requestId);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        // Проверка в черном списке
        public bool CheckVisitorInBlacklist(int visitorId)
        {
            string query = "SELECT is_blacklisted FROM visitors WHERE visitor_id = @id";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", visitorId);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value && Convert.ToBoolean(result);
            }
        }

        // Обновление статуса заявки
        public bool UpdateRequestStatus(int requestId, string status, string rejectionReason, DateTime? visitDate)
        {
            string query = @"UPDATE visitrequests 
                             SET status = @status, 
                                 rejection_reason = @rejectionReason,
                                 start_date = COALESCE(@visitDate, start_date)
                             WHERE request_id = @requestId";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@rejectionReason", string.IsNullOrEmpty(rejectionReason) ? DBNull.Value : (object)rejectionReason);
                cmd.Parameters.AddWithValue("@visitDate", visitDate.HasValue ? (object)visitDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@requestId", requestId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Отправка уведомления
        public void SendNotification(int requestId, string message)
        {
            System.Diagnostics.Debug.WriteLine($"Уведомление для заявки {requestId}: {message}");
        }
    }
}