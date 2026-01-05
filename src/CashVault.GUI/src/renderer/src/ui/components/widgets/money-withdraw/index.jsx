/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import Widget from '../widget'
import { useTranslation } from '@domain/administration/stores'
import { useNavigate } from 'react-router-dom'
import IconBanknote01 from '@ui/components/icons/IconBanknote01'
import { useIsBillDispenserReady } from '@src/app/domain/device/stores'
import { useCreditsAmount } from '@src/app/domain/transactions/store'

const MoneyWithdrawWidget = ({ path = 'withdraw-money', size, ...rest }) => {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const isBillDispenserReady = useIsBillDispenserReady()
  const creditsAmount = useCreditsAmount()

  return (
    <Widget
      icon={<IconBanknote01 color={'var(--primary-light)'} size="l" />}
      text={t('Withdraw money')}
      disabled={isBillDispenserReady === false || creditsAmount?.amount === 0}
      onClick={() => navigate(path)}
      size={size}
      {...rest}
    />
  )
}

export default MoneyWithdrawWidget
