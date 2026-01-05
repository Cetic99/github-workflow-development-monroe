/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import IconBanknote01 from '../icons/IconBanknote01'
import { useTranslation } from '@domain/administration/stores'
import { useDecmialNumberFormat } from '@domain/administration/stores/regional'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  text-align: right;

  & .cbv1-value {
    font-size: 4.375rem;
    line-height: 4.688rem;
    font-weight: bold;
    color: var(--text-dark);
  }

  & .cbv1-text {
    font-size: 1.875rem;
    line-height: 2.125rem;
    font-weight: 600;
    color: var(--secondary-dark);
  }
`

const CurrentBalanceV1 = ({ value = 0.0, currency = 'KM' }) => {
  const { t } = useTranslation()
  const formattedValue = useDecmialNumberFormat(value)

  return (
    <Container>
      <div className="cbv1-icon">
        <IconBanknote01 size="xl" />
      </div>
      <div className="cbv1-value">{`${formattedValue} ${currency}`}</div>
      <div className="cbv1-text">{t('Your current balance')}</div>
    </Container>
  )
}

export default CurrentBalanceV1
