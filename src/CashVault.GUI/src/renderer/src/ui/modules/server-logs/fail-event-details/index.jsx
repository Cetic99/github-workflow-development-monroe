import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import Divider from '@ui/components/divider'
import TextInput from '@ui/components/inputs/text-input'
import TextareaInput from '@ui/components/inputs/textarea-input'
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
`

const Row = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 2rem;

  border-bottom: 1px solid var(--primary-medium);
  padding-bottom: 1rem;
`

const EventFailDetails = forwardRef(({ data = {}, onClose }, ref) => {
  const { t } = useTranslation()

  return (
    <Modal ref={ref} hasClose={true} size="l" onClose={onClose}>
      <Container>
        <div className="details-header">{t('Fail Event details')}</div>

        <div className="details-content">
          <Row>
            <TextInput size="m" label={t('Name')} value={data?.name} disabled={true} />
            <TextInput
              size="m"
              label={t('Timestamp')}
              value={sanitizeTimestamp(data?.timestamp) ?? ''}
              disabled={true}
            />
          </Row>

          <TextareaInput
            label={t('Message')}
            rows={6}
            value={data?.message ?? ''}
            disabled={true}
          />

          <Divider color="var(--primary-medium)" />
        </div>
      </Container>
    </Modal>
  )
})

export default EventFailDetails
