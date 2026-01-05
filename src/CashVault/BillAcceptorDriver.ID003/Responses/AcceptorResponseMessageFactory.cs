using CashVault.BillAcceptorDriver.ID003.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillAcceptorDriver.ID003.Responses;

internal class AcceptorResponseMessageFactory
{
    private List<BaseResponse> messageResponseEmptyInstances;

    private static readonly object syncRoot = new();
    private static AcceptorResponseMessageFactory? customInstance;
    private static readonly Lazy<AcceptorResponseMessageFactory> lazyInstance = new(() => new AcceptorResponseMessageFactory());

    public static AcceptorResponseMessageFactory Current
    {
        get => customInstance != null ? customInstance : lazyInstance.Value;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            lock (syncRoot)
            {
                customInstance = value;
            }
        }
    }

    public static void Initialize()
    {
        Current = new();
    }

    private AcceptorResponseMessageFactory()
    {
        messageResponseEmptyInstances = GetAllImplementationEmptyInstancesFromCurrentAssembly();
    }

    public IAcceptorResponse TryCreateAcceptorResponse(Span<byte> data)
    {
        if (messageResponseEmptyInstances.Count == 0)
        {
            throw new InvalidOperationException("There is no implementation of IAcceptorResponse interface.");
        }

        if (data.Length == 0)
        {
            return null;
        }

        IAcceptorResponse response = null;

        try
        {
            byte key = data[0];
            var matchingResponses = this.messageResponseEmptyInstances.Where(o => o.Key == key).ToList();

            if (matchingResponses.Count == 0)
            {
                throw new InvalidOperationException("There is no implementation of IBillDispenserResponse interface that can handle the message.");
            }

            if (matchingResponses.Count > 1)
            {
                throw new InvalidOperationException("There are multiple implementations of IAcceptorResponse interface that can handle the message.");
            }

            if (data.Length > 1)
            {
                // instance message with bytes as data
                var dataParam = data.Slice(1).ToArray();
                response = Activator.CreateInstance(matchingResponses[0].GetType(), dataParam) as IAcceptorResponse;
            }
            else
            {
                // return instance with default constructor
                response = Activator.CreateInstance(matchingResponses[0].GetType()) as IAcceptorResponse;
            }
        }
        catch (Exception e)
        {
            // TODO: log the exception
        }
        
        return response;
    }

    private List<BaseResponse> GetAllImplementationEmptyInstancesFromCurrentAssembly()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        var implementations = assembly.GetTypes()
            .Where(type => typeof(BaseResponse).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

        return implementations.Select(t => Activator.CreateInstance(t) as BaseResponse).ToList();
    }
}
