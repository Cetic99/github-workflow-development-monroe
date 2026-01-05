/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

//=======================================================================

const getConfigurationMonenyBillAcceptorData = async (currency) => {
  const { data } = await api.get(`api/configuration/terminal/bill-acceptor`, {
    params: {
      currency
    }
  })

  // // temp
  // data.currencies = {
  //   label: 'Currency',
  //   items: data?.supportedCurrencies?.map(x => ({
  //     value: x,
  //     name: x.toUpperCase()
  //   }))
  // };

  // //TODO: Chech this code!
  // data.cashTransactionLimit = {
  //   label: 'Cash transaction limit',
  //   value: 300.0
  // }

  // data.consecutiveCashToCashLimit = {
  //   label: 'Consecutive cash-to-cash limit',
  //   value: 0.0
  // }

  // data.stackerCapacity = {
  //   label: 'Stacker Capacity',
  //   value: 980
  // }

  return data
}

export const useConfigurationMoneyBillAcceptor = (currency) => {
  return useQuery({
    queryKey: ['configuration-money-bill-acceptor'],
    queryFn: async () => getConfigurationMonenyBillAcceptorData(currency)
  })
}

//=======================================================================

const getConfigurationMonenyBillDispenserData = async () => {
  const { data } = await api.get(`api/configuration/terminal/bill-dispenser`)
  return data
}

export const useConfigurationMoneyBillDispenser = () => {
  return useQuery({
    queryKey: ['configuration-money-bill-dispenser'],
    queryFn: async () => getConfigurationMonenyBillDispenserData()
  })
}

//=======================================================================

const getConfigurationMonenyCoinDispenserData = async () => {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        currencies: [
          { value: 1, name: 'USD' },
          { value: 2, name: 'BAM' },
          { value: 3, name: 'EUR' }
        ],
        enableCoinConfiguration: {
          name: 'enableCoinConfiguration',
          label: 'Enable coin configuration',
          value: true
        },
        hoppers: {
          name: 'hoppers',
          label: 'Hoppers',
          value: 4
        },
        maxCoinDispense: {
          name: 'maxCoinDispense',
          label: 'Max. coin dispense',
          value: 22
        },
        roundUpFromValue: {
          name: 'roundUpFromValue',
          label: 'Round up from Value',
          value: 0.0
        },
        manualHopperEmpty: {
          name: 'manualHopperEmpty',
          label: 'Manual hopper empty',
          value: false
        },
        roundToSmallest: {
          name: 'roundToSmallest',
          label: 'Round to smallest',
          value: false
        },
        items: [
          {
            id: 1,
            name: 'type',
            label: '1',
            value: 0.1,
            currency: {
              name: 'currency',
              label: 'Currency',
              value: 1
            },
            hopper: {
              name: 'hoppr',
              label: 'Hopper',
              value: 1
            },
            minStock: {
              name: 'minStock',
              label: 'Min. stock',
              value: 20
            },
            refill: {
              name: 'refill',
              label: 'Refill',
              value: 35
            }
          },
          {
            id: 2,
            name: 'type',
            label: '2',
            value: 0.2,
            currency: {
              name: 'currency',
              label: 'Currency',
              value: 1
            },
            hopper: {
              name: 'hoppr',
              label: 'Hopper',
              value: 2
            },
            minStock: {
              name: 'minStock',
              label: 'Min. stock',
              value: 20
            },
            refill: {
              name: 'refill',
              label: 'Refill',
              value: 35
            }
          },
          {
            id: 3,
            name: 'type',
            label: '3',
            value: 0.5,
            currency: {
              name: 'currency',
              label: 'Currency',
              value: 1
            },
            hopper: {
              name: 'hoppr',
              label: 'Hopper',
              value: 3
            },
            minStock: {
              name: 'minStock',
              label: 'Min. stock',
              value: 20
            },
            refill: {
              name: 'refill',
              label: 'Refill',
              value: 35
            }
          },
          {
            id: 4,
            name: 'type',
            label: '4',
            value: 1.0,
            currency: {
              name: 'currency',
              label: 'Currency',
              value: 1
            },
            hopper: {
              name: 'hoppr',
              label: 'Hopper',
              value: 4
            },
            minStock: {
              name: 'minStock',
              label: 'Min. stock',
              value: 20
            },
            refill: {
              name: 'refill',
              label: 'Refill',
              value: 35
            }
          }
        ]
      })
    }, 1500)
  })
}

export const useConfigurationMoneyCoinDispenser = () => {
  return useQuery({
    queryKey: ['configuration-money-coin-dispenser'],
    queryFn: async () => getConfigurationMonenyCoinDispenserData()
  })
}

//=======================================================================
export const getConfigurationMoneyTITOPrinter = async () => {
  const { data } = await api.get(`api/configuration/terminal/tito-printer`)
  return data
}

export const useConfigurationMoneyTITOPrinter = () => {
  return useQuery({
    queryKey: ['configuration-money-tito-printer'],
    queryFn: async () => getConfigurationMoneyTITOPrinter()
  })
}

//=======================================================================

const getConfigurationMonenyCoinAcceptorData = async (currency) => {
  const { data } = await api.get(`api/configuration/terminal/coin-acceptor`, {
    params: {
      currency
    }
  })

  return data
}

export const useConfigurationMonenyCoinAcceptorData = (currency) => {
  return useQuery({
    queryKey: ['configuration-money-coin-acceptor', currency],
    queryFn: async () => getConfigurationMonenyCoinAcceptorData(currency)
  })
}

//=======================================================================
