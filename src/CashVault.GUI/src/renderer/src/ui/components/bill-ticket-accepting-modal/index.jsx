/* eslint-disable prettier/prettier */
import { useEffect } from 'react'
import {
  useAcceptingInProgress,
  useAcceptingMessages,
  useCreditsActions
} from '@domain/transactions/store'
import Button from '@ui/components/button'
import styled from '@emotion/styled'
import AcceptingSpinner from '@ui/components/accepting-spinner'
import IconExclamationMarkCircle from '@ui/components/icons/IconExclamationMarkCircle'
import { useTranslation } from '@src/app/domain/administration/stores'

const Overlay = styled.div`
  position: fixed;
  inset: 0;
  background: rgba(255, 255, 255, 0.5);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  z-index: 9999;

  .message {
    margin-top: 1rem;
    font-size: 1rem;
    color: var(--text-primary);
  }

  .error {
    color: var(--error-dark);
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }
`

const BillTicketAcceptingOverlay = () => {
  const accepting = useAcceptingInProgress()
  const messages = useAcceptingMessages()
  const { t } = useTranslation()

  const { toggleAcceptingInProgress, clearAcceptingMessages } = useCreditsActions()

  const hasMessages = messages.length > 0

  const onClose = () => {
    toggleAcceptingInProgress(false)
    clearAcceptingMessages()
  }

  // Auto dismiss
  useEffect(() => {
    if (accepting === false && messages?.length > 0) {
      const timer = setTimeout(() => {
        onClose()
      }, 2000)

      return () => clearTimeout(timer)
    }
  }, [accepting, messages])

  if (!accepting && !hasMessages) return null

  return (
    <Overlay>
      {accepting && <AcceptingSpinner />}

      {!accepting && hasMessages && (
        <div className={`message ${!messages[0].success ? 'error' : ''}`}>
          {!messages[0].success && <IconExclamationMarkCircle color="var(--error-dark)" size="m" />}
          {messages[0].text}
        </div>
      )}

      {!accepting && hasMessages && (
        <Button size="s" onClick={onClose}>
          {t('Close')}
        </Button>
      )}
    </Overlay>
  )
}

export default BillTicketAcceptingOverlay
