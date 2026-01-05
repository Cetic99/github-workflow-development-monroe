/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import Widget from '../widget'
import { useTranslation } from '@domain/administration/stores'
import { useNavigate } from 'react-router-dom'

import IconReceipt from '@ui/components/icons/IconReceipt'
import { useCreditsAmount } from '@src/app/domain/transactions/store'
import { useIsTITOPrinterReady } from '@src/app/domain/device/stores'

const TITOPrinterWidget = ({ path = 'print-ticket', size, ...rest }) => {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const creditsAmount = useCreditsAmount()
  const isPrinterReady = useIsTITOPrinterReady()

  return (
    <Widget
      icon={<IconReceipt color={'var(--primary-light)'} size="l" />}
      text={t('Print your ticket')}
      disabled={isPrinterReady === false || creditsAmount?.amount === 0}
      onClick={() => navigate(path)}
      size={size}
      {...rest}
    />
  )
}

export default TITOPrinterWidget
