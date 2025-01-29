using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        Random rand = new Random();
        string connectionString = "Server=DESKTOP-U0S8Q19\\SQLEXPRESS;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;";

        // Supposons que les ID des clients sont dans la plage de 1 à 1000 (ou la plage réelle que tu utilises)
        int maxCustomerId = 1000;
        int totalOrders = 10000;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            for (int i = 0; i < totalOrders; i++)
            {
                int customerId = rand.Next(1, maxCustomerId + 1); // ID du client aléatoire
                DateTime createdAt = DateTime.Now.AddDays(-rand.Next(1, 365)); // Date aléatoire dans l'année écoulée
                DateTime updatedAt = createdAt.AddMinutes(rand.Next(1, 1440)); // Date de mise à jour, aléatoire dans la journée

                string query = @"
                    INSERT INTO Orders (customer_id, created_at, updated_at)
                    VALUES (@customerId, @createdAt, @updatedAt)";

                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customerId", customerId);
                        command.Parameters.AddWithValue("@createdAt", createdAt);
                        command.Parameters.AddWithValue("@updatedAt", updatedAt);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting order {i + 1}: {ex.Message}");
                }

                // Affichage du progrès
                if ((i + 1) % 500 == 0) // Affiche le progrès tous les 500 enregistrements
                {
                    Console.WriteLine($"{i + 1} orders inserted...");
                }
            }
        }

        Console.WriteLine("25,000 orders inserted successfully!");
    }
}

