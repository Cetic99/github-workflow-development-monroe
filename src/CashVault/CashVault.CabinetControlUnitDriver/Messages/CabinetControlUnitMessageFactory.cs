using CashVault.CabinetControlUnitDriver.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages
{
    internal class CabinetControlUnitMessageFactory
    {
        private List<Type> messageResponseTypes;

        private static readonly object syncRoot = new();
        private static CabinetControlUnitMessageFactory? customInstance;
        private static readonly Lazy<CabinetControlUnitMessageFactory> lazyInstance = new(() => new CabinetControlUnitMessageFactory());

        public static CabinetControlUnitMessageFactory Current
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


        public CabinetControlUnitMessageFactory()
        {
            messageResponseTypes = GetInterfaceImplementationsFromCurrentAssembly(typeof(ICabinetControlUnit));
        }

        public ICabinetControlUnit TryCreateCabinetControlUnitResponse(byte[] data)
        {
            if (messageResponseTypes.Count == 0)
            {
                throw new InvalidOperationException("There is no implementation of ICabinetControlUnit interface.");
            }

            foreach (var type in messageResponseTypes)
            {
                try
                {
                    var instance = Activator.CreateInstance(type, data) as ICabinetControlUnit;
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

            throw new InvalidOperationException("There is no implementation of ICabinetControlUnit interface that can handle the message.");
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

