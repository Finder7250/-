using System;
using System.Data;
using Npgsql;

namespace KeeperPRO.GeneralDepartment.Data
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

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

        public DataTable GetAllRequests()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM ViewListRequests ORDER BY created_at DESC";
            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        public DataTable FilterRequests(string type, int? departmentId, string status)
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM FilteringRequests(@type, @departmentId, @status)";

            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@type", string.IsNullOrEmpty(type) ? DBNull.Value : (object)type);
                cmd.Parameters.AddWithValue("@departmentId", departmentId.HasValue ? (object)departmentId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? DBNull.Value : (object)status);

                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

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

        public void SendNotification(int requestId, string message)
        {
            System.Diagnostics.Debug.WriteLine($"Уведомление для заявки {requestId}: {message}");
        }

        // ========== ОТЧЕТЫ ==========

        public DataTable GetVisitsCountReport(string period, int? departmentId = null)
        {
            DataTable dt = new DataTable();
            string dateTrunc = "";

            switch (period.ToLower())
            {
                case "day": dateTrunc = "day"; break;
                case "month": dateTrunc = "month"; break;
                case "year": dateTrunc = "year"; break;
                default: dateTrunc = "day"; break;
            }

            string query = $@"
                SELECT 
                    DATE_TRUNC('{dateTrunc}', r.start_date) as period,
                    d.name as department_name,
                    COUNT(r.request_id) as visits_count
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                WHERE r.status = 'одобрена'
                GROUP BY DATE_TRUNC('{dateTrunc}', r.start_date), d.name
                ORDER BY period DESC, department_name";

            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        public DataTable GetCurrentVisitorsList()
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 
                    d.name as department_name,
                    v.last_name || ' ' || v.first_name || ' ' || COALESCE(v.middle_name, '') as visitor_name,
                    v.passport_series || ' ' || v.passport_number as passport,
                    v.phone,
                    r.start_date,
                    r.access_start_time,
                    r.check_in_time
                FROM visitrequests r
                JOIN departments d ON r.department_id = d.department_id
                LEFT JOIN visitorrequestlink vrl ON r.request_id = vrl.request_id
                LEFT JOIN visitors v ON vrl.visitor_id = v.visitor_id
                WHERE r.status = 'одобрена'
                  AND r.access_start_time IS NOT NULL
                  AND r.check_out_time IS NULL
                ORDER BY d.name, v.last_name";

            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        public DataTable GetThreeHourReport()
        {
            DataTable dt = new DataTable();
            string query = @"
                WITH time_slots AS (
                    SELECT 
                        d.name as department_name,
                        DATE_TRUNC('hour', r.created_at) as hour_slot,
                        COUNT(r.request_id) as visits_count
                    FROM visitrequests r
                    JOIN departments d ON r.department_id = d.department_id
                    WHERE r.status = 'одобрена'
                      AND r.created_at >= CURRENT_DATE
                    GROUP BY d.name, DATE_TRUNC('hour', r.created_at)
                )
                SELECT 
                    department_name,
                    hour_slot,
                    visits_count,
                    EXTRACT(HOUR FROM hour_slot) as hour
                FROM time_slots
                ORDER BY department_name, hour_slot";

            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }
    }
}