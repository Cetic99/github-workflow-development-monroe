/* eslint-disable react/display-name */
/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import TextInput from '@ui/components/inputs/text-input'
import DecimalInput from '@ui/components/inputs/decimal-input'
import { sanitizeTimestamp } from '@src/app/services/utils'
import Modal from '@ui/components/modal'
import { forwardRef } from 'react'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 3rem;

  & .details-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    padding-top: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid var(--primary-medium);
  }

  & .details-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  & .form {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  & .form-item {
    display: flex;
    flex-direction: column;
  }

  & .check-container {
    display: flex;
    gap: 0.5rem;
    align-items: center;
    box-sizing: border-box;
    padding: 1.5rem 0 0 1.5rem;

    font-weight: 600;
    font-size: 1.625rem;
    line-height: 2.125rem;
  }
`

const Row = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 2rem;

  border-bottom: 1px solid var(--primary-medium);
  padding-bottom: 1rem;
`

const TicketDetails = forwardRef(({ data = {}, onClose }, ref) => {
  const { t } = useTranslation()

  return (
    <Modal ref={ref} hasClose={true} size="l" onClose={onClose}>
      <Container>
        <div className="details-header">{t('Ticket details')}</div>

        <div className="details-content">
          <Row>
            <TextInput size="m" label={t('Number')} value={data?.number ?? ''} disabled={true} />
            <TextInput
              size="m"
              label={t('Days valid')}
              value={data?.daysValid ?? ''}
              disabled={true}
            />
          </Row>

          <Row>
            <TextInput size="m" label={t('Barcode')} value={data?.barcode ?? ''} disabled={true} />
            <TextInput size="m" label={t('Type')} value={data?.type ?? ''} disabled={true} />
          </Row>

          <Row>
            <DecimalInput
              size="m"
              label={t('Amount')}
              value={data?.amount ?? ''}
              sufix={data?.currency?.symbol}
              disabled={true}
              hasClearButton={false}
            />
            <TextInput
              size="m"
              label={t('Expiration date')}
              value={sanitizeTimestamp(data?.expirationDateTime) ?? ''}
              disabled={true}
            />
          </Row>

          <Row>
            <TextInput
              size="m"
              label={t('Printing requested at')}
              value={sanitizeTimestamp(data?.printingRequestedAt) ?? ''}
              disabled={true}
            />
            <TextInput
              size="m"
              label={t('Printing completed at')}
              value={sanitizeTimestamp(data?.printingCompletedAt) ?? ''}
              disabled={true}
            />
          </Row>

          <Row>
            <CheckboxInput label={t('Is local')} value={data?.isLocal ?? false} />
            <CheckboxInput label={t('Is printed')} value={data?.isPrinted ?? false} />
          </Row>
          <Row>
            <CheckboxInput label={t('Is stacked')} value={data?.isStacked ?? false} />
            <CheckboxInput label={t('Is expired')} value={data?.isExpired ?? false} />
          </Row>
          <Row>
            <CheckboxInput label={t('Is valid')} value={data?.isValid ?? false} />
            <CheckboxInput label={t('Can be redeemed')} value={data?.canBeRedeemed || false} />
          </Row>
        </div>
      </Container>
    </Modal>
  )
})

export default TicketDetails
