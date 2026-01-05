/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import { DEVICE_TYPE } from '@domain/device/constants'
import useDevices from '@src/app/domain/administration/hooks/use-devices'
import BillAcceptorID003Config from './ID003-config'
import Alert from '@ui/components/alert'
import BillAcceptorNV10Config from './NV10-config'

const MoneyConfigBillAcceptor = () => {
  const { t } = useTranslation()
  const { data: devices } = useDevices()

  if (
    (Array.isArray(devices) ? devices : []).find((item) => item?.type === DEVICE_TYPE.BILL_ACCEPTOR)
      ?.name == 'CashVault.BillAcceptorDriver.ID003'
  ) {
    return <BillAcceptorID003Config />
  }

  if (
    (Array.isArray(devices) ? devices : []).find((item) => item?.type === DEVICE_TYPE.BILL_ACCEPTOR)
      ?.name == 'CashVault.BillAcceptorDriver.NV10'
  ) {
    return <BillAcceptorNV10Config />
  }

  return <Alert severity="warning" text={t('Bill acceptor device not available')} />
}

export default MoneyConfigBillAcceptor
