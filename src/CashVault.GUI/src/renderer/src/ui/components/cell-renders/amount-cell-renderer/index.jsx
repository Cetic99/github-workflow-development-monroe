import styled from '@emotion/styled'
import { TransactionStatus, TransactionType } from '@domain/transactions/contants'
import IconArrowUp from '@ui/components/icons/IconArrowUp'
import IconArrowDown from '@ui/components/icons/IconArrowDown'

const AmountCell = styled.div`
  display: flex;
  gap: 0.25rem;
  text-align: left;
  align-items: center;

  color: ${(p) => (p.isCompleted ? 'var(--primary-medium)' : 'var(--error-dark)')};

  & .amount-cell-content {
    display: flex;
    gap: 0.25rem;
    flex-direction: column;
    flex-grow: 10;

    & .amount {
      font-weight: 700;
      font-size: 1.5rem;
      line-height: 1.75rem;
    }

    & .currency {
      font-weight: 500;
      font-size: 1.125rem;
      line-height: 1.35rem;
    }
  }

  & .amount-cell-icon {
    flex-shrink: 0;
  }
`

const AmountCellRender = ({ status, type, amount, currency }) => {
  return (
    <AmountCell isCompleted={status === TransactionStatus.COMPLETED}>
      <div className="amount-cell-content">
        <div className="amount">{amount}</div>
        <div className="currency">{currency?.symbol}</div>
      </div>

      <div className="amount-cell-icon">
        {type === TransactionType.CREDIT && <IconArrowUp color="var(--primary-dark)" size="m" />}
        {type === TransactionType.DEBIT && <IconArrowDown color="var(--error-dark)" size="m" />}
      </div>
    </AmountCell>
  )
}

export default AmountCellRender
