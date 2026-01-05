import styled from '@emotion/styled'
import { TransactionStatus } from '@domain/transactions/contants'
import { useTranslation } from '@domain/administration/stores'
import IconExclamationMarkCircle from '@ui/components/icons/IconExclamationMarkCircle'
import IconCheckCircle from '@ui/components/icons/IconCheckCircle'

const TransactionStatusCell = styled.div`
  display: flex;
  gap: 0.25rem;
  align-items: center;

  & .cell-icon {
    flex-shrink: 0;
  }

  & .cell-content {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    font-weight: 500;
    font-size: 1.125rem;
    line-height: 1.5rem;
  }

  & .blue {
    color: var(--border-light);
  }

  & .green {
    color: var(--primary-medium);
  }

  & .orange {
    color: orange;
  }

  & .red {
    color: var(--error-dark);
  }
`

const TransactionStatusCellRender = ({ status, description, requestedAmount }) => {
  const { t } = useTranslation()

  return (
    <TransactionStatusCell status={status}>
      <div className="cell-icon">
        {status === TransactionStatus.PENDING && (
          <IconCheckCircle size="m" color="var(--border-light)" />
        )}
        {status === TransactionStatus.COMPLETED && (
          <IconCheckCircle size="m" color="var(--primary-dark)" />
        )}
        {status === TransactionStatus.PARTIALLY_COMPLETED && (
          <IconExclamationMarkCircle size="m" color="orange" />
        )}
        {status === TransactionStatus.FAILED && (
          <IconExclamationMarkCircle size="m" color="var(--error-dark)" />
        )}
      </div>

      <div className="cell-content">
        {status === TransactionStatus.PENDING && <div className="status blue">{t('Pending')}</div>}
        {status === TransactionStatus.COMPLETED && (
          <div className="status green">{t('Completed')}</div>
        )}
        {status === TransactionStatus.PARTIALLY_COMPLETED && (
          <div className="status orange">{t('Partially completed')}</div>
        )}
        {status === TransactionStatus.FAILED && <div className="status red">{t('Failed')}</div>}

        {description && <div className="description">{description}</div>}

        {requestedAmount && (
          <div>
            {t('Requested')}: {requestedAmount}
          </div>
        )}
      </div>
    </TransactionStatusCell>
  )
}

export default TransactionStatusCellRender
