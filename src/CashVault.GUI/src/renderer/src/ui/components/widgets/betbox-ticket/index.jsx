/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import Widget from '../widget'
import { useTranslation } from '@domain/administration/stores'
import { useNavigate } from 'react-router-dom'

import IconReceipt from '@ui/components/icons/IconReceipt'

const BetboxTicketWidget = ({
  path = 'betbox-ticket',
  size,
  ...rest
}) => {
  const { t } = useTranslation()
  const navigate = useNavigate()

  return (
    <Widget
      icon={<IconReceipt color={'var(--primary-light)'} size="l" />}
      text={t('Redeem betbox ticket')}
      onClick={() => navigate(path)}
      size={size}
      {...rest}
    />
  )
}

export default BetboxTicketWidget
