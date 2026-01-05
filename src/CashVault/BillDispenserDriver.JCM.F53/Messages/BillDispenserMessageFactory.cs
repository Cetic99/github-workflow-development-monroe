using CashVault.BillDispenserDriver.JCM.F53.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages
{
    internal class BillDispenserMessageFactory
    {
        private List<Type> messageResponseTypes;

        private static readonly object syncRoot = new();
        private static BillDispenserMessageFactory? customInstance;
        private static readonly Lazy<BillDispenserMessageFactory> lazyInstance = new(() => new BillDispenserMessageFactory());

        public static BillDispenserMessageFactory Current
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


        public BillDispenserMessageFactory()
        {
            messageResponseTypes = GetInterfaceImplementationsFromCurrentAssembly(typeof(IBillDispenserResponse));
        }

        public IBillDispenserResponse TryCreateDispenserResponse(byte[] data)
        {
            if (messageResponseTypes.Count == 0)
            {
                throw new InvalidOperationException("There is no implementation of IBillDispenserResponse interface.");
            }

            foreach (var type in messageResponseTypes)
            {
                try
                {
                    var instance = Activator.CreateInstance(type, data) as IBillDispenserResponse;
                    if (instance != null && instance.IsValidMessage())
                    {
                        return instance;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            throw new InvalidOperationException("There is no implementation of IBillDispenserResponse interface that can handle the message.");
        }

        private List<Type> GetInterfaceImplementationsFromCurrentAssembly(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (!targetType.IsInterface)
            {
                throw new ArgumentException("Target type must be an interface", nameof(targetType));
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            var implementations = assembly.GetTypes()
                .Where(type => targetType.IsAssignableFrom(type) && !type.IsInterface);

            return implementations.ToList();
        }
    }
}
