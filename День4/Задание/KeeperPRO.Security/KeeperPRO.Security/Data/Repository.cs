using System;
using System.Data;
using Npgsql;
using System.Media;

namespace KeeperPRO.Security.Data
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Авторизация сотрудника охраны (department_id = 4)
        public bool LoginSecurityEmployee(int code)
        {
            string query = "SELECT COUNT(*) FROM employees WHERE employee_id = @code AND department_id = 4";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@code", code);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Получение всех одобренных заявок (без access_start_time, access_end_time)
        public DataTable GetApprovedRequests()
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
                    r.created_at,
                    v.visitor_id, 
                    v.last_name, 
                    v.first_name, 
                    v.middle_name,
                    v.passport_series || ' ' || v.passport_number as passport, 
                    v.phone
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                LEFT JOIN visitorrequestlink vrl ON r.request_id = vrl.request_id
                LEFT JOIN visitors v ON vrl.visitor_id = v.visitor_id
                WHERE r.status = 'одобрена'
                ORDER BY r.start_date DESC";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // Получение одобренных заявок с фильтрацией
        public DataTable GetApprovedRequestsFiltered(DateTime? date, string type, int? departmentId)
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
                    r.created_at,
                    v.visitor_id, 
                    v.last_name, 
                    v.first_name, 
                    v.middle_name,
                    v.passport_series || ' ' || v.passport_number as passport, 
                    v.phone
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                LEFT JOIN visitorrequestlink vrl ON r.request_id = vrl.request_id
                LEFT JOIN visitors v ON vrl.visitor_id = v.visitor_id
                WHERE r.status = 'одобрена'
                  AND (@date IS NULL OR r.start_date::DATE = @date::DATE)
                  AND (@type IS NULL OR r.type = @type)
                  AND (@departmentId IS NULL OR r.department_id = @departmentId)
                ORDER BY r.start_date DESC";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@date", date.HasValue ? (object)date.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("@type", string.IsNullOrEmpty(type) ? DBNull.Value : (object)type);
                cmd.Parameters.AddWithValue("@departmentId", departmentId.HasValue ? (object)departmentId.Value : DBNull.Value);
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // Поиск заявки по ФИО или паспорту
        public DataTable SearchRequests(string searchText)
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
                    r.created_at,
                    v.visitor_id, 
                    v.last_name, 
                    v.first_name, 
                    v.middle_name,
                    v.passport_series || ' ' || v.passport_number as passport, 
                    v.phone
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                LEFT JOIN visitorrequestlink vrl ON r.request_id = vrl.request_id
                LEFT JOIN visitors v ON vrl.visitor_id = v.visitor_id
                WHERE r.status = 'одобрена'
                  AND (v.last_name ILIKE @search OR v.first_name ILIKE @search 
                       OR v.middle_name ILIKE @search OR v.passport_series ILIKE @search 
                       OR v.passport_number ILIKE @search)
                ORDER BY r.start_date DESC";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        // Получение заявки по ID (без access_start_time, access_end_time)
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
                    r.created_at,
                    v.visitor_id, 
                    v.last_name, 
                    v.first_name, 
                    v.middle_name,
                    v.passport_series || ' ' || v.passport_number as passport, 
                    v.phone, 
                    v.email,
                    v.birth_date
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                LEFT JOIN visitorrequestlink vrl ON r.request_id = vrl.request_id
                LEFT JOIN visitors v ON vrl.visitor_id = v.visitor_id
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

        // Разрешение доступа (открытие турникета) - ОБНОВЛЕН
        public bool GrantAccess(int requestId, int visitorId, DateTime accessTime)
        {
            // Проверяем, существует ли колонка access_start_time
            string checkColumnQuery = @"
                SELECT COUNT(*) 
                FROM information_schema.columns 
                WHERE table_name = 'visitrequests' AND column_name = 'access_start_time'";

            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(checkColumnQuery, conn))
            {
                conn.Open();
                long columnExists = (long)cmd.ExecuteScalar();

                if (columnExists > 0)
                {
                    // Если колонка есть, обновляем её
                    string updateQuery = "UPDATE visitrequests SET access_start_time = @accessTime WHERE request_id = @requestId";
                    using (var updateCmd = new NpgsqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@accessTime", accessTime);
                        updateCmd.Parameters.AddWithValue("@requestId", requestId);
                        return updateCmd.ExecuteNonQuery() > 0;
                    }
                }
                else
                {
                    // Если колонки нет, просто логируем
                    System.Diagnostics.Debug.WriteLine($"Доступ разрешен для заявки {requestId} в {accessTime}");
                    return true;
                }
            }
        }

        // Фиксация времени убытия - ОБНОВЛЕН
        public bool SetDepartureTime(int requestId, int visitorId, DateTime departureTime)
        {
            // Проверяем, существует ли колонка access_end_time
            string checkColumnQuery = @"
                SELECT COUNT(*) 
                FROM information_schema.columns 
                WHERE table_name = 'visitrequests' AND column_name = 'access_end_time'";

            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(checkColumnQuery, conn))
            {
                conn.Open();
                long columnExists = (long)cmd.ExecuteScalar();

                if (columnExists > 0)
                {
                    string updateQuery = "UPDATE visitrequests SET access_end_time = @departureTime WHERE request_id = @requestId";
                    using (var updateCmd = new NpgsqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@departureTime", departureTime);
                        updateCmd.Parameters.AddWithValue("@requestId", requestId);
                        return updateCmd.ExecuteNonQuery() > 0;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Выход зафиксирован для заявки {requestId} в {departureTime}");
                    return true;
                }
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

        // Отправка сигнала на сервер (эмуляция открытия турникета)
        public void SendOpenGateSignal(int requestId)
        {
            System.Diagnostics.Debug.WriteLine($"Сигнал: открыть турникет для заявки {requestId}");
            SystemSounds.Beep.Play();
        }
    }
}