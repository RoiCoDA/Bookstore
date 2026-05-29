using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;



public class NotificationHub : Hub
{
    // Handles when a new client connects to the hub
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.All.SendAsync("ReceiveMessage", "A new client has connected.");
    }

    // Handles when a client disconnects from the hub
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
        await Clients.All.SendAsync("ReceiveMessage", "A client has disconnected.");
    }

    // Handles receiving messages from clients
    public async Task ReceiveMessage(string receivedMessage)
    {
        try
        {
            // Parse the received JSON message
            using (JsonDocument document = JsonDocument.Parse(receivedMessage))
            {
                JsonElement root = document.RootElement;

                if (root.TryGetProperty("event", out JsonElement eventTypeElement))
                {
                    string eventType = eventTypeElement.GetString();
                    string responseMessage;

                    switch (eventType)
                    {
                        case "InitialData":
                            responseMessage = JsonSerializer.Serialize(new { @event = "InitialDataResponse", data = "Initial data processed" });
                            break;

                        case "ChatMessage":
                            responseMessage = JsonSerializer.Serialize(new { @event = "ChatMessageResponse", data = "Chat message received" });
                            break;

                        case "Notification":
                            Console.WriteLine("Processing Notification event...");
                            responseMessage = JsonSerializer.Serialize(new { @event = "NotificationResponse", data = "Notification processed" });
                            break;

                        case "SaleStart":
                            responseMessage = JsonSerializer.Serialize(new { @event = "SaleStartResponse", data = "Sale started" });
                            break;

                        case "SaleEnd":
                            responseMessage = JsonSerializer.Serialize(new { @event = "SaleEndResponse", data = "Sale ended" });
                            break;

                        default:
                            responseMessage = JsonSerializer.Serialize(new { @event = "Error", data = "Unknown event type" });
                            break;
                    }

                    // Send the response to all connected clients
                    await Clients.All.SendAsync("ReceiveMessage", responseMessage);
                }
                else
                {
                    string errorResponse = JsonSerializer.Serialize(new { @event = "Error", data = "No event type specified" });
                    await Clients.All.SendAsync("ReceiveMessage", errorResponse);
                }
            }
        }
        catch (JsonException jsonEx)
        {
            string errorResponse = JsonSerializer.Serialize(new { @event = "Error", data = "Invalid JSON format" });
            await Clients.All.SendAsync("ReceiveMessage", errorResponse);
        }
    }

    // Method to broadcast messages to all clients (equivalent to SendMessageToAllAsync)
    public async Task SendMessageToAllAsync(object message)
    {
        string jsonMessage = JsonSerializer.Serialize(message);
        await Clients.All.SendAsync("ReceiveMessage", jsonMessage);
    }
}
