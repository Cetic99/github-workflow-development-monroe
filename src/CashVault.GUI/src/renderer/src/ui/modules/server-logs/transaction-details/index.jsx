import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import Divider from '@ui/components/divider'
import TextInput from '@ui/components/inputs/text-input'
import DecimalInput from '@ui/components/inputs/decimal-input'
import TextareaInput from '@ui/components/inputs/textarea-input'
import Modal from '@ui/components/modal'
import { forwardRef } from 'react'
import DispenserBillItems from '@ui/modules/server-logs/dispenser-bill-items'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 3rem;

  & .details-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
  }

  & .details-content {
  }

  & .form {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  & .form-item {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-gap: 1rem;
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

  & .bill-dispenser-items {
    display: flex;
    flex-direction: column;
    gap: 2rem;
    width: 100%;

    & .bdi-header {
      font-weight: 600;
      font-size: 1.75rem;
      line-height: 2.25rem;
    }
  }
`

const TransactionDetails = forwardRef(({ data = {}, onClose }, ref) => {
  const { t } = useTranslation()

  return (
    <Modal ref={ref} hasClose={true} size="l" onClose={onClose}>
      <Container>
        <div className="details-header">{t('Transaction details')}</div>

        <div className="details-content">
          <div className="form">
            <div className="form-item">
              <TextInput disabled={true} size="m" label={t('Type')} value={data?.type} />
              <TextInput disabled={true} size="m" label={t('Status')} value={data?.status} />
            </div>

            <Divider color="var(--primary-medium)" />

            <div className="form-item">
              <TextInput disabled={true} size="l" label={t('Kind')} value={data?.kind} />
              <TextInput
                disabled={true}
                size="l"
                label={t('Reference')}
                value={data?.externalReference}
              />
            </div>

            <Divider color="var(--primary-medium)" />

            <div className="form-item">
              <DecimalInput
                size="m"
                label={t('Amount requested')}
                value={data?.amountRequested}
                sufix={data?.currency?.symbol}
                disabled={true}
              />
              <DecimalInput
                size="m"
                label={t('Amount')}
                value={data?.amount}
                sufix={data?.currency?.symbol}
                disabled={true}
              />
            </div>

            <div className="form-item">
              <DecimalInput
                size="m"
                label={t('Previous balance')}
                value={data?.previousCreditAmount}
                sufix={data?.currency?.symbol}
                disabled={true}
              />
              <DecimalInput
                size="m"
                label={t('New balance')}
                value={data?.newCreditAmount}
                sufix={data?.currency?.symbol}
                disabled={true}
              />
            </div>

            <Divider color="var(--primary-medium)" />

            <div className="form-item">
              <TextInput
                disabled={true}
                size="m"
                label={t('Started')}
                value={data?.processingStarted}
              />
              <TextInput
                disabled={true}
                size="m"
                label={t('Ended')}
                value={data?.processingEnded}
              />
            </div>

            <Divider color="var(--primary-medium)" />

            <div className="form-item">
              <TextareaInput
                disabled={true}
                size="m"
                label={t('Description')}
                value={data?.description}
              />

              <div className="check-container">
                <CheckboxInput size="m" value={data?.isCms} disabled={true} />
                <label>{t('Is CMS')}</label>
              </div>
            </div>

            <Divider color="var(--primary-medium)" />

            {data?.dispenserBillItems?.length > 0 && (
              <div className="bill-dispenser-items">
                <div className="bdi-header">{t('Bill Dispenser Items')}</div>

                <DispenserBillItems data={data?.dispenserBillItems || []} />
              </div>
            )}
          </div>
        </div>
      </Container>
    </Modal>
  )
})

export default TransactionDetails
