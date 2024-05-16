using System.Data.SqlClient;
using System.Data;

namespace SportFIT.Controllers
{
    public class EstadisticasController
    {
        private readonly string connectionString;

        public EstadisticasController(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DataTable ObtenerReservasPorInstalacion(string pueblo)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                                SELECT
                                    i.nombre_instalacion AS Instalacion,
                                    COUNT(r.id_reserva) AS Total_Reservas
                                FROM
                                    Instalacion i
                                JOIN
                                    Reserva r ON i.id_instalacion = r.id_instalacion
                                JOIN Pueblo P on i.id_pueblo = P.id_pueblo
                                WHERE p.nombre_pueblo = @NombrePueblo
                                GROUP BY
                                    i.nombre_instalacion";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombrePueblo", pueblo);
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public DataTable ObtenerPorcentajesReservas(string pueblo)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        u.nombre_usuario AS Usuario,
                        COUNT(*) AS Total_Reservas,
                        ROUND(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER (), 2) AS Porcentaje_Reservas
                    FROM
                        Usuario u
                    JOIN
                        Reserva r ON u.id_usuario = r.id_usuario
                    JOIN
                        Instalacion i ON r.id_instalacion = i.id_instalacion
                    JOIN
                        Pueblo P ON i.id_pueblo = P.id_pueblo
                    WHERE
                        p.nombre_pueblo = @NombrePueblo
                    GROUP BY
                        u.nombre_usuario";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombrePueblo", pueblo);
                    connection.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public DataTable TendenciaReservasMes(string pueblo)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        YEAR(r.fecha_reserva) AS Anio,
                        DATENAME(MONTH, r.fecha_reserva) AS Mes,
                        COUNT(*) AS Total_Reservas
                    FROM
                        Reserva r
                    JOIN
                        Instalacion i ON r.id_instalacion = i.id_instalacion
                    JOIN Pueblo P on i.id_pueblo = P.id_pueblo
                    WHERE p.nombre_pueblo = @NombrePueblo
                    GROUP BY
                        YEAR(r.fecha_reserva),
                        DATENAME(MONTH, r.fecha_reserva)
                    ORDER BY
                        Anio,
                        mes";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombrePueblo", pueblo);
                    connection.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }
    }
}
