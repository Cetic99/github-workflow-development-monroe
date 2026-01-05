/* eslint-disable react/prop-types */
/* eslint-disable react/display-name */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Modal from '@ui/components/modal'
import { forwardRef } from 'react'
import { useTranslation } from '@domain/administration/stores'
import Button from '@ui/components/button'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;

  & .confirmation-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    text-align: left;
  }

  & .confirmation-content {
    display: flex;
    flex-direction: row-reverse;
    gap: 1.25rem;
  }
`

const ConfirmationDialog = forwardRef((props, ref) => {
  const { t } = useTranslation()

  const { onConfirm = () => {}, onCancel = () => {}, header = t('Are you sure?') } = props

  return (
    <Modal ref={ref} hasClose={false} size="s">
      <Container>
        <div className="confirmation-header">{header}</div>

        <div className="confirmation-content">
          <Button size="l" color="dark" onClick={onConfirm}>
            {t('Yes')}
          </Button>

          <Button size="l" color="light" onClick={onCancel}>
            {t('No')}
          </Button>
        </div>
      </Container>
    </Modal>
  )
})

export default ConfirmationDialog
