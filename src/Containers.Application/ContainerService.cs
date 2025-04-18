using Containers.Models;
using Microsoft.Data.SqlClient;

namespace Containers.Application;

public class ContainerService : IContainerService
{
    private string _connectionString;

    public ContainerService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Container> GetAllContainers()
    {
        List<Container> containers = new List<Container>();

        string queryString = "SELECT * FROM Containers";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var container = new Container
                        {
                            Id = reader.GetInt32(0),
                            ContainerTypeId = reader.GetInt32(1),
                            isHazardious = reader.GetBoolean(2),
                            Name = reader.GetString(3),
                        };
                        containers.Add(container);
                    }
                }

                return containers;
            }
            finally
            {
                reader.Close();
            }

        }

        throw new NotImplementedException();
    }

    public bool CreateContainer(Container container)
    {
        const string insertString  = 
            "INSERT INTO Containers (ContainerTypeId, isHazardious, Name) VALUES (@ContainerTypeId, @isHazardious, @Name);";
        int countRowsAdded = -1;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(insertString, connection);
            command.Parameters.AddWithValue("@ContainerTypeId", container.ContainerTypeId);
            command.Parameters.AddWithValue("@isHazardious", container.isHazardious);
            command.Parameters.AddWithValue("@Name", container.Name);
            
            connection.Open();
            countRowsAdded = command.ExecuteNonQuery();
        }
        return countRowsAdded != -1;
    }
}