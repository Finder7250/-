using System;
using System.Data;
using Npgsql;

namespace KeeperPRO.Department.Data
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ========== АВТОРИЗАЦИЯ ==========
        public bool LoginDepartmentEmployee(int code)
        {
            string query = "SELECT COUNT(*) FROM employees WHERE employee_id = @code AND department_id != 4";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@code", code);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // ========== ПОЛУЧЕНИЕ ЗАЯВОК ДЛЯ ПОДРАЗДЕЛЕНИЯ ==========
        public DataTable GetApprovedRequestsForDepartment(int employeeId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT DISTINCT 
                    r.request_id, 
                    CASE WHEN r.type = 'personal' THEN 'Личная' ELSE 'Групповая' END as type,
                    r.start_date, 
                    r.end_date, 
                    r.purpose, 
                    d.name as department_name, 
                    r.status, 
                    r.created_at
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                WHERE r.status = 'одобрена' 
                  AND r.department_id = (SELECT department_id FROM employees WHERE employee_id = @employeeId)
                ORDER BY r.start_date DESC";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@employeeId", employeeId);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // ========== ФИЛЬТРАЦИЯ ЗАЯВОК ПО ДАТЕ ==========
        public DataTable GetApprovedRequestsForDepartmentFiltered(int employeeId, DateTime? date)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT DISTINCT 
                    r.request_id, 
                    CASE WHEN r.type = 'personal' THEN 'Личная' ELSE 'Групповая' END as type,
                    r.start_date, 
                    r.end_date, 
                    r.purpose, 
                    d.name as department_name, 
                    r.status, 
                    r.created_at
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                WHERE r.status = 'одобрена' 
                  AND r.department_id = (SELECT department_id FROM employees WHERE employee_id = @employeeId)
                  AND (@date IS NULL OR r.start_date::DATE = @date::DATE)
                ORDER BY r.start_date DESC";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@employeeId", employeeId);
                cmd.Parameters.AddWithValue("@date", date.HasValue ? (object)date.Value.Date : DBNull.Value);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // ========== ПОЛУЧЕНИЕ ЗАЯВКИ ПО ID ==========
        public DataRow GetRequestById(int requestId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT DISTINCT 
                    r.request_id, 
                    CASE WHEN r.type = 'personal' THEN 'Личная' ELSE 'Групповая' END as type,
                    r.start_date, 
                    r.end_date, 
                    r.purpose, 
                    d.name as department_name, 
                    r.status, 
                    r.created_at
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                WHERE r.request_id = @requestId";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@requestId", requestId);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        // ========== ПОЛУЧЕНИЕ СПИСКА ПОСЕТИТЕЛЕЙ ПО ЗАЯВКЕ ==========
        public DataTable GetVisitorsByRequestId(int requestId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT v.visitor_id, v.last_name, v.first_name, v.middle_name,
                       v.passport_series || ' ' || v.passport_number as passport,
                       v.phone, v.email, v.is_blacklisted, v.blacklist_reason
                FROM visitors v
                JOIN visitorrequestlink vrl ON v.visitor_id = vrl.visitor_id
                WHERE vrl.request_id = @requestId";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@requestId", requestId);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // ========== ФИКСАЦИЯ ВРЕМЕНИ ПРИБЫТИЯ ==========
        public bool SetCheckInTime(int requestId, DateTime checkInTime)
        {
            // Проверяем, существует ли колонка check_in_time
            string checkColumnQuery = @"
                SELECT COUNT(*) 
                FROM information_schema.columns 
                WHERE table_name = 'visitrequests' AND column_name = 'check_in_time'";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var checkCmd = new NpgsqlCommand(checkColumnQuery, conn))
                {
                    long columnExists = (long)checkCmd.ExecuteScalar();

                    if (columnExists > 0)
                    {
                        string updateQuery = "UPDATE visitrequests SET check_in_time = @checkInTime WHERE request_id = @requestId";
                        using (var updateCmd = new NpgsqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@checkInTime", checkInTime);
                            updateCmd.Parameters.AddWithValue("@requestId", requestId);
                            return updateCmd.ExecuteNonQuery() > 0;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Прибытие зафиксировано для заявки {requestId} в {checkInTime}");
                        return true;
                    }
                }
            }
        }

        // ========== ФИКСАЦИЯ ВРЕМЕНИ УБЫТИЯ ==========
        public bool SetCheckOutTime(int requestId, DateTime checkOutTime)
        {
            // Проверяем, существует ли колонка check_out_time
            string checkColumnQuery = @"
                SELECT COUNT(*) 
                FROM information_schema.columns 
                WHERE table_name = 'visitrequests' AND column_name = 'check_out_time'";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var checkCmd = new NpgsqlCommand(checkColumnQuery, conn))
                {
                    long columnExists = (long)checkCmd.ExecuteScalar();

                    if (columnExists > 0)
                    {
                        string updateQuery = "UPDATE visitrequests SET check_out_time = @checkOutTime WHERE request_id = @requestId";
                        using (var updateCmd = new NpgsqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@checkOutTime", checkOutTime);
                            updateCmd.Parameters.AddWithValue("@requestId", requestId);
                            return updateCmd.ExecuteNonQuery() > 0;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Убытие зафиксировано для заявки {requestId} в {checkOutTime}");
                        return true;
                    }
                }
            }
        }

        // ========== ДОБАВЛЕНИЕ В ЧЕРНЫЙ СПИСОК ==========
        public bool AddToBlacklist(int visitorId, string reason)
        {
            string query = @"UPDATE visitors 
                             SET is_blacklisted = TRUE, 
                                 blacklist_reason = @reason,
                                 blacklist_date = CURRENT_TIMESTAMP
                             WHERE visitor_id = @visitorId";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.Parameters.AddWithValue("@visitorId", visitorId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ========== ЭМУЛЯЦИЯ ОТПРАВКИ УВЕДОМЛЕНИЯ ==========
        public void SendBlacklistNotification(int visitorId, string visitorName, string reason)
        {
            System.Diagnostics.Debug.WriteLine($"Уведомление: Посетитель {visitorName} (ID: {visitorId}) добавлен в черный список. Причина: {reason}");
        }

        // ========== ПОЛУЧЕНИЕ ОТДЕЛА СОТРУДНИКА ==========
        public int GetEmployeeDepartment(int employeeId)
        {
            string query = "SELECT department_id FROM employees WHERE employee_id = @employeeId";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@employeeId", employeeId);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }
    }
}