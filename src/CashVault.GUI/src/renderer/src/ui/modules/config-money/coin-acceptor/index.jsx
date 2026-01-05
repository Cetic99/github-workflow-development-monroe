/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import { DEVICE_TYPE } from '@domain/device/constants'
import useDevices from '@src/app/domain/administration/hooks/use-devices'
import Alert from '@ui/components/alert'
import CoinAcceptorRM5HDConfig from './RM5HD-config'

const MoneyConfigCoinAcceptor = () => {
  const { t } = useTranslation()
  const { data: devices } = useDevices()

  if (
    (Array.isArray(devices) ? devices : []).find((item) => item?.type === DEVICE_TYPE.COIN_ACCEPTOR)
      ?.name == 'CashVault.ccTalkCoinAcceptor.RM5HD'
  ) {
    return <CoinAcceptorRM5HDConfig />
  }

  return <Alert severity="warning" text={t('Coin acceptor device not available')} />
}

export default MoneyConfigCoinAcceptor
