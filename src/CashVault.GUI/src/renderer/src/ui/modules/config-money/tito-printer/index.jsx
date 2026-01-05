/* eslint-disable prettier/prettier */

import useDevices from '@src/app/domain/administration/hooks/use-devices'
import TITOPrinterFutureLogicConfiguration from './future-logic-config'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'
import { useTranslation } from '@src/app/domain/administration/stores'
import Alert from '@ui/components/alert'

const MoneyConfigTITOPrinter = () => {
  const { t } = useTranslation()
  const { data: devices } = useDevices()

  if (
    (Array.isArray(devices) ? devices : []).find((item) => item?.type === DEVICE_TYPE.TITO_PRINTER)
      ?.name == 'CashVault.TicketPrinterDriver.FutureLogic'
  ) {
    return <TITOPrinterFutureLogicConfiguration />
  }

  return <Alert severity="warning" text={t('TITO printer device not available')} />
}

export default MoneyConfigTITOPrinter
