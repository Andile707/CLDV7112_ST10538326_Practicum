
using System.Net;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace agristreamiotst10538326.Functions
{
    public class TriggerIrrigationFunction
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<TriggerIrrigationFunction> _logger;

        public TriggerIrrigationFunction(
            QueueServiceClient queueServiceClient,
            ILogger<TriggerIrrigationFunction> logger)
        {
            _queueClient = queueServiceClient
                .GetQueueClient("irrigation-commands");

            _logger = logger;
        }

        [Function("TriggerIrrigation")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "post",
                Route = "irrigation/trigger")]
            HttpRequestData request)
        {
            try
            {
                var command =
                    await JsonSerializer.DeserializeAsync<
                        IrrigationCommandRequest>(
                        request.Body,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (command == null ||
                    string.IsNullOrWhiteSpace(command.DeviceId) ||
                    string.IsNullOrWhiteSpace(command.FieldId))
                {
                    var badResponse = request.CreateResponse(
                        HttpStatusCode.BadRequest);

                    await badResponse.WriteAsJsonAsync(new
                    {
                        message = "DeviceId and FieldId are required."
                    });

                    return badResponse;
                }

                await _queueClient.CreateIfNotExistsAsync();

                var queueMessage = new IrrigationQueueMessage
                {
                    CommandId = Guid.NewGuid().ToString(),
                    DeviceId = command.DeviceId,
                    FieldId = command.FieldId,
                    Command = "START_IRRIGATION",
                    RequestedAt = DateTimeOffset.UtcNow
                };

                string messageJson =
                    JsonSerializer.Serialize(queueMessage);

                await _queueClient.SendMessageAsync(messageJson);

                _logger.LogInformation(
                    "Irrigation command queued for device {DeviceId}",
                    command.DeviceId);

                var response = request.CreateResponse(
                    HttpStatusCode.Accepted);

                await response.WriteAsJsonAsync(new
                {
                    message = "Irrigation command queued successfully.",
                    commandId = queueMessage.CommandId,
                    deviceId = queueMessage.DeviceId,
                    status = "Queued"
                });

                return response;
            }
            catch (JsonException)
            {
                var response = request.CreateResponse(
                    HttpStatusCode.BadRequest);

                await response.WriteAsJsonAsync(new
                {
                    message = "Invalid JSON request."
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to queue irrigation command.");

                var response = request.CreateResponse(
                    HttpStatusCode.InternalServerError);

                await response.WriteAsJsonAsync(new
                {
                    message = "Unable to queue irrigation command."
                });

                return response;
            }
        }
    }

    public class IrrigationCommandRequest
    {
        public string DeviceId { get; set; } = string.Empty;

        public string FieldId { get; set; } = string.Empty;
    }

    public class IrrigationQueueMessage
    {
        public string CommandId { get; set; } = string.Empty;

        public string DeviceId { get; set; } = string.Empty;

        public string FieldId { get; set; } = string.Empty;

        public string Command { get; set; } = string.Empty;

        public DateTimeOffset RequestedAt { get; set; }
    }
}
