using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using shared.V1.Events;
using shared.V1.HelperClasses.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackgroundJobFunctions.V1.Media;

public class MediaDeletionFunction(IBlobServiceClientProvider _blobClientProvider,
        IConfiguration _configuration,
        ISecretClient _secretClient,
        ILogger<MediaDeletionFunction> _logger)
{
    private readonly BlobServiceClient _blobServiceClient = _blobClientProvider.Client;

    [FunctionName("MediaDeletionFunction")]
    public async Task Run([EventHubTrigger(
        EventNames.MediaDeleted,
        Connection = "EventHubConnection", // from configuration -> check Program.cs
        ConsumerGroup = "$Default")] EventData[] events)
    {

        foreach (var eventData in events)
        {
            try
            {
                var eventJson = System.Text.Encoding.UTF8.GetString(eventData.EventBody.ToArray());
                var eventPayload = JsonSerializer.Deserialize<MediaDeletedEvent>(eventJson);
                if (eventPayload == null || eventPayload.MediaIds.Count == 0)
                {
                    _logger.LogWarning("Invalid event payload: {Payload}", eventJson);
                    continue;
                }


                var containerName = _configuration["AzureBlobStorage:ContainerName"]; // from configuration -> check Program.cs

                var sqlConnectionString = await _secretClient.GetSecretValueAsync("SqlConnection");
                using var connection = new SqlConnection(sqlConnectionString);
                await connection.OpenAsync();

                var mediaIds = eventPayload.MediaIds.ToList();

                // Step 1: Retrieve all file URLs
                var parameterNames = mediaIds.Select((id, index) => $"@id{index}").ToList();
                var inClause = string.Join(", ", parameterNames);
                var query = $"SELECT media_id, file_url FROM [healthcare].[media] WHERE media_id IN ({inClause})";

                var mediaMap = new Dictionary<int, string>();

                using (var command = new SqlCommand(query, connection))
                {
                    for (int i = 0; i < mediaIds.Count; i++)
                        command.Parameters.AddWithValue(parameterNames[i], mediaIds[i]);

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var id = reader.GetInt32(0);
                        var url = reader.GetString(1);
                        mediaMap[id] = url;
                    }
                }

                // Step 2: Delete blobs from Azure Blob Storage
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                foreach (var fileUrl in mediaMap.Values)
                {
                    var blobName = Path.GetFileName(fileUrl);
                    var blobClient = containerClient.GetBlobClient(blobName);
                    await blobClient.DeleteIfExistsAsync();
                }

                // Step 3: Delete media records from the database
                parameterNames = mediaMap.Keys.Select((id, index) => $"@delId{index}").ToList();
                inClause = string.Join(", ", parameterNames);
                var deleteQuery = $"DELETE FROM [healthcare].[media] WHERE media_id IN ({inClause})";

                using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    int i = 0;
                    foreach (var id in mediaMap.Keys)
                    {
                        deleteCommand.Parameters.AddWithValue(parameterNames[i++], id);
                    }

                    await deleteCommand.ExecuteNonQueryAsync();
                }

                _logger.LogInformation("Successfully deleted all media from database and blob storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process event for media deletion");
                throw; // Re-throw to trigger retry or dead-lettering
            }
        }
    }
}
