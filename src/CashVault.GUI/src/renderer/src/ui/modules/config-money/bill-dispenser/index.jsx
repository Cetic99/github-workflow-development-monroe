/* eslint-disable prettier/prettier */

import { useTranslation } from '@src/app/domain/administration/stores'
import BillDispenserJCMF53Config from './JCM-F53-config'
import useDevices from '@src/app/domain/administration/hooks/use-devices'
import Alert from '@ui/components/alert'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'

const MoneyConfigBillDispenser = () => {
  const { t } = useTranslation()
  const { data: devices } = useDevices()
  if (
    (Array.isArray(devices) ? devices : []).find(
      (item) => item?.type === DEVICE_TYPE.BILL_DISPENSER
    )?.name == 'CashVault.BillDispenserDriver.JCM.F53'
  ) {
    return <BillDispenserJCMF53Config />
  }

  return <Alert severity="warning" text={t('Bill dispenser device not available')} />
}

export default MoneyConfigBillDispenser
