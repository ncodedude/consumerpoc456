using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

class Program
{
    const string ServiceBusConnectionString = "Endpoint=sb://pocservicebus456.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8lvZ8CKkDV4qxMFXnEdM2ckVTitRVBN0R+ASbPrB61c="; // Substitua pela sua Connection String
    const string QueueName = "paymentqueue";
    static ServiceBusClient client;
    static ServiceBusReceiver receiver;

    static async Task Main()
    {
        await MainAsync();
    }

    static async Task MainAsync()
    {
        client = new ServiceBusClient(ServiceBusConnectionString);
        receiver = client.CreateReceiver(QueueName);

        RegisterOnMessageHandlerAndReceiveMessages();

        Console.WriteLine("Pressione Enter para encerrar a aplicação.");
        Console.ReadLine();

        await client.DisposeAsync();
    }

    static void RegisterOnMessageHandlerAndReceiveMessages()
    {
        var options = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages=false
        };

        var processor = client.CreateProcessor(QueueName, options);
        processor.ProcessMessageAsync += ProcessMessagesAsync;
        processor.ProcessErrorAsync += ErrorHandler;

        processor.StartProcessingAsync();
    }

    static async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        Console.WriteLine($"Recebida mensagem: {body}");

        await args.CompleteMessageAsync(args.Message);
    }

    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Mensagem de erro: {args.Exception}");
        return Task.CompletedTask;
    }
}
